using IntelligentMiner.WPF.Game;
using IntelligentMiner.WPF.Main;
using IntelligentMiner.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Data;
using System.Threading;
using IntelligentMiner.Common.Utilities;
using IntelligentMiner.Common.Enums;
using IntelligentMiner.Common.Entities;

namespace IntelligentMiner.WPF.Game
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        IntelligentMiner.Common.Game game;
        Player player;
        Dashboard dashboard;
        public GameWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private DataView gridData;
        public DataView GridData { get { return gridData; } set { gridData = value; NotifyPropertyChanged("GridData"); } }

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        public void LoadGame(GameOptions gameOptions)
        {
            game = new Common.Game(gameOptions.Size);
            player = new Player()
            {
                Position = new Position()
                {
                    Row = 0,
                    Column = 0
                }
            };
            game.Map[player.Position.Row, player.Position.Column] = player;

            //Add objects to map
            var goldCoordinates = gameOptions.Gold.Split(',');
            game.AddGold(int.Parse(goldCoordinates[0]), int.Parse(goldCoordinates[1]));

            Parallel.Invoke(
                () =>
                {
                    foreach (var item in gameOptions.Pits)
                    {
                        var pitCoordinates = item.Split(',');
                        game.AddPit(int.Parse(pitCoordinates[0]), int.Parse(pitCoordinates[1]));
                    }
                },
                () =>
                {
                    foreach (var item in gameOptions.Beacons)
                    {
                        var beaconCoordinates = item.Split(',', '=');
                        game.AddBeacon(int.Parse(beaconCoordinates[0]), int.Parse(beaconCoordinates[1]), int.Parse(beaconCoordinates[2]));
                    }
                }
            );

            dashboard = new Dashboard(player);
            dashboard.Show();
            
            RefreshGrid();
        }

        public void PlayIntelligently()
        {
            Task.Run(() =>
            {
                // create initial node
                Node root = new Node();
                root.Position.Row = player.Position.Row;
                root.Position.Column = player.Position.Column;
                root.CellItemType = player.CellItemType;

                // add the root node to the search space
                game.SearchSpace = root;

                // add the node object to the dictionary to prevent duplicate objects per cell.
                game.NodeMemo.Add((root.Position.Row, root.Position.Column), root);
                game.CurrentNode = root;

                // player to discover the rest of the nodes
                bool end = false;
                while (!end)
                {
                    // discover the map
                    player.Facing = Direction.East;
                    player.Discover(game);

                    player.Facing = Direction.South;
                    player.Discover(game);

                    player.Facing = Direction.North;
                    player.Discover(game);

                    player.Facing = Direction.West;
                    player.Discover(game);

                    // move the player to the popped element at the top of the fringe
                    var t = player.MoveWithStrategy(game);
                    if (t == CellItemType.GoldenSquare)
                    {
                        end = true;
                        var action = ActionType.Win;
                        dashboard.UpdateDashboard(player, action);
                    }

                    this.Dispatcher.Invoke(() => RefreshGrid());

                }
            });
        }

      

        public void PlayRandom()
        {
            Task.Run(() =>
            {
                bool end = false;
                while (!end)
                {
                    // 1. randomize move between Rotate or Move
                    ActionType action = player.RandomizeAction();

                    if (action == ActionType.Rotate)
                    {
                        // 2. if Rotate, randomize how many times it will rotate 90-degrees
                        player.RotateRandomTimes(game.Size);
                    }
                    else if (action == ActionType.Move)
                    {
                        // 3. if Move, randomize how many times it will move
                        var cell = player.MoveForward(game, true);
                        if (cell.CellItemType == Common.Enums.CellItemType.GoldenSquare)
                        {
                            end = true;
                            dashboard.UpdateDashboard(player, action); // update move
                            action = ActionType.Win;
                        }
                        else if (cell.CellItemType == Common.Enums.CellItemType.Pit)
                        {
                            end = true;
                            dashboard.UpdateDashboard(player, action);
                            action = ActionType.Die;
                        }
                        this.Dispatcher.Invoke(() => RefreshGrid());
                    }

                    dashboard.UpdateDashboard(player, action);
                    //Thread.Sleep(100);


                    //stepCount++;
                    Console.WriteLine();
                }

            });

        }

        private void RefreshGrid()
        {
            c_dataGrid.ItemsSource = null;
            GridData = GetBindable2DArray<BaseCellItem>(game.Map);
            c_dataGrid.ItemsSource = GridData;
        }

        private void c_dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTextColumn column = e.Column as DataGridTextColumn;
            Binding binding = column.Binding as Binding;
            binding.Path = new PropertyPath(binding.Path.Path + ".Value.Symbol");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public DataView GetBindable2DArray<T>(T[,] array)
        {
            // create table
            DataTable dataTable = new DataTable();
            // add empty columns
            for (int i = 0; i < array.GetLength(1); i++)
            {
                dataTable.Columns.Add(i.ToString(), typeof(Ref<T>));
            }
            // add empty rows
            for (int i = 0; i < array.GetLength(0); i++)
            {
                DataRow dataRow = dataTable.NewRow();
                dataTable.Rows.Add(dataRow);
            }
            // parse to dataview
            DataView dataView = new DataView(dataTable);
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    int a = i;
                    int b = j;
                    Ref<T> refT = new Ref<T>(() => array[b, a], z => { array[b, a] = z; });
                    // assign cell
                    dataView[i][j] = refT;
                }
            }
            return dataView;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            dashboard.Close();
            Owner.Show();
        }
    }

    public class Ref<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;
        public Ref(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
        public T Value { get { return getter(); } set { setter(value); } }
    }
}
