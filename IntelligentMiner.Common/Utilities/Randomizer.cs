using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentMiner.Common.Utilities
{
    public static class Randomizer
    {
        public static int RandomizeNumber(int min = 1, int max = 100)
        {
            var random = new Random();
            return random.Next(min, max);
        }
    }
}
