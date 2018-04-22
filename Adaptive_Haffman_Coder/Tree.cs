using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaptive_Haffman_Coder
{
    class Tree
    {
        private Node root;
        private Node head;
        private Node tail;

        public Tree()
        {
            InitTree();
        }

        private void InitTree()
        {
            root = new Node
            {
                Prev = head,
                Next = tail
            };
            head = new Node { Next = root };

            tail = new Node
            {
                Frequency = int.MaxValue,
                Prev = root
            };
            
        }
        /// <summary>
        /// Encode input string into BitArray
        /// </summary>
        /// <param name="input">String fo encode</param>
        /// <returns>BitArray with encoded bits</returns>
        public BitArray Encode(string input)
        {
            var encodedBits = new List<bool>();
            // Encode all characters in input string
            foreach (var character in input)
            {
                // Check character for contains in Tree
                var charNode = FindCharacter(character);
                // Add new encoded range into encoded list
                encodedBits.AddRange(GetCharacterBits(charNode, character));

                //Rebuid tree with new character
                RebuildTree(charNode, character);
            }
            // Clear Haffman's tree
            ClearTree();
            return new BitArray(encodedBits.ToArray());
        }
        /// <summary>
        /// Decode bits into string
        /// </summary>
        /// <param name="bits"></param>
        /// <returns>Decoded string</returns>
        public string Decode(BitArray bits)
        {
            var charFound = new List<char>();
            // Begin from root
            var currentDecodePos = root;
            var legtOverBits = new List<bool>(8);
            var bitIndex = 0;

            while (true)
            {
                // Check that node is leaf
                if (currentDecodePos.IsLeaf)
                {
                    // Looking for emty node. If we are using founded char 
                    // at the first time
                    if (currentDecodePos.IsEmpty)
                    {
                        const int charSize = 8;
                        // Add 8 bits from encoding for make character
                        for (; legtOverBits.Count < charSize && bitIndex < bits.Length; bitIndex++)
                        {
                            legtOverBits.Add(bits.Get(bitIndex));
                        }

                        // If We have character (8 bits), 
                        // Conver it from binary to char 
                        // and rebuild the Haffman's tree
                        if (legtOverBits.Count == charSize)
                        {
                            var charBits = new BitArray(legtOverBits.ToArray());
                            var s = "";
                            for (int i = 0; i < charBits.Count; i++)
                            {
                                s += charBits.Get(i) ? 1 : 0;
                            }
                            var b = new byte[1];
                            b[0] = Convert.ToByte(s, 2);

                            var newChar = Encoding.GetEncoding(1251).GetChars(b)[0];
                            charFound.Add(newChar);
                            legtOverBits.Clear();
                            RebuildTree(currentDecodePos, newChar);
                            // Return to the root
                            currentDecodePos = root;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // We are allready have character in the tree.
                    // Rebuild tree and go back to the root
                    else
                    {
                        charFound.Add(currentDecodePos.Character);
                        RebuildTree(currentDecodePos, currentDecodePos.Character);
                        currentDecodePos = root;
                    }
                }
                // Go to the Leaf
                else if(bitIndex < bits.Count)
                {
                    currentDecodePos = bits.Get(bitIndex) ? currentDecodePos.Left : currentDecodePos.Right;
                    bitIndex++;
                }
                // Run out
                else
                {
                    break;
                }
            }

            return new string(charFound.ToArray());
        }
        /// <summary>
        /// Build binary code of charater and add escape symbol
        /// </summary>
        /// <param name="charNode">Node with input character</param>
        /// <param name="character">input character</param>
        /// <returns></returns>
        private List<bool> GetCharacterBits(Node charNode, char character)
        {
            var bits = new List<bool>();

            // Go to the root from charNode and add reverse of escape symbol.
            // Stop when current will be a root (root has no parent)
            for (var current = charNode;current.Parent != null;current = current.Parent)
            {
                bits.Add(current.Parent.Right != current);
            }

            // Reverse bits for going from root to charNode
            bits.Reverse();

            // If character isn't in the tree add binary code of character
            if (charNode.IsEmpty)
            {
                var bStr = string.Concat(Encoding.GetEncoding(1251).GetBytes(character.ToString()).Select(b => Convert.ToString(b, 2)));
                // If bits.Count % 8 != 0 add 0 at the begin
                for (int i = 0; i < 8 - bStr.Length; i++)
                {
                    bits.Add(false);
                }
                foreach (var c in bStr)
                {
                    bits.Add(c != '0');
                }
            }

            return bits;
        }
        /// <summary>
        /// Rebuild and balanced the Haffman's tree
        /// </summary>
        /// <param name="charNode"></param>
        /// <param name="character"></param>
        private void RebuildTree(Node charNode,char character)
        {
            // Add new character into the tree if needed
            if (charNode.IsEmpty)
            {
                charNode = AddCharNode(character);
            }

            var nodeToUpdate = charNode;

            while (nodeToUpdate != null)
            {
                nodeToUpdate.Frequency++;

                if (!SiblingPropertiesHold(nodeToUpdate))
                {
                    
                    var nodeToSwitchPositionWith = FindNodeToSwitchWith(nodeToUpdate);
                    SwitchNodes(nodeToUpdate, nodeToSwitchPositionWith);
                }
                nodeToUpdate = nodeToUpdate.Parent;
            }
        }

        /// <summary>
        /// Switch position of given nodes
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void SwitchNodes(Node a, Node b)
        {

            var aParent = a.Parent;
            var bParent = b.Parent;

            if (aParent == bParent)
            {
                aParent.Left = b;
                aParent.Right = a;
            }
            else
            {
                if (aParent.Left == a)
                    aParent.Left = b;
                else
                    aParent.Right = b;

                if (bParent.Left == b)
                    bParent.Left = a;
                else
                    bParent.Right = a;

                a.Parent = bParent;
                b.Parent = aParent;
            }


            var aPrevious = a.Prev;
            var aNext = a.Next;
            var bPrevious = b.Prev;
            var bNext = b.Next;

            aPrevious.Next = b;
            b.Prev = aPrevious;

            if (a.Next == b)
            {
                b.Next = a;
                a.Prev = b;
            }
            else
            {
                b.Next = aNext;
                aNext.Prev = b;

                bPrevious.Next = a;
                a.Prev = bPrevious;
            }

            a.Next = bNext;
            bNext.Prev = a;
        }

        /// <summary>
        /// Find node to switch
        /// </summary>
        /// <param name="startingNode"></param>
        /// <returns></returns>
        private Node FindNodeToSwitchWith(Node startingNode)
        {
            var targetNode = startingNode.Next;

            while (startingNode.Frequency > targetNode.Frequency)
                targetNode = targetNode.Next;

            targetNode = targetNode.Prev;

            if (startingNode.Parent == targetNode)
                targetNode = targetNode.Prev;

            return targetNode;
        }

        /// <summary>
        /// we have to keep the list sorted with frequency
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool SiblingPropertiesHold(Node node)
        {
            if (node == root)
                return true;

            if (node.Next == root)
                return true;

            if (node.Next != node.Parent)
                return node.Frequency <= node.Next.Frequency;

            return node.Frequency <= node.Next.Next.Frequency;
        }

        private Node AddCharNode(char character)
        {
            var newNode = head.Next;
            newNode.Left = new Node();
            newNode.Right = new Node();

            newNode.Left.Parent = newNode;
            newNode.Right.Parent = newNode;

            newNode.Right.Character = character;

            head.Next = newNode.Left;
            head.Next.Prev = head;

            newNode.Left.Next = newNode.Right;
            newNode.Right.Prev = newNode.Left;

            newNode.Right.Next = newNode;
            newNode.Prev = newNode.Right;

            return newNode.Right;
        }
        /// <summary>
        /// Clear Haffman's tree and reset all fields
        /// </summary>
        public void ClearTree()
        {
            var currentNode = head;

            while (currentNode != null)
            {
                if (currentNode.Prev != null)
                    currentNode.Prev.Next = null;

                currentNode.Parent = null;
                currentNode.Left = null;
                currentNode.Right = null;
                currentNode.Prev = null;

                currentNode = currentNode.Next;
            }

            InitTree();
        }

        /// <summary>
        /// Return Node for character. If character is not in the tree
        /// return empty Node 
        /// </summary>
        /// <param name="character">Character to find</param>
        /// <returns></returns>
        private Node FindCharacter(char character)
        {
            //Begin from end of list
            var current = tail.Prev;

            while (true)
            {
                //Find Leaf
                if (current.IsLeaf)
                {
                    //Check node for contains character
                    if (current.IsEmpty || current.Character == character)
                    {
                        return current;
                    }
                }
                //next iter
                current = current.Prev;
            }
        }
    }
}
