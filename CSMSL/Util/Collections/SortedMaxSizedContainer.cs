using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Util.Collections
{
    /// <summary>
    /// A container that stores items in sorted order with a specified maximum capacity.
    /// <example>
    /// Storing the top 5 peptide spectral matches for a given spectrum, sorted on match score
    /// </example>
    /// </summary>
    /// <typeparam name="T">The type of the object to store in this container</typeparam>
    public class SortedMaxSizedContainer<T> : IEnumerable<T>
    {
        /// <summary>
        /// The breaking point between using a linear search or a binary search.
        /// </summary>
        private const int SizeForLinearOrBinaryInsert = 20;

        /// <summary>
        /// The collection of items, in sorted order, 0 being the loweset value
        /// </summary>
        private readonly T[] _items;

        /// <summary>
        /// The comparer to compare two items in this collection
        /// </summary>
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// Gets the number of items that are currently stored in this container
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the max number of items that can be stored in this container
        /// </summary>
        public int MaxSize { get; private set; }

        /// <summary>
        /// Creates a new container with a specified maximum size and comparer
        /// </summary>
        /// <param name="maxSize">The maximum number of items to store in this container</param>
        /// <param name="comparer">The comparer to compare two items in this collection</param>
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

        /// <summary>
        /// Creates a new container with a specified maximum size and the default comparer
        /// </summary>
        /// <param name="maxSize">The maximum number of items to store in this container</param>
        public SortedMaxSizedContainer(int maxSize)
            : this(maxSize, Comparer<T>.Default) { }


        public override string ToString()
        {
            return string.Format("Count = {0:N0} (Max = {1:N0})", Count, MaxSize);
        }

        /// <summary>
        /// Attempts to add an item to this container
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True if the item was added, false otherwise</returns>
        public bool Add(T item)
        {
            if (Count == 0)
            {
                // Nothing is stored yet, just store the item as the lowest value
                _items[0] = item;
            }
            else if (Count == MaxSize)
            {
                // The conatiner is full, check if the item is lower than the last item in this conatiner
                if (_comparer.Compare(_items[Count - 1], item) <= 0)
                {
                    return false;
                }

                // Special case for a maxsize of one, just replace the only item in the conatiner
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
                ShiftAndInsert(item, start);
                return;
            }
            for (int i = start; i < Count; i++)
            {
                if (_comparer.Compare(_items[i], item) > 0)
                {
                    ShiftAndInsert(item, i);
                    return;
                }
            }
            _items[Count] = item;
        }

        /// <summary>
        /// Shift items down to make room for the new insert
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <param name="index">The index to insert the item at</param>
        private void ShiftAndInsert(T item, int index)
        {
            if (Count >= MaxSize)
            {
                Count = MaxSize - 1;
            }
            for (int i = Count; i > index; i--)
            {
                _items[i] = _items[i - 1];
            }
            _items[index] = item;
        }

        /// <summary>
        /// Get the enumerator of all non-null items in this container
        /// </summary>
        /// <returns>The enumerator of all non-null items in this container</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _items.Take(Count).GetEnumerator();
        }

        /// <summary>
        /// Get the item stored at this index
        /// </summary>
        /// <param name="index">The index of the item to get</param>
        /// <returns>The item stored at this index</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Count)
                    throw new IndexOutOfRangeException();
                return _items[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.Take(Count).GetEnumerator();
        }
    }
}
