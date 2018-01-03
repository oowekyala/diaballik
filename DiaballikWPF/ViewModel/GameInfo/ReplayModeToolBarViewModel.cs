using System.Diagnostics;
using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.GameScreenViewModel;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
    public class ReplayModeToolBarViewModel : AbstractGameScreenToolBar {
        private Game _game;

        public Game Game {
            get => _game;
            set {
                Set(ref _game, value);
                NotifyGameUpdate();
            }
        }

        private bool _canResume;

        public bool CanResume {
            get => _canResume;
            set {
                Set(ref _canResume, value);
                NotifyGameUpdate();
            }
        }

        public void NotifyGameUpdate() {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
            RedoTillLastCommand.RaiseCanExecuteChanged();
            UndoTillRootCommand.RaiseCanExecuteChanged();
            ResumeCommand.RaiseCanExecuteChanged();
            ForkGameCommand.RaiseCanExecuteChanged();
        }


        public ReplayModeToolBarViewModel(IMessenger messenger) {
            MessengerInstance = messenger;
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

        #region RedoTillLast

        private RelayCommand _redoTillLastCommand;

        public RelayCommand RedoTillLastCommand =>
            _redoTillLastCommand ?? (_redoTillLastCommand =
                new RelayCommand(RedoTillLastCommandExecute, RedoTillLastCommandCanExecute));

        private bool RedoTillLastCommandCanExecute() => Game.CanRedo;


        private void RedoTillLastCommandExecute() {
            MessengerInstance.Send(new NotificationMessage("player requests redo till last"),
                                   token: RedoTillLastMessageToken);
        }

        #endregion

        #region UndoTillRoot

        private RelayCommand _undoTillRootCommand;

        public RelayCommand UndoTillRootCommand =>
            _undoTillRootCommand ?? (_undoTillRootCommand =
                new RelayCommand(UndoTillRootCommandExecute, UndoTillRootCommandCanExecute));

        private bool UndoTillRootCommandCanExecute() => Game.CanUndo;


        private void UndoTillRootCommandExecute() {
            MessengerInstance.Send(new NotificationMessage("player has requested Undo till root"),
                                   token: UndoTillRootMessageToken);
        }

        #endregion

        #region Resume

        private RelayCommand _resumeCommand;

        public RelayCommand ResumeCommand =>
            _resumeCommand ?? (_resumeCommand =
                new RelayCommand(ResumeCommandExecute, ResumeCommandCanExecute));

        private bool ResumeCommandCanExecute() => CanResume;


        private void ResumeCommandExecute() {
            Debug.WriteLine("Resume sent");
            MessengerInstance.Send(new NotificationMessage("player has requested resume game"),
                                   token: ResumeGameMessageToken);
        }

        #endregion

        #region ForkGame

        private RelayCommand _forkGame;

        public RelayCommand ForkGameCommand =>
            _forkGame ?? (_forkGame =
                new RelayCommand(ForkGameCommandExecute, ForkGameCommandCanExecute));

        private bool ForkGameCommandCanExecute() => Game.CanRedo; // not the last state, it's already the primary game


        private void ForkGameCommandExecute() {
            MessengerInstance.Send(
                new NotificationMessage<OptionStrategy>(OptionStrategy.Force, "player has requested fork game"),
                token: ForkGameMessageToken);
        }

        #endregion
    }
}