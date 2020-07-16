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

		public Queue<int[,]> PositionHistory { get; set; }

		public Player()
		{
			Symbol = "P";
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

		public void MoveRandomly()
		{
			var possibleDirections = Enum.GetValues(typeof(Directions));
			Thread.Sleep(100);
			var random = new Random();

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

		}

		public void EnqueueLastPosition(int[,] pos)
		{

			// save last position
			PositionHistory.Enqueue(pos);
		}
	}


}
