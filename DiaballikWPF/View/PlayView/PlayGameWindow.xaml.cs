using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Diaballik.Core;

namespace DiaballikWPF
{
    /// <summary>
    /// Logique d'interaction pour PlayGameWindow.xaml
    /// </summary>
    public partial class PlayGameWindow : Window
    {
        GameBoard gb;
        public PlayGameWindow()
        {
            InitializeComponent();
            board.Rows = gb.BoardSize;
            board.Columns = gb.BoardSize;
        }
    }
}
