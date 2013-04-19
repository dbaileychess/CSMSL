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
         
        public QuantitationChannelSetMassType MassType { get; set; }

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

        public QuantitationChannelSet(string name, QuantitationChannelSetMassType massType = QuantitationChannelSetMassType.Average)
        {
            Name = name;
            MassType = massType;
            _totalMass = new Mass();
            _channels = new SortedList<double, IQuantitationChannel>();                    
        }

        public IQuantitationChannel Add(IQuantitationChannel channel)
        {
            _channels.Add(channel.ReporterMass.Monoisotopic, channel);
            _totalMass.Add(channel.Mass);        
            return channel;
        }

        public void Clear()
        {
            _channels.Clear();
            _totalMass = new Mass();
        }

        public bool Remove(IQuantitationChannel channel)
        {
            if (_channels.Remove(channel.Mass.Monoisotopic))
            {
                _totalMass.Remove(channel.Mass);
                return true;
            }
            return false;
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

        public Mass GetMass(QuantitationChannelSetMassType massType)
        {
            if (Count <= 1)
            {
                return new Mass(_totalMass);
            }

            switch (massType)
            {
                default:
                case QuantitationChannelSetMassType.Average:
                    return _totalMass / Count;
                case QuantitationChannelSetMassType.Lightest:
                    return new Mass(LighestChannel.Mass);
                case QuantitationChannelSetMassType.Heaviest:
                    return new Mass(HeaviestChannel.Mass);
                case QuantitationChannelSetMassType.Median:
                    if (Count % 2 == 0)
                    {
                        return (_channels.Values[(Count / 2) - 1].Mass + _channels.Values[Count / 2].Mass) / 2.0;
                    }
                    else
                    {
                        return new Mass(_channels.Values[Count / 2].Mass);
                    }
            }
        }

        Mass IMass.Mass
        {
            get
            {
                return GetMass(MassType);
            }
        }

        #region Static 

        public static IEnumerable<Peptide> GetUniquePeptides(Peptide peptide)
        {           
            QuantitationChannelSet quantSetMod;
            IMass mod;
            ModificationCollection modCol;
            HashSet<QuantitationChannelSet> sets = new HashSet<QuantitationChannelSet>();
            Dictionary<IQuantitationChannel, HashSet<int>> locations = new Dictionary<IQuantitationChannel, HashSet<int>>();   
            HashSet<int> residues;

            IMass[] mods = peptide.Modifications;
            int modLength = mods.Length;

            for (int i = 0; i < modLength; i++)
            {
                if (mods[i] != null)
                {
                    mod = mods[i];
                    
                    List<QuantitationChannelSet> channelsets = new List<QuantitationChannelSet>();

                    if ((modCol = mod as ModificationCollection) != null)                     
                    {
                        foreach (IMass mod2 in modCol)
                        {                            
                            if ((quantSetMod = mod2 as QuantitationChannelSet) != null)
                            {
                                channelsets.Add(quantSetMod);
                            }
                        }
                    }
                    else if ((quantSetMod = mod as QuantitationChannelSet) != null)
                    {
                        channelsets.Add(quantSetMod);
                    }

                    foreach (QuantitationChannelSet channelset in channelsets)
                    {
                        sets.Add(channelset);
                        foreach (IQuantitationChannel channel in channelset.GetChannels())
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
                        foreach (int residue in locations[channel])
                        {
                            toReturn.Modifications[residue] = channel;
                        }
                        toReturn.IsDirty = true;                  
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
                    Dictionary<int, IMass> modsToAdd = new Dictionary<int, IMass>();
                    IMass modToAdd;
                    foreach (IQuantitationChannel channel in channelset)
                    {
                        foreach (int residue in locations[channel])
                        {
                            if (modsToAdd.TryGetValue(residue, out modToAdd))
                            {
                                ModificationCollection col = new ModificationCollection(channel.ToString() + ", " + modToAdd.ToString());
                                col.Add(channel);
                                col.Add(modToAdd);
                                modsToAdd[residue] = col;
                            }
                            else
                            {
                                modsToAdd.Add(residue, channel);
                            }
                        }
                    }
                    foreach (KeyValuePair<int, IMass> kvp in modsToAdd)
                    {
                        toReturn.Modifications[kvp.Key] = kvp.Value;                        
                    }
                    toReturn.IsDirty = true;
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
