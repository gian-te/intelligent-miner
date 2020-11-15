using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using IntelligentMiner.Common.Entities;
using IntelligentMiner.Common.Enums;
using IntelligentMiner.Common.Utilities;

namespace IntelligentMiner.Common
{


    public class Player : BaseCellItem
    {

        public List<Tuple<int, int>> PositionHistory { get; set; }

        private Direction _facing;
        public Direction Facing
        {
            get { return _facing; }
            set
            {
                if (value == Direction.East)
                {
                    _facing = value;
                    Symbol = "\u2133(\u2192)";
                }
                else if (value == Direction.South)
                {
                    _facing = value;
                    Symbol = "\u2133(\u2193)";
                }
                else if (value == Direction.West)
                {
                    _facing = value;
                    Symbol = "\u2133(\u2190)";
                }
                else if (value == Direction.North)
                {
                    _facing = value;
                    Symbol = "\u2133(\u2191)";
                }

                Metrics.rotateCount++;
            }
        }

        public bool steppedOnBeacon { get; set; }
        
        public bool steppedOnSecond { get; set; }

        public Beacon beaconValue { get; set; }

        public int maxRow { get; set; }

        public int maxColumn { get; set; }

        public Tuple<int, int, Direction> currentBeaconTarget { get; set; }

        public PlayerMetrics Metrics { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            Symbol = "M";
            PositionHistory = new List<Tuple<int, int>>();
            CellItemType = CellItemType.Player;
            Metrics = new PlayerMetrics();
            steppedOnSecond = false;
            Metrics.gameSpeed = 50;
            //RandomizeFacing();
        }

        public Tuple<int, int> Rotate()
        {
            Tuple<int, int> cellInFront = null;

            var initialDirection = Facing.ToString();
            if (Facing == Direction.North)
            {
                Facing = Direction.East;
                cellInFront = new Tuple<int, int>(Position.Row + 1, Position.Column);
            }
            else if (Facing == Direction.East)
            {
                Facing = Direction.South;
                cellInFront = new Tuple<int, int>(Position.Row, Position.Column - 1);

            }
            else if (Facing == Direction.South)
            {
                Facing = Direction.West;
                cellInFront = new Tuple<int, int>(Position.Row - 1, Position.Column);
            }
            else if (Facing == Direction.West)
            {
                Facing = Direction.North;
                cellInFront = new Tuple<int, int>(Position.Row, Position.Column + 1);
            }

            Console.WriteLine("Rotated from {0} to {1}", initialDirection, Facing.ToString());
            // return the cell which the player is facing after rotating 90 degrees
            return cellInFront;
        }

        public (CellItemType, Beacon) MoveWithStrategy(Game game)
        {
            CellItemType retVal;
            Beacon beaconValue = new Beacon();
            var poppedNode = new Node();
            if (game.CurrentNode.Children.Count > 0)
            {
                poppedNode = game.CurrentNode.Children.Pop();
                Position.Row = poppedNode.Position.Row;
                Position.Column = poppedNode.Position.Column;
                game.CurrentNode = poppedNode;
                retVal = poppedNode.CellItemType;
            }
            else
            {
                poppedNode = game.CurrentNode.Parent;
                Position.Row = poppedNode.Position.Row;
                Position.Column = poppedNode.Position.Column;
                game.CurrentNode = poppedNode;
                retVal = poppedNode.CellItemType;
                Metrics.backtrackCount++;
            }
            Metrics.moveCount++;

            if (retVal == CellItemType.Beacon)
            {
                BaseCellItem cell = game.Map[poppedNode.Position.Row, poppedNode.Position.Column];
                beaconValue  = cell as Beacon;
                if (beaconValue != null && beaconValue.Value < 1)
                {
                    retVal = CellItemType.Empty;
                    beaconValue = null;
                }
            }
            if (retVal == CellItemType.GoldenSquare)
            {
                // fix facing
                Symbol = "\u2133";
            }

            game.AssignPlayerToCell(this);
            return (retVal, beaconValue);
        }

        public ActionType RandomizeAction()
        {
            var value = Randomizer.RandomizeNumber();

            if (value >= 1 && value <= 50)
            {
                return ActionType.RotateRandom;
            }
            else
            {
                return ActionType.MoveRandom;
            }
        }

