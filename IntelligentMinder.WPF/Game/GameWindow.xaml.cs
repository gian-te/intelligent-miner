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
                },
                maxRow = 0,
                maxColumn = 0
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
                int gameSpeed = 100;

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
                bool changeTarget = true;
                List<(int, int, Direction)> genTargets = new List<(int, int, Direction)>();
                while (!end)
                {

                    while (dashboard.pauseStatus())
                    {

                        //No Beacon Stepped Yet
                        if(!player.steppedOnBeacon)
                        {

                            ActionType action;
                            BaseCellItem cell;
                            List<(Node, double)> priorityChildren = new List<(Node, double)>();
                            int prio = 0;
                            Node node = new Node();

                            // if not existing in the map,
                            if (game.NodeMemo[(player.Position.Row, player.Position.Column)] != null)
                            {
                                // discover the map
                                //Rotate and Scan to East
                                player.Facing = Direction.East;
                                action = ActionType.Rotate;
                                player.Symbol = "M(\u2192)";
                                player.Metrics.rotateCount++;
                                dashboard.UpdateDashboard(player, action); // update move
                                this.Dispatcher.Invoke(() => RefreshGrid());

                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update move
                                Thread.Sleep(gameSpeed);

                                //Rotate and Scan to South
                                player.Facing = Direction.South;
                                action = ActionType.Rotate;
                                player.Symbol = "M(\u2193)";
                                player.Metrics.rotateCount++;
                                dashboard.UpdateDashboard(player, action); // update move
                                this.Dispatcher.Invoke(() => RefreshGrid());

                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update move
                                Thread.Sleep(gameSpeed);

                                //Rotate and Scan to North
                                player.Facing = Direction.North;
                                action = ActionType.Rotate;
                                player.Symbol = "M(\u2191)";
                                player.Metrics.rotateCount++;
                                dashboard.UpdateDashboard(player, action); // update move
                                this.Dispatcher.Invoke(() => RefreshGrid());

                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update move
                                Thread.Sleep(gameSpeed);

                                //Rotate and Scan to West
                                player.Facing = Direction.West;
                                action = ActionType.Rotate;
                                player.Symbol = "M(\u2190)";
                                player.Metrics.rotateCount++;
                                dashboard.UpdateDashboard(player, action); // update move
                                this.Dispatcher.Invoke(() => RefreshGrid());

                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update move
                                Thread.Sleep(gameSpeed);

                                player.Symbol = "M";
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(gameSpeed / 2);

                                //Sort list based on ascending (greater value is more priority)
                                priorityChildren.Sort((pair1, pair2) => pair1.Item2.CompareTo(pair2.Item2));
                                foreach (var item in priorityChildren)
                                {
                                    game.CurrentNode.Children.Push(item.Item1);
                                }
                            }
                            else
                            {
                                   // dont discover if current node is already discovered, do no thing in this case
                            }

                            // move the player to the popped element at the top of the fringe
                            CellItemType t = new CellItemType();
                            try
                            {
                                (t, player.beaconValue) = player.MoveWithStrategy(game);
                                action = ActionType.Move;
                                if (t == CellItemType.Beacon) 
                                { 
                                    player.steppedOnBeacon = true;

                                    Node beaconRoot = new Node();
                                    beaconRoot.Position.Row = player.Position.Row;
                                    beaconRoot.Position.Column = player.Position.Column;
                                    beaconRoot.CellItemType = player.CellItemType;

                                    // add the node object to the dictionary to prevent duplicate objects per cell.

                                    game.BeaconMemo.Add((player.Position.Row, player.Position.Column), beaconRoot);
                                    genTargets = player.GenerateTargetGrids(game);
                                    //string prints = "Possible Beacons:\n";
                                    //foreach (var item in genTargets)
                                    //{
                                    //    prints += String.Concat(item.Item1, ",", item.Item2, " : ", item.Item3.ToString(), Environment.NewLine);
                                    //}
                                    //MessageBox.Show(prints);
                                }
                            }
                            catch (Exception)
                            {
                                end = true;
                                action = ActionType.NoPossible;
                                t = CellItemType.Empty;
                                //throw;
                            }

                            if (t == CellItemType.GoldenSquare)
                            {
                                end = true;
                                action = ActionType.Win;
                            }

                            dashboard.UpdateDashboard(player, action); // update move
                            this.Dispatcher.Invoke(() => RefreshGrid());
                            Thread.Sleep(gameSpeed);

                        }
                        //Beacon Stepped
                        else
                        {
                            //Head towards that direction
                            if (!changeTarget)
                            {
                                ActionType action;
                                Direction priorityDirection = player.currentBeaconTarget.Item3;
                                List<(Node, double)> priorityChildren = new List<(Node, double)>();
                                //Dictionary<Direction, Node> priorityChildren = new Dictionary<Direction, Node>();

                                //Scan and rotate 4 times surroudings but prioritize direction
                                for (int i = 0; i < 4; i++)
                                {
                                    action = ActionType.Rotate;
                                    player.Rotate();
                                    dashboard.UpdateDashboard(player, action);
                                    this.Dispatcher.Invoke(() => RefreshGrid());

                                    BaseCellItem cell = new BaseCellItem();
                                    if(changeTarget)
                                    {
                                        player.DiscoverUsingBeacon(game, cell, priorityChildren, genTargets);
                                    }
                                    else
                                    {
                                        changeTarget = player.DiscoverUsingBeacon(game, cell, priorityChildren, genTargets);
                                    }
                                    action = ActionType.Scan;
                                    dashboard.UpdateDashboard(player, action, cell.CellItemType);
                                    Thread.Sleep(gameSpeed);
                                }

                                player.Symbol = "M";
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(gameSpeed / 2);

                                if (changeTarget) {  continue; }

                                //Sort list based on ascending (greater value is more priority)
                                priorityChildren.Sort((pair1, pair2) => pair1.Item2.CompareTo(pair2.Item2));
                                string priorities = String.Format("Current target: {0},{1}:{2}{3}",
                                        player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2,
                                        player.currentBeaconTarget.Item3.ToString(), Environment.NewLine);
                                foreach (var item in priorityChildren)
                                {
                                    game.CurrentNode.Children.Push(item.Item1);
                                    priorities += String.Format("{0},{1}:{2}{3}",
                                    item.Item1.Position.Row, item.Item1.Position.Column, item.Item2, Environment.NewLine);
                                }
                                //if (priorityChildren.Count > 0)
                                //{ MessageBox.Show(priorities); }

                                // move the player to the popped element at the top of the fringe
                                CellItemType t = new CellItemType();
                                try
                                {
                                    Beacon beacon = new Beacon();
                                    (t, beacon) = player.MoveWithStrategy(game);
                                    action = ActionType.Move;
                                    //if (t == CellItemType.Beacon)
                                    //{
                                    //    player.steppedOnBeacon = true;
                                    //    genTargets = player.GenerateTargetGrids(game);
                                    //}
                                }
                                catch (Exception)
                                {
                                    end = true;
                                    action = ActionType.NoPossible;
                                    t = CellItemType.Empty;
                                    //throw;
                                }

                                if (t == CellItemType.GoldenSquare)
                                {
                                    end = true;
                                    action = ActionType.Win;
                                }

                                dashboard.UpdateDashboard(player, action); // update move
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(gameSpeed);

                            }
                            //Set a beacon target for the robot to lean towards a certain direction
                            else
                            {
                                int row = 0, col = 0, index = 0;

                                if (genTargets.Count > 1)
                                {
                                    index = Randomizer.RandomizeNumber(0, genTargets.Count);

                                    row = genTargets[index].Item1;
                                    col = genTargets[index].Item2;
                                    player.currentBeaconTarget = new Tuple<int, int, Direction>(row, col, genTargets[index].Item3);
                                    genTargets.RemoveAt(index);
                                }
                                else if (genTargets.Count == 1)
                                {
                                    row = genTargets[0].Item1;
                                    col = genTargets[0].Item2;
                                    player.currentBeaconTarget = new Tuple<int, int, Direction>(row, col, genTargets[0].Item3);
                                    genTargets.RemoveAt(index);
                                }
                                //else
                                //{
                                //    dashboard.UpdateDashboard(player, ActionType.NoPossible); // update move
                                //    this.Dispatcher.Invoke(() => RefreshGrid());
                                //    end = true;
                                //}

                                MessageBox.Show(String.Format("Change target to: {0},{1}", player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2));
                                changeTarget = false;

                            }

                        }

                        if (end) { break; }
                    }

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
