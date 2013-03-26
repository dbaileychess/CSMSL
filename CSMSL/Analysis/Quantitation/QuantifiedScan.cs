using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
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
        public Dictionary<Channel, QuantifiedPeak>[] QuantifiedPeaks;
        internal QuantifiedPeptide QuantifiedPeptideParent { get; set; }

        public QuantifiedScan(MSDataScan dataScan, int charge = 0)
	    {
		    DataScan = dataScan;
		    QuantifiedPeaks = new Dictionary<Channel, QuantifiedPeak>[NumIsotopes];
		    for (int i = 0; i < NumIsotopes; i++)
		    {
			    QuantifiedPeaks[i] = new Dictionary<Channel, QuantifiedPeak>();
		    }
            Charge = charge;
	    }

        public void AddQuant(Channel channel, QuantifiedPeak peak, int isotope = 0)
        {
            // Check for an invalid isotope
            if (isotope < 0 || isotope >= NumIsotopes)
            {
                throw new IndexOutOfRangeException("invalid isotope");
            }

            // Check for invalid channel
            if (channel == null)
            {
                throw new NullReferenceException("null channel");
            }
            QuantifiedPeak duplicate = null;
            if (QuantifiedPeaks[isotope].TryGetValue(channel, out duplicate))
            {
                throw new DuplicateKeyException("duplicate channel");
            }
            
            // Check for a null peak
            if (peak == null)
            {
                peak = empty;
            }

            peak.QuantScanParent = this;
            QuantifiedPeaks[isotope].Add(channel, peak);
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

        public QuantifiedPeak GetQuantifiedPeak(Channel channel, int isotope = 0)
        {
            QuantifiedPeak peak = null;
            // Check for an invalid isotope
            if (isotope < 0 || isotope >= NumIsotopes)
            {
                throw new IndexOutOfRangeException("invalid isotope");
            }

            // Check for invalid channel
            if (channel == null)
            {
                throw new NullReferenceException("null channel");
            }
            if (!QuantifiedPeaks[isotope].TryGetValue(channel, out peak))
            {
                throw new KeyNotFoundException("channel not found");
            }

            return peak;
        }

        public int ChannelCount
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
