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
        public int Value { get; set; }

        public BaseCellItem()
        {
            Symbol = "-";
            CellItemType = CellItemType.Empty; // default cell type
            Value = 0; //For Beacon
            Position = new Position();
            Initialize();
        }

        protected virtual void Initialize()
        {
            // override if you need to initialize something in the derived classes
           
        }

    }
}
