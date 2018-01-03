using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
    public abstract class AbstractGameScreenToolBar : ViewModelBase {

        #region BackToMainMenuCommand

        private RelayCommand _backToMainMenuCommand;

        public RelayCommand BackToMainMenuCommand =>
            _backToMainMenuCommand ?? (_backToMainMenuCommand =
                new RelayCommand(BackToMainMenuCommandExecute, BackToMainMenuCommandCanExecute));

        private bool BackToMainMenuCommandCanExecute() => true;

        private void BackToMainMenuCommandExecute() {
            // TODO show save popup
            MessengerInstance.Send(new NotificationMessage("go back to main menu"),
                                   token: ShowMainMenuMessageToken);
        }

        #endregion
    }
}