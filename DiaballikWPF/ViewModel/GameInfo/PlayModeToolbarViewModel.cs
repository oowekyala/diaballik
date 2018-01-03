using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.Messages;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Controls the toolbar when in play mode.
    /// </summary>
    public class PlayModeToolBarViewModel : AbstractGameScreenToolBar {
        private Game _game;

        public Game Game {
            get => _game;
            set {
                Set(ref _game, value);
                NotifyGameUpdate();
            }
        }

        public PlayModeToolBarViewModel(IMessenger messenger, Game game) {
            Game = game;
            MessengerInstance = messenger;
        }

        public void NotifyGameUpdate() {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
            PassCommand.RaiseCanExecuteChanged();
        }


        #region UndoCommand

        private RelayCommand _undoCommand;

        public RelayCommand UndoCommand =>
            _undoCommand ?? (_undoCommand = new RelayCommand(UndoCommandExecute, UndoCommandCanExecute));

        private bool UndoCommandCanExecute() => !Game.State.CurrentPlayer.IsAi && Game.CanUndo;


        private void UndoCommandExecute() {
            UndoMessage.Send(MessengerInstance);
        }

        #endregion

        #region RedoCommand

        private RelayCommand _redoCommand;

        public RelayCommand RedoCommand =>
            _redoCommand ?? (_redoCommand = new RelayCommand(RedoCommandExecute, RedoCommandCanExecute));

        private bool RedoCommandCanExecute() => !Game.State.CurrentPlayer.IsAi && Game.CanRedo;


        private void RedoCommandExecute() {
            RedoMessage.Send(MessengerInstance);
        }

        #endregion

        #region PassCommand

        private RelayCommand _passCommand;

        public RelayCommand PassCommand =>
            _passCommand ?? (_passCommand = new RelayCommand(PassCommandExecute, PassCommandCanExecute));

        private bool PassCommandCanExecute() => !Game.State.CurrentPlayer.IsAi && PassAction.IsValid(Game.State);

        private void PassCommandExecute() {
            CommitMoveMessage.Send(MessengerInstance, new PassAction());
        }

        #endregion

        #region ReplayModeCommand

        private RelayCommand _replayModeCommand;

        public RelayCommand ReplayModeCommand =>
            _replayModeCommand ?? (_replayModeCommand =
                new RelayCommand(ReplayModeCommandExecute, ReplayModeCommandCanExecute));

        private bool ReplayModeCommandCanExecute() => true;

        private void ReplayModeCommandExecute() {
            SwitchGameViewMode.Send(MessengerInstance, ViewMode.Replay);
        }

        #endregion
    }
}