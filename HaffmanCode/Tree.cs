using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaffmanCode
{
    class Tree
    {

        private List<Node> nodes;
        private List<bool> encodedTree;
        private Node _root;
        private Dictionary<char, int> _frequencies;
        private readonly string input;

        /// <summary>
        /// Constructor for encode new string
        /// </summary>
        /// <param name="input"></param>
        public Tree(string input) : this()
        {
            this.input = input;
            _frequencies = new Dictionary<char, int>(256);
            foreach (var s in input)
            {
                if (!_frequencies.ContainsKey(s))
                {
                    _frequencies.Add(s, 0);
                }

                _frequencies[s]++;
            }
        }
        /// <summary>
        /// Constructor for decode
        /// </summary>
        public Tree()
        {
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

            var encodedSource = new List<bool>(256 * 8);
            EncodeNode(_root);

            var count = input.Length;
            var index = 1.0D;
            
            foreach (var i in input)
            {
                var encodedSymbol = _root.Traverse(i, new List<bool>());
                encodedSource.AddRange(encodedSymbol);
                Util.ShowPercents(count, ref index);
            }
            var bits = new BitArray(encodedSource.ToArray());
            return bits;
            
        }
       
        /// <summary>
        /// Encode Dictionary
        /// </summary>
        /// <param name="node"></param>
        private void EncodeNode(Node node)
        {
            if (node.IsLeaf)
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
        /// <summary>
        /// Decode Dictionary
        /// </summary>
        /// <param name="num">counter for bits</param>
        /// <returns></returns>
        Node ReadNode(ref int num)
        {
            if (encodedTree[num])
            {
                num++;
                var bits = new BitArray(8);
                for (int i = 0; i < 8; i++, num++)
                {
                    bits.Set(i, encodedTree[num]);
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
                var n = new Node() {Symbol = null, Left = leftChild, Right = rightChild};
                return n;
            }
        }
       
        public BitArray getEncodedTree()
        {
            return new BitArray(encodedTree.ToArray());
        }
        /// <summary>
        /// Decode input bits
        /// </summary>
        /// <param name="dictBits">Dictionary</param>
        /// <param name="bits">bits for decode</param>
        /// <returns>decoded string</returns>
        public string Decode(BitArray dictBits,BitArray bits)
        {
            for (int i = 0; i < dictBits.Count; i++)
            {
               encodedTree.Add(dictBits.Get(i));
            }

            var num = 0;
            _root = ReadNode(ref num);

            var current = _root;
            var decoded = new StringBuilder();

            var count = bits.Length;
            var index = 1.0D;

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
                Util.ShowPercents(count,ref index);
                if (!current.IsLeaf) continue;
                decoded.Append(current.Symbol);
                current = _root;
            }

            return decoded.ToString();
        }

       
    }
}
