using System;
using System.Collections.Generic;
using System.Text;
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.Common
{
    public class Pit : BaseCellItem
    {
        protected override void Initialize()
        {
            Symbol = "P";
            CellItemType = CellItemType.Pit;
        }
    }
}
