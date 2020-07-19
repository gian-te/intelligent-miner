using IntelligentMiner.WPF.Game;
using IntelligentMiner.Common.Utilities;
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
                MovesRandomly = true,
                MovesIntelligently = false,
                _pits = "2,2\r\n2,1",
                _beacons = "1,1",
                Gold = "1,2",
                Size = 3 
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

                if (options.MovesRandomly)
                {
                    game.PlayRandom();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void rdoRandomInit_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> _pits = null;
                List<string> _beacons = null;
                List<int> _beaconValues = null;
                Tuple<int, int> goldenSquare = null;

                (goldenSquare, _beacons, _beaconValues, _pits) = GenerateRandomInit(_viewModel.Size);

                //txtGoldenSquare.Text = String.Concat(goldenSquare.Item1, ",", goldenSquare.Item2);
                
                // let the binding do the work
                _viewModel.Gold = String.Concat(goldenSquare.Item1, ",", goldenSquare.Item2); 

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //Still has bugs
        public (Tuple<int, int>, List<string>, List<int>, List<string>) GenerateRandomInit(int gridSize)
        {
            int row = 0;
            int col = 0;
            int i = 0;

            //20% Pits & Beacons
            double numberOfPitsandBeacons = Math.Round((gridSize * gridSize) * 0.20);
            List<string> _beacons = new List<string>();
            List<int> _beaconValues = new List<int>();
            List<string> _pits = new List<string>();

            //Create Golden Square
            while (row == 0 && col == 0)
            {
                row = Randomizer.RandomizeNumber(0, gridSize);
                col = Randomizer.RandomizeNumber(0, gridSize);
            }

            //Map[row, col] = new GoldenSquare();
            Tuple<int, int> _goldensquare = new Tuple<int, int>(row, col);

            //Create Beacons
            while (i < numberOfPitsandBeacons)
            {
                int beaconValue;
                int chooseAlignment = Randomizer.RandomizeNumber(0, 2);

                //0 Create beacon in row of golden square
                if (chooseAlignment == 0)
                {
                    row = _goldensquare.Item1;
                    col = Randomizer.RandomizeNumber(0, gridSize);
                }
                //Create beacon in column of golden square
                else
                {
                    row = Randomizer.RandomizeNumber(0, gridSize);
                    col = _goldensquare.Item2;

                }

                string coordinates = String.Concat(row, ',', col);
                if (chooseAlignment == 0) { beaconValue = Math.Abs(_goldensquare.Item1 - row); }
                else { beaconValue = Math.Abs(_goldensquare.Item1 - col); }

                if (
                        (row != 0 && col != 0) //Check if not in player intial position
                    && (row != _goldensquare.Item1 && col != _goldensquare.Item2) //Check if not in Golden Square
                    && (!_beacons.Contains(coordinates)) //Not in list of Beacons
                   )
                {
                    //Map[row, col] = beacon;
                    _beacons.Add(coordinates);
                    _beaconValues.Add(beaconValue);
                    i++;
                }
            }

            i = 0;

            //Create Pits - for polishing
            while (i < numberOfPitsandBeacons)
            {

                row = Randomizer.RandomizeNumber(0, gridSize);
                col = Randomizer.RandomizeNumber(0, gridSize);
                string coordinates = String.Concat(row, ',', col);

                if (
                        (row != 0 && col != 0) //Check if not in player intial position
                    && (row != _goldensquare.Item1 && col != _goldensquare.Item2) //Check if not in Golden Square
                    && (!_beacons.Contains(coordinates)) //Not in list of Beacons
                    && (!_pits.Contains(coordinates)) //Not in list of Beacons
                   )
                {
                    //Map[row, col] = new Pit();
                    _pits.Add(coordinates);
                    i++;
                }
            }

            return (_goldensquare, _beacons, _beaconValues, _pits);

        }

    }


}
