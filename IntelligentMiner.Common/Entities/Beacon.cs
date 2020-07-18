using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class Beacon : BaseCellItem
    {
        public Beacon()
        {
            Symbol = "B";
            CellItemType = CellItemType.Beacon;
        }
    }
}
