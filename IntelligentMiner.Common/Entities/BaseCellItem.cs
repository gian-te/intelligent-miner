using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentMiner.Common
{
    public class BaseCellItem
    {
        public string Symbol { get; set; }
        public CellItemType CellItemType { get; set; }
        public Position Position { get; set; }

        public BaseCellItem()
        {
            Initialize();
            CellItemType = CellItemType.Empty; // default cell type
            Position = new Position();
        }

        protected virtual void Initialize()
        {
            // override if you need to initialize something in the derived classes
           
        }

    }
}
