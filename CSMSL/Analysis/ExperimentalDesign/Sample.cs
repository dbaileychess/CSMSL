using System.Collections.Generic;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class Sample : IEnumerable<ExperimentalCondition>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        private readonly List<ExperimentalCondition> _conditions;
         
        public ExperimentalCondition AddCondition(string name, string description = "")
        {
            ExperimentalCondition condition = new ExperimentalCondition(this, name, description);
            _conditions.Add(condition);
            return condition;
        }

        public Sample(string name, string description = "")
        {         
            Name = name;
            Description = description;
            _conditions = new List<ExperimentalCondition>();
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
