using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Analysis.Quantitation;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalCondition
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public Sample Sample { get; private set; }

        public IQuantitationChannel QuantChannel {get; private set;}

        internal ExperimentalCondition(Sample sample, string name, string description)
        {
            Sample = sample;
            Name = name;
            Description = description;
        }

        public ExperimentalCondition SetQuantChannel(IQuantitationChannel channel)
        {
            QuantChannel = channel;
            return this;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Sample, Name);
        }
    }
}
