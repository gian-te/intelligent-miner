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

        public Direction Facing { get; set; }

        public PlayerMetrics Metrics { get; set; }


        protected override void Initialize()
        {
            base.Initialize();
            Symbol = "M";
            PositionHistory = new List<Tuple<int, int>>();
            CellItemType = CellItemType.Player;
            Metrics = new PlayerMetrics();
            RandomizeFacing();
        }

        public Tuple<int, int> Rotate()
        {
            Tuple<int, int> cellInFront = null;

            var initialDirection = Facing.ToString();
            if (Facing == Direction.North)
            {
                Facing = Direction.East;
                Symbol = "M(\u2192)";
                cellInFront = new Tuple<int, int>(Position.Row + 1, Position.Column);
            }
            else if (Facing == Direction.East)
            {
                Facing = Direction.South;
                Symbol = "M(\u2193)";
                cellInFront = new Tuple<int, int>(Position.Row, Position.Column - 1);

            }
            else if (Facing == Direction.South)
            {
                Facing = Direction.West;
                Symbol = "M(\u2190)";
                cellInFront = new Tuple<int, int>(Position.Row - 1, Position.Column);
            }
            else if (Facing == Direction.West)
            {
                Facing = Direction.North;
                Symbol = "M(\u2191)";
                cellInFront = new Tuple<int, int>(Position.Row, Position.Column + 1);
            }

            Metrics.rotateCount++;
            Console.WriteLine("Rotated from {0} to {1}", initialDirection, Facing.ToString());
            // return the cell which the player is facing after rotating 90 degrees
            return cellInFront;
        }

        public CellItemType MoveWithStrategy(Game game)
        {
            CellItemType retVal;
            game.ClearCell(Position.Row, Position.Column);
            if (game.CurrentNode.Children.Count > 0)
            {
                var poppedNode = game.CurrentNode.Children.Pop();
                Position.Row = poppedNode.Position.Row;
                Position.Column = poppedNode.Position.Column;
                game.CurrentNode = poppedNode;
                retVal = poppedNode.CellItemType;
            }
            else
            {
                var poppedNode = game.CurrentNode.Parent;
                Position.Row = poppedNode.Position.Row;
                Position.Column = poppedNode.Position.Column;
                game.CurrentNode = poppedNode;
                retVal = poppedNode.CellItemType;
            }
            Metrics.moveCount++;
            
            game.AssignPlayerToCell(this);
            return retVal;
        }

        public ActionType RandomizeAction()
        {
            var value = Randomizer.RandomizeNumber();

            if (value >= 1 && value <= 50)
            {
                return ActionType.Rotate;
            }
            else
            {
                return ActionType.Move;
            }
        }

        public void RotateRandomTimes(int num = 10)
        {
            // arbitrary range of 1 to 10
            var times = Randomizer.RandomizeNumber(1, num);
            Console.WriteLine(string.Format("The player will rotate {0} times!", times));

            
            for (int i = 0; i < times; i++)
            {
                Rotate();
                Thread.Sleep(100);
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

        public BaseCellItem MoveForward(Game game, bool random)
        {
            int times = 1;
            if (random) { times = Randomizer.RandomizeNumber(1, game.Size); }
            Console.WriteLine(string.Format("The player will move to the {0} for {1} time(s)!", Facing.ToString(), times));

            BaseCellItem cell = null;
            for (int i = 0; i < times; i++)
            {
                Thread.Sleep(100);
                cell = ScanForward(game);
                //scanCount += 1;
                Metrics.scanCount++;
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

        public void Discover(Game game)
        {
            BaseCellItem cell;
            cell = ScanForward(game);
            Metrics.scanCount++;
            if (cell.CellItemType != CellItemType.Wall && cell.CellItemType != CellItemType.Pit)
            {
                if (!game.NodeMemo.ContainsKey((cell.Position.Row, cell.Position.Column)))
                {
                    // create initial node
                    Node node = new Node();
                    node.Position.Row = cell.Position.Row;
                    node.Position.Column = cell.Position.Column;
                    node.CellItemType = cell.CellItemType;
                    node.Parent = game.CurrentNode;
                    // add the node object to the dictionary to prevent duplicate objects per cell.
                    game.NodeMemo.Add((cell.Position.Row, cell.Position.Column), node);

                    game.CurrentNode.Children.Push(node);
                }
            }
          
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

        public void Think()
        {
            // build a graph of known visited nodes using the position history


            // if there is a beacon in the visited notes, reduce the grid to a smaller M by M grid where M is the size of the grid from the beacon to the maximum number of steps where the golden square is
            // for example, if the beacon is in cell 1,1 with a value of 3, reduce the grid to a 4x4 grid, from 0,0 to 4,4.
            // we do this to minimize the search space, we don't need the rest of the tiles since we know that the golden square is within 4 cells from the beacon.



            // 
        }
    }




}
