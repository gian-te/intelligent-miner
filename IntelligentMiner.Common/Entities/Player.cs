using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using IntelligentMiner.Common.Enums;
using IntelligentMiner.Common.Utilities;

namespace IntelligentMiner.Common
{


    public class Player : BaseCellItem
    {

        public List<Tuple<int, int>> PositionHistory { get; set; }

        public Direction Facing { get; set; }

        public int scanCount { get; set; }

        public int moveCount { get; set; }

        public int rotateCount { get; set; }

        public int backtrackCount { get; set; }


        protected override void Initialize()
        {
            base.Initialize();
            Symbol = "M";
            PositionHistory = new List<Tuple<int, int>>();
            CellItemType = CellItemType.Player;
            RandomizeFacing();
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

        public void MoveSmartly()
        {
            //Dito totoo
        }

        public void MoveWithStrategy(string strat)
        {
            //Gawin yung strat experiment
        }

        public string RandomizeAction()
        {
            var value = Randomizer.RandomizeNumber();

            if (value >= 1 && value <= 50)
            {
                return "rotate";
            }
            else
            {
                return "move";
            }
        }

        public void RotateRandomTimes()
        {
            // arbitrary range of 1 to 10
            var times = Randomizer.RandomizeNumber(1, 10);
            Console.WriteLine(string.Format("The player will rotate {0} times!", times));

            
            for (int i = 0; i < times; i++)
            {
                Rotate();
                Thread.Sleep(500);
            }
            
        }

        public BaseCellItem Move(Game game)
        {
            var times = Randomizer.RandomizeNumber(1, game.Size + 1);
            Console.WriteLine(string.Format("The player will move to the {0} for {1} times!", Facing.ToString(), times));

            BaseCellItem cell = null;
            for (int i = 0; i < times; i++)
            {
                Thread.Sleep(100);
                cell = game.Scan(Position.Row, Position.Column, Facing, "front");
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

                Console.WriteLine(string.Format("Player moved to coordinates [{0},{1}]", Position.Row, Position.Column));
                PositionHistory.Add(new Tuple<int, int>(Position.Row, Position.Column));

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
                    Console.WriteLine("The player found a beacon.");
                    break;
                }


            }
            return cell;
        }

        public void RandomizeFacing()
        {
            // 25% for each direction

            var num = Randomizer.RandomizeNumber();
            if (num >= 1 && num <= 25)
            {
                Facing = Direction.North;
            }
            else if (num >= 26 && num <= 50)
            {
                Facing = Direction.East;
            }
            else if (num >= 51 && num <= 75)
            {
                Facing = Direction.South;
            }
            else if (num >= 76 && num <= 100)
            {
                Facing = Direction.West;
            }

            Console.WriteLine(string.Format("The player is initially facing {0}" , Facing.ToString()));
        }
    }




}
