using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class Pit : BaseCellItem
    {
        // does nothing for now
        public Pit()
        {
            Symbol = "T";
            CellItemType = CellItemType.Pit;
        }
    }
}
