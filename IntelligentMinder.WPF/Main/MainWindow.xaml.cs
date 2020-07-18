using IntelligentMiner.WPF.Game;
using IntelligentMiner.Common;
using IntelligentMiner.WPF.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace IntelligentMiner.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        GameOptions _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            // MVVM pattern
            _viewModel = new GameOptions()
            {
                IsRandom = true,
                IsIntelligent = false,
                _pits = "2,2\r\n2,1",
                _beacons = "1,1",
                Gold = "1,2",
                Size = 4
            };
            this.DataContext = _viewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate here
                var options = _viewModel;
                string[] stringSeparators = new string[] { "\r\n" };
                List<string> _pits = options._pits.Split(stringSeparators, StringSplitOptions.None).ToList();
                List<string> _beacons = options._beacons.Split(stringSeparators, StringSplitOptions.None).ToList();
                options.Beacons = _pits;
                options.Pits = _beacons;

                GameWindow game = new GameWindow();
                game.LoadGame(options);
                game.Show();
                this.Hide();

                if (options.IsRandom)
                {
                    game.PlayRandom();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }


}
