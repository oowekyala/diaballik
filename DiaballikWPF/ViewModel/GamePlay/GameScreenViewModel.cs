using System;
using System.Diagnostics;
using System.Threading;
using Diaballik.AlgoLib;
using Diaballik.Core;
using DiaballikWPF.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    using GameId = String;


    /// <summary>
    ///     View modes for the game view.
    /// </summary>
    public enum ViewMode {
        Play,
        Replay
    }

    /// <summary>
    ///     View model for the screen in which the game can be played.
    /// 
    ///     One "master" game can be loaded at a time, which is updated when in Play mode.
    ///     Updates on that game overwrite the previous saves for that game. The rationale
    ///     is that previous saves are contained in the memento anyway (most of the times),
    ///     and can be accessed from Replay mode.
    /// 
    ///     That game can be forked from the Replay mode, which creates a new game with a 
    ///     new id.
    /// </summary>
    public class GameScreenViewModel : ViewModelBase {
        #region Constructor

        public GameScreenViewModel(IMessenger UImessenger, Game game) {
            PrimaryGame = game;
            MessengerInstance = UImessenger;

            BoardViewModel = new BoardViewModel(MessengerInstance, PrimaryGame.State);
            PlayModeToolBarViewModel = new PlayModeToolBarViewModel(MessengerInstance, PrimaryGame);
            ReplayModeToolBarViewModel = new ReplayModeToolBarViewModel(MessengerInstance);
            Player1Tag = new PlayerTagViewModel(PrimaryGame.Player1);
            Player2Tag = new PlayerTagViewModel(PrimaryGame.Player2);


            UpdatePlayerTags(game.State);

            InitMessageHandlers();
        }

        /// Sets the given game as the currently played game, using the given ID.
        /// The previous game is discarded.
        public void Load(GameId id, Game game) {
            PrimaryGame = game;
            GameId = id;
            BoardViewModel.Reset(PrimaryGame.State);
            PlayModeToolBarViewModel.Game = PrimaryGame;
            Player1Tag.Player = PrimaryGame.Player1;
            Player2Tag.Player = PrimaryGame.Player2;
            ActiveMode = ViewMode.Play;
            UpdatePlayerTags(PrimaryGame.State);
        }


        #region Message handling

        private void InitMessageHandlers() {
            // Receive any move and update the primary game's state.
            CommitMoveMessage.Register(MessengerInstance, this, action => UpdateGame(action, PrimaryGame));


            // These message handlers stay irrespective of the ActiveMode
            UndoMessage.Register(MessengerInstance, this, () => Undo(ModeSpecificGame()));
            RedoMessage.Register(MessengerInstance, this, () => Redo(ModeSpecificGame()));
            SwitchGameViewMode.Register(MessengerInstance, this, mode => ActiveMode = mode);

            // someone requested that the current state of the game be saved
            RequestSaveToGameScreenMessage.Register(
                MessengerInstance,
                this,
                () => SaveGameMessage.Send(MessengerInstance, (GameId, PrimaryGame.Memento)));


            // These are specific to the Replay mode
            UndoTillRootMessage.Register(MessengerInstance, this, () => UndoTillRoot(ReplayGame));
            RedoTillLastMessage.Register(MessengerInstance, this, () => RedoTillLast(ReplayGame));
            ResumeGameMessage.Register(MessengerInstance, this, () => {
                BoardViewModel.Reset(PrimaryGame.State);
                ActiveMode = ViewMode.Play;
            });


            ForkGameMessage.Register(MessengerInstance, this, () => {
                // this assigns a new id to the primary game, so that we don't overwrite the previous
                OpenNewGame.Send(MessengerInstance, (ReplayGame, ViewMode.Play));
            });
        }

        #endregion

        #endregion

        #region Properties

        #region ActiveMode

        private ViewMode _activeMode;

        public ViewMode ActiveMode {
            get => _activeMode;
            set {
                _activeMode = value;
                RegisterMode(value);
                RaisePropertyChanged();
            }
        }

        private void RegisterMode(ViewMode mode) {
            if (mode == ViewMode.Play) {
                Debug.WriteLine("Start game loop bc ActivMode = Play");

                //UpdatePlayerTags(PrimaryGame);
                StartGameLoop();
            } else {
                // Enter replay mode
                ReplayGame = Game.Fork(PrimaryGame);
                _aiLoopCanRun = false;
                ReplayModeToolBarViewModel.ReplayGame = ReplayGame;
                ReplayModeToolBarViewModel.CanResume = !PrimaryGame.State.IsVictory;
                BoardViewModel.UnlockedPlayer = null;
            }
        }


        private Game ModeSpecificGame() => ActiveMode == ViewMode.Play ? PrimaryGame : ReplayGame;

        #endregion

        #region ViewModels

        public BoardViewModel BoardViewModel { get; }
        public PlayModeToolBarViewModel PlayModeToolBarViewModel { get; }
        public ReplayModeToolBarViewModel ReplayModeToolBarViewModel { get; }

        public PlayerTagViewModel Player1Tag { get; }
        public PlayerTagViewModel Player2Tag { get; }

        private PlayerTagViewModel PlayerTagOf(Player player) {
            if (player == Player1Tag.Player) return Player1Tag;
            if (player == Player2Tag.Player) return Player2Tag;
            throw new ArgumentException("Unknown player");
        }

        #endregion

        #region Games

        private Game _primaryGame;

        /// Game which is updated when in Play mode
        private Game PrimaryGame {
            get => _primaryGame;
            set {
                Debug.WriteLine("Primary game set (fork)");
                _primaryGame = Game.Fork(value);
            }
        }


        /// Copy used for navigation when in Replay mode.
        /// No update action must ever be taken on that one.
        private Game ReplayGame { get; set; }


        public int BoardSize => PrimaryGame.State.BoardSize;

        #endregion

        #region GameId

        /// Identifies the current game for save operations
        public GameId GameId { get; private set; }

        #endregion

        /// Delay in ms before an AI move. We don't want it to play as fast as it can.
        private const int AiStepTimeMillis = 500;

        #endregion


        #region Private methods

        #region Update methods

        /// Update the underlying Game instance and requests a visual update.
        /// Changes the current player if need be.
        private void UpdateGame(IUpdateAction action, Game game) {
            var previousPlayer = game.State.CurrentPlayer;

            game.Update(action);

            UpdateHelper(game.State, previousPlayer, game.State.CurrentPlayer, action);
        }


        private void Undo(Game game) {
            var previousPlayer = game.Memento.State.CurrentPlayer;

            var action = (game.Memento as MementoNode)?.Action;

            if (action == null) {
                // can happen when clicking repeatedly on the undo button
                // and the game has no time to update 
                return;
            } else if (action is MoveAction move) {
                action = move.Reverse(); // for the animation, in case we implement one
            }

            game.Undo();

            UpdateHelper(game.State, previousPlayer, game.State.CurrentPlayer, action);
        }

        private void Redo(Game game) {
            game.Redo();

            var previousPlayer = game.Memento.Parent.State.CurrentPlayer;
            var action = ((MementoNode) game.Memento).Action;
            UpdateHelper(game.State, previousPlayer, game.State.CurrentPlayer, action);
        }


        /// <summary>
        ///     Updates the visual state of the game with an action.
        ///     Doesn't modify the game.
        /// </summary>
        private void UpdateHelper(GameState state, Player actor, Player currentPlayer, IUpdateAction action) {
            if (action is MoveAction move) {
                DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UpdateState(state, move));
            }

            DispatcherHelper.UIDispatcher.Invoke(() => UpdatePlayerTags(state));
            DispatcherHelper.UIDispatcher.Invoke(() => PlayModeToolBarViewModel.NotifyGameUpdate());
            DispatcherHelper.UIDispatcher.Invoke(() => ReplayModeToolBarViewModel.NotifyGameUpdate());

            if (ActiveMode == ViewMode.Play) {
                if (action is MovePieceAction movePiece && actor.IsHuman) {
                    BoardViewModel.TileAt(movePiece.Src).IsSelectable = false;

                    if (actor == currentPlayer) {
                        BoardViewModel.TileAt(movePiece.Dst).IsSelectable = true;
                    }
                }

                if (action is MoveAction moveAction && actor.IsHuman && actor == currentPlayer) {
                    // select the destination to chain moves more easily
                    BoardViewModel.SelectedTile = BoardViewModel.TileAt(moveAction.Dst);
                } else {
                    BoardViewModel.SelectedTile = null;
                }

                if (state.IsVictory) {
                    HandleVictory();
                    return;
                }

                if (actor != currentPlayer) {
                    // player changed, ask for new player to play
                    if (actor.IsHuman && currentPlayer.IsAi) {
                        DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UnlockedPlayer = null);
                        StartAiStepThread();
                    } else {
                        DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UnlockedPlayer = currentPlayer);
                    }
                }
            }
        }

        private void UpdatePlayerTags(GameState state) {
            PlayerTagOf(state.CurrentPlayer).NumMovesLeft = state.NumMovesLeft;
            PlayerTagOf(state.GetOtherPlayer(state.CurrentPlayer)).NumMovesLeft = 0;
        }

        #endregion

        #region Replay mode specific methods

        private void UndoTillRoot(Game game) {
            while (game.CanUndo) {
                Undo(game);
            }
        }

        private void RedoTillLast(Game game) {
            while (game.CanRedo) {
                Redo(game);
            }
        }

        #endregion

        #region Play mode specific methods

        /// Starts the game by requesting the first moves.
        /// Only valid in Play mode.
        private void StartGameLoop() {
            if (ActiveMode == ViewMode.Replay) {
                throw new ArgumentException("Game loop cannot be started in replay mode");
            }

            if (PrimaryGame.State.CurrentPlayer.IsAi) {
                StartAiStepThread();
            } else {
                BoardViewModel.UnlockedPlayer = PrimaryGame.State.CurrentPlayer;
            }
        }

        private void HandleVictory() {
            Debug.WriteLine($"Victory of {PrimaryGame.State.VictoriousPlayer}");
            _aiLoopCanRun = false;
            DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UnlockedPlayer = null);

            ShowVictoryPopupMessage.Send(MessengerInstance, PrimaryGame.State.VictoriousPlayer);
        }

        #endregion

        #region Ai step thread

        private Thread _loopThread;
        private volatile bool _aiLoopCanRun;
        private readonly object _gameUpdateLock = new object();

        /// Start a loop which queries and applies the moves of an AI player automatically.
        /// The loop stops when the current player is human.
        private void StartAiStepThread() {
            _aiLoopCanRun = true;

            // Loops until the current player is human, then unlocks the human player
            // the loop is interrupted by _aiLoopCanRun = false, and in case of victory
            void AiDecisionLoop() {
                while (true) {
                    lock (_gameUpdateLock) {
                        if (PrimaryGame.State.CurrentPlayer.IsAi && !PrimaryGame.State.IsVictory && _aiLoopCanRun) {
                            StepAi(PrimaryGame.State.CurrentPlayer);
                            Thread.Sleep(AiStepTimeMillis);
                        } else break;
                    }
                }
            }

            _loopThread = new Thread(AiDecisionLoop);
            _loopThread.Start();
        }

        private readonly NoobAiAlgo _noobAiAlgo = new NoobAiAlgo();
        private readonly StartingAiAlgo _startingAiAlgo = new StartingAiAlgo();
        private readonly ProgressiveAiAlgo _progressiveAiAlgo = new ProgressiveAiAlgo();


        /// If the player is an AI, updates the game with their next move.
        public void StepAi(Player player) {
            AiDecisionAlgo algo;
            switch (player.Type) {
                case PlayerType.NoobAi:
                    algo = _noobAiAlgo;
                    break;
                case PlayerType.StartingAi:
                    algo = _startingAiAlgo;
                    break;
                case PlayerType.ProgressiveAi:
                    algo = _progressiveAiAlgo;
                    break;
                default: return;
            }

            // reentrant lock
            lock (_gameUpdateLock) {
                var move = algo.NextMove(PrimaryGame.State, player);
                UpdateGame(move, PrimaryGame);
            }
        }

        #endregion

        #endregion
    }
}