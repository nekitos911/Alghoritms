using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4
{
   
    class HashMap<TKey, TValue>
    {
        internal class KeyValue<TKey, TValue>
        {
            public TKey Key { get; }
            public TValue Value { get; set; }
            public KeyValue<TKey, TValue> Next { get; set; }
            public int Hash { get; }

            public KeyValue(int hash, TKey key, TValue value, KeyValue<TKey, TValue> next)
            {
                Hash = hash;
                Key = key;
                Value = value;
                Next = next;
            }

            public override bool Equals(object obj)
            {
                if (obj == this) return true;
                var e = (KeyValue<TKey, TValue>)obj;
                return e != null && Key.Equals(e.Key) && Value.Equals(e.Value);
            }

            public override int GetHashCode()
            {
                return Key.GetHashCode() ^ Value.GetHashCode();
            }
        }
        public const int DefaultCapacity = 1 << 4;
        public const int MaxCapacity = 1 << 30;
        public const float DefaultLoadFactor = 2.0f;

        private int capacity;
        public float LoadFactor { get; set; }
        public int Threshold { get; private set; }
        /// <summary>
        /// The total number of entries in the hash table.
        /// </summary>
        public int Count { get; private set; }
        private KeyValue<TKey, TValue>[] table;

        public bool IsEmty => Count == 0;
        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="capacity">the initial capacity of the hashmap.</param>
        /// <param name ="loadFactor">the load factor of the hashmap.</param>
        public HashMap(int capacity, float loadFactor)
        {
            if (capacity < 0)
                throw new ArgumentException();
            if (capacity > MaxCapacity)
            {
                capacity = MaxCapacity;
            }

            if (loadFactor <= 0 || float.IsNaN(loadFactor))
            {
                throw new ArgumentException();
            }

            this.LoadFactor = loadFactor;
            this.capacity = capacity;
            table = new KeyValue<TKey, TValue>[capacity];
            Threshold = (int) Math.Min(capacity * loadFactor, MaxCapacity + 1);
        }

        public HashMap(int capacity) : this(capacity, DefaultLoadFactor) { }

        public HashMap() : this(DefaultCapacity,DefaultLoadFactor) { }

        private uint Hash(object key)
        {
            uint h;
            return (key == null) ? 0 : (h = (uint) key.GetHashCode()) ^ (h >> 16);
        }
        /// <summary>
        /// Add new pair into hash table
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>default type value if table is not contain this key.
        /// Or replace previos value and return it </returns>
        public TValue Add(TKey key, TValue value)
        {
            if(value == null) 
                throw new ArgumentNullException();
            var tab = table;
            var hash = key.GetHashCode();
            var index = (hash & 0x7FFFFFFF) % tab.Length;
            var entry = tab[index];
            for (; entry != null; entry = entry.Next)
            {
                if ((entry.Hash == hash) && entry.Key.Equals(key))
                {
                    var old = entry.Value;
                    entry.Value = value;
                    return old;
                }
            }

            AddVal(hash, key, value, index);
            return default(TValue);
        }
        /// <summary>
        /// Add new pair
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        private void AddVal(int hash, TKey key, TValue value, int index)
        {
            var tab = table;
            if (Count >= Threshold)
            {
                ReHash();

                tab = table;
                hash = key.GetHashCode();
                index = (hash & 0x7FFFFFFF) % tab.Length;
            }

            var e = tab[index];
            tab[index] = new KeyValue<TKey, TValue>(hash,key,value,e);
            Count++;
        }

        private void ReHash()
        {
            var oldCap = table.Length;
            var oldTab = table;

            var newCap = (oldCap << 1) + 1;
            if (newCap - MaxCapacity > 0)
            {
                if(oldCap == MaxCapacity)
                    return;
                newCap = MaxCapacity;
            }

            var newTab = new KeyValue<TKey, TValue>[newCap];

            Threshold = (int)Math.Min(newCap * LoadFactor, MaxCapacity + 1);
            table = newTab;
            for (int i = oldCap; i-- > 0;)
            {
                for (var old = oldTab[i]; old != null;)
                {
                    var e = old;
                    old = old.Next;

                    var index = (e.Hash & 0x7FFFFFFF) % newCap;
                    e.Next = newTab[index];
                    newTab[index] = e;
                }
            }
        }

        public TValue Remove(TKey key)
        {
            var tab = table;
            var hash = key.GetHashCode();
            var index = (hash & 0x7FFFFFFF) % tab.Length;
            var e = tab[index];
            for (KeyValue<TKey,TValue> prev = null; e != null; prev = e, e = e.Next)
            {
                if ((e.Hash == hash) && e.Key.Equals(key))
                {
                    if (prev != null)
                    {
                        prev.Next = e.Next;
                    }
                    else
                    {
                        tab[index] = e.Next;
                    }
                    Count--;
                    var oldValue = e.Value;
                    e.Value = default(TValue);
                    return oldValue;
                }
            }
            return default(TValue);

        }

        public void Clear()
        {
            if (table == null || Count <= 0) return;
            Count = 0;
            for (int i = 0; i < table.Length; i++)
            {
                table[i] = null;
            }
        }
        
        public TValue this[TKey key]
        {
            get
            {
                var tab = table;
                var h = key.GetHashCode();
                var index = (h & 0x7FFFFFFF) % tab.Length;
                for (var i = tab[index]; i != null; i = i.Next)
                {
                    if ((i.Hash == h) && i.Key.Equals(key))
                    {
                        return i.Value;
                    }
                }
                return default(TValue);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var tmp = table;
            var index = 0;
            foreach (var t in tmp)
                if (t != null && index < Count)
                {
                    yield return new KeyValuePair<TKey, TValue>(t.Key, t.Value);
                    index++;
                }
        }

    }
}