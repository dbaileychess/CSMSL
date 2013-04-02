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

        public ExperimentalCondition Condition { get; set; }

        public Sample(ExperimentalCondition condition, string name = "", string description = "")
        {
            Condition = condition;
            Name = name;
            Description = description;

            if (Name.Length == 0)
            {
                Name = Condition.Name;
            }

            if (Description.Length == 0)
            {
                Description = Condition.Description;
            }
        }

    }
}
