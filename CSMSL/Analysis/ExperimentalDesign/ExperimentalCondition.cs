using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalCondition
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public ExperimentalCondition(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
