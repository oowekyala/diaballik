﻿using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Builders;
using Diaballik.Core.Util;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     View modes for the game view.
    /// </summary>
    public enum ViewMode {
        Play,
        Replay
    }

    /// <summary>
    ///     View model for the screen in which the game can be played.
    /// </summary>
    public class GameScreenViewModel : ViewModelBase {
        #region Constructor

        /// <summary>
        ///     Creates a new Game presenter, with the given game builder.
        /// </summary>
        /// <param name="builder">The builder used to build the game</param>
        public GameScreenViewModel(IMessenger UImessenger, Game game) {
            PrimaryGame = game;
            MessengerInstance = UImessenger;

            BoardViewModel = new BoardViewModel(MessengerInstance, PrimaryGame);
            PlayModeToolBarViewModel = new PlayModeToolBarViewModel(MessengerInstance, PrimaryGame);
            ReplayModeToolBarViewModel = new ReplayModeToolBarViewModel(MessengerInstance);
            Player1Tag = new PlayerTagViewModel(PrimaryGame.Player1);
            Player2Tag = new PlayerTagViewModel(PrimaryGame.Player2);


            UpdatePlayerTags(game.State);

            InitMessageHandlers();
        }

        public void Reset(Game game) {
            PrimaryGame = game;
            BoardViewModel.Reset(PrimaryGame);
            PlayModeToolBarViewModel.Game = PrimaryGame;
            Player1Tag.Player = PrimaryGame.Player1;
            Player2Tag.Player = PrimaryGame.Player2;
            ActiveMode = ViewMode.Play;
            UpdatePlayerTags(PrimaryGame.State);
        }


        private void InitMessageHandlers() {
            // Receive any move and update the primary game's state.
            MessengerInstance.Register<IUpdateAction>(
                recipient: this,
                token: CommittedMoveMessageToken,
                receiveDerivedMessagesToo: true, // receive MoveAction and PassAction
                action: action => {
                    UpdateGame(action, PrimaryGame);
                    BoardViewModel.SelectedTile = null;
                });

            // These message handlers stay irrespective of the ActiveMode
            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: UndoMessageToken,
                action: message => Undo(ModeSpecificGame()));

            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: RedoMessageToken,
                action: message => Redo(ModeSpecificGame()));

            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: SwitchToReplayModeMessageToken,
                action: message => ActiveMode = ViewMode.Replay);


            // These are specific to the Replay mode
            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: UndoTillRootMessageToken,
                action: message => UndoTillRoot(ReplayGame)
            );

            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: RedoTillLastMessageToken,
                action: message => RedoTillLast(ReplayGame)
            );

            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: ResumeGameMessageToken,
                action: message => {
                    Debug.WriteLine("Resume received");
                    BoardViewModel.Reset(PrimaryGame);
                    ActiveMode = ViewMode.Play;
                });


            MessengerInstance.Register<NotificationMessage<OptionStrategy>>(
                recipient: this,
                token: ForkGameMessageToken,
                action: message => ForkGame(message.Content)
            );
        }

        private void ForkGame(OptionStrategy strat) {
            if (strat == OptionStrategy.Save) {
                // save game
            }

            PrimaryGame = ReplayGame;
            ActiveMode = ViewMode.Play;
        }

        public enum OptionStrategy {
            // Save the game before resume
            Save,

            // Lose previous state of the game and fork
            Force
        }

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
                ReplayModeToolBarViewModel.Game = ReplayGame;
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

        public GameMemento Memento => PrimaryGame.Memento;
        public int BoardSize => PrimaryGame.State.BoardSize;


        /// Delay in ms before an AI move. We don't want it to play as fast as it can.
        private const int AiStepTimeMillis = 1000;

        #endregion

        #region Public methods

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
            BoardViewModel.UnlockedPlayer = null;

            MessengerInstance.Send(new NotificationMessage<Player>(PrimaryGame.State.VictoriousPlayer, "show popup"),
                                   token: ShowVictoryPopupMessageToken);
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