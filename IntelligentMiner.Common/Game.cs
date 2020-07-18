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

        public CellItemType GetCellType(int row, int col, string Facing, string scan_type = "current")
        {

            if (scan_type == "front")
            {

                if (Facing == "N")
                {
                    col--;
                }
                else if (Facing == "E")
                {
                    row++;
                }
                else if (Facing == "S")
                {
                    col++;
                }
                else
                {
                    row--;
                }

            }

            var cell = Map[row, col];
            CellItemType type = cell.CellItemType;

            try
            {
                if (cell != null)
                {
                    type = cell.CellItemType;
                }
                else
                {
                    type = CellItemType.Empty;
                }
            }
            catch (Exception)
            {

                type = CellItemType.Wall;
            }

            return type;
        }
    }
}
