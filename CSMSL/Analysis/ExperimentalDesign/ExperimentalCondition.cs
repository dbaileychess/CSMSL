using CSMSL.Analysis.Quantitation;
using System.Collections.Generic;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalCondition : IEnumerable<IQuantitationChannel>
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public Sample Sample { get; private set; }

        private HashSet<IQuantitationChannel> QuantChannels { get; set; }

        public MassTolerance MassTolerance { get; set; }

        internal ExperimentalCondition(Sample sample, string name, string description)
        {
            Sample = sample;
            Name = name;
            Description = description;
            QuantChannels = new HashSet<IQuantitationChannel>();
        }

        public ExperimentalCondition AddQuantChannel(params IQuantitationChannel[] channels)
        {
            foreach (IQuantitationChannel channel in channels)
            {
                QuantChannels.Add(channel);
            }
            return this;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Sample, Name);
        }


        public IEnumerator<IQuantitationChannel> GetEnumerator()
        {
            return QuantChannels.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return QuantChannels.GetEnumerator();
        }
    }
}
