using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    public class SavePopupViewModel : ViewModelBase {
        public string Message { get; }
        public bool HasCancelButton { get; }

        private HashSet<Action> hardReferences = new HashSet<Action>();

        public SavePopupViewModel(string message,
                                  Action toConfirm,
                                  bool hasCancelButton) {
            Message = message;
            HasCancelButton = hasCancelButton;
            hardReferences.Add(toConfirm);

            // we have to keep a hard reference to the action passed to Register,
            // otherwise it's garbage collected (because it uses closures).
            // see https://stackoverflow.com/questions/25730530/bug-in-weakaction-in-case-of-closure-action

            Action saveCommandExecute = () => {
                RequestSaveToGameScreenMessage.Send(MessengerInstance);
                toConfirm();
                CloseSavePopupMessage.Send(MessengerInstance);
            };

            hardReferences.Add(saveCommandExecute);

            SaveCommand = new RelayCommand(saveCommandExecute);

            Action confirmCommandExecute = () => {
                toConfirm();
                CloseSavePopupMessage.Send(MessengerInstance);
            };

            hardReferences.Add(confirmCommandExecute);

            ConfirmCommand = new RelayCommand(confirmCommandExecute);
            CancelCommand = new RelayCommand(() => CloseSavePopupMessage.Send(MessengerInstance));
        }


        public RelayCommand SaveCommand { get; }
        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }
    }
}