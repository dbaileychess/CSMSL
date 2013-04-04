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

        public Sample Sample { get; private set; }

        internal ExperimentalCondition(Sample sample, string name, string description)
        {
            Sample = sample;
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Sample, Name);
        }
    }
}
