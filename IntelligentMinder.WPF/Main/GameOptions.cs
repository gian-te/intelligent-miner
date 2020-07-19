using IntelligentMiner.Common.Enums;
using IntelligentMiner.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMiner.WPF.Main
{
    public class GameOptions
    {
        public int Size { get; set; }

        public bool MovesIntelligently { get; set; }

        public bool MovesRandomly { get; set; }

        public bool RandomInit { get; set; }

        public bool ManualInit { get; set; }

        public string _pits { get; set; }

        public string _beacons { get; set;}

        public string Gold { get; set; }
        
        public List<string> Pits { get; set; }

        public List<string> Beacons { get; set; }
    }
}
