// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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