using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
    public class StartupScreenViewModel : ViewModelBase {
        #region Constructors

        public StartupScreenViewModel(IMessenger messenger) {
            MessengerInstance = messenger;
        }

        #endregion

        #region Commands

        private RelayCommand _newGameCommand;

        public RelayCommand NewGameCommand => _newGameCommand ?? (_newGameCommand = new RelayCommand(NewGame));

        #endregion

        #region Methods

        private void NewGame() {

            MessengerInstance.Send(new NotificationMessage("show game creation"), token: ShowGameCreationScreenMessageToken);


//            var vm = new GameCreationScreenViewModel(_dock);
//            var view = new GameCreationScreen {
//                DataContext = vm
//            };

//            _dock.ContentViewModel = vm;
        }

        #endregion
    }
}