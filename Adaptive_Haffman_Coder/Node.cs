using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaptive_Haffman_Coder
{
    class Node
    {
        private bool isCharSet;
        private char character;

        public Node Next { get; set; }
        public Node Prev { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Parent { get; set; }

        public bool IsLeaf => Left == null && Right == null;
        public int Frequency { get; set; }
        public bool IsEmpty => IsLeaf && !isCharSet;
        public char Character
        {
            get => character;
            set
            {
                character = value;
                isCharSet = true;
            }
        }
    }
}
