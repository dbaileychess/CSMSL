using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using CSMSL.Analysis.Quantitation;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalSet
    {
        public List<IQuantitationChannel> IQuantitationChannels;
        public Dictionary<Sample, IQuantitationChannel> Samples;
        public Dictionary<ExperimentalCondition, List<Sample>> Conditions;

        private Dictionary<IQuantitationChannel, ExperimentalCondition> _data;


        public ExperimentalSet()
        {
            IQuantitationChannels = new List<IQuantitationChannel>();
            Samples = new Dictionary<Sample, IQuantitationChannel>();
            Conditions = new Dictionary<ExperimentalCondition, List<Sample>>();
            _data = new Dictionary<IQuantitationChannel, ExperimentalCondition>();
        }

        public void Add(ExperimentalCondition condition, IQuantitationChannel IQuantitationChannel)
        {
            _data.Add(IQuantitationChannel, condition);
        }


        public void AddIQuantitationChannel(IQuantitationChannel IQuantitationChannel)
        {
            if (IQuantitationChannel == null)
            {
                throw new ArgumentNullException("null IQuantitationChannel");
            }

            if (IQuantitationChannels.Contains(IQuantitationChannel))
            {
                throw new ArgumentException("duplicate IQuantitationChannel");
            }

            IQuantitationChannels.Add(IQuantitationChannel);
        }
        
        public void AddSample(Sample sample, IQuantitationChannel IQuantitationChannel)
        {
            if (sample == null)
            {
                throw new ArgumentNullException("null sample");
            }

            if (IQuantitationChannel == null)
            {
                throw new ArgumentNullException("null IQuantitationChannel");
            }

            if (Samples.ContainsKey(sample))
            {
                throw new ArgumentException("duplicate sample");
            }

            try
            {
                AddIQuantitationChannel(IQuantitationChannel);
            }
            catch (ArgumentException)
            {
                // IQuantitationChannel already in list --> make sure it is not associated with another sample
                if (Samples.ContainsValue(IQuantitationChannel))
                {
                    throw new ArgumentException("IQuantitationChannel already associated with another sample");
                }
            }

            Samples.Add(sample, IQuantitationChannel);

            //List<Sample> conditionSamples = null;
            //if (Conditions.TryGetValue(sample.Condition, out conditionSamples))
            //{
            //    conditionSamples.Add(sample);
            //}
            //else
            //{
            //    conditionSamples = new List<Sample>();
            //    conditionSamples.Add(sample);
            //    Conditions.Add(sample.Condition, conditionSamples);
            //}
        }
    }
}
