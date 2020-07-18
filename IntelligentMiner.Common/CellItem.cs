using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentMiner.Common
{
    public class CellItem
    {
        public string Symbol { get; set; }
        public CellItemType CellItemType { get; set; }

        public CellItem()
        {
            Initialize();
        }

        protected void Initialize()
        {
            // override if you need to initialize something in the derived classes
        }

    }
}
