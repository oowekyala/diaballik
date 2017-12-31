using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Diaballik.AlgoLib;
using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using static DiaballikWPF.ViewModel.TilePresenterConstants;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     TODO Share MessengerInstance across only a game screen and descendants
    /// </summary>
    public class PlayGameScreenViewModel : ViewModelBase {
        #region Constructor

        public PlayGameScreenViewModel(Game game) {
            Game = game;
            BoardViewModel = new BoardViewModel(MessengerInstance, game.State);
        }

        #endregion

        #region Properties

        public BoardViewModel BoardViewModel { get; set; }

        public Game Game { get; }

        public GameMemento Memento => Game.Memento;
        public int BoardSize => Game.State.BoardSize;

        public const int AiStepTimeMillis = 500;

        #endregion

        #region Public methods

        public void StartGameLoop() {
            if (Game.State.CurrentPlayer.IsAi) {
                StartAiStepThread();
            } else {
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


        public static void LogReception<T>(NotificationMessage<T> message, Action<T> action) {
            Debug.WriteLine(message.Notification);
            action(message.Content);
        }

        #endregion
    }
}