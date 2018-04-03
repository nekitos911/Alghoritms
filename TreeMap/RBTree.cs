using System;
using System.Collections.Generic;

namespace lab3
{
    public class RBTree<T> where T : IComparable<T>
    {
        enum Color
        {
            Red,
            Black
        }

        private class Node //узел красно-черного дерева
        {
            public Node Left; //левый потомок
            public Node Right; //правый потомок
            public Node Parent; //родитель

            public Color Colour; // цвет

            public T Data; //значение

            //конструктор принимает значение узла
            public Node(T data)
            {
                this.Data = data;

                this.Left = null;
                this.Right = null;
                this.Parent = null;
                this.Colour = Color.Red;
            }
        }

        private Node root;
        private List<T> tree = new List<T>();
        public int Count { get; private set;}

        //Test
        public string GetRootColorTest()
        {
            return root.Colour == 0 ? "red" : "black";
        }

        public void DisplayTreeTest()
        {
            if (root == null)
            {
                Console.WriteLine("Tree is clear");
                return;
            }
            if (root != null)
            {
                InOrderDisplayTest(root);
            }
        }
        private void InOrderDisplayTest(Node current)
        {
            if (current != null)
            {
                InOrderDisplayTest(current.Left);
                Console.Write("({0}) - {1} ", current.Data, current.Colour);
                InOrderDisplayTest(current.Right);
            }
        }

        private int HeightTest(Node node)
        {
            if (node == null) return 0;
            //находим высоту правой и левой ветки, и из них берем максимальную
            return 1 + Math.Max(HeightTest(node.Left), HeightTest(node.Right));
        }

        public int GetHeightTest()
        {
            return HeightTest(root);
        }

        private void CheckNodeTest(Node current)
        {
            if (current != null)
            {
                CheckNodeTest(current.Left);
                if (current.Colour == Color.Red)
                {
                    if ((current.Left == null || current.Left.Colour == Color.Black) &&
                        (current.Right == null || current.Right.Colour == Color.Black))
                        colorTest = true;
                    else
                    {
                        colorTest = false;
                    }
                }
                CheckNodeTest(current.Right);
            }

        }

        private bool colorTest = true;
        public bool GetNodesColourTest()
        {
            CheckNodeTest(root);
            return colorTest;
        }

        public void Insert(T item)
        {
            Count++;
            var newItem = new Node(item);
            if (root == null)
            {
                root = newItem;
                root.Colour = Color.Black;
                return;
            }

            Node Y = null;
            var X = root;
            while (X != null)
            {
                Y = X;
                X = newItem.Data.CompareTo(Y.Data) < 0 ? X.Left : X.Right;
            }

            newItem.Parent = Y;
            if (Y == null)
            {
                root = newItem;
            }
            else if (newItem.Data.CompareTo(Y.Data) < 0)
            {
                Y.Left = newItem;
            }
            else
            {
                Y.Right = newItem;
            }

            InsertFixUp(newItem);
        }

        private void InsertFixUp(Node item)
        {
            while (item != root && item.Parent.Colour == Color.Red)
            {
                if (item.Parent == item.Parent.Parent.Left)
                {
                    var Y = item.Parent.Parent.Right;
                    if (Y != null && Y.Colour == Color.Red) //Case1: Uncle is Red
                    {
                        item.Parent.Colour = Color.Black;
                        Y.Colour = Color.Black;
                        item.Parent.Parent.Colour = Color.Red;
                        item = item.Parent.Parent;
                    }
                    else //Case2: Uncle is black
                    {
                        if (item == item.Parent.Right)
                        {
                            item = item.Parent;
                            RotateLeft(item);
                        }
                        //Case 3
                        item.Parent.Colour = Color.Black;
                        item.Parent.Parent.Colour = Color.Red;
                        RotateRight(item.Parent.Parent);
                    }
                }
                else
                {
                    var X = item.Parent.Parent.Left;
                    if (X != null && X.Colour == Color.Red) //Case 1
                    {
                        item.Parent.Colour = Color.Black;
                        X.Colour = Color.Black;
                        item.Parent.Parent.Colour = Color.Red;
                        item = item.Parent.Parent;
                    }
                    else //Case 2
                    {
                        if (item == item.Parent.Left)
                        {
                            item = item.Parent;
                            RotateRight(item);
                        }

                        //Case 3
                        item.Parent.Colour = Color.Black;
                        item.Parent.Parent.Colour = Color.Red;
                        RotateLeft(item.Parent.Parent);
                    }
                }
                root.Colour = Color.Black;
            }
        }

        private void RotateRight(Node Y)
        {
            var X = Y.Left;
            Y.Left = X.Right;
            if (X.Right != null)
            {
                X.Right.Parent = Y;
            }

            if (X != null)
            {
                X.Parent = Y.Parent;
            }

            if (Y.Parent == null)
            {
                root = X;
            }
            else
            {
                if (Y == Y.Parent.Right)
                {
                    Y.Parent.Right = X;
                }
                else
                {
                    Y.Parent.Left = X;
                }
            }

            X.Right = Y;
            Y.Parent = X;
        }

        private void RotateLeft(Node X)
        {
            var Y = X.Right;
            X.Right = Y.Left;
            if (Y.Left != null)
            {
                Y.Left.Parent = X;
            }

            if (Y != null)
            {
                Y.Parent = X.Parent;
            }

            if (X.Parent == null)
            {
                root = Y;
            }
            else
            {
                if (X == X.Parent.Left)
                {
                    X.Parent.Left = Y;
                }
                else
                {
                    X.Parent.Right = Y;
                }
            }

            Y.Left = X;
            X.Parent = Y;
        }

        public bool Delete(T key)
        {
            var item = FindNode(key);
            Node X = null;
            Node Y = null;

            if (item == null) return false;

            Count--;
            
            if (item.Left == null || item.Right == null)
            {
                Y = item;
            }
            else
            {
                Y = TreeSuccessor(item);
            }

            X = Y.Left ?? Y.Right;
            if (X != null)
            {
                X.Parent = item.Parent;
                if (item.Parent == null)
                    root = X;
                else if (item == item.Parent.Left)
                    item.Parent.Left = X;
                else
                    item.Parent.Right = X;

                item.Left = item.Right = item.Parent = null;

                if (item.Colour == Color.Black)
                    DeleteFixUp(X);
            }
            else if (item.Parent == null)
            {
                root = null;
            }
            else
            {
                if (item.Colour == Color.Black)
                    DeleteFixUp(item);

                if (item.Parent != null)
                {
                    if (item == item.Parent.Left)
                        item.Parent.Left = null;
                    else if (item == item.Parent.Right)
                        item.Parent.Right = null;
                    item.Parent = null;
                }
            }

            return true;
        }

        private void DeleteFixUp(Node X)
        {

            while (X != root && X.Colour == Color.Black)
            {
                if (X == X.Parent.Left)
                {
                    var W = X.Parent.Right;
                    if (W.Colour == Color.Red)
                    {
                        W.Colour = Color.Black; //case 1
                        X.Parent.Colour = Color.Red; //case 1
                        RotateLeft(X.Parent); //case 1
                        W = X.Parent.Right; //case 1
                    }

                    Color tmpColor1 = Color.Red;
                    Color tmpColor2 = Color.Red;
                    if (W.Left == null || W.Left.Colour == Color.Black) tmpColor1 = Color.Black;
                    if (W.Right == null || W.Right.Colour == Color.Black) tmpColor2 = Color.Black;
                    if  (tmpColor1 == Color.Black && tmpColor2 == Color.Black)
                    {
                        W.Colour = Color.Red; //case 2
                        X = X.Parent; //case 2
                    }
                    else
                    {
                        if (W.Right.Colour == Color.Black)
                        {
                            W.Left.Colour = Color.Black; //case 3
                            W.Colour = Color.Red; //case 3
                            RotateRight(W); //case 3
                            W = X.Parent.Right; //case 3
                        }

                        W.Colour = X.Parent.Colour; //case 4
                        X.Parent.Colour = Color.Black; //case 4
                        W.Right.Colour = Color.Black; //case 4
                        RotateLeft(X.Parent); //case 4
                        X = root; //case 4
                    }
                }
                else //mirror code from above with "right" & "left" exchanged
                {
                    var W = X.Parent.Left;
                    if (W.Colour == Color.Red)
                    {
                        W.Colour = Color.Black;
                        X.Parent.Colour = Color.Red;
                        RotateRight(X.Parent);
                        W = X.Parent.Left;
                    }

                    Color tmpColor1 = Color.Red;
                    Color tmpColor2 = Color.Red;
                    if (W.Left == null || W.Left.Colour == Color.Black) tmpColor1 = Color.Black;
                    if (W.Right == null || W.Right.Colour == Color.Black) tmpColor2 = Color.Black;
                    if (tmpColor1 == Color.Black && tmpColor2 == Color.Black)
                    {
                        W.Colour = Color.Red; //case 2
                        X = X.Parent; //case 2
                    }
                    else
                    {
                        if (W.Left.Colour == Color.Black)
                        {
                            W.Right.Colour = Color.Black;
                            W.Colour = Color.Red;
                            RotateLeft(W);
                            W = X.Parent.Left;
                        }

                        W.Colour = X.Parent.Colour;
                        X.Parent.Colour = Color.Black;
                        W.Left.Colour = Color.Black;
                        RotateRight(X.Parent);
                        X = root;
                    }
                }
            }

            if (X != null)
                X.Colour = Color.Black;
        }
        private Node TreeSuccessor(Node X)
        {
            if (X.Left != null)
            {
                return Minimum(X);
            }
            else
            {
                var Y = X.Parent;
                while (Y != null && X == Y.Right)
                {
                    X = Y;
                    Y = Y.Parent;
                }
                return Y;
            }
        }
       
        private Node Minimum(Node X)
        {
            while (X.Left.Left != null)
            {
                X = X.Left;
            }
            if (X.Left.Right != null)
            {
                X = X.Left.Right;
            }
            return X;
        }

        private Node FindNode(T key)
        {
            var isFound = false;
            var tmp = root;
            Node item = null;
            while (!isFound)
            {
                if (tmp == null)
                {
                    break;
                }

                if (key.CompareTo(tmp.Data) < 0)
                {
                    tmp = tmp.Left;
                }

                else if (key.CompareTo(tmp.Data) > 0)
                {
                    tmp = tmp.Right;
                }

                else if (key.CompareTo(tmp.Data) == 0)
                {
                    isFound = true;
                    item = tmp;
                }
            }
            return isFound ? item : default(Node);
        }

        public T Find(T key)
        {
            return FindNode(key) == null ? default(T) : FindNode(key).Data;
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void Clear()
        {
            tree.Clear();
            root = null;
            Count = 0;
        }
        public List<T> GetTree()
        {   tree.Clear();       
            Tree(root);
            return tree;
        }

        private void Tree(Node current)
        {
            if (current == null) return;
            Tree(current.Left);
            tree.Add(current.Data);
            Tree(current.Right);
        }
    }
}