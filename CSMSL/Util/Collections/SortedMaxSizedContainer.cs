using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Util.Collections
{
    public class SortedMaxSizedContainer<T> : IEnumerable<T>
    {
        /// <summary>
        /// The breaking point between using a linear search or a binary search.
        /// </summary>
        private const int SizeForLinearOrBinaryInsert = 20;

        private readonly T[] _items;
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// Gets the number of items that are currently stored in this container
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the max number of items to store in this container
        /// </summary>
        public int MaxSize { get; private set; }

        public SortedMaxSizedContainer(int maxSize, IComparer<T> comparer)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException("maxSize", "Max size must be a positive, non-zero value");
            }
            MaxSize = maxSize;
            Count = 0;
            _items = new T[maxSize];
            _comparer = comparer;
        }

        public SortedMaxSizedContainer(int maxSize)
            : this(maxSize, Comparer<T>.Default) { }

        public override string ToString()
        {
            return string.Format("Count = {0:N0} (Max = {1:N0})", Count, MaxSize);
        }

        /// <summary>
        /// Adds an item to this container
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True if the item was added, false otherwise</returns>
        public bool Add(T item)
        {
            if (Count == 0)
            {
                _items[0] = item;
            }
            else if (Count == MaxSize)
            {
                if (_comparer.Compare(_items[Count - 1], item) <= 0)
                {
                    return false;
                }
                if (MaxSize == 1)
                {
                    _items[0] = item;
                }
                else
                {
                    Insert(item);
                }
                return true;
            }
            else if (Count == 1)
            {
                if (_comparer.Compare(_items[0], item) <= 0)
                {
                    _items[1] = item;
                }
                else
                {
                    _items[1] = _items[0];
                    _items[0] = item;
                }
            }
            else
            {
                if (_comparer.Compare(_items[Count - 1], item) <= 0)
                {
                    _items[Count] = item;
                }
                else
                {
                    Insert(item);
                }
            }
            Count++;
            return true;
        }

        /// <summary>
        /// Remove all the items from this container
        /// </summary>
        public void Clear()
        {
            Array.Clear(_items, 0, Count);
            Count = 0;
        }

        /// <summary>
        /// Insert the item into the data structure
        /// </summary>
        /// <param name="item">The item to insert</param>
        private void Insert(T item)
        {
            int start = 0;
            if (Count >= SizeForLinearOrBinaryInsert)
            {
                start = Array.BinarySearch(_items, 0, Count, item, _comparer);
                if (start < 0)
                {
                    start = ~start;
                }
                ShiftAndInsert(item, start, Count);
                return;
            }
            for (int i = start; i < Count; i++)
            {
                if (_comparer.Compare(_items[i], item) > 0)
                {
                    ShiftAndInsert(item, i, Count);
                    return;
                }
            }
            _items[Count] = item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <param name="maxIndex"></param>
        private void ShiftAndInsert(T item, int index, int maxIndex)
        {
            if (maxIndex >= MaxSize)
            {
                maxIndex = MaxSize - 1;
            }
            for (int i = maxIndex; i > index; i--)
            {
                _items[i] = _items[i - 1];
            }
            _items[index] = item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.Take(Count).GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Count)
                    throw new IndexOutOfRangeException();
                return _items[index];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.Take(Count).GetEnumerator();
        }
    }
}