        public void RotateRandomTimes(int gameSpeed, int num = 10)
        {
            // arbitrary range of 1 to 10
            var times = Randomizer.RandomizeNumber(1, num);
            Console.WriteLine(string.Format("The player will rotate {0} times!", times));

            for (int i = 0; i < times; i++)
            {
                Rotate();
                Thread.Sleep(gameSpeed);
            }
            
        }

        /// <summary>
        /// get the cell without moving to the cell
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public BaseCellItem ScanForward(Game game)
        {
            Metrics.scanCount++;
            return game.GetCell(Position.Row, Position.Column, Facing, "front");
        }

        public BaseCellItem MoveForward(Game game, bool random, int gameSpeed)
        {
            int times = 1;
            if (random) { times = Randomizer.RandomizeNumber(1, game.Size); }
            Console.WriteLine(string.Format("The player will move to the {0} for {1} time(s)!", Facing.ToString(), times));

            BaseCellItem cell = null;
            for (int i = 0; i < times; i++)
            {
                Thread.Sleep(gameSpeed);
                cell = ScanForward(game);
                //scanCount += 1;
                if (cell.CellItemType == CellItemType.Wall)
                {
                    Console.WriteLine("The player ran into a thick wall and cannot move forward. Aborting the remaining moves, if any.");
                    break;
                }

                // remove the player from its current cell
                game.ClearCell(Position.Row, Position.Column);

                // assign new coordinates to the player
                Position.Row = cell.Position.Row;
                Position.Column = cell.Position.Column;

                game.AssignPlayerToCell(this);

                var newCoordinates = new Tuple<int, int>(Position.Row, Position.Column);

                Console.WriteLine(string.Format("Player moved to coordinates [{0},{1}]", Position.Row, Position.Column));
                if (PositionHistory.Contains(newCoordinates)) { Metrics.backtrackCount++; }
                PositionHistory.Add(newCoordinates);
                //moveCount += 1;
                Metrics.moveCount++;
 
                if (cell.CellItemType == CellItemType.Pit)
                {
                    // die
                    Console.WriteLine("The player died a horrible death.");
                    break;
                }
                else if (cell.CellItemType == CellItemType.GoldenSquare)
                {
                    // win
                    Console.WriteLine("The player has struck gold.");
                    break;
                }
                else if (cell.CellItemType == CellItemType.Beacon)
                {
                    // clue
                    Console.WriteLine("The player has found a beacon.");
                    break;
                }

            }
            return cell;
        }

        public (BaseCellItem, Node, int) Discover(Game game)
        {
            BaseCellItem cell;
            cell = ScanForward(game);
            Node node = new Node();
            int prio = 0;
            if (cell.CellItemType != CellItemType.Wall && cell.CellItemType != CellItemType.Pit)
            {
                if (!game.NodeMemo.ContainsKey((cell.Position.Row, cell.Position.Column)))
                {
                    // create initial node
                    node.Position.Row = cell.Position.Row;
                    node.Position.Column = cell.Position.Column;
                    node.CellItemType = cell.CellItemType;
                    node.Parent = game.CurrentNode;
                    // add the node object to the dictionary to prevent duplicate objects per cell.
                    game.NodeMemo.Add((cell.Position.Row, cell.Position.Column), node);

                    //game.CurrentNode.Children.Push(node);
                    //Determine which node should be prioritized
                    if(cell.CellItemType == CellItemType.GoldenSquare) { prio = 3; }
                    else if (cell.CellItemType == CellItemType.Beacon) { prio = 2; }
                    else if (cell.CellItemType == CellItemType.Empty)  { prio = 1; }
                }
            }
            else if (cell.CellItemType == CellItemType.Pit)
            {
                game.PitMemo.Add((cell.Position.Row, cell.Position.Column));
            }
            else if (cell.CellItemType == CellItemType.Wall)
            {
                if (Facing == Direction.East && maxRow == 0)
                {
                    maxRow = cell.Position.Row;
                }
                else if (Facing == Direction.South && maxColumn == 0)
                {
                    maxColumn = cell.Position.Column;
                }
            }
            return (cell, node, prio);
          
        }

        public bool DiscoverUsingBeacon(Game game, BaseCellItem cell,
            List<(Node, double)> priorityChildren, List<(int, int, Direction)> genTargets)
        {
            // set this to true if the cell in front is the same as the current beacon target.
            bool isPit_Wall_Target_Limit = false;
            cell = ScanForward(game);
            if (cell.CellItemType != CellItemType.Wall && cell.CellItemType != CellItemType.Pit)
            {
                if (!game.BeaconMemo.ContainsKey((cell.Position.Row, cell.Position.Column)))
                {

                        double prio = 1;
                        // create initial node
                        Node node = new Node();
                        node.Position.Row = cell.Position.Row;
                        node.Position.Column = cell.Position.Column;
                        node.CellItemType = cell.CellItemType;
                        node.Parent = game.CurrentNode;
                        // add the node object to the dictionary to prevent duplicate objects per cell.
                        game.BeaconMemo.Add((cell.Position.Row, cell.Position.Column), node);

                        if (!game.NodeMemo.ContainsKey((cell.Position.Row, cell.Position.Column)))
                        {
                            game.NodeMemo.Add((cell.Position.Row, cell.Position.Column), node);
                        }

                        if (cell.CellItemType == CellItemType.GoldenSquare) { prio = game.Size * game.Size + 2; }
                        else if (cell.CellItemType == CellItemType.Beacon) 
                        {
                            if(steppedOnSecond)
                            {
                                prio = ComputeDistance(currentBeaconTarget.Item1, currentBeaconTarget.Item2,
                                cell.Position.Row, cell.Position.Column);
                        }
                            else
                            {
                                prio = game.Size * game.Size + 1;
                            }
                        }
                        else if (cell.CellItemType == CellItemType.Empty)
                        {
                            prio = ComputeDistance(currentBeaconTarget.Item1, currentBeaconTarget.Item2,
                                cell.Position.Row, cell.Position.Column);
                        }

                    if (currentBeaconTarget.Item1 == cell.Position.Row 
                        && currentBeaconTarget.Item2 == cell.Position.Column
                        && cell.CellItemType != CellItemType.GoldenSquare)
                    {
                        isPit_Wall_Target_Limit = true;
                    }
                    else

                    priorityChildren.Add((node, prio));

                        //game.CurrentNode.Children.Push(node);
                }
            }
            else if (cell.CellItemType == CellItemType.Wall || cell.CellItemType == CellItemType.Pit)
            {
                
                //Update targets if wall or pit is found.
                for (int i = 0; i < genTargets.Count; i++)
                {
                    if (genTargets[i].Item1 == cell.Position.Row && genTargets[i].Item2 == cell.Position.Column)
                    {
                        genTargets.RemoveAt(i);
                        break;
                    }
                }

                if (cell.CellItemType == CellItemType.Wall)
                {
                    if (Facing == Direction.East)
                    {
                        if(maxRow == 0) { maxRow = Position.Row; }
                        if(currentBeaconTarget.Item1 > maxRow) { isPit_Wall_Target_Limit = true; }
                    }
                    else if (Facing == Direction.South)
                    {
                        if(maxColumn == 0) { maxColumn = Position.Column; }
                        if (currentBeaconTarget.Item2 > maxColumn) { isPit_Wall_Target_Limit = true; }
                    }
                }
                else
                {
                    //If discovered is current target, change target
                    if (currentBeaconTarget.Item1 == cell.Position.Row && currentBeaconTarget.Item2 == cell.Position.Column)
                    {
                        isPit_Wall_Target_Limit = true;
                    }

                    if (!game.PitMemo.Contains((cell.Position.Row, cell.Position.Column)))
                    {
                        game.PitMemo.Add((cell.Position.Row, cell.Position.Column));
                    }

                }
            }

            return isPit_Wall_Target_Limit;
        }

        public List<(int, int, Direction)> GenerateTargetGrids(Game game)
        {
            int row = 0, col = 0;
            List<(int, int, Direction)> genTargets = new List<(int, int, Direction)>();

            //Get North Target
            row = beaconValue.Position.Row - beaconValue.Value;
            col = beaconValue.Position.Column;
            if (row >= 0 && !game.PitMemo.Contains((row,col)) && !game.NodeMemo.ContainsKey((row, col)))
            {
                genTargets.Add((row, col, Direction.West));
            }

            //Get South Target
            row = beaconValue.Position.Row;
            col = beaconValue.Position.Column + beaconValue.Value;
            if (!game.PitMemo.Contains((row, col)) && !game.NodeMemo.ContainsKey((row, col)))
            {
                if (maxColumn > 0)
                {
                    if (col <= maxColumn) { genTargets.Add((row, col, Direction.South)); }
                }
                else
                {
                    genTargets.Add((row, col, Direction.South));
                }
            }

            //Get East Target
            row = beaconValue.Position.Row + beaconValue.Value;
            col = beaconValue.Position.Column;
            if (!game.PitMemo.Contains((row, col)) && !game.NodeMemo.ContainsKey((row, col)))
            {
                if (maxRow > 0)
                {
                    if (row <= maxRow) { genTargets.Add((row, col, Direction.East)); }
                }
                else
                {
                    genTargets.Add((row, col, Direction.East));
                }
            }

            //Get West Target
            row = beaconValue.Position.Row;
            col = beaconValue.Position.Column - beaconValue.Value;
            if (col >= 0 && !game.PitMemo.Contains((row, col)) && !game.NodeMemo.ContainsKey((row, col)))
            {
                genTargets.Add((row, col, Direction.North));
            }

            return genTargets;
        }

        public List<(int, int, Direction)> RegenTargetGrids(Game game, Beacon beacon)
        {
            int row = 0, col = 0;
            List<(int, int, Direction)> genTargets = new List<(int, int, Direction)>();

            //Get North Target
            row = beacon.Position.Row - beacon.Value;
            col = beacon.Position.Column;
            if (row >= 0 && !game.PitMemo.Contains((row, col)) 
                && !game.NodeMemo.ContainsKey((row, col))
                && !game.BeaconMemo.ContainsKey((row, col)))
            {
                genTargets.Add((row, col, Direction.West));
            }

            //Get South Target
            row = beacon.Position.Row;
            col = beacon.Position.Column + beacon.Value;
            if (!game.PitMemo.Contains((row, col))
                && !game.NodeMemo.ContainsKey((row, col))
                && !game.BeaconMemo.ContainsKey((row, col)))
            {
                if (maxColumn > 0)
                {
                    if (col <= maxColumn) { genTargets.Add((row, col, Direction.South)); }
                }
                else
                {
                    genTargets.Add((row, col, Direction.South));
                }
            }

            //Get East Target
            row = beacon.Position.Row + beacon.Value;
            col = beacon.Position.Column;
            if (!game.PitMemo.Contains((row, col))
                && !game.NodeMemo.ContainsKey((row, col))
                && !game.BeaconMemo.ContainsKey((row, col)))
            {
                if (maxRow > 0)
                {
                    if (row <= maxRow) { genTargets.Add((row, col, Direction.East)); }
                }
                else
                {
                    genTargets.Add((row, col, Direction.East));
                }
            }

            //Get West Target
            row = beacon.Position.Row;
            col = beacon.Position.Column - beacon.Value;
            if (col >= 0 && !game.PitMemo.Contains((row, col))
                && !game.NodeMemo.ContainsKey((row, col))
                && !game.BeaconMemo.ContainsKey((row, col)))
            {
                genTargets.Add((row, col, Direction.North));
            }

            return genTargets;

        }

        public void RandomizeFacing()
        {
            // 25% for each direction

            var num = Randomizer.RandomizeNumber();
            if (num >= 1 && num <= 25)
            {
                Facing = Direction.North;
                Symbol = "M(\u2191)";
            }
            else if (num >= 26 && num <= 50)
            {
                Facing = Direction.East;
                Symbol = "M(\u2192)";
            }
            else if (num >= 51 && num <= 75)
            {
                Facing = Direction.South;
                Symbol = "M(\u2193)";
            }
            else if (num >= 76 && num <= 100)
            {
                Facing = Direction.West;
                Symbol = "M(\u2190)";
            }

            Metrics.Facing = Facing.ToString();
            Console.WriteLine(string.Format("The player is initially facing {0}" , Facing.ToString()));
        }

        public double ComputeDistance(int rowA, int colA, int rowB, int colB)
        {
            //return Math.Abs((rowA - rowB) + (colA - colB));
            double row = Math.Pow((double)(rowA - rowB), 2);
            double col = Math.Pow((double)(colA - colB), 2);
            double to_square = row + col;
            //Make negative for prioritization stack
            return Math.Sqrt(to_square) * -1;
        }
    }

}
