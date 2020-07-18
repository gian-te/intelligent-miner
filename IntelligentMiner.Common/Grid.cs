using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;

namespace IntelligentMiner.Common
{
    public class Grid
    {
        private CellItem[,] _map;

        /// <summary>
        /// Property for a 2D Array
        /// </summary>
        public CellItem[,] Map
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


        public Grid(int n)
        {
            Map = new CellItem[n,n];
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

        public CellItemType GetCellType(int row, int col)
        {
            var cell = Map[row, col];
            CellItemType type;
            if (cell != null)
            {
                type = cell.CellItemType;
            }
            else
            {
                type = CellItemType.Empty;
            }

            return type;
        }
    }
}
