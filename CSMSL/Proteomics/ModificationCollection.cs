using System.Text;
using CSMSL.Chemistry;
using System.Collections.Generic;

namespace CSMSL.Proteomics
{
    public class ModificationCollection : ICollection<IMass>, IMass
    {
        private readonly List<IMass> _modifications;

        public double MonoisotopicMass { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IMass mod in _modifications)
            {
                sb.Append(mod);
                sb.Append(" | ");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 3, 3);
            }
            return sb.ToString();
        }

        public ModificationCollection(IMass mod1, IMass mod2)
        {
            _modifications = new List<IMass> {mod1, mod2};
            MonoisotopicMass = mod1.MonoisotopicMass + mod2.MonoisotopicMass;
        }

        public void Add(IMass item)
        {
            _modifications.Add(item);
            MonoisotopicMass += item.MonoisotopicMass;
        }

        public void Clear()
        {
            _modifications.Clear();
            MonoisotopicMass = 0;
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
                MonoisotopicMass -= item.MonoisotopicMass;
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
