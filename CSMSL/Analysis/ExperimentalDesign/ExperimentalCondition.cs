using System.Collections.Generic;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalCondition : IEnumerable<Modification>
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public int Count { get { return Modifications.Count; } }

        public HashSet<Modification> Modifications { get; set; }
        
        public ExperimentalCondition(string name, string description = "")
        {
            Name = name;
            Description = description;
            Modifications = new HashSet<Modification>();
        }

        public void AddModification(Modification mod)
        {
            Modifications.Add(mod);
        }

        public void AddModifications(params Modification[] mods)
        {
            foreach (Modification mod in mods)
            {
                Modifications.Add(mod);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerator<Modification> GetEnumerator()
        {
            return Modifications.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Modifications.GetEnumerator();
        }
    }
}
