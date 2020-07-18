using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentMiner.Common.Utilities
{
    public static class Randomizer
    {
        public static Random Random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomizeNumber(int min = 1, int max = 101)
        {
            // for some reason .NET implemented this to be inclusive of lower bound, but exclusive of upper bound lol
            // https://docs.microsoft.com/en-us/dotnet/api/system.random.next?view=netcore-3.1
            lock (syncLock)
            {
                return Random.Next(min, max);
            }
        }
      
    }
}
