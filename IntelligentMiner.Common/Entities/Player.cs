using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IntelligentMiner.Common
{


    public class Player : BaseCellItem
    {
        /// <summary>
        /// Container of the 0-based indices of X and Y
        /// </summary>
        public Position Position { get; set; }

        public List<Tuple<int, int>> PositionHistory { get; set; }

        public string Facing { get; set; }

        public int scanCount { get; set; }

        public int moveCount { get; set; }

        public int rotateCount { get; set; }

        public int backtrackCount { get; set; }

        public Player()
        {
            Symbol = "P";
            Facing = "E";
            PositionHistory = new List<Tuple<int, int>>();
            CellItemType = CellItemType.Player;
        }

        public void MoveUp()
        {
            // row = row + 0, col = col + 1
            Position.Column += 1;
        }

        public void MoveDown()
        {
            // row = row + 0, col = col - 1
            Position.Column -= 1;
        }

        public void MoveLeft()
        {
            // row = row - 1, col = col + 0
            Position.Row -= 1;
        }

        public void MoveRight()
        {
            // row = row + 1, col = col + 0
            Position.Row += 1;
        }

        public Tuple<int, int> Rotate()
        {
            Tuple<int, int> cellInFront;
            if (Facing == "N")
            {
                Facing = "E";
                cellInFront = new Tuple<int, int>(Position.Row + 1, Position.Column);
            }
            else if (Facing == "E")
            {
                Facing = "S";
                cellInFront = new Tuple<int, int>(Position.Row, Position.Column - 1);

            }
            else if (Facing == "S")
            {
                Facing = "W";
                cellInFront = new Tuple<int, int>(Position.Row - 1, Position.Column);
            }
            else
            {
                Facing = "N";
                cellInFront = new Tuple<int, int>(Position.Row, Position.Column + 1);
            }

            // return the cell which the player is facing after rotating 90 degrees
            return cellInFront;
        }


        public void MoveRandomly(int gridSize)
        {
            var possibleDirections = Enum.GetValues(typeof(Directions));
            var random = new Random();
            Thread.Sleep(200);
            var direction = (Directions)possibleDirections.GetValue(random.Next(0, possibleDirections.Length));
            switch (direction)
            {
                case Directions.North:
                    MoveUp();
                    break;
                case Directions.South:
                    MoveDown();
                    break;
                case Directions.East:
                    MoveLeft();
                    break;
                case Directions.West:
                    MoveRight();
                    break;
                default:
                    break;
            }

            // if out of bounds, or  if negative
            if ((Math.Abs(Position.Row) >= gridSize || Math.Abs(Position.Column) >= gridSize) || (Position.Row < 0 || Position.Column < 0))
            {
                // get last coordinates and make it the current position
                if (PositionHistory.Count > 0)
                {
                    Position.Row = PositionHistory[PositionHistory.Count - 1].Item1;
                    Position.Column = PositionHistory[PositionHistory.Count - 1].Item2;
                }
                else
                {
                    Position.Row = 0;
                    Position.Column = 0;
                }

                // then go again
                MoveRandomly(gridSize);
            }
            else
            {
                // historize steps if random move is valid.
                PositionHistory.Add(new Tuple<int, int>(Position.Row, Position.Column));
            }

        }

        public void MoveSmartly()
        {
            //Dito totoo
        }

        public void MoveWithStrategy(string strat)
        {
            //Gawin yung strat experiment
        }

    }




}
