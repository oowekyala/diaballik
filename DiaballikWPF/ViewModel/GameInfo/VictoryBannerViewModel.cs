using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    public class VictoryBannerViewModel : ViewModelBase {
        public VictoryBannerViewModel(IMessenger messenger) {
            MessengerInstance = messenger;
        }

        private Player _victoriousPlayer;

        public Player VictoriousPlayer {
            get => _victoriousPlayer;
            set {
                Set(ref _victoriousPlayer, value);
                RaisePropertyChanged("IsVictory");
            }
        }


        public bool IsVictory => VictoriousPlayer != null;


        private void ClosePopup() {
            VictoriousPlayer = null;
            // CloseVictoryPopupMessage.Send(MessengerInstance);
        }

        #region ReplayModeCommand

        private RelayCommand _replayModeCommand;

        public RelayCommand ReplayModeCommand =>
            _replayModeCommand ?? (_replayModeCommand =
                new RelayCommand(ReplayModeCommandExecute, ReplayModeCommandCanExecute));

        private bool ReplayModeCommandCanExecute() => true;

        private void ReplayModeCommandExecute() {
            SwitchGameViewModeMessage.Send(MessengerInstance, ViewMode.Replay);
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
            ShowMainMenuMessage.Send(MessengerInstance);
            ClosePopup();
        }

        #endregion
    }
}