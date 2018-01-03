using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static DiaballikWPF.ViewModel.Messages;

namespace DiaballikWPF.ViewModel {
    public abstract class AbstractGameScreenToolBar : ViewModelBase {
        #region BackToMainMenuCommand

        private RelayCommand _backToMainMenuCommand;

        public RelayCommand BackToMainMenuCommand =>
            _backToMainMenuCommand ?? (_backToMainMenuCommand =
                new RelayCommand(BackToMainMenuCommandExecute, BackToMainMenuCommandCanExecute));

        private bool BackToMainMenuCommandCanExecute() => true;

        private void BackToMainMenuCommandExecute() {
            ShowMainMenuMessage.Send(MessengerInstance);
        }

        #endregion
    }
}