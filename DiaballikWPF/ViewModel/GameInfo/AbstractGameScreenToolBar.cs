using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    public abstract class AbstractGameScreenToolBar : ViewModelBase {
        protected AbstractGameScreenToolBar() {
            BackToMainMenuCommand = new RelayCommand(() => ShowMainMenuMessage.Send(MessengerInstance));
        }


        public RelayCommand BackToMainMenuCommand { get; }
    }
}