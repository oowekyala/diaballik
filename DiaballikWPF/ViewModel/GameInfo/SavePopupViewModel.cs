using System;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class SavePopupViewModel : ViewModelBase {
        public string Message { get; }
        public Action Action { get; }
        public bool HasCancelButton { get; }


        public SavePopupViewModel(string message, Action toConfirm, bool hasCancelButton) {
            Message = message;
            Action = toConfirm;
            HasCancelButton = hasCancelButton;
        }
    }
}