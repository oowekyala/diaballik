using System.Windows;
using System.Windows.Navigation;
using DiaballikWPF.View;
using DiaballikWPF.ViewModel;
using GalaSoft.MvvmLight.Threading;

namespace DiaballikWPF {
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            // Setup Quick Converter.
            // Add the System namespace so we can use primitive types (i.e. int, etc.).
            QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
            // Add the System.Windows namespace so we can use Visibility.Collapsed, etc.
            QuickConverter.EquationTokenizer.AddNamespace(typeof(System.Windows.Visibility));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(System.Windows.Media.SolidColorBrush));

            // Add our conversion extensions
            QuickConverter.EquationTokenizer.AddExtensionMethods(typeof(Converters.ConvertUtil));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Converters.ConvertUtil));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(ViewMode));
            DispatcherHelper.Initialize();
        }


        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

//            var dockView = new DockWindow();
//            var dock = new DockWindowViewModel(dockView);
//            dockView.DataContext = dock;

//            var mainScreen = new StartupScreen();
//            var mainScreenVm = new StartupScreenViewModel(mainScreen, dock);

//            dock.ContentViewModel = mainScreenVm;

            var dockView = new ScreenOverlayWindow();
            var vm = new OverlayWindowViewModel(dockView);
            dockView.DataContext = vm;

            dockView.Show();
        }
    }
}