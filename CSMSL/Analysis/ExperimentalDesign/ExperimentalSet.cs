using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalSet : IEnumerable<ExperimentalCondition>
    {       
        private readonly HashSet<ExperimentalCondition> _conditions;

        public string Name { get; private set; }

        public ExperimentalSet(string name = "")
        {
            Name = name;
            _conditions = new HashSet<ExperimentalCondition>();
        }

        public void Add(ExperimentalCondition condition)
        {
            _conditions.Add(condition);        
        }

        public IEnumerable<Modification> GetAllModifications()
        {
            return _conditions.SelectMany(c => c.Modifications);
        } 

        public bool Contains(Modification mod)
        {
            return _conditions.Any(c => c.Modifications.Contains(mod));
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerator<ExperimentalCondition> GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }
    }
}
