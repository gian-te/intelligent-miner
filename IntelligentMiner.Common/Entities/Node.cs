using IntelligentMiner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentMiner.Common.Entities
{
    public class Node
    {

        public Node Parent { get; set; }

        public Stack<Node> Children { get; set; }

        public Position Position { get; set; }

        public CellItemType CellItemType { get; set; }

        public Node()
        {
            Children = new Stack<Node>();
            Position = new Position();

        }
      
    }
}
