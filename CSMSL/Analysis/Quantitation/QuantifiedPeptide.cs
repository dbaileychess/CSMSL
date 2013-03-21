using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeptide
    {
        List<QuantifiedScan> QuantifiedScans;
        Peptide Peptide;

        public int QuantifiedScanCount
        {
            get
            {
                return QuantifiedScans.Count;
            }
        }

        public QuantifiedPeptide(Peptide peptide)
        {
            Peptide = peptide;
            QuantifiedScans = new List<QuantifiedScan>();
        }

        public void AddQuantifiedScan(QuantifiedScan quantScan)
        {
            if (quantScan == null)
            {
                throw new ArgumentNullException("null quantified scan");
            }
            
            // Do not add quantification if scan already exists
            if (QuantifiedScans.Contains(quantScan))
            {
                throw new ArgumentException("quantified scan already exists");
            }

            QuantifiedScans.Add(quantScan);
        }

        public double GetRatio(double numerator, double denominator)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException();
            }

            return numerator / denominator;
        }
        
        public double GetSummedChannelIntensity(Channel channel, bool noiseBandCap = false)
        {
            QuantifiedPeak peak = null;
            double totalIntensitySum = 0;
            foreach (QuantifiedScan scan in QuantifiedScans)
            {
                for (int i = 0; i < QuantifiedScan.NumIsotopes; i++)
                {
                    peak = scan.GetQuantifiedPeak(channel, i);
                    totalIntensitySum += peak.DenormalizedIntensity(noiseBandCap);
                }
            }

            return totalIntensitySum;
        }

        public double GetAverageChannelIntensity(Channel channel, bool noiseBandCap = false)
        {
            if (QuantifiedScanCount == 0)
            {
                throw new DivideByZeroException();
            }
            
            return GetSummedChannelIntensity(channel, noiseBandCap) / ((double) QuantifiedScanCount);
        }

        public double GetOverallRatioSum(Channel numerator, Channel denominator, bool noiseBandCap = false)
        {
            return GetRatio(GetSummedChannelIntensity(numerator, noiseBandCap), GetSummedChannelIntensity(denominator, noiseBandCap));
        }

        public double GetOverallRatioAverage(Channel numerator, Channel denominator, bool noiseBandCap = false)
        {
            return GetAverageChannelIntensity(numerator, noiseBandCap) / GetAverageChannelIntensity(denominator, noiseBandCap);
        }

        public double GetRatioAverage(Channel numerator, Channel denominator, bool noiseBandCap = false)
        {
            double ratioAverage = 1;

            return ratioAverage;
        }

        public double GetRatioMedian(Channel numerator, Channel denominator, bool noiseBandCap = false)
        {
            double ratioMedian = 1;

            return ratioMedian;
        }

        public double GetRatioVariation(Channel numerator, Channel denominator, bool noiseBandCap = false)
        {
            double ratioVariation = 0;

            return ratioVariation;
        }
    }
}
