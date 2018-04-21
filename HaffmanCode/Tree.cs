using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HaffmanCode
{
    class Tree
    {

        private List<Node> nodes;
        private List<bool> encodedTree;
        private Node _root;
        [ProtoMember(1)]
        private Dictionary<char, int> _frequencies;
        private readonly string input;
        private readonly string seriaziableTreeName;

        public Tree(string input, string seriaziableTreeName) : this(seriaziableTreeName)
        {
            this.input = input;
            _frequencies = new Dictionary<char, int>();
            foreach (var s in input)
            {
                if (!_frequencies.ContainsKey(s))
                {
                    _frequencies.Add(s, 0);
                }

                _frequencies[s]++;
            }
        }

        public Tree(string seriaziableTreeName)
        {
            this.seriaziableTreeName = seriaziableTreeName;
            nodes = new List<Node>();
            encodedTree = new List<bool>();
        }


        /// <summary>
        /// Build Haffman's tree
        /// </summary>
        private void Build()
        {             
                foreach (var symbol in _frequencies)
                {
                    nodes.Add(new Node() {Symbol = symbol.Key, Frequency = symbol.Value});
                }

                if (nodes.Count == 1)
                {
                    nodes[0].Left = new Node
                    {
                        Symbol = nodes[0].Symbol,
                        Frequency = nodes[0].Frequency
};
                }

                while (nodes.Count > 1)
                {
                    var orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();

                    if (orderedNodes.Count >= 2)
                    {
                        // Take first two items
                        var taken = orderedNodes.Take(2).ToList();

                        // Create a parent node by combining the frequencies
                        var parent = new Node()
                        {
                            Symbol = null,
                            Frequency = taken[0].Frequency + taken[1].Frequency,
                            Left = taken[0],
                            Right = taken[1]
                        };

                        nodes.Remove(taken[0]);
                        nodes.Remove(taken[1]);
                        nodes.Add(parent);
                    }
                }
                _root = nodes.FirstOrDefault();
        }
        /// <summary>
        /// Encode string
        /// </summary>
        /// <returns></returns>
        public BitArray Encode()
        {
            Build();

            var encodedSource = new List<bool>();
            EncodeNode(_root);
            
            foreach (var i in input)
            {
                var encodedSymbol = _root.Traverse(i, new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }
            var bits = new BitArray(encodedSource.ToArray());
            return bits;
        }
        /// <summary>
        /// Check node's children
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

        private void EncodeNode(Node node)
        {
            if (node.Left == null)
            {
                encodedTree.Add(true);
                var bStr = string.Concat(Encoding.GetEncoding(1251).GetBytes(node.Symbol.ToString()).Select(b => Convert.ToString(b, 2)));
                for (int i = 0; i < 8 - bStr.Length; i++)
                {
                    encodedTree.Add(false);
                }
                foreach (var c in bStr)
                {
                    encodedTree.Add(c != '0');
                }
            }
            else
            {
                encodedTree.Add(false);
                EncodeNode(node.Left);
                EncodeNode(node.Right);
            }
        }
        Node ReadNode(ref int num)
        {
            if (encodedTree[num])
            {
                num++;
                var bits = new BitArray(8);
                for (int i = 0; i < 8; i++,num++)
                {
                    bits.Set(i,encodedTree[num]);
                }

                var s = "";
                for (int i = 0; i < bits.Count; i++)
                {
                    s += bits.Get(i) ? 1 : 0;
                }



                var b = new byte[1];
                b[0] = Convert.ToByte(s, 2);


                var n = new Node {Symbol = Encoding.GetEncoding(1251).GetChars(b)[0]};
                return n;
            }
            else
            {
                num++;
                var leftChild = ReadNode(ref num);
                var rightChild = ReadNode(ref num);
                var n = new Node() {Symbol = null,Left = leftChild,Right = rightChild};
                return n;
            }
        }
         
        public BitArray getEncodedTree()
        {
            return new BitArray(encodedTree.ToArray());
        }

        public string Decode(BitArray treeBits,BitArray bits)
        {
            for (int i = 0; i < treeBits.Count; i++)
            {
               encodedTree.Add(treeBits.Get(i));
            }

            var num = 0;
            _root = ReadNode(ref num);

            var current = _root;
            var decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }
              
                if (!IsLeaf(current)) continue;
                decoded += current.Symbol;
                current = _root;
            }

            return decoded;
        }
    }
}
