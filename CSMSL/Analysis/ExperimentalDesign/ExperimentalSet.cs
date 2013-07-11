using System.Collections.Generic;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalSet : IEnumerable<ExperimentalCondition>
    {       
        private HashSet<ExperimentalCondition> _conditions;

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
