using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using lab3;

namespace Lab3
{
    class RBTreeTest
    {
        private RBTree<int> tree = new RBTree<int>();
        private int count;
        private Stopwatch sw;

        public void StartTest()
        {
            Console.WriteLine("Test with " + count + " elements: ");
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("***Insert*** " + count + " elements test: " + InsertTimeTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("***Search*** test: " + SearchTimeTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Vertexs count after insert(using List):  " + VertexListTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Vertexs count after insert(using Count):  " + VertexCountTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Root color(passed if root is black):  " +
                              (tree.GetRootColorTest() == "black" ? "passed" : "failer"));
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Descendants are black when parent is red test:  " + NodesColorTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Tree height test(h(root) <= 2log(N + 1)):  " + HeightTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("***Delete*** " + count / 2 + " elements test: " + DeleteTimeTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Root color(passed if root is black):  " +
                              (tree.GetRootColorTest() == "black" ? "passed" : "failer"));
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Descendants are black when parent is red test:  " + NodesColorTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Tree height test(h(root) <= 2log(N + 1)):  " + HeightTest());
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
        }

        public void ReTest(int newCount)
        {
            count = newCount;
            tree = new RBTree<int>();
            StartTest();
        }

        private string HeightTest()
        {
            return (double)tree.GetHeightTest() + 1 <= 2 * Math.Log(tree.Count + 1, 2) ? "passed" : "failer";
        }

        private string NodesColorTest()
        {
            return tree.GetNodesColourTest() ? "passed" : "failer";
        }

        private string VertexListTest()
        {
            return tree.GetTree().Count == count ? "passed" : "failer";
        }

        private string VertexCountTest()
        {
            return tree.Count == count ? "passed" : "failer";
        }

        public RBTreeTest(int countToAdd)
        {
            count = countToAdd;
        }

        private string InsertTimeTest()
        {
            sw = Stopwatch.StartNew();
            for (int i = 1; i <= count; i++)
            {
                tree.Insert(i);
                //DisplayTree();
            }
            /*tree.Insert(13);
            DisplayTree();
            tree.Insert(8);
            DisplayTree();
            tree.Insert(1);
            DisplayTree();
            tree.Insert(6);
            tree.Insert(11);
            tree.Insert(17);
            tree.Insert(15);
            tree.Insert(25);
            tree.Insert(22);
            tree.Insert(27);*/
            long elapsed = sw.ElapsedMilliseconds;
            var isPassed = true;
            var treeList = tree.GetTree();
            for (int i = 1; i <= count; i++)
            {
                if (treeList.Contains(i)) continue;
                isPassed = false;
                break;
            }

            var tmp = isPassed ? "passed in " : "failer in ";
            return  tmp + elapsed + "ms";
        }

        private string DeleteTimeTest(int countForDelete)
        {
            sw = Stopwatch.StartNew();
            for (int i = 1; i <= countForDelete; i++)
            {
                tree.Delete(i);
                //DisplayTree();
            }
            /*tree.Delete(27);
            tree.Delete(22);
            tree.Delete(25);
            tree.Delete(15);
            tree.Delete(17);
            tree.Delete(11);
            DisplayTree();
            tree.Delete(6);
            tree.Delete(1);
            tree.Delete(8);
            tree.Delete(13);*/
            long elapsed = sw.ElapsedMilliseconds;
            var isPassed = true;
            var treeList = tree.GetTree();
            for (int i = 1; i <= countForDelete; i++)
            {
                if (!treeList.Contains(i)) continue;
                isPassed = false;
                break;
            }
            var tmp = isPassed ? "passed in " : "failer in ";
            return tmp + elapsed + "ms";
        }

        private string DeleteTimeTest()
        {
            return DeleteTimeTest(count / 2);
        }

        private string SearchTimeTest()
        {
            var element = new Random().Next(1, count - 1);
            var isFound = 0;
            sw = Stopwatch.StartNew();
            isFound = tree.Find(element);
            return isFound == 0 ? "failer in " : "passed in " +  $"{sw.ElapsedMilliseconds} ms";
        }

        public void DisplayTree()
        {
            tree.DisplayTreeTest();
            Console.WriteLine();
        }
    }
}
