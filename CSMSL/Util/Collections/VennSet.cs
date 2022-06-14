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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Util.Collections
{
    public class VennSet<T> : IEnumerable<T> where T : IEquatable<T>
    {
        private readonly Dictionary<T, T> _data;

        public int Count
        {
            get { return _data.Count; }
        }

        public VennSet(IEnumerable<T> items, string name = "")
        {
            Name = name;
            _data = items.ToDictionary(item => item);
        }

        public VennSet(string name = "")
        {
            Name = name;
            _data = new Dictionary<T, T>();
        }

        public string Name { get; set; }

        public void Add(T item)
        {
            _data[item] = item;
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                _data[item] = item;
            }
        }

        public bool TryGetValue(T key, out T value)
        {
            return _data.TryGetValue(key, out value);
        }

        public override string ToString()
        {
            return string.Format("{0} Count = {1:G0}", Name, Count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.Values.GetEnumerator();
        }
    }
}