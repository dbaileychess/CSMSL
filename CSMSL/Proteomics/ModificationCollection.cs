using System.Collections.Generic;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class ModificationCollection : ICollection<IMass>, IMass
    {
        public string Name { get; set; }

        private readonly List<IMass> _modifications;

        private Mass _totalMass;

        public Mass Mass { get { return _totalMass; } }

        public override string ToString()
        {            
            return Name;
        }

        public ModificationCollection(string name = "")
        {
            Name = name;
            _modifications = new List<IMass>(2);
            _totalMass = new Mass();
        }        

        public void Add(IMass item)
        {
            _modifications.Add(item);
            _totalMass.Add(item.Mass);
        }

        public void Clear()
        {
            _modifications.Clear();
            _totalMass = new Mass();
        }

        public bool Contains(IMass item)
        {
            return _modifications.Contains(item);
        }

        public void CopyTo(IMass[] array, int arrayIndex)
        {            
            _modifications.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _modifications.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IMass item)
        {
            if (_modifications.Remove(item))
            {
                _totalMass.Remove(item.Mass);
                return true;
            }
            return false;
        }

        public IEnumerator<IMass> GetEnumerator()
        {
            return _modifications.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _modifications.GetEnumerator();
        }
    }
}
