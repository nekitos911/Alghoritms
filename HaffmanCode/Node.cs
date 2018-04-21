using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HaffmanCode
{
    [ProtoContract]
    class Node
    {
        [ProtoMember(1)]
        public char? Symbol { get; set; }     
        public int Frequency { get; set; }
        [ProtoMember(2)]
        public Node Right { get; set; }
        [ProtoMember(3)]
        public Node Left { get; set; }

        public List<bool> Traverse(char symbol, List<bool> data)
        {
            if (Right == null && Left == null)
            {
                return symbol.Equals(Symbol) ? data : null;
            }
           
            List<bool> left = null;
            List<bool> right = null;
            if (Left != null)
            {
                var leftPath = new List<bool>();
                leftPath.AddRange(data);
                leftPath.Add(false);

                left = Left.Traverse(symbol, leftPath);
            }
            if (Right != null)
            {
                var rightPath = new List<bool>();
                rightPath.AddRange(data);
                rightPath.Add(true);
                right = Right.Traverse(symbol, rightPath);
            }
            return left ?? right;
        }
    }
}
