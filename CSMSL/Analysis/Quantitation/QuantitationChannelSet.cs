using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantitationChannelSet: IMass
    {
        public string Name { get; set; }

        private SortedList<double, IQuantitationChannel> _channels;
                      
        private ChemicalFormula _totalFormula;

        private Mass _mass;

        public Mass AverageMass
        {
            get { return _mass / Count; }
        }

        public IQuantitationChannel this[int index]
        {
            get
            {
                return _channels.Values[index];
            }
        }

        public QuantitationChannelSet(string name)
        {
            Name = name; 
            _mass = new Mass();
            _channels = new SortedList<double, IQuantitationChannel>();          
            _totalFormula = new ChemicalFormula();
        }

        public IQuantitationChannel Add(IQuantitationChannel channel)
        {
            _channels.Add(channel.Mass.Monoisotopic, channel);
            _mass += channel.Mass;
            //_totalFormula.Add(channel);
            return channel;
        }       

        public IQuantitationChannel LighestChannel
        {
            get
            {
                if (_channels.Count < 1)
                    return null;
                return _channels.Values[0];
            }
        }

        public IQuantitationChannel HeaviestChannel
        {
            get
            {
                if (_channels.Count < 1)
                    return null;
                return _channels.Values[_channels.Count - 1];
            }
        }

        public IEnumerable<IQuantitationChannel> GetChannels()
        {
            return _channels.Values;
        }

        public MassRange MassRange
        {
            get
            {
                return new MassRange(LighestChannel.Mass.Monoisotopic, HeaviestChannel.Mass.Monoisotopic);
            }
        }

        public int Count
        {
            get
            {
                return _channels.Count;
            }
        }

        public override string ToString()
        {
            return Name;
        }


        Mass IMass.Mass
        {
            get { return AverageMass; }
        }
    }
}
