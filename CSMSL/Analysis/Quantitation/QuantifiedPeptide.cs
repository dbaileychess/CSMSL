using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using CSMSL.Spectral;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeptide
    {
        List<PeptideSpectralMatch> PSMs;
        public List<QuantifiedScan> QuantifiedScans;
        Peptide Peptide;

        public int QuantifiedScanCount
        {
            get
            {
                return QuantifiedScans.Count;
            }
        }

        public int PsmCount
        {
            get
            {
                return PSMs.Count;
            }
        }

        public PeptideSpectralMatch BestPSM
        {
            get
            {
                List<PeptideSpectralMatch> sortedPSMs = PSMs.OrderBy(psm => psm.Score).ToList();
                return sortedPSMs[0];
            }
        }

        public QuantifiedPeptide(Peptide peptide)
        {
            Peptide = peptide;
            PSMs = new List<PeptideSpectralMatch>();
            QuantifiedScans = new List<QuantifiedScan>();
        }

        public void AddPeptideSpectralMatch(PeptideSpectralMatch psm)
        {
            if (psm == null)
            {
                throw new ArgumentNullException("null psm");
            }
            
            if (PSMs.Contains(psm))
            {
                throw new ArgumentException("peptide spectral match already exists");
            }

            PSMs.Add(psm);
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

            // For MS2-based quantification, check to make sure relevant PSM exists
            bool psmFound = false;
            int count = 0;
            PeptideSpectralMatch psm;
            while (!psmFound && count < PsmCount)
            {
                psm = PSMs[count];
                if (quantScan.DataScan.MsnOrder == 2)
                {
                    if (((MSDataScan)psm.Spectrum).Equals(quantScan.DataScan))
                    {
                        psmFound = true;
                    }
                    count++;
                }
                else
                {
                    if (((int)psm.Spectrum.PrecursorCharge).Equals(quantScan.Charge))
                    {
                        psmFound = true;
                    }
                    count++;
                }
            }

            if (!psmFound)
            {
                throw new ArgumentOutOfRangeException("psm not found");
            }
            
            quantScan.QuantifiedPeptideParent = this;
            QuantifiedScans.Add(quantScan);
        }

        public double GetRatio(double numerator, double denominator)
        {
            if (denominator == 0.0 || numerator == 0.0)
            {
                throw new DivideByZeroException();
            }

            return numerator / denominator;
        }
        
        public double GetChannelIntensity(Channel channel, IntensityWeightingType method, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            double channelIntensitySum = 0;
            QuantifiedPeak peak = null;
            List<double> intensities = new List<double>();
            double channelIntensity = 0;
            int count = 0;

            foreach (QuantifiedScan scan in QuantifiedScans)
            {

                for (int i = 0; i < QuantifiedScan.NumIsotopes; i++)
                {
                    try
                    {
                        peak = scan.GetQuantifiedPeak(channel, i);
                        if (peak == null)
                        {
                            continue;
                        }
                        if (peak.SignalToNoise >= signalToNoiseThreshold || noiseBandCap)
                        {
                            channelIntensity = peak.DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
                            channelIntensitySum += channelIntensity;
                            intensities.Add(channelIntensity);
                            count++;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }

            }

            intensities.Sort();

            if (method == IntensityWeightingType.Summed)
            {
                return channelIntensitySum;
            }

            if (method == IntensityWeightingType.Average)
            {
                return GetRatio(channelIntensitySum, ((double)count));
            }

            if (method == IntensityWeightingType.Median)
            {
                if (count == 0)
                {
                    return 0;
                }
                
                if (count % 2 == 0)
                {
                    return (intensities[(count / 2) - 1] + intensities[count / 2]) / 2;
                }
                else
                {
                    return intensities[count / 2];
                }
            }

            return 0;
        }

        public double GetOverallRatio(Channel numerator, Channel denominator, IntensityWeightingType method, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            return GetRatio(GetChannelIntensity(numerator, method, noiseBandCap, signalToNoiseThreshold), GetChannelIntensity(denominator, method, noiseBandCap, signalToNoiseThreshold));
        }

        public List<double> GetRatioList(Channel numerator, Channel denominator, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            List<double> log2Ratios = new List<double>();
            double peakNumerator = 0;
            double peakDenominator = 0;
            double ratio = 0;

            foreach (QuantifiedScan scan in QuantifiedScans)
            {
                for (int i = 0; i < QuantifiedScan.NumIsotopes; i++)
                {
                    try
                    {
                        peakNumerator = scan.GetQuantifiedPeak(numerator, i).DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
                        peakDenominator = scan.GetQuantifiedPeak(denominator, i).DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
                        ratio = GetRatio(peakNumerator, peakDenominator);
                        log2Ratios.Add(Math.Log(ratio, 2));
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                    catch (DivideByZeroException)
                    {
                        continue;
                    }
                }
            }

            return log2Ratios;
        }

        public double GetIndividualRatio(Channel numerator, Channel denominator, IntensityWeightingType method, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            double finalRatio = 0;
            List<double> log2Ratios = GetRatioList(numerator, denominator, noiseBandCap, signalToNoiseThreshold);
            log2Ratios.Sort();
            int ratioCount = log2Ratios.Count;

            if (method == IntensityWeightingType.Average)
            {
                double ratioSum = 0;

                foreach (double ratio in log2Ratios)
                {
                    ratioSum += ratio;
                }

                finalRatio = ratioSum / ((double)ratioCount);
                return Math.Pow(2, finalRatio);
            }

            if (method == IntensityWeightingType.Median)
            {
                if (ratioCount % 2 == 0)
                {
                    finalRatio = (log2Ratios[(ratioCount / 2)] + log2Ratios[ratioCount / 2]) / 2;
                }
                else
                {
                    finalRatio = log2Ratios[ratioCount / 2];
                }
                return Math.Pow(2, finalRatio);
            }

            return Math.Pow(2, finalRatio);
        }

        public double GetRatioVariation(Channel numerator, Channel denominator, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            List<double> log2Ratios = GetRatioList(numerator, denominator, noiseBandCap);
            double ratioAverage = Math.Log(GetIndividualRatio(numerator, denominator, IntensityWeightingType.Average, noiseBandCap, signalToNoiseThreshold), 2);
            int ratioCount = log2Ratios.Count;
            double variance = 0;

            foreach (double ratio in log2Ratios)
            {
                variance += Math.Pow(ratio - ratioAverage, 2);
            }

            return Math.Pow(2, Math.Sqrt(variance / ((double)ratioCount - 1)));
        }
    }
}
