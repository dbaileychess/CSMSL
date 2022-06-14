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

namespace CSMSL.Proteomics
{
    public class FragmentCollection : ICollection<Fragment>
    {
        private readonly AminoAcidPolymer _parent;

        private readonly List<Fragment> _fragments;

        public int Count
        {
            get { return _fragments.Count; }
        }

        public AminoAcidPolymer Parent
        {
            get { return _parent; }
        }

        public int NTerminalFragments
        {
            get { return _fragments.Count(frag => frag.Type < FragmentTypes.x); }
        }

        public int CTerminalFragments
        {
            get { return _fragments.Count(frag => frag.Type >= FragmentTypes.x); }
        }

        public FragmentCollection(IEnumerable<Fragment> fragments, AminoAcidPolymer parent = null)
        {
            _fragments = new List<Fragment>(fragments);
            _parent = parent;
        }

        public FragmentCollection(AminoAcidPolymer parent = null)
        {
            _fragments = new List<Fragment>();
            _parent = parent;
        }

        public FragmentCollection Filter(Func<Fragment, bool> predicate)
        {
            return new FragmentCollection(_fragments.Where(predicate));
        }

        public IEnumerator<Fragment> GetEnumerator()
        {
            return _fragments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _fragments.GetEnumerator();
        }

        public void Add(Fragment item)
        {
            _fragments.Add(item);
        }

        public void Clear()
        {
            _fragments.Clear();
        }

        public bool Contains(Fragment item)
        {
            return _fragments.Contains(item);
        }

        public void CopyTo(Fragment[] array, int arrayIndex)
        {
            _fragments.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Fragment item)
        {
            return _fragments.Remove(item);
        }
    }
}