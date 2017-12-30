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
            BoardViewModel = new BoardViewModel(game.State);


            // Loops until the current player is human, then unlocks the human player
            void AiDecisionLoop() {
                while (Game.State.CurrentPlayer.IsAi) {
                    Debug.WriteLine("inside loop");
                    Thread.Sleep(250);
                    StepAi(Game.State.CurrentPlayer);
                }
                Debug.WriteLine("outside loop");
                BoardViewModel.UnlockedPlayer = Game.State.CurrentPlayer;
            }

            _loopThread = new Thread(AiDecisionLoop);

            // Receive any move and update the game's state.
            MessengerInstance.Register(
                this,
                token: CommittedMoveMessageToken,
                receiveDerivedMessagesToo: true, // receive MoveAction and PassAction
                action: LogReception<IUpdateAction>(UpdateGame));
        }

        #endregion

        #region Properties

        public BoardViewModel BoardViewModel { get; set; }

        public Game Game { get; }

        public GameMemento Memento => Game.Memento;
        public int BoardSize => Game.State.BoardSize;

        #endregion

        #region Public methods

        private readonly Thread _loopThread;

        public void StartGameLoop() {
            _loopThread.Start();
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

//            MessengerInstance.Send(token: CommittedMoveMessageToken,
//                                   message: new NotificationMessage<IUpdateAction>(algo.NextMove(Game.State, player),
//                                                                                   "An AI player took their decision"));
        }


        private static Action<NotificationMessage<T>> LogReception<T>(Action<T> action) {
            return message => {
                Debug.WriteLine(message.Notification);
                action(message.Content);
            };
        }

        #endregion
    }
}