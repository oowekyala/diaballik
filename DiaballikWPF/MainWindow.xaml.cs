using System.Windows;
using System.Windows.Navigation;

namespace DiaballikWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NavigationWindow nw = new NavigationWindow();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            nw.Navigate(new System.Uri("GameConfiguration.xaml", System.UriKind.Relative));
        }

        private void ReplayGame_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
