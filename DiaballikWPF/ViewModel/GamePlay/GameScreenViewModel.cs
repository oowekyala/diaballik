using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Builders;
using Diaballik.Core.Util;
using GalaSoft.MvvmLight;
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
        public GameScreenViewModel(Game game, ViewMode activeMode) {
            Game = game;
            ActiveMode = activeMode;
            RegisterMode(activeMode); // in case activeMode is the default value


            BoardViewModel = new BoardViewModel(MessengerInstance, Game.State);
            PlayModeToolBarViewModel = new PlayModeToolBarViewModel(MessengerInstance, Game);
            Player1Tag = new PlayerTagViewModel(Game.Player1);
            Player2Tag = new PlayerTagViewModel(Game.Player2);

            if (Game.State.CurrentPlayer == Game.Player1) {
                Player1Tag.IsActive = true;
            } else {
                Player2Tag.IsActive = true;
            }
        }

        #endregion

        #region Properties

        #region ActiveMode

        private ViewMode _activeMode;

        public ViewMode ActiveMode {
            get => _activeMode;
            set {
                if (value != ActiveMode) {
                    RegisterMode(value);
                    _activeMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void RegisterMode(ViewMode mode) {
            if (mode == ViewMode.Play) {
                // Receive any move and update the game's state.
                MessengerInstance.Register<IUpdateAction>(
                    recipient: this,
                    token: CommittedMoveMessageToken,
                    receiveDerivedMessagesToo: true, // receive MoveAction and PassAction
                    action: action => {
                        Debug.WriteLine("Received message");
                        UpdateGame(action);
                        BoardViewModel.SelectedTile = null;
                    });

                MessengerInstance.Register<NotificationMessage>(
                    recipient: this,
                    token: UndoMessageToken,
                    action: message => Undo());

                MessengerInstance.Register<NotificationMessage>(
                    recipient: this,
                    token: RedoMessageToken,
                    action: message => Redo());
            } else {
                MessengerInstance.Unregister<IUpdateAction>(this, CommittedMoveMessageToken);
            }
        }

        #endregion


        #region ViewModels

        public BoardViewModel BoardViewModel { get; }
        public PlayModeToolBarViewModel PlayModeToolBarViewModel { get; }

        public PlayerTagViewModel Player1Tag { get; }
        public PlayerTagViewModel Player2Tag { get; }

        #endregion

        private Game Game { get; }
        public GameMemento Memento => Game.Memento;
        public int BoardSize => Game.State.BoardSize;

        /// Delay in ms before an AI move. We don't want it to play as fast as it can.
        private const int AiStepTimeMillis = 500;

        #endregion

        #region Public methods

        public void StartGameLoop() {
            if (Game.State.CurrentPlayer.IsAi) {
                StartAiStepThread();
            } else {
                BoardViewModel.UnlockedPlayer = Game.State.CurrentPlayer;
            }
        }

        #endregion

        #region Fields

        private readonly NoobAiAlgo _noobAiAlgo = new NoobAiAlgo();
        private readonly StartingAiAlgo _startingAiAlgo = new StartingAiAlgo();
        private readonly ProgressiveAiAlgo _progressiveAiAlgo = new ProgressiveAiAlgo();

        #endregion

        #region Private methods

        /// Update the underlying Game instance and requests a visual update.
        /// Changes the current player if need be.
        private void UpdateGame(IUpdateAction action) {
            var previousPlayer = Game.State.CurrentPlayer;

            Game.Update(action);

            UpdateHelper(previousPlayer, Game.State.CurrentPlayer, action);
        }


        private void Undo() {
            var previousPlayer = Memento.State.CurrentPlayer;

            var action = ((MementoNode) Memento).Action;

            if (action is MoveAction move) {
                action = move.Reverse(); // for the animation, in case we implement one
            }

            Game.Undo();

            UpdateHelper(previousPlayer, Game.State.CurrentPlayer, action);
        }

        private void Redo() {
            Game.Redo();

            var previousPlayer = Memento.Parent.State.CurrentPlayer;
            var action = ((MementoNode) Memento).Action;
            UpdateHelper(previousPlayer, Game.State.CurrentPlayer, action);
        }


        private void UpdateHelper(Player previous, Player player, IUpdateAction action) {
            if (action is MoveAction move) {
                DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UpdateState(Game.State, move));
            }

            if (player.IsAi) {
                StartAiStepThread();
            } else {
                DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UnlockedPlayer = player);
            }

            if (player != previous) {
                (Player1Tag, Player2Tag).Foreach(tag => tag.IsActive = !tag.IsActive);
            }
            DispatcherHelper.UIDispatcher.Invoke(() => PlayModeToolBarViewModel.NotifyGameUpdate());
            if (Game.State.IsVictory) {
                HandleVictory();
            }
        }


        public void HandleVictory() {
            Debug.WriteLine($"Victory of {Game.State.VictoriousPlayer}");
        }

        private Thread _loopThread;

        /// Start a loop which queries and applies the moves of an AI player automatically.
        /// The loop stops when the current player is human.
        private void StartAiStepThread() {
            // Loops until the current player is human, then unlocks the human player
            void AiDecisionLoop() {
                while (Game.State.CurrentPlayer.IsAi) {
                    Debug.WriteLine("inside loop");
                    Thread.Sleep(AiStepTimeMillis);
                    StepAi(Game.State.CurrentPlayer);
                }
                Debug.WriteLine("outside loop");
                DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UnlockedPlayer = Game.State.CurrentPlayer);
            }

            _loopThread = new Thread(AiDecisionLoop);
            _loopThread.Start();
        }

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

            UpdateGame(algo.NextMove(Game.State, player));
        }

        #endregion
    }
}