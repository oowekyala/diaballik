using DiaballikWPF.View;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Window in which only one screen is ever visible at a time.
    /// </summary>
    public class DockWindowViewModel : AbstractMainWindowViewModel {
        #region Constructors

        public DockWindowViewModel(DockWindow view) : base(view) {
            View = view;
        }

        #endregion

        #region Properties

        protected override void SwitchToGameCreationScreen() {
            ContentViewModel = GameCreationScreenViewModel;
        }

        protected override void SwitchToGameScreen() {
            ContentViewModel = GameScreenViewModel;
        }

        protected override void SwitchToGameLoadScreen() {
            ContentViewModel = LoadGameScreenViewModel;
        }

        protected override void SwitchToStartupScreen() {
            ContentViewModel = StartupScreenViewModel;
        }

        protected override bool IsOnGameScreen() {
            return ContentViewModel == GameScreenViewModel;
        }

        public new DockWindow View { get; }

        private ViewModelBase _contentViewModel;

        public ViewModelBase ContentViewModel {
            get => _contentViewModel;
            set => Set(ref _contentViewModel, value);
        }

        #endregion
    }
}