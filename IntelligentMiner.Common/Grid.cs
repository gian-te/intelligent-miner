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
            Map[row, col] = new Trap();
           
        }

        public void AddGold(int row, int col)
        {
            Map[row, col] = new Gold();
        }
    }
}
