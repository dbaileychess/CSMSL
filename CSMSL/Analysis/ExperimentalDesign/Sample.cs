using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class Sample
    {
        public string Name { get; set; }

        public string Description { get; set; }

        private List<ExperimentalCondition> _conditions;

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

    }
}
