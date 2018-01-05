using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    public abstract class AbstractGameScreenToolBar : ViewModelBase {
        protected AbstractGameScreenToolBar() {
            BackToMainMenuCommand = new RelayCommand(BackToMainMenuCommandExecute);
        }


        void BackToMainMenuCommandExecute() {
            void BackAction() {
                PauseGameMessage.Send(MessengerInstance); // stops the ai thread
                ShowMainMenuMessage.Send(MessengerInstance);
            }

            if (PrimaryNeedsSaving) {
                const string message = "Would you like to save the current game before exiting?";

                ShowSavePopupMessage.Send(MessengerInstance, (message, BackAction, true));
            } else {
                BackAction();
            }
        }

        public RelayCommand BackToMainMenuCommand { get; }


        private bool _primaryNeedsSaving;

        public bool PrimaryNeedsSaving {
            get => _primaryNeedsSaving;
            set => Set(ref _primaryNeedsSaving, value);
        }
    }
}