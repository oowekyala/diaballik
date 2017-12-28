using System.Windows;
using System.Windows.Navigation;
using DiaballikWPF.View;
using DiaballikWPF.ViewModel;

namespace DiaballikWPF {
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var dockView = new DockWindow();
            var dock = new DockWindowViewModel(dockView);
            dockView.DataContext = dock;

            var mainScreen = new StartupScreen();
            var mainScreenVm = new StartupScreenViewModel(mainScreen, dock);

            dock.ContentViewModel = mainScreenVm;

            dockView.Show();
        }
    }
}