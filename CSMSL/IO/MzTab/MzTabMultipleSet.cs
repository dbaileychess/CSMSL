// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabMultipleSet.cs) is part of CSMSL.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.IO.MzTab
{
    public class MzTabMultipleSet<T>
    {
        private readonly Dictionary<int, List<T>> _data;

        public int MaxIndex1
        {
            get { return _data.Keys.Max(); }
        }

        public int MaxIndex2
        {
            get { return _data.Values.Max(i => i.Count); }
        }

        public MzTabMultipleSet()
        {
            _data = new Dictionary<int, List<T>>();
        }

        public MzTabMultipleSet(int index1, T value)
        {
            _data = new Dictionary<int, List<T>>();
            var temp = new List<T> {value};
            _data.Add(index1, temp);
        }

        public T this[int index1, int index2]
        {
            get { return GetValue(index1, index2); }
            set { SetValue(index1, index2, value); }
        }

        public T GetValue(int index1, int index2)
        {
            index2 = index2 - MzTab.IndexBased;
            List<T> temp;
            if (_data.TryGetValue(index1, out temp))
            {
                return temp[index2];
            }
            return default(T);
        }

        public int AddValue(int index1, T value)
        {
            List<T> temp;
            if (!_data.TryGetValue(index1, out temp))
            {
                temp = new List<T>();
                _data.Add(index1, temp);
            }

            temp.Add(value);
            return temp.Count - 1 + MzTab.IndexBased;
        }

        public void SetValue(int index1, int index2, T value)
        {
            index2 = index2 - MzTab.IndexBased;
            List<T> temp;
            if (_data.TryGetValue(index1, out temp))
            {
                temp[index2] = value;
            }
            else
            {
                temp = new List<T>(index2 + 1);
                temp[index2] = value;
                _data.Add(index1, temp);
            }
        }

        public bool TryGetValue(int index1, int index2, out T value)
        {
            index2 = index2 - MzTab.IndexBased;
            value = default(T);
            List<T> temp;
            if (_data.TryGetValue(index1, out temp))
            {
                if (temp.Count >= index2)
                    return false;
                value = temp[index2];
                return true;
            }
            return false;
        }

        public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs(string baseName)
        {
            foreach (KeyValuePair<int, List<T>> values in _data)
            {
                int index1 = values.Key;
                for (int index2 = 0; index2 < values.Value.Count; index2++)
                {
                    string name = MzTab.GetArrayName(baseName, index1, index2 + MzTab.IndexBased);
                    string value = values.Value[index2].ToString();
                    yield return new KeyValuePair<string, string>(name, value);
                }
            }
        }
    }
}