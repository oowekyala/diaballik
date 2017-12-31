using System;
using System.Diagnostics;
using System.Threading;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Builders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using static DiaballikWPF.ViewModel.TilePresenterConstants;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     View model for the screen in which the game can be played.
    /// </summary>
    public class PlayGameScreenViewModel : ViewModelBase {
        #region Constructor

        /// <summary>
        ///     Creates a new Game presenter, with the given game builder.
        /// </summary>
        /// <param name="builder">The builder used to build the game</param>
        public PlayGameScreenViewModel(GameBuilder builder) {
            Game = builder.Build();
            BoardViewModel = new BoardViewModel(MessengerInstance, Game.State);

            // Receive any move and update the game's state.
            Messenger.Default.Register<NotificationMessage<IUpdateAction>>(
                this,
                token: CommittedMoveMessageToken,
                receiveDerivedMessagesToo: true, // receive MoveAction and PassAction
                action: message => {
                    Debug.WriteLine(message.Notification);
                    UpdateGame(message.Content);
                    BoardViewModel.SelectedTile = null;
                });
        }

        #endregion

        #region Properties

        public BoardViewModel BoardViewModel { get; }

        private Game Game { get; }
        public GameMemento Memento => Game.Memento;

        public int BoardSize => Game.State.BoardSize;

        /// Delay in ms before an AI move. We don't want it to play as fast as it can.
        public const int AiStepTimeMillis = 500;

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
            Game.Update(action);

            if (action is MoveAction move) {
                DispatcherHelper.UIDispatcher.Invoke(() => BoardViewModel.UpdateState(Game.State, move));
            }

            var player = Game.State.CurrentPlayer;
            if (player.IsAi) {
                StartAiStepThread();
            } else {
                BoardViewModel.UnlockedPlayer = player;
            }
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
                BoardViewModel.UnlockedPlayer = Game.State.CurrentPlayer;
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