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
            _channels.Add(channel.ReporterMass.Monoisotopic, channel);
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
            double monomass = 0;
            QuantitationChannelSet quantSetMod;
            IMass mod;
            Dictionary<IQuantitationChannel, HashSet<int>> channels = new Dictionary<IQuantitationChannel, HashSet<int>>();
            int count = 0;
            for (int i = 1; i <= peptide.Length; i++)
            {
                if ((mod = peptide.GetModification(i)) != null)
                {
                    quantSetMod = mod as QuantitationChannelSet;
                    if (quantSetMod != null)
                    {
                        HashSet<int> residues;
                        foreach (IQuantitationChannel channel in quantSetMod.GetChannels())
                        {
                            if (channels.TryGetValue(channel, out residues))
                            {
                                residues.Add(i);
                            }
                            else
                            {
                                residues = new HashSet<int>() { i };
                                channels.Add(channel, residues);
                            }
                        }
                        count++;
                    }
                }
            }


            if (count == 0)
            {
                yield return new Peptide(peptide, true);
            }
            else
            {
                foreach (KeyValuePair<IQuantitationChannel, HashSet<int>> kvp in channels)
                {
                    Peptide toReturn = new Peptide(peptide, true);
                    foreach (int residue in kvp.Value)
                    {
                        toReturn.SetModification(kvp.Key, residue);
                    }
                    yield return toReturn;
                }
            }
            yield break;

        }
        
        public static IEnumerable<double> GetPrecursorMasses(AminoAcidPolymer peptide)
        {
            double monomass = 0;            
            QuantitationChannelSet quantSetMod;
            IMass mod;
            HashSet<double> masses = new HashSet<double>();
            int count = 0;
            for (int i = 1; i <= peptide.Length; i++)
            {
                if ((mod = peptide.GetModification(i)) != null)
                {
                    quantSetMod = mod as QuantitationChannelSet;
                    if (quantSetMod != null)
                    {
                        foreach (IQuantitationChannel channel in quantSetMod.GetChannels())
                        {
                            masses.Add(channel.Mass.Monoisotopic);
                        }
                        count++;
                    }
                    else
                    {
                        monomass += mod.Mass.Monoisotopic;
                    }   
                }
                monomass += peptide[i].Mass.Monoisotopic;
            }

            monomass += peptide.NTerminus.Mass.Monoisotopic;
            monomass += peptide.CTerminus.Mass.Monoisotopic;

            if (count == 0)
            {
                yield return monomass;
            }
            else
            {
                foreach (double mass in masses)
                {
                    yield return monomass + count * mass;
                }
            }
            yield break;
        }

        public static IEnumerable<double> GetPrecursorMZes(AminoAcidPolymer peptide, int charge)
        {
            List<double> masses = GetPrecursorMasses(peptide).ToList();
            masses.ForEach(v => Mass.MzFromMass(v, charge));
            return masses;
        }

        #endregion
    }
}
