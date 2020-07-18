using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class GoldenSquare : BaseCellItem
    {
        public GoldenSquare()
        {
            Symbol = "G";
            CellItemType = CellItemType.GoldenSquare;
        }
    }
}
