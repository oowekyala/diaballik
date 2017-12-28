using System.Windows.Controls;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Main window of the app, in which the various screens dock.
    /// </summary>
    public class DockWindowViewModel : ViewModelBase {
        #region Constructors

        public DockWindowViewModel(DockWindow view) {
            View = view;
            View.DataContext = this;
        }

        #endregion

        #region Properties

        public DockWindow View { get; }

        private ViewModelBase _contentViewModel;

        public ViewModelBase ContentViewModel {
            get => _contentViewModel;
            set => Set(ref _contentViewModel, value);
        }

        #endregion
    }
}