// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (VennSet.cs) is part of CSMSL.
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