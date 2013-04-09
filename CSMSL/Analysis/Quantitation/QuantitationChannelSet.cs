using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantitationChannelSet: IMass
    {
        public string Name { get; set; }

        private SortedList<double, IQuantitationChannel> _channels;
                      
        private ChemicalFormula _totalFormula;

        private Mass _totalMass;

        public Mass AverageMass
        {
            get { return _totalMass / Count; }
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
            _totalMass = new Mass();
            _channels = new SortedList<double, IQuantitationChannel>();          
            _totalFormula = new ChemicalFormula();
        }

        public IQuantitationChannel Add(IQuantitationChannel channel)
        {
            _channels.Add(channel.Mass.Monoisotopic, channel);
            _totalMass += channel.Mass;
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

        #region Static 

        public static IEnumerable<Peptide> GetUniquePeptides(Peptide peptide)
        {           
            QuantitationChannelSet quantSetMod;
            HashSet<QuantitationChannelSet> sets = new HashSet<QuantitationChannelSet>();
            Dictionary<IQuantitationChannel, HashSet<int>> locations = new Dictionary<IQuantitationChannel, HashSet<int>>();   
            HashSet<int> residues;
            for (int i = 1; i <= peptide.Length; i++)
            {
                if (peptide.TryGetModification<QuantitationChannelSet>(i, out quantSetMod))
                {
                    sets.Add(quantSetMod);
                    foreach (IQuantitationChannel channel in quantSetMod.GetChannels())
                    {
                        if (locations.TryGetValue(channel, out residues))
                        {
                            residues.Add(i);
                        }
                        else
                        {
                            residues = new HashSet<int>() { i };
                            locations.Add(channel, residues);
                        }
                    }
                }
            }

            if (sets.Count == 0)
            {
                yield return new Peptide(peptide, true);
            }
            else if (sets.Count == 1)
            {  
                foreach (QuantitationChannelSet set in sets)
                {
                    foreach (IQuantitationChannel channel in set.GetChannels())
                    {
                        Peptide toReturn = new Peptide(peptide, true);
                        toReturn.SetModification(channel, locations[channel].ToArray());                       
                        yield return toReturn;
                    }
                }
            }
            else
            {
                List<HashSet<IQuantitationChannel>> quantChannels = new List<HashSet<IQuantitationChannel>>();

                GetUniquePeptides_helper(sets.ToList(), 0, new HashSet<IQuantitationChannel>(), quantChannels);

                foreach (HashSet<IQuantitationChannel> channelset in quantChannels)
                {
                    Peptide toReturn = new Peptide(peptide, true);
                    foreach (IQuantitationChannel channel in channelset)
                    {
                        toReturn.SetModification(channel, locations[channel].ToArray());
                    }                   
                    yield return toReturn;
                }
            }
            yield break;
        }

        private static void GetUniquePeptides_helper(IList<QuantitationChannelSet> sets, int setIndex, HashSet<IQuantitationChannel> channels, List<HashSet<IQuantitationChannel>> result)
        {
            if (setIndex >= sets.Count)
            {
                result.Add(new HashSet<IQuantitationChannel>(channels));               
            }
            else
            {
                QuantitationChannelSet currentSet = sets[setIndex];

                foreach (IQuantitationChannel channel in currentSet.GetChannels())
                {
                    channels.Add(channel);
                    GetUniquePeptides_helper(sets, setIndex + 1, channels, result);
                    channels.Remove(channel);
                }
            }
        }
        

        #endregion
    }
}
