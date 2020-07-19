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
                pits = "2,2\r\n2,1\r\n",
                beacons = "1,1\r\n",
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
                List<string> _pits = options.pits.Split(stringSeparators, StringSplitOptions.None).ToList();
                List<string> _beacons = options.beacons.Split(stringSeparators, StringSplitOptions.None).ToList();
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
                List<int> _beaconValues = null;
                //Tuple<int, int> goldenSquare = null;

                // let the binding do the work
                GenerateRandomInit();

                //txtGoldenSquare.Text = String.Concat(goldenSquare.Item1, ",", goldenSquare.Item2);
                
                //_viewModel.Gold = String.Concat(goldenSquare.Item1, ",", goldenSquare.Item2); 

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //Still has bugs
        public void GenerateRandomInit()
        {
            int gridSize = _viewModel.Size;
            _viewModel.pits = String.Empty;
            _viewModel.beacons = String.Empty;

            int row = 0;
            int col = 0;
            int i = 0;

            //20% Pits & Beacons
            double numberOfPitsandBeacons = Math.Round((gridSize * gridSize) * 0.20);
            //List<string> _beacons = new List<string>();
            //List<int> _beaconValues = new List<int>();
            //List<string> _pits = new List<string>();
            List<Tuple<int, int>> existingCoordinates = new List<Tuple<int, int>>();

            existingCoordinates.Add(new Tuple<int, int>(0, 0));

            //Create Golden Square
            while (row == 0 && col == 0)
            {
                row = Randomizer.RandomizeNumber(0, gridSize);
                col = Randomizer.RandomizeNumber(0, gridSize);
            }

            Tuple<int, int> _goldensquare = new Tuple<int, int>(row, col);
            _viewModel.Gold = string.Concat(_goldensquare.Item1, ',', _goldensquare.Item2);
            existingCoordinates.Add(new Tuple<int, int>(row, col));

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
                Tuple<int, int> coord = new Tuple<int, int>(row, col);

                if (!existingCoordinates.Contains(coord))
                {

                    if (chooseAlignment == 0) { beaconValue = Math.Abs(_goldensquare.Item1 - row); }
                    else { beaconValue = Math.Abs(_goldensquare.Item1 - col); }
                    _viewModel.beacons += (coordinates + "=" + beaconValue.ToString() + "\r\n");
                    //_beaconValues.Add(beaconValue);
                    existingCoordinates.Add(coord);
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
                Tuple<int, int> coord = new Tuple<int, int>(row, col);

                if (!existingCoordinates.Contains(coord))
                {
                    _viewModel.pits += (coordinates + "\r\n");
                    existingCoordinates.Add(coord);
                    i++;
                }
            }

        }

    }


}
