using System;
using System.Diagnostics;
using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.GameScreenViewModel;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    public class ReplayModeToolBarViewModel : AbstractGameScreenToolBar {
        private Game _replayGame;

        public Game ReplayGame {
            get => _replayGame;
            set {
                Set(ref _replayGame, value);
                NotifyGameUpdate();
            }
        }

        public Game PrimaryGame { get; set; }

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

        private bool UndoCommandCanExecute() => ReplayGame.CanUndo;


        private void UndoCommandExecute() {
            UndoMessage.Send(MessengerInstance);
        }

        #endregion

        #region RedoCommand

        private RelayCommand _redoCommand;

        public RelayCommand RedoCommand =>
            _redoCommand ?? (_redoCommand = new RelayCommand(RedoCommandExecute, RedoCommandCanExecute));

        private bool RedoCommandCanExecute() => ReplayGame.CanRedo;


        private void RedoCommandExecute() {
            RedoMessage.Send(MessengerInstance);
        }

        #endregion

        #region RedoTillLast

        private RelayCommand _redoTillLastCommand;

        public RelayCommand RedoTillLastCommand =>
            _redoTillLastCommand ?? (_redoTillLastCommand =
                new RelayCommand(RedoTillLastCommandExecute, RedoTillLastCommandCanExecute));

        private bool RedoTillLastCommandCanExecute() => ReplayGame.CanRedo;


        private void RedoTillLastCommandExecute() {
            RedoTillLastMessage.Send(MessengerInstance);
        }

        #endregion

        #region UndoTillRoot

        private RelayCommand _undoTillRootCommand;

        public RelayCommand UndoTillRootCommand =>
            _undoTillRootCommand ?? (_undoTillRootCommand =
                new RelayCommand(UndoTillRootCommandExecute, UndoTillRootCommandCanExecute));

        private bool UndoTillRootCommandCanExecute() => ReplayGame.CanUndo;


        private void UndoTillRootCommandExecute() {
            UndoTillRootMessage.Send(MessengerInstance);
        }

        #endregion

        #region Resume

        private RelayCommand _resumeCommand;

        public RelayCommand ResumeCommand =>
            _resumeCommand ?? (_resumeCommand =
                new RelayCommand(ResumeCommandExecute, ResumeCommandCanExecute));

        private bool ResumeCommandCanExecute() => CanResume;


        private void ResumeCommandExecute() {
            ResumeGameMessage.Send(MessengerInstance);
        }

        #endregion

        #region ForkGame

        private RelayCommand _forkGame;

        public RelayCommand ForkGameCommand =>
            _forkGame ?? (_forkGame =
                new RelayCommand(ForkGameCommandExecute, ForkGameCommandCanExecute));

        private bool ForkGameCommandCanExecute() =>
            ReplayGame.CanRedo; // not the last state, it's already the primary game


        private void ForkGameCommandExecute() {
            const string message = "Forking will create a new game. Would you like to save the current game ?";

            void ForkAction() => ForkGameMessage.Send(MessengerInstance);
            ShowSavePopupMessage.Send(MessengerInstance, (message, ForkAction, true));
        }

        #endregion
    }
}