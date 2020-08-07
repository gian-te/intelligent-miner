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
using System.Media;

namespace IntelligentMiner.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        GameOptions _viewModel;
        private static readonly object syncLock = new object();
        public MainWindow()
        {
            InitializeComponent();
            // MVVM pattern
            _viewModel = new GameOptions()
            {
                MovesRandomly = false,
                MovesIntelligently = true,
                pits = "2,1\r\n6,0\r\n0,7\r\n6,7\r\n7,4\r\n7,1\r\n1,3\r\n2,5\r\n2,4\r\n5,5\r\n4,3\r\n1,6\r\n3,7",
                beacons = "0,4 = 5\r\n5,0 = 4\r\n6,4=1",
                Gold = "5,4",
                Size = 8,
                ClearInit = true
            };
            this.DataContext = _viewModel;
            PlaySound();
        }

        private void PlaySound()
        {
           Task.Run(() =>
           {
               SoundPlayer sound = new SoundPlayer();
               sound.SoundLocation = "Audio\\cw.wav";
               sound.PlayLooping();
           });
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

                if (_pits[_pits.Count - 1] == String.Empty) { _pits.RemoveAt(_pits.Count - 1); }
                if (_beacons[_beacons.Count - 1] == String.Empty) { _beacons.RemoveAt(_beacons.Count - 1); }

                options.Beacons = _beacons;
                options.Pits = _pits;

                GameWindow game = new GameWindow();
                game.LoadGame(options);
                game.Show();
                game.Owner = this;
                this.Hide();
                this.Topmost = false;

                if (options.MovesRandomly)
                {
                    game.PlayRandomly();
                }
                else if (options.MovesIntelligently)
                {
                    game.PlayIntelligently();
                }
            }
            catch (Exception ex)
            {
                // swallow
                MessageBox.Show("There were some errors in parsing the coordinates", "Error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

            }

        }

        private void rdoRandomInit_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Genenrate values only
                _viewModel.Size = _viewModel.Size < 2 ? Randomizer.RandomizeNumber(2, 15) : _viewModel.Size;
                GenerateRandomInit();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void rdoManualInit_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.pits = "2,1\r\n6,0\r\n0,7\r\n6,7\r\n7,4\r\n7,1\r\n1,3\r\n2,5\r\n2,4\r\n5,5\r\n4,3\r\n1,6\r\n3,7";
            _viewModel.beacons = "0,4 = 5\r\n5,0 = 4\r\n6,4=1";
            _viewModel.Gold = "5,4";
            _viewModel.Size = 8;
        }

        private void rdoClearInit_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.pits = String.Empty;
            _viewModel.beacons = String.Empty;
            _viewModel.Gold = String.Empty;
            _viewModel.Size = 0;
            txtGridSize.Focus();
        }

        public void GenerateRandomInit()
        {
            int gridSize = _viewModel.Size;
            _viewModel.pits = String.Empty;
            _viewModel.beacons = String.Empty;

            int row = 0;
            int col = 0;
            //int i = 0;

            //20% Pits &  5% Beacons
            double numberOfBeacons = Math.Round((gridSize * gridSize) * 0.05);
            double numberOfPits = Math.Round((gridSize * gridSize) * 0.20);
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

            Parallel.Invoke(
                        () =>
                        {
                            //Create Beacons
                            for (int j = 0; j < numberOfBeacons; j++)
                            {
                                Thread beaconThread = new Thread(() => createBeaconCoords(gridSize, _goldensquare, existingCoordinates));
                                beaconThread.IsBackground = true;
                                beaconThread.Start();
                            }

                        },
                        () =>
                        {
                            //Create Pits 
                            for (int j = 0; j < numberOfPits; j++)
                            {
                                Thread pitThread = new Thread(() => createPitCoords(gridSize, existingCoordinates));
                                pitThread.IsBackground = true;
                                pitThread.Start();
                            }
                        }
                        );
        }

        private void createPitCoords(int gridSize, List<Tuple<int, int>> existingCoordinates)
        {
            bool done = false;

            while (!done)
            {
                int row = Randomizer.RandomizeNumber(0, gridSize);
                int col = Randomizer.RandomizeNumber(0, gridSize);
                Tuple<int, int> coord = new Tuple<int, int>(row, col);

                lock (syncLock)
                {

                    if (!existingCoordinates.Contains(coord))
                    {
                        _viewModel.pits += (String.Concat(row, ',', col) + "\r\n");
                        existingCoordinates.Add(coord);
                        done = true;
                    }

                }
            }

        }

        private void createBeaconCoords(int gridSize, Tuple<int, int> _goldensquare, List<Tuple<int, int>> existingCoordinates)
        {
            bool done = false;

            while (!done)
            {

                int beaconValue;
                int chooseAlignment = Randomizer.RandomizeNumber(0, 2);
                int row = 0;
                int col = 0;

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

                Tuple<int, int> coord = new Tuple<int, int>(row, col);

                lock (syncLock)
                {

                    if (!existingCoordinates.Contains(coord))
                    {
                        if (chooseAlignment == 0) { beaconValue = Math.Abs(_goldensquare.Item2 - col); }
                        else { beaconValue = Math.Abs(_goldensquare.Item1 - row); }
                        _viewModel.beacons += (String.Concat(row, ',', col) + "=" + beaconValue.ToString() + "\r\n");
                        existingCoordinates.Add(coord);
                        done = true;
                    }

                }
            }

        }

    }


}
