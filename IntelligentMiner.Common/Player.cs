using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IntelligentMiner.Common
{
	

	public class Player : CellItem
    {
		private int _positionX;
		private int _positionY;

		public int PositionX
		{
			get { return _positionX; }
			set { _positionX = value; }
		}

		public int PositionY
		{
			get { return _positionY; }
			set { _positionY = value; }
		}

		public List<Tuple<int,int>> PositionHistory { get; set; }

		public Player()
		{
			Symbol = "P";
			PositionHistory = new List<Tuple<int, int>>();
		}

		public void MoveUp()
		{
			// row = row + 0, col = col + 1
			PositionY += 1;
		}

		public void MoveDown()
		{
			// row = row + 0, col = col - 1
			PositionY -= 1;
		}

		public void MoveLeft()
		{
			// row = row - 1, col = col + 0
			PositionX -= 1;
		}

		public void MoveRight()
		{
			// row = row + 1, col = col + 0
			PositionX += 1;
		}

		public void MoveRandomly(int gridSize)
		{
			var possibleDirections = Enum.GetValues(typeof(Directions));
			var random = new Random();
			Thread.Sleep(100);
			var direction = (Directions)possibleDirections.GetValue(random.Next(0,possibleDirections.Length));
			switch (direction)
			{
				case Directions.Up:
					MoveUp();
					break;
				case Directions.Down:
					MoveDown();
					break;
				case Directions.Left:
					MoveLeft();
					break;
				case Directions.Right:
					MoveRight();
					break;
				default:
					break;
			}

			// if out of bounds,
			if (Math.Abs(PositionX) >= gridSize || Math.Abs(PositionY) >= gridSize)
			{
				// get last coordinates
				if (PositionHistory.Count > 0)
				{
					PositionX = PositionHistory[PositionHistory.Count - 1].Item1;
					PositionY = PositionHistory[PositionHistory.Count - 1].Item2;
				}
				else
				{
					PositionX = 0;
					PositionY = 0;
				}
				
				// then go again
				MoveRandomly(gridSize);
			}
			else
			{
				// historize steps
				PositionHistory.Add(new Tuple<int, int>(PositionX, PositionY));
			}

		}

	
	}


}
