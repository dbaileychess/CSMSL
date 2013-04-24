using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.IO;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedScan : IEquatable<QuantifiedScan>
    {
        public static int NumIsotopes = 3;
        static QuantifiedPeak empty = new QuantifiedPeak();
        public MSDataScan DataScan;
        public int Charge { get; set; }
        public Dictionary<IQuantitationChannel, QuantifiedPeak>[] QuantifiedPeaks;        
        internal QuantifiedPeptide QuantifiedPeptideParent { get; set; }

        public QuantifiedScan(MSDataScan dataScan, int charge = 0)
	    {
		    DataScan = dataScan;
            //Dictionary<IIonDesignation, List<QuantifiedPeak>> 
		    QuantifiedPeaks = new Dictionary<IQuantitationChannel, QuantifiedPeak>[NumIsotopes];
		    for (int i = 0; i < NumIsotopes; i++)
		    {
			    QuantifiedPeaks[i] = new Dictionary<IQuantitationChannel, QuantifiedPeak>();
		    }
            Charge = charge;
	    }

        public void AddQuant(IQuantitationChannel IQuantitationChannel, QuantifiedPeak peak, int isotope = 0)
        {
            // Check for an invalid isotope
            if (isotope < 0 || isotope >= NumIsotopes)
            {
                throw new IndexOutOfRangeException("invalid isotope");
            }

            // Check for invalid IQuantitationChannel
            if (IQuantitationChannel == null)
            {
                throw new NullReferenceException("null IQuantitationChannel");
            }
            QuantifiedPeak duplicate = null;
            if (QuantifiedPeaks[isotope].TryGetValue(IQuantitationChannel, out duplicate))
            {
                throw new DuplicateKeyException("duplicate IQuantitationChannel");
            }
            
            // Check for a null peak
            if (peak == null)
            {
                peak = empty;
            }

            peak.QuantScanParent = this;
            QuantifiedPeaks[isotope].Add(IQuantitationChannel, peak);
        }

        public double InjectionTime
        {
            get
            {
                return DataScan.InjectionTime;
            }
        }

        public int ScanNumber
        {
            get
            {
                return DataScan.SpectrumNumber;
            }
        }

        public bool TryGetQuantifiedPeak(IQuantitationChannel IQuantitationChannel, out QuantifiedPeak peak, int isotope = 0)
        {
            peak = null;
            // Check for an invalid isotope
            if (isotope < 0 || isotope >= NumIsotopes || IQuantitationChannel == null)
            {
                return false;
                //throw new IndexOutOfRangeException("invalid isotope");
            }

            // Check for invalid IQuantitationChannel
            //if (IQuantitationChannel == null)
            //{
            //    throw new NullReferenceException("null IQuantitationChannel");
            //}
            if (QuantifiedPeaks[isotope].TryGetValue(IQuantitationChannel, out peak))
            {
                return true;               
            }

            return false;
        }

        public double GetTheoMz(IQuantitationChannel IQuantitationChannel, int isotope = 0)
        {
            return double.NaN;
        }

        public int IQuantitationChannelCount
        {
            get
            {
                return QuantifiedPeaks[0].Keys.Count;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is QuantifiedScan && Equals((QuantifiedScan)obj);
        }

        public bool Equals(QuantifiedScan other)
        {
            return DataScan == other.DataScan && Charge == other.Charge;
        }
    }
}
