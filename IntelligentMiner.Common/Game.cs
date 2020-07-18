using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;

namespace IntelligentMiner.Common
{
    public class Game
    {
        public int Size { get; set; }

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


        public Game(int n, GoldenSquare gold = null, Beacon beacon = null, List<Pit> pits = null)
        {
            Map = new BaseCellItem[n,n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Map[i, j] = new BaseCellItem();
                }
            }
            Size = n;

            if (gold != null)
            {
                Map[gold.Position.Row, gold.Position.Column] = gold;
            }
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
                // empty cell but not wall
                if (cell == null)
                {
                    cell = new BaseCellItem();
                }
                cell.Position.Row = row;
                cell.Position.Column = col;
            }
            catch
            {
                cell = new BaseCellItem()
                {
                    CellItemType = CellItemType.Wall
                };
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


        /// <summary>
        /// Must clear the player if the player moves away in a cell
        /// </summary>
        public void ClearCell(int row, int col)
        {
            Map[row, col] = new BaseCellItem();
        }

        /// <summary>
        /// Assignts a player to a cell in the Map
        /// </summary>
        /// <param name="game"></param>
        public void AssignPlayerToCell(Player player)
        {
            Map[player.Position.Row, player.Position.Column] = player;
        }
    }
}
