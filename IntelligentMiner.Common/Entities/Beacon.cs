using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class Beacon : BaseCellItem
    {
        public int Value { get; set; }

        protected override void Initialize()
        {
            Symbol = "B";
            Value = 0;
            CellItemType = CellItemType.Beacon;
        }
    }
}
