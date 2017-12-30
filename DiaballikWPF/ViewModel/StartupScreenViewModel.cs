using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DiaballikWPF.ViewModel {
    public class StartupScreenViewModel : ViewModelBase {
        #region Fields

        private readonly StartupScreen _view;
        private readonly DockWindowViewModel _dock;

        #endregion

        #region Constructors

        public StartupScreenViewModel(StartupScreen view, DockWindowViewModel dock) {
            _view = view;
            _dock = dock;
        }

        #endregion

        #region Commands

        private RelayCommand _newGameCommand;

        public RelayCommand NewGameCommand => _newGameCommand ?? (_newGameCommand = new RelayCommand(NewGame));

        #endregion

        #region Methods

        private void NewGame() {
            var vm = new GameCreationScreenViewModel(_dock);
            var view = new GameCreationScreen {
                DataContext = vm
            };

            _dock.ContentViewModel = vm;
        }

        #endregion
    }
}