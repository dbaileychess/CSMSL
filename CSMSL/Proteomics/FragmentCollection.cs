using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Proteomics
{
    public class FragmentCollection : ICollection<Fragment>
    {
        private readonly AminoAcidPolymer _parent;

        private readonly List<Fragment> _fragments;

        public int Count { get { return _fragments.Count; } }

        public AminoAcidPolymer Parent { get { return _parent; } }

        public int NTerminalFragments { get { return _fragments.Count(frag => frag.Type < FragmentTypes.x); } }

        public int CTerminalFragments { get { return _fragments.Count(frag => frag.Type >= FragmentTypes.x); } }

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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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
