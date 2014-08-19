// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (SortedMaxSizedContainer.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

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
    public class SortedMaxSizedContainer<T> : ICollection<T>
    {
        /// <summary>
        /// The breaking point between using a linear search or a binary search.
        /// </summary>
        private const int SizeForLinearOrBinarySearch = 20;

        private const int DefaultSizeOfArray = 4;

        /// <summary>
        /// The collection of items, in sorted order, 0 being the lowest value
        /// </summary>
        private T[] _items;

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
            _comparer = comparer;
            Clear();
        }

        /// <summary>
        /// Creates a new container with a specified maximum size and the default comparer
        /// </summary>
        /// <param name="maxSize">The maximum number of items to store in this container</param>
        public SortedMaxSizedContainer(int maxSize)
            : this(maxSize, Comparer<T>.Default)
        {
        }

        public override string ToString()
        {
            return string.Format("Count = {0:N0} (Max = {1:N0})", Count, MaxSize);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
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
                // The container is full, check if the item is lower than the last item in this container
                if (_comparer.Compare(_items[Count - 1], item) <= 0)
                {
                    return false;
                }

                // Special case for a max size of one, just replace the only item in the container
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
                    int arraySize = _items.Length;
                    if (Count == arraySize)
                    {
                        ExtendArray(arraySize*2);
                    }
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
            int size = Math.Min(MaxSize, DefaultSizeOfArray);
            _items = new T[size];
            Count = 0;
        }

        private void ExtendArray(int size)
        {
            int newSize = Math.Min(MaxSize, size);
            Array.Resize(ref _items, newSize);
        }

        private void ShrinkArray(int size)
        {
            int newSize = Math.Max(Math.Max(Count, size), DefaultSizeOfArray);
            if (_items.Length != newSize)
                Array.Resize(ref _items, newSize);
        }

        /// <summary>
        /// Insert the item into the data structure
        /// </summary>
        /// <param name="item">The item to insert</param>
        private void Insert(T item)
        {
            int arraySize = _items.Length;
            if (Count == arraySize)
            {
                ExtendArray(arraySize*2);
            }
            int start = 0;
            if (Count >= SizeForLinearOrBinarySearch)
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

        private bool RemoveAndShift(int index)
        {
            if (Count == 0 || index < 0 || index > Count)
            {
                return false;
            }

            for (int i = index; i + 1 < Count; i++)
            {
                _items[i] = _items[i + 1];
            }

            Count--;
            int arraySize = _items.Length;
            if (Count < (arraySize/2))
            {
                ShrinkArray(arraySize/2);
            }
            _items[Count] = default(T);
            return true;
        }

        public bool Remove(T item)
        {
            if (Count >= SizeForLinearOrBinarySearch)
            {
                int start = Array.BinarySearch(_items, 0, Count, item, _comparer);
                return start >= 0 && RemoveAndShift(start);
            }

            // Go backwards, to reduce number of shifts
            for (int i = Count - 1; i >= 0; i--)
            {
                if (_items[i].Equals(item))
                {
                    return RemoveAndShift(i);
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the item exists in this container
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>True if it exists, false otherwise</returns>
        public bool Contains(T item)
        {
            if (Count < SizeForLinearOrBinarySearch)
                return _items.Contains(item);
            return Array.BinarySearch(_items, 0, Count, item, _comparer) >= 0;
        }

        /// <summary>
        /// Copies the contents of this collection to an array
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The start index of the array</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, Count);
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
            get { return _items[index]; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.Take(Count).GetEnumerator();
        }

        /// <summary>
        /// This Collection is not read only by design
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}