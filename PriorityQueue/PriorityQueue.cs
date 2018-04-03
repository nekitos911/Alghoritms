using System;

using System.Collections.Generic;

namespace Lab2
{
    public class Heap<T> where T : IComparable<T>
    {
        private List<T> heap = new List<T>();

        public int Count => heap.Count;

        private void heapify()
        {
            var i = 0;

            while(i < Count)
            {
                var leftChild = 2 * i + 1;
                var rightChild = 2 * i + 2;
                var largestChild = i;

                if (leftChild < Count && heap[leftChild].CompareTo(heap[largestChild]) > 0)
                {
                    largestChild = leftChild;
                }

                if (rightChild < Count && heap[rightChild].CompareTo(heap[largestChild]) > 0)
                {
                    largestChild = rightChild;
                }

                if (largestChild == i)
                {
                    break;
                }

                var temp = heap[i];
                heap[i] = heap[largestChild];
                heap[largestChild] = temp;
                i = largestChild;
            }
        }

        public void Add(T element)
        {
            heap.Add(element);
            var c = heap.Count - 1;
            var parent = (c - 1) >> 1;
            while (c > 0 && heap[c].CompareTo(heap[parent]) > 0)
            {
                var tmp = heap[c];
                heap[c] = heap[parent];
                heap[parent] = tmp;
                c = parent;
                parent = (c - 1) >> 1;
            }
        }

        public T Peek()
        {
            return heap[0];
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public T RemoveMax()
        {
            var ret = heap[0];
            heap[0] = heap[Count - 1];
            heap.RemoveAt(Count - 1);
            heapify();
            return ret;
        }

    }
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private class Node : IComparable<Node>
        {
            public int Priority;
            public T Element;

            public int CompareTo(Node other)
            {
                return Priority.CompareTo(other.Priority);
            }
        }
        private Heap<Node> heap = new Heap<Node>();
        public int Count => heap.Count;

        public void Add(int priority, T element)
        {
            try
            {
                if (element == null)
                    throw new ArgumentNullException();
                heap.Add(new Node() {Priority = priority, Element = element});
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public T Peek()
        {
            try
            {
                if (Count == 0)
                    throw new IndexOutOfRangeException("Exception: Queue is empty!");
                return heap.Peek().Element;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default(T);
        }

        public bool IsEmpty()
        {
            return heap.IsEmpty();
        }

        public T RemoveMax()
        {
            try
            {
                if (Count == 0)
                    throw new IndexOutOfRangeException("Exception: Queue is empty!");
                return heap.RemoveMax().Element;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return default(T);
        }
    }
}
