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
        public List<Channel> Channels;
        public Dictionary<Sample, Channel> Samples;
        public Dictionary<ExperimentalCondition, List<Sample>> Conditions;

        public ExperimentalSet()
        {
            Channels = new List<Channel>();
            Samples = new Dictionary<Sample, Channel>();
            Conditions = new Dictionary<ExperimentalCondition, List<Sample>>();
        }

        public void AddChannel(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("null channel");
            }

            if (Channels.Contains(channel))
            {
                throw new ArgumentException("duplicate channel");
            }

            Channels.Add(channel);
        }
        
        public void AddSample(Sample sample, Channel channel)
        {
            if (sample == null)
            {
                throw new ArgumentNullException("null sample");
            }

            if (channel == null)
            {
                throw new ArgumentNullException("null channel");
            }

            if (Samples.ContainsKey(sample))
            {
                throw new ArgumentException("duplicate sample");
            }

            try
            {
                AddChannel(channel);
            }
            catch (ArgumentException)
            {
                // Channel already in list --> make sure it is not associated with another sample
                if (Samples.ContainsValue(channel))
                {
                    throw new ArgumentException("channel already associated with another sample");
                }
            }

            Samples.Add(sample, channel);

            List<Sample> conditionSamples = null;
            if (Conditions.TryGetValue(sample.Condition, out conditionSamples))
            {
                conditionSamples.Add(sample);
            }
            else
            {
                conditionSamples = new List<Sample>();
                conditionSamples.Add(sample);
                Conditions.Add(sample.Condition, conditionSamples);
            }
        }
    }
}
