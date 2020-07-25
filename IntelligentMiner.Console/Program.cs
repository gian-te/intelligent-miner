using IntelligentMiner.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            int stepCount = 0;
            int size = 0;
            bool end = false;

            try
            {
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

                Console.WriteLine("Enter the indices of the golden square, comma-separated. Example: [2,3] (without the square brackets) ");
                var coords = Console.ReadLine().Split(',');
                int goldX, goldY = 0;
                int.TryParse( coords[0], out goldX);
                int.TryParse( coords[1], out goldY);

                var gold = new GoldenSquare();
                gold.Position.Row = goldX;
                gold.Position.Column = goldY;
               

                Game game = new Game(size);
                game.AddGold(goldX, goldY);
                Player player = new Player();

                game.Map[player.Position.Row, player.Position.Column] = player;

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
                                game = new Game(size);
                                player.Position.Row = 0;
                                player.Position.Column = 0;

                                game.Map[player.Position.Row, player.Position.Column] = player;
                                game.AddGold(gold.Position.Row, gold.Position.Column);

                                while (stepCount < stepsLimit || (player.Position.Row != gold.Position.Row && player.Position.Column != gold.Position.Column))
                                {
                                    
                                    player.Metrics.scanCount++;
                                    var front = game.GetCell(player.Position.Row, player.Position.Column, player.Facing, "front");
                                    var current = game.GetCell(player.Position.Row, player.Position.Column, player.Facing);


                                    //player.MoveWithStrategy(generator.Situations.IndexOf((current.CellItemType.ToString(), front.CellItemType.ToString())).ToString());
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
                        Thread.Sleep(500);
                        // 1. randomize move between Rotate or Move
                        var action = player.RandomizeAction();

                        if (action == Common.Enums.ActionType.Rotate)
                        {
                            // 2. if Rotate, randomize how many times it will rotate 90-degrees
                            player.RotateRandomTimes();
                        }
                        else if (action == Common.Enums.ActionType.Move)
                        {
                            // 3. if Move, randomize how many times it will move
                            var cell = player.MoveForward(game, true);
                            if (cell.CellItemType == Common.Enums.CellItemType.GoldenSquare || cell.CellItemType == Common.Enums.CellItemType.Pit)
                            {
                                end = true;
                            }

                        }

                        stepCount++;
                        Console.WriteLine();
                    }

                }

                Console.WriteLine(string.Format("Found in {0} moves, and presumably a lot of rotations.", stepCount));
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
