// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (FragmentCollection.cs) is part of CSMSL.
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