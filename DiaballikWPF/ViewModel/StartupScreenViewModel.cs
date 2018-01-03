using System.Windows;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.Messages;

namespace DiaballikWPF.ViewModel {
    public class StartupScreenViewModel : ViewModelBase {
        #region Constructors

        public StartupScreenViewModel(IMessenger messenger) {
            MessengerInstance = messenger;
        }

        #endregion

        #region New game

        private RelayCommand _newGameCommand;

        public RelayCommand NewGameCommand =>
            _newGameCommand ?? (_newGameCommand = new RelayCommand(NewGameCommandExecute));


        private void NewGameCommandExecute() {
            ShowNewGameMessage.Send(MessengerInstance);
        }

        #endregion

        #region Load game

        private RelayCommand _loadGameCommand;

        public RelayCommand LoadGameCommand =>
            _loadGameCommand ?? (_loadGameCommand = new RelayCommand(LoadGameCommandExecute));


        private void LoadGameCommandExecute() {
            ShowLoadMenuMessage.Send(MessengerInstance);
        }

        #endregion

        #region Load game

        private RelayCommand _quitCommand;

        public RelayCommand QuitCommand =>
            _quitCommand ?? (_quitCommand = new RelayCommand(QuitCommandExecute));


        private void QuitCommandExecute() {
            Application.Current.Shutdown(0);
        }

        #endregion
    }
}