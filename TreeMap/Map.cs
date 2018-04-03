using System;
using System.Collections.Generic;
using lab3;

namespace Lab3
{
    public interface IDeepCloneable
    {
        object DeepClone();
    }
    public interface IDeepCloneable<T> : IDeepCloneable
    {
        T DeepClone();
    }

    public class Map<TKey, TValue> : IDeepCloneable<Map<TKey,TValue>> where TKey : IComparable<TKey>
    {
        public class Node : IComparable<Node>
        {
            public TValue Value;
            public TKey Key;

            public int CompareTo(Node other)
            {
                return Key.CompareTo(other.Key);
            }
        }
        RBTree<Node> array = new RBTree<Node>();
        public Map<TKey,TValue> DeepClone()
        {
            var tmp = new RBTree<Node>();
            var list = array.GetTree();
            foreach (var e in list)
            {
                tmp.Insert(e);
            }
            return new Map<TKey, TValue>()
            {
                array = tmp
            };
        }
        object IDeepCloneable.DeepClone()
        {
            return DeepClone();
        }


        public void Add(TKey key, TValue value)
        {
            try
            {
                var tmp = array.Find(new Node() { Key = key, Value = value });
                if(tmp == null)
                    array.Insert(new Node { Key = key, Value = value });
                else throw new ArgumentException();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Exception: Key " + key + " is already exist: Add(" + key + "," + value + ")");
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                try
                {
                    var tmp = array.Find(new Node() {Key = key, Value = default(TValue)});
                    if (tmp == null)
                        throw new ArgumentException();
                    return tmp.Value;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("Exception : Key " + key + " is not found");
                    return default(TValue);
                }
            }
        }

        public bool Delete(TKey key)
        {
            return array.Delete(new Node() {Key = key, Value = default(TValue)});
        }

        public bool Contains(TKey key)
        {
            var tmp = array.Find(new Node() {Key = key, Value = default(TValue)});
            return tmp != null;
        }

        public bool IsEmpty()
        {
            return array.IsEmpty();
        }

        public void Clear()
        {
            array.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var tmp = array.GetTree();
            foreach (var t in tmp)
                yield return new KeyValuePair<TKey, TValue>(t.Key, t.Value);
        }

    }
}
