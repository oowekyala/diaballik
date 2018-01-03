using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
    public class VictoryPopupViewModel : ViewModelBase {
        public VictoryPopupViewModel(IMessenger messenger, Player victoriousPlayer) {
            MessengerInstance = messenger;
            VictoriousPlayer = victoriousPlayer;
        }


        public Player VictoriousPlayer { get; }


        private void ClosePopup() {
            MessengerInstance.Send(new NotificationMessage("close the victory popup"),
                                   token: CloseVictoryPopupMessageToken);
        }

        #region ReplayModeCommand

        private RelayCommand _replayModeCommand;

        public RelayCommand ReplayModeCommand =>
            _replayModeCommand ?? (_replayModeCommand =
                new RelayCommand(ReplayModeCommandExecute, ReplayModeCommandCanExecute));

        private bool ReplayModeCommandCanExecute() => true;

        private void ReplayModeCommandExecute() {
            MessengerInstance.Send(new NotificationMessage("player requests replay mode"),
                                   token: SwitchToReplayModeMessageToken);
            ClosePopup();
        }

        #endregion


        #region BackToMainMenu

        private RelayCommand _backToMainMenuCommand;

        public RelayCommand BackToMainMenuCommand =>
            _backToMainMenuCommand ?? (_backToMainMenuCommand =
                new RelayCommand(BackToMainMenuCommandExecute, BackToMainMenuCommandCanExecute));

        private bool BackToMainMenuCommandCanExecute() => true;

        private void BackToMainMenuCommandExecute() {
            MessengerInstance.Send(new NotificationMessage("go back to main menu"),
                                   token: ShowMainMenuMessageToken);
            ClosePopup();
        }

        #endregion
    }
}