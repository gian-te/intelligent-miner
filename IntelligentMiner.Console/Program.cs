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
                Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }

        private static void Play()
        {
            try
            {
                var size = 0;
                var goldX = 0;
                var goldY = 0;

                //Choose initial intelligence
                string intell = "";

                while (intell != "r" && intell != "s" && intell != "test")
                {
                    Console.Clear();
                    Console.WriteLine("Choose Intelligence Level (R/S): ");
                    intell = Console.ReadLine().ToLower();
                }

                Console.WriteLine("Enter the map size: ");
                int.TryParse(Console.ReadLine(), out size);

                Console.WriteLine("Enter the X-coordinate of the gold: ");
                int.TryParse(Console.ReadLine(), out goldX);

                Console.WriteLine("Enter the Y-coordinate of the gold: ");
                int.TryParse(Console.ReadLine(), out goldY);

                bool end = false;
                var stepCount = 0;

                while (!end)
                {
                    if (intell == "test") //Moves intelligently
                    {

                        // Declare parameters
                        int generations = 100;
                        int mutationChance = 30;
                        int crossoverChance = 10;
                        double stepsLimit = size * size;
                        double fitnessMeasure = 0;

                        // Generate strategies mapping
                        var generator = new Generators();
                        generator.GeneratePossibilities();
                        generator.GenerateStrategies(200);

                        for (int gen = 0; gen < generations; gen++)
                        {

                            foreach (string strategy in generator.Strategies)
                            {
                                var game = new Game(size);
                                var player = new Player();
                                player.PositionX = 0;
                                player.PositionY = 0;

                                game.Map[player.PositionX, player.PositionY] = player;
                                game.AddGold(goldX, goldY);

                                while (stepCount < stepsLimit || (player.PositionX != goldX && player.PositionY != goldY))
                                {
                                    player.scanCount++;

                                    player.MoveWithStrategy(generator.Situations.IndexOf(()).ToString());
                                    stepCount++;
                                }

                            }

                            fitnessMeasure = Math.Abs(Math.Round(stepsLimit / 2) - stepCount);

                        }

                        end = true;
                    }
                    else if (intell == "s")
                    {
                        //Dito yung totoo
                    }
                    else //Moves randomly
                    {
                        // create 2x2 array
                        var game = new Game(size);
                        //game.AddGold(1, 1);
                        //game.AddTrap(1, 1);
                        //game.AddBeacon(1, 1);

                        var player = new Player();
                        player.PositionX = 0;
                        player.PositionY = 0;

                        // set the player to its position in the grid
                        game.Map[player.PositionX, player.PositionY] = player;

                        player.MoveRandomly(size);
                        Console.WriteLine(string.Format("Player is trying to move to coordinates [{0},{1}]", player.PositionX, player.PositionY));

                        stepCount++;

                        if (player.PositionX == goldX && player.PositionY == goldY)
                        {
                            end = true;
                        }
                    }

                }

                Console.WriteLine(string.Format("Found in {0} steps", stepCount));
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}
