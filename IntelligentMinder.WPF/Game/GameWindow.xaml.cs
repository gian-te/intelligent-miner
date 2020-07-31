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
using System.Windows.Threading;

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
        public int stepDelay { get; set; }

        public GameWindow()
        {
            InitializeComponent();
            DataContext = this;
            stepDelay = 100; //50 millisec
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

            Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();

            
            RefreshGrid();
        }

        private void ThreadStartingPoint()
        {
            dashboard = new Dashboard(player);
            dashboard.Show();
            System.Windows.Threading.Dispatcher.Run();
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
                bool changeTarget = true;
                bool stillRotate = true;
                List<(int, int, Direction)> genTargets = new List<(int, int, Direction)>();
                while (!end)
                {

                    while (!dashboard.pauseStatus())
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
                            if (!player.PositionHistory.Any(item => item.Item1 == player.Position.Row && item.Item2 == player.Position.Column))
                            {
                                // discover the map
                                //Rotate and Scan to East
                                player.Facing = Direction.East;
                                action = ActionType.Rotate;
                                dashboard.UpdateDashboard(player, action); // update rotate count
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(stepDelay);
                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update scan count
                                Thread.Sleep(stepDelay);

                                //Rotate and Scan to South
                                player.Facing = Direction.South;
                                action = ActionType.Rotate;
                                dashboard.UpdateDashboard(player, action); // update rotate count
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(stepDelay);
                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update scan count

                                //Rotate and Scan to West
                                player.Facing = Direction.West;
                                action = ActionType.Rotate;
                                dashboard.UpdateDashboard(player, action); // update rotate count
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(stepDelay);
                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update scan count

                                //Rotate and Scan to North
                                player.Facing = Direction.North;
                                action = ActionType.Rotate;
                                dashboard.UpdateDashboard(player, action); // update rotate count
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(stepDelay);
                                (cell, node, prio) = player.Discover(game);
                                if (prio > 0) { priorityChildren.Add((node, prio)); }
                                prio = 0;
                                action = ActionType.Scan;
                                dashboard.UpdateDashboard(player, action, cell.CellItemType); // update scan count

                                //Sort list based on ascending (greater value is more priority)
                                priorityChildren.Sort((pair1, pair2) => pair1.Item2.CompareTo(pair2.Item2));
                                foreach (var item in priorityChildren)
                                {
                                    game.CurrentNode.Children.Push(item.Item1);
                                }

                                if (game.CurrentNode.Children.Count > 0)
                                {
                                    // rotate here
                                    var priorityCell = game.CurrentNode.Children.Peek();
                                    var cellinFront = player.ScanForward(game);
                                    while (priorityCell.CellItemType != CellItemType.Wall && priorityCell.CellItemType != CellItemType.Empty && cellinFront.Position.Row != priorityCell.Position.Row && cellinFront.Position.Column != priorityCell.Position.Column)
                                    {
                                        player.Rotate();
                                        dashboard.UpdateDashboard(player, ActionType.Rotate); // update rotate
                                        Thread.Sleep(stepDelay);
                                        cellinFront = player.ScanForward(game);
                                        dashboard.UpdateDashboard(player, ActionType.Rotate); // update csan
                                        this.Dispatcher.Invoke( () => RefreshGrid());
                                        Thread.Sleep(stepDelay);
                                    }
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

                                    game.BeaconMemo.Add((player.Position.Row, player.Position.Column), beaconRoot);
                                    genTargets = player.GenerateTargetGrids(game);
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
                            Thread.Sleep(stepDelay);

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

                                BaseCellItem cell = new BaseCellItem();
                                //Scan and rotate 4 times surroudings but prioritize direction
                                if (stillRotate)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        action = ActionType.Rotate;
                                        player.Rotate();
                                        dashboard.UpdateDashboard(player, action);
                                        this.Dispatcher.Invoke(() => RefreshGrid());

                                        if (changeTarget)
                                        {
                                            player.DiscoverUsingBeacon(game, cell, priorityChildren, genTargets);
                                        }
                                        else
                                        {
                                            changeTarget = player.DiscoverUsingBeacon(game, cell, priorityChildren, genTargets);
                                        }
                                        action = ActionType.Scan;
                                        dashboard.UpdateDashboard(player, action, cell.CellItemType);
                                        Thread.Sleep(stepDelay);
                                    }
                                }
                                else
                                {
                                    changeTarget = player.DiscoverUsingBeacon(game, cell, priorityChildren, genTargets);
                                }

                                //player.Symbol = "M";
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(stepDelay);

                                //if (changeTarget && !priorityChildren.Any(item=>item.Item1.CellItemType == CellItemType.GoldenSquare)
                                //&& !priorityChildren.Any(item => item.Item1.CellItemType == CellItemType.Beacon))
                                //{ continue; }

                                //Sort list based on ascending (greater value is more priority)
                                priorityChildren.Sort((pair1, pair2) => pair1.Item2.CompareTo(pair2.Item2));

                                //Commented parts are for checking

                                //=========================================
                                //string priorities = String.Format("Current target: {0},{1}:{2}{3}",
                                //        player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2,
                                //        player.currentBeaconTarget.Item3.ToString(), Environment.NewLine);

                                foreach (var item in priorityChildren)
                                {
                                    game.CurrentNode.Children.Push(item.Item1);
                                    //priorities += String.Format("{0},{1}:{2}{3}",
                                    //item.Item1.Position.Row, item.Item1.Position.Column, item.Item2, Environment.NewLine);
                                }

                                //if (priorityChildren.Count > 0)
                                //{ MessageBox.Show(priorities); }
                                //=========================================

                                if (game.CurrentNode.Children.Count > 0)
                                {
                                    // rotate here
                                    var priorityCell = game.CurrentNode.Children.Peek();
                                    var cellinFront = player.ScanForward(game);
                                    while (priorityCell != null && cellinFront.Position.Row != priorityCell.Position.Row && cellinFront.Position.Column != priorityCell.Position.Column)
                                    {
                                        player.Rotate();
                                        cellinFront = player.ScanForward(game);
                                        dashboard.UpdateDashboard(player, ActionType.Rotate); // update move
                                        this.Dispatcher.Invoke(() => RefreshGrid());
                                    }
                                }

                                // move the player to the popped element at the top of the fringe
                                CellItemType t = new CellItemType();
                                try
                                {
                                    Beacon beacon = new Beacon();
                                    (t, beacon) = player.MoveWithStrategy(game);
                                    action = ActionType.Move;

                                    try
                                    {
                                        if (t == CellItemType.Beacon)
                                        {
                                            List<(int, int, Direction)> anotherBeaconTargets = player.RegenTargetGrids(game, beacon);
                                            for (int i = 0; i < genTargets.Count; i++)
                                            {
                                                foreach (var anotherBeacon in anotherBeaconTargets)
                                                {
                                                    if (genTargets[i].Item1 == anotherBeacon.Item1 && genTargets[i].Item2 == anotherBeacon.Item2)
                                                    {
                                                        player.currentBeaconTarget = new Tuple<int, int, Direction>(genTargets[i].Item1, genTargets[i].Item2, genTargets[i].Item3);
                                                        genTargets.RemoveAt(i);
                                                        stillRotate = false;
                                                        //MessageBox.Show(String.Format("Rushing to the cell: {0},{1}", player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2), "Recalculating...", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

                                                        //MessageBox.Show(String.Concat(player.currentBeaconTarget.Item1, ",", player.currentBeaconTarget.Item2));
                                                        //MessageBox.Show(String.Format("Changed target to: {0},{1}", player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2), "Recalculating...", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                                                        break;
                                                    }
                                                    else if (player.currentBeaconTarget.Item1 == anotherBeacon.Item1 && player.currentBeaconTarget.Item2 == anotherBeacon.Item2)
                                                    {
                                                        stillRotate = false;
                                                        //MessageBox.Show(String.Format("Rushing to the cell: {0},{1}", player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2), "Recalculating...", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                                                        break;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                }
                                catch (Exception)
                                {
                                    end = true;
                                    action = ActionType.NoPossible;
                                    t = CellItemType.Empty;
                                }

                                if (t == CellItemType.GoldenSquare)
                                {
                                    end = true;
                                    action = ActionType.Win;
                                }

                                dashboard.UpdateDashboard(player, action); // update move
                                this.Dispatcher.Invoke(() => RefreshGrid());
                                Thread.Sleep(stepDelay);

                            }
                            //Set a beacon target for the robot to lean towards a certain direction
                            else
                            {
                                int row = 0, col = 0, index = 0;

                                if (genTargets.Count > 1)
                                {
                                    // randomize target to choose
                                    index = Randomizer.RandomizeNumber(0, genTargets.Count);

                                    row = genTargets[index].Item1;
                                    col = genTargets[index].Item2;
                                    player.currentBeaconTarget = new Tuple<int, int, Direction>(row, col, genTargets[index].Item3);
                                    genTargets.RemoveAt(index);
                                    MessageBox.Show(String.Format("Changed target to: {0},{1}", player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2), "Recalculating...", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                                }
                                else if (genTargets.Count == 1)
                                {
                                    row = genTargets[0].Item1;
                                    col = genTargets[0].Item2;
                                    player.currentBeaconTarget = new Tuple<int, int, Direction>(row, col, genTargets[0].Item3);
                                    genTargets.RemoveAt(index);
                                    MessageBox.Show(String.Format("Changed target to: {0},{1}", player.currentBeaconTarget.Item1, player.currentBeaconTarget.Item2), "Recalculating...", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                                }
                                //else
                                //{
                                //    dashboard.UpdateDashboard(player, ActionType.NoPossible); // update move
                                //    this.Dispatcher.Invoke(() => RefreshGrid());
                                //    end = true;
                                //}

                                changeTarget = false;

                            }

                        }

                        if (end) { break; }
                    }

                }
            });
        }

        public void PlayRandomly()
        {
            Task.Run(() =>
            {
                bool end = false;
                while (!end)
                {
                    // 1. randomize move between Rotate or Move
                    ActionType action = player.RandomizeAction();

                    if (action == ActionType.RotateRandom)
                    {
                        // 2. if Rotate, randomize how many times it will rotate 90-degrees

                        // arbitrary range of 1 to 10
                        player.Rotate();
                        dashboard.UpdateDashboard(player, ActionType.Rotate); // update move

                        this.Dispatcher.Invoke(() => RefreshGrid());
                        Thread.Sleep(stepDelay);

                    }
                    else if (action == ActionType.MoveRandom)
                    {
                        // 3. if Move, randomize how many times it will move
                        //var cell = player.MoveForward(game, true, gameSpeed);

                        var cell = player.ScanForward(game);
                        if (cell.CellItemType == CellItemType.Wall)
                        {
                            dashboard.UpdateDashboard(player, action, CellItemType.Wall);
                        }
                        else
                        {
                            // remove the player from its current cell
                            game.ClearCell(player.Position.Row, player.Position.Column);

                            // assign new coordinates to the player
                            player.Position.Row = cell.Position.Row;
                            player.Position.Column = cell.Position.Column;

                            game.AssignPlayerToCell(player);

                            var newCoordinates = new Tuple<int, int>(player.Position.Row, player.Position.Column);

                            if (player.PositionHistory.Contains(newCoordinates)) { player.Metrics.backtrackCount++; }
                            player.PositionHistory.Add(newCoordinates);

                            if (cell.CellItemType == Common.Enums.CellItemType.GoldenSquare)
                            {
                                end = true;
                                action = ActionType.Win;
                                dashboard.UpdateDashboard(player, action); // update move
                            }
                            else if (cell.CellItemType == Common.Enums.CellItemType.Pit)
                            {
                                end = true;
                                action = ActionType.Die;
                                dashboard.UpdateDashboard(player, action);
                            }
                            else
                            {
                                action = ActionType.Move;
                                dashboard.UpdateDashboard(player, action);
                            }
                        }

                        this.Dispatcher.Invoke(() => RefreshGrid());
                        Thread.Sleep(stepDelay);
                    }

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
            dashboard.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(dashboard.Close));
            Owner.Show();
            Owner.Topmost = true;
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
