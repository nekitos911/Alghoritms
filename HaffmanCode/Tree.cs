using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaffmanCode
{
    class Tree
    {

        private List<Node> nodes;
        private Node _root;
        private Dictionary<char, int> _frequencies;
        private readonly string input;

        public Tree(string input)
        {
            this.input = input;
            nodes = new List<Node>();
            _frequencies = new Dictionary<char, int>();
            Build();
        }

        /// <summary>
        /// Build Haffman's tree
        /// </summary>
        private void Build()
        {
                foreach (var s in input)
                {
                    if (!_frequencies.ContainsKey(s))
                    {
                        _frequencies.Add(s, 0);
                    }

                    _frequencies[s]++;
                }

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

        public BitArray Encode()
        {
            var encodedSource = new List<bool>();

            foreach (var i in input)
            {
                var encodedSymbol = _root.Traverse(i, new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            var bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        private bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

        public string Decode(BitArray bits)
        {
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
