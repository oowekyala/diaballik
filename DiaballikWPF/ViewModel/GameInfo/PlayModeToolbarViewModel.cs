using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Controls the toolbar when in play mode.
    /// </summary>
    public class PlayModeToolBarViewModel : ViewModelBase {
        public Game Game { get; }

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

        private bool UndoCommandCanExecute() => Game.CanUndo;


        private void UndoCommandExecute() {
            MessengerInstance.Send(new NotificationMessage("player has requested Undo"),
                                   token: UndoMessageToken);
        }

        #endregion

        #region RedoCommand

        private RelayCommand _redoCommand;

        public RelayCommand RedoCommand =>
            _redoCommand ?? (_redoCommand = new RelayCommand(RedoCommandExecute, RedoCommandCanExecute));

        private bool RedoCommandCanExecute() => Game.CanRedo;


        private void RedoCommandExecute() {
            MessengerInstance.Send(new NotificationMessage("player requests redo"),
                                   token: RedoMessageToken);
        }

        #endregion

        #region PassCommand

        private RelayCommand _passCommand;

        public RelayCommand PassCommand =>
            _passCommand ?? (_passCommand = new RelayCommand(PassCommandExecute, PassCommandCanExecute));

        private bool PassCommandCanExecute() => PassAction.IsValid(Game.State);

        private void PassCommandExecute() {
            MessengerInstance.Send(message: new PassAction(),
                                   token: CommittedMoveMessageToken);
        }

        #endregion

        #region ReplayModeCommand

        private RelayCommand _replayModeCommand;

        public RelayCommand ReplayModeCommand =>
            _replayModeCommand ?? (_replayModeCommand =
                new RelayCommand(ReplayModeCommandExecute, ReplayModeCommandCanExecute));

        private bool ReplayModeCommandCanExecute() => true;

        private void ReplayModeCommandExecute() {
            MessengerInstance.Send(new NotificationMessage("player requests replay mode"),
                                   token: SwitchToReplayModeMessageToken);
        }

        #endregion
    }
}