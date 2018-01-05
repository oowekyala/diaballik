using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    public class StartupScreenViewModel : ViewModelBase {
        public StartupScreenViewModel(IMessenger messenger) {
            MessengerInstance = messenger;


            QuitCommand = new RelayCommand(() => AppShutdownMessage.Send(MessengerInstance));
            LoadGameCommand = new RelayCommand(() => ShowLoadMenuMessage.Send(MessengerInstance));
            NewGameCommand = new RelayCommand(() => ShowNewGameMessage.Send(MessengerInstance));
        }


        public RelayCommand NewGameCommand { get; }
        public RelayCommand LoadGameCommand { get; }
        public RelayCommand QuitCommand { get; }
    }
}