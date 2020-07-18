using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class Beacon : CellItem
    {
        public Beacon()
        {
            Symbol = "B";
            CellItemType = CellItemType.Beacon;
        }
    }
}
