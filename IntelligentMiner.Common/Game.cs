using IntelligentMiner.Common.Enums;
using IntelligentMiner.Common.Utilities;
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

        public (Tuple<int, int>, List<string>, List<string>) AddRandom(int gridSize)
        {
            int row = 0;
            int col = 0;
            int i = 0;

            //20% Pits & Beacons
            double numberOfPitsandBeacons = Math.Round((gridSize*gridSize) * 0.20);
            List<string> _traps = new List<string>();
            List<string> _beacons = new List<string>();

            //Create Golden Square
            while (row == 0 && col == 0)
            {
                row = Randomizer.RandomizeNumber(0, gridSize);
                col = Randomizer.RandomizeNumber(0, gridSize);
            }

            //Map[row, col] = new GoldenSquare();
            Tuple<int, int> _goldensquare = new Tuple<int, int>(row, col);

            //Create Beacons
            while (i < numberOfPitsandBeacons)
            {

                var beacon = new Beacon();
                int chooseAlignment = Randomizer.RandomizeNumber(0, 2);

                //0 Create beacon in row of golden square
                if (chooseAlignment  == 0)
                {
                    row = _goldensquare.Item1;
                    col = Randomizer.RandomizeNumber(0, gridSize);
                }
                //Create beacon in column of golden square
                else
                {
                    row = Randomizer.RandomizeNumber(0, gridSize);
                    col = _goldensquare.Item2;

                }

                string coordinate = String.Concat(row, ',', col);

                if (
                        (row != 0 && col != 0) //Check if not in player intial position
                    && (row != _goldensquare.Item1 && col != _goldensquare.Item2) //Check if not in Golden Square
                    && (!_beacons.Contains(coordinate)) //Not in list of Beacons
                   )
                {
                    if (chooseAlignment == 0) { beacon.Value = Math.Abs(_goldensquare.Item1 - row); }
                    else { beacon.Value = Math.Abs(_goldensquare.Item1 - col); }
                    //Map[row, col] = beacon;
                    _beacons.Add(coordinate);
                    i++;
                }
            }

            i = 0;

            //Create Pits - for polishing
            while (i < numberOfPitsandBeacons)
            {

                row = Randomizer.RandomizeNumber(0, gridSize);
                col = Randomizer.RandomizeNumber(0, gridSize);
                string coordinate = String.Concat(row, ',', col);

                if (
                        (row != 0 && col != 0) //Check if not in player intial position
                    && (row != _goldensquare.Item1 && col != _goldensquare.Item2) //Check if not in Golden Square
                    && (!_beacons.Contains(coordinate)) //Not in list of Beacons
                    && (!_traps.Contains(coordinate)) //Not in list of Beacons
                   )
                {
                    //Map[row, col] = new Pit();
                    _traps.Add(coordinate);
                    i++;
                }
            }

            return (_goldensquare, _beacons, _traps);

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
