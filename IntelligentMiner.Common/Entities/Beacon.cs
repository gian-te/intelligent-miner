using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class Beacon : BaseCellItem
    {
        protected override void Initialize()
        {
            Symbol = "B";
            CellItemType = CellItemType.Beacon;
        }
    }
}
