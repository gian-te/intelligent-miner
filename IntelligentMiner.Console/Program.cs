using IntelligentMiner.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMiner.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var size = 0;
                Console.WriteLine("Enter map size: ");
                int.TryParse(Console.ReadLine(), out size);

                // create 2x2 array
                var game = new Grid(size);

                var player = new Player();
                player.PositionX = 0;
                player.PositionY = 0;

                // set the player to its position in the grid
                game.Map[player.PositionX, player.PositionY] = player;


                for (int i = 0; i < size; i++)
                {
                    player.MoveRandomly();
                    Console.WriteLine(string.Format("Player is trying to move to coordinates [{0},{1}]", player.PositionX, player.PositionY));
                }

                // TODO: ask for trap location
                //Console.WriteLine(string.Format("Player is at {0}"));

                // TODO: ask for gold location


                // TODO: render 2x2 array in console.
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
