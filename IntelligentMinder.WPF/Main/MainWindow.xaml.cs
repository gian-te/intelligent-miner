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
        private static readonly object syncLock = new object();
        public MainWindow()
        {
            InitializeComponent();
            // MVVM pattern
            _viewModel = new GameOptions()
            {
                MovesRandomly = true,
                MovesIntelligently = false,
                pits = "2,2\r\n2,1\r\n",
                beacons = "1,1=1\r\n2,2=1\r\n",
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
                List<string> _pits = options.pits.Split(stringSeparators, StringSplitOptions.None).ToList();
                List<string> _beacons = options.beacons.Split(stringSeparators, StringSplitOptions.None).ToList();
                options.Beacons = _beacons;
                options.Pits = _pits;

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
                // Genenrate values only
                GenerateRandomInit();

            }
            catch (Exception)
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

            //while (i < numberOfPitsandBeacons)
            //{
            //    int beaconValue;
            //    int chooseAlignment = Randomizer.RandomizeNumber(0, 2);

            //    //0 Create beacon in row of golden square
            //    if (chooseAlignment == 0)
            //    {
            //        row = _goldensquare.Item1;
            //        col = Randomizer.RandomizeNumber(0, gridSize);
            //    }
            //    //Create beacon in column of golden square
            //    else
            //    {
            //        row = Randomizer.RandomizeNumber(0, gridSize);
            //        col = _goldensquare.Item2;

            //    }

            //    string coordinates = String.Concat(row, ',', col);
            //    Tuple<int, int> coord = new Tuple<int, int>(row, col);

            //    if (!existingCoordinates.Contains(coord))
            //    {

            //        if (chooseAlignment == 0) { beaconValue = Math.Abs(_goldensquare.Item2 - col); }
            //        else { beaconValue = Math.Abs(_goldensquare.Item1 - row); }
            //        _viewModel.beacons += (coordinates + "=" + beaconValue.ToString() + "\r\n");
            //        existingCoordinates.Add(coord);
            //        i++;
            //    }
            //}

            Task.Run(
                () =>

                
                    Parallel.Invoke(
                        () =>
                        {
                            //Create Beacons
                            for (int j = 0; j < numberOfPitsandBeacons; j++)
                            {
                                Thread beaconThread = new Thread(() => createBeaconCoords(gridSize, _goldensquare, existingCoordinates));
                                beaconThread.IsBackground = true;
                                beaconThread.Start();
                            }

                        },
                        () =>
                        {
                            //Create Pits 
                            for (int j = 0; j < numberOfPitsandBeacons; j++)
                            {
                                Thread pitThread = new Thread(() => createPitCoords(gridSize, existingCoordinates));
                                pitThread.IsBackground = true;
                                pitThread.Start();
                            }
                        }
                        )
                
                );

            //while (i < numberOfPitsandBeacons)
            //{

            //    row = Randomizer.RandomizeNumber(0, gridSize);
            //    col = Randomizer.RandomizeNumber(0, gridSize);
            //    string coordinates = String.Concat(row, ',', col);
            //    Tuple<int, int> coord = new Tuple<int, int>(row, col);

            //    if (!existingCoordinates.Contains(coord))
            //    {
            //        _viewModel.pits += (coordinates + "\r\n");
            //        existingCoordinates.Add(coord);
            //        i++;
            //    }
            //}



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
                int row = Randomizer.RandomizeNumber(0, gridSize);
                int col = Randomizer.RandomizeNumber(0, gridSize);
                Tuple<int, int> coord = new Tuple<int, int>(row, col);

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
