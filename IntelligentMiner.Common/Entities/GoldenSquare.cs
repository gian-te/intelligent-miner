using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class GoldenSquare : BaseCellItem
    {
        protected override void Initialize()
        {
            Symbol = "G";
            CellItemType = CellItemType.GoldenSquare;
        }
    }
}
