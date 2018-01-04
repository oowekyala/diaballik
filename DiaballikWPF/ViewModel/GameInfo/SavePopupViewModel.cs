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