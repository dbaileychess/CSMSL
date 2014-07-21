using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.IO.MzTab
{
    public class MzTabMultipleSet<T>
    {
        private readonly Dictionary<int,Dictionary<int,T>> _data;

        public int MaxIndex1 { get { return _data.Keys.Max(); } }
        public int MaxIndex2 { get { return _data.Values.SelectMany(i => i.Keys).Max(); } }

        public MzTabMultipleSet()
        {
            _data = new Dictionary<int, Dictionary<int, T>>();
        }

        public MzTabMultipleSet(int index1, int index2, T value)
        {
            _data = new Dictionary<int,Dictionary<int,T>>();
            var temp = new Dictionary<int,T>();
            temp.Add(index2, value);
            _data.Add(index1, temp);
        }

        public T this[int index1, int index2]
        {
            get { return GetValue(index1, index2); }
            set { SetValue(index1, index2, value); }
        }

        public T GetValue(int index1, int index2)
        {
            Dictionary<int, T> temp;
            if (_data.TryGetValue(index1, out temp))
            {
                T value;
                if (temp.TryGetValue(index2, out value))
                {
                    return value;
                }
            }
            return default(T);
        }

        public void SetValue(int index1, int index2, T value)
        {
            Dictionary<int, T> temp;
            if (_data.TryGetValue(index1, out temp))
            {
                temp[index2] = value;
            }
            else
            {
                temp = new Dictionary<int, T>();
                temp[index2] = value;
                _data.Add(index1, temp);
            }
        }

        public bool TryGetValue(int index1, int index2, out T value)
        {
            value = default(T);
            Dictionary<int, T> temp;
            if (_data.TryGetValue(index1, out temp))
            {
                if (temp.TryGetValue(index2, out value))
                {
                    return true;
                }
            }
            return false;
        }

        
    }
}
