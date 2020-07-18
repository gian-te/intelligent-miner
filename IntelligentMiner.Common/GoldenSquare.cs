using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class GoldenSquare : CellItem
    {
        public GoldenSquare()
        {
            Symbol = "G";
            CellItemType = CellItemType.GoldenSquare;
        }
    }
}
