using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;

namespace IntelligentMiner.Common
{
    public class Game
    {
        private BaseCellItem[,] _map;

        /// <summary>
        /// Property for a 2D Array
        /// </summary>
        public BaseCellItem[,] Map
        {
            get { return _map; }
            set { _map = value; }
        }

        private List<int[,]> _traps;

        // collection of traps
        public List<int[,]> Traps
        {
            get { return _traps; }
            set { _traps = value; }
        }


        public Game(int n)
        {
            Map = new BaseCellItem[n,n];
        }

        public void AddTrap(int row, int col)
        {
            Map[row, col] = new Pit();
           
        }

        public void AddGold(int row, int col)
        {
            Map[row, col] = new GoldenSquare();
        }

        public void AddBeacon(int row, int col)
        {
            Map[row, col] = new Beacon();
        }

        public BaseCellItem Scan(int row, int col, Direction Facing, string scan_type = "current")
        {

            if (scan_type == "front")
            {

                if (Facing == Direction.North)
                {
                    col--;
                }
                else if (Facing == Direction.East)
                {
                    row++;
                }
                else if (Facing == Direction.South)
                {
                    col++;
                }
                else if (Facing == Direction.West)
                {
                    row--;
                }
            }

            BaseCellItem cell = null;
            try
            {
                cell = Map[row, col];
            }
            catch
            {
                return cell;
            }

            return cell;
        }

        public bool IsCellValid(int row, int col)
        {
            var retVal = false;
            try
            {
                var cell = Map[row, col];
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
            }

            return retVal;
        }
    }
}
