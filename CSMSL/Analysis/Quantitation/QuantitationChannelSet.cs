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
        public static readonly QuantitationChannelSet TMT6PlexHeavy;
        public static readonly QuantitationChannelSet TMT6PlexLight;
        public static readonly QuantitationChannelSet TMT8Plex;
        public static readonly QuantitationChannelSet TMT10Plex;

        static QuantitationChannelSet()
        {
            IsobaricTag tmt126 = new IsobaricTag("C8 H15 N", "C{13}4 H4 N{15} O2", "126");
            IsobaricTag tmt127l = new IsobaricTag("C8 H15 N{15}", "C{13}4 H4 N O2", "127l");
            IsobaricTag tmt127h = new IsobaricTag("C7 C{13} H15 N", "C C{13}3 H4 N{15} O2", "127h");
            IsobaricTag tmt128l = new IsobaricTag("C7 C{13} H15 N{15}", "C C{13}3 H4 N O2", "128l");
            IsobaricTag tmt128h = new IsobaricTag("C6 C{13}2 H15 N", "C2 C{13}2 H4 N{15} O2", "128h");
            IsobaricTag tmt129l = new IsobaricTag("C6 C{13}2 H15 N{15}", "C2 C{13}2 H4 N O2", "129l");
            IsobaricTag tmt129h = new IsobaricTag("C5 C{13}3 H15 N", "C3 C{13} H4 N{15} O2", "129h");
            IsobaricTag tmt130l = new IsobaricTag("C5 C{13}3 H15 N{15}", "C3 C{13} H4 N O2", "130l");
            IsobaricTag tmt130h = new IsobaricTag("C4 C{13}4 H15 N", "C4 H4 N{15} O2", "130h");
            IsobaricTag tmt131 = new IsobaricTag("C4 C{13}4 H15 N{15}", "C4 H4 N O2", "131");

            TMT6PlexHeavy = new QuantitationChannelSet("TMT 6-Pex (Heavy)", QuantitationChannelSetMassType.Average, false);
            TMT6PlexHeavy.Add(tmt126);
            TMT6PlexHeavy.Add(tmt127h);
            TMT6PlexHeavy.Add(tmt128l);
            TMT6PlexHeavy.Add(tmt129h);
            TMT6PlexHeavy.Add(tmt130l);
            TMT6PlexHeavy.Add(tmt131);

            TMT6PlexLight = new QuantitationChannelSet("TMT 6-Pex (Light)", QuantitationChannelSetMassType.Average, false);
            TMT6PlexLight.Add(tmt126);
            TMT6PlexLight.Add(tmt127l);
            TMT6PlexLight.Add(tmt128l);
            TMT6PlexLight.Add(tmt129l);
            TMT6PlexLight.Add(tmt130l);
            TMT6PlexLight.Add(tmt131);

            TMT8Plex = new QuantitationChannelSet("TMT 8-Pex", QuantitationChannelSetMassType.Average, false);
            TMT8Plex.Add(tmt126);
            TMT8Plex.Add(tmt127l);
            TMT8Plex.Add(tmt127h);
            TMT8Plex.Add(tmt128h);
            TMT8Plex.Add(tmt129l);
            TMT8Plex.Add(tmt129h);
            TMT8Plex.Add(tmt130h);
            TMT8Plex.Add(tmt131);

            TMT10Plex = new QuantitationChannelSet("TMT 10-Pex", QuantitationChannelSetMassType.Average, false);
            TMT10Plex.Add(tmt126);
            TMT10Plex.Add(tmt127l);
            TMT10Plex.Add(tmt127h);
            TMT10Plex.Add(tmt128l);
            TMT10Plex.Add(tmt128h);
            TMT10Plex.Add(tmt129l);
            TMT10Plex.Add(tmt129h);
            TMT10Plex.Add(tmt130l);
            TMT10Plex.Add(tmt130h);
            TMT10Plex.Add(tmt131);

        }

        public string Name { get; set; }

        private SortedList<double, IQuantitationChannel> _channels;
         
        public QuantitationChannelSetMassType MassType { get; set; }

        public double MonoisotopicMass { get; private set; }

        public IQuantitationChannel this[int index]
        {
            get
            {
                return _channels.Values[index];
            }
        }

        public bool IsSequenceDependent { get; set;}

        public IQuantitationChannel this[string name]
        {
            get
            {
                foreach (IQuantitationChannel channel in _channels.Values)
                {
                    if (channel.Name.Equals(name))
                        return channel;
                }
                throw new ArgumentException("Could not find the quantitation channel " + name + " in this set");
            }
        }

        public QuantitationChannelSet(string name, QuantitationChannelSetMassType massType = QuantitationChannelSetMassType.Average, bool sequenceDependent = true)
        {
            Name = name;
            MassType = massType;
            MonoisotopicMass = 0;
            _channels = new SortedList<double, IQuantitationChannel>();
            IsSequenceDependent = sequenceDependent;
        }

        public IQuantitationChannel Add(IQuantitationChannel channel)
        {
            _channels.Add(channel.ReporterMass.MonoisotopicMass, channel);
            MonoisotopicMass += channel.MonoisotopicMass;        

            return channel;
        }

        public void Clear()
        {
            _channels.Clear();
            MonoisotopicMass = 0;
        }

        public bool Remove(IQuantitationChannel channel)
        {
            if (_channels.Remove(channel.MonoisotopicMass))
            {
                MonoisotopicMass -= channel.MonoisotopicMass;
                return true;
            }
            return false;
        }

        public IQuantitationChannel LightestChannel
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
                return new MassRange(LightestChannel.ReporterMass.MonoisotopicMass, HeaviestChannel.ReporterMass.MonoisotopicMass);
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

        public double GetMass(QuantitationChannelSetMassType massType = QuantitationChannelSetMassType.Average)
        {
            if (Count <= 1)
            {
                return MonoisotopicMass;
            }

            switch (massType)
            {
                default:
                case QuantitationChannelSetMassType.Average:
                    return MonoisotopicMass / Count;
                case QuantitationChannelSetMassType.Lightest:
                    return LightestChannel.MonoisotopicMass;
                case QuantitationChannelSetMassType.Heaviest:
                    return HeaviestChannel.MonoisotopicMass;
                case QuantitationChannelSetMassType.Median:
                    if (Count % 2 == 0)
                    {
                        return (_channels.Values[(Count / 2) - 1].MonoisotopicMass + _channels.Values[Count / 2].MonoisotopicMass) / 2.0;
                    }
                    else
                    {
                        return _channels.Values[Count / 2].MonoisotopicMass;
                    }
            }
        }

        double IMass.MonoisotopicMass
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

        public static HashSet<QuantitationChannelSet> GetQuantChannelModifications(Peptide peptide)
        {
            HashSet<QuantitationChannelSet> sets = new HashSet<QuantitationChannelSet>();
            IMass[] mods = peptide.Modifications;
            IMass mod;
            ModificationCollection modCol;
            QuantitationChannelSet quantSetMod;
            int modLength = mods.Length;

            for (int i = 0; i < modLength; i++)
            {
                if (mods[i] != null)
                {
                    mod = mods[i];                  

                    if ((modCol = mod as ModificationCollection) != null)
                    {
                        foreach (IMass mod2 in modCol)
                        {
                            if ((quantSetMod = mod2 as QuantitationChannelSet) != null)
                            {
                                sets.Add(quantSetMod);
                            }
                        }
                    }
                    else if ((quantSetMod = mod as QuantitationChannelSet) != null)
                    {
                        sets.Add(quantSetMod);
                    }
                }
            }
            return sets;
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
