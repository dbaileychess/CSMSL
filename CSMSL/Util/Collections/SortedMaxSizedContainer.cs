using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Util.Collections
{
    public class SortedMaxSizedContainer<T>
    {
        private const int SizeForLinearOrBinaryInsert = 20;

        private int _maxSize;
        private int _currentSize;
        private T[] _items;
        private IComparer<T> _comparer;

        /// <summary>
        /// The number of items
        /// </summary>
        public int Count { get { return _currentSize; } }

        public int MaxSize { get { return _maxSize; } }

        public SortedMaxSizedContainer(int maxSize, IComparer<T> comparer)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException("Max size must be a postive, non-zero value");
            }
            _maxSize = maxSize;
            _currentSize = 0;
            _items = new T[maxSize];
            _comparer = comparer;
        }

        public SortedMaxSizedContainer(int maxSize)
            : this(maxSize, Comparer<T>.Default) { }

        /// <summary>
        /// Adds an item to this structure
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True if the item was added, false otherwise</returns>
        public bool Add(T item)
        {
            if (_currentSize == 0)
            {
                _items[0] = item;
            }
            else if (_currentSize == _maxSize)
            {
                if (_comparer.Compare(_items[_currentSize - 1], item) <= 0)
                {
                    return false;
                }
                else
                {
                    Insert(item);
                    return true;
                }
            }
            else if (_currentSize == 1)
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
                if (_comparer.Compare(_items[_currentSize - 1], item) <= 0)
                {
                    _items[_currentSize] = item;
                }
                else
                {
                    Insert(item);
                }
            }
            _currentSize++;
            return true;
        }

        public void Clear()
        {
            Array.Clear(_items, 0, _currentSize);
            _currentSize = 0;
        }

        /// <summary>
        /// Insert the item into the data structure
        /// </summary>
        /// <param name="item">The item to insert</param>
        private void Insert(T item)
        {
            int start = 0;
            if (_currentSize >= SizeForLinearOrBinaryInsert)
            {
                start = Array.BinarySearch<T>(_items, 0, _currentSize, item, _comparer);
                if (start < 0)
                {
                    start = ~start;
                }
                ShiftAndInsert(item, start, _currentSize);
                return;
            }
            else
            {
                for (int i = start; i < _currentSize; i++)
                {
                    if (_comparer.Compare(_items[i], item) > 0)
                    {
                        ShiftAndInsert(item, i, _currentSize);
                        return;
                    }
                }
                _items[_currentSize] = item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="maxIndex"></param>
        private void ShiftAndInsert(T item, int index, int maxIndex)
        {
            if (maxIndex >= _maxSize)
            {
                maxIndex = _maxSize - 1;
            }
            for (int i = maxIndex; i > index; i--)
            {
                _items[i] = _items[i - 1];
            }
            _items[index] = item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > _currentSize)
                    throw new ArgumentOutOfRangeException();
                return _items[index];
            }
        }
    }
}
