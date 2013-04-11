using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using CSMSL.Spectral;
using CSMSL.Analysis.ExperimentalDesign;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeptide
    {
        //internal ExperimentalSet ParentExperimentalSet;
        HashSet<PeptideSpectralMatch> PSMs;
        public List<QuantifiedScan> QuantifiedScans;
        public Peptide Peptide { get; private set; }        

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
            get;
            set;
        }

        public QuantifiedPeptide(Peptide peptide)
        {
            Peptide = peptide;
            PSMs = new HashSet<PeptideSpectralMatch>();
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

            // Check for new best PSM
            if (PsmCount > 0)
            {
                if (psm.ScoreType == PeptideSpectralMatchScoreType.EValue)
                {
                    if (psm.Score < BestPSM.Score)
                    {
                        BestPSM = psm;
                    }
                }
                else if (psm.ScoreType == PeptideSpectralMatchScoreType.XCorr || psm.ScoreType == PeptideSpectralMatchScoreType.Morpheus)
                {
                    if (psm.Score > BestPSM.Score)
                    {
                        BestPSM = psm;
                    }
                }
            }
            else
            {
                BestPSM = psm;
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
            foreach(PeptideSpectralMatch psm in PSMs)         
            {             
                if (quantScan.DataScan.MsnOrder == 2)
                {
                    if (((MSDataScan)psm.Spectrum).Equals(quantScan.DataScan))
                    {
                        psmFound = true;
                        break;
                    }                   
                }
                else
                {
                    if (((int)psm.Spectrum.PrecursorCharge).Equals(quantScan.Charge))
                    {
                        psmFound = true;
                        break;
                    }                  
                }
            }

            if (!psmFound)
            {
                throw new ArgumentOutOfRangeException("psm not found");
            }
            
            quantScan.QuantifiedPeptideParent = this;
            QuantifiedScans.Add(quantScan);
        }
                
        public double GetIQuantitationChannelIntensity(IQuantitationChannel IQuantitationChannel, IntensityWeightingType method, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            double IQuantitationChannelIntensitySum = 0;
            QuantifiedPeak peak = null;
            List<double> intensities = new List<double>();
            double IQuantitationChannelIntensity = 0;         

            foreach (QuantifiedScan scan in QuantifiedScans)
            {
                for (int i = 0; i < QuantifiedScan.NumIsotopes; i++)
                {                                          
                    if (scan.TryGetQuantifiedPeak(IQuantitationChannel, out peak, i))
                    {
                        if (peak.SignalToNoise >= signalToNoiseThreshold || noiseBandCap)
                        {
                            IQuantitationChannelIntensity = peak.DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
                            IQuantitationChannelIntensitySum += IQuantitationChannelIntensity;
                            intensities.Add(IQuantitationChannelIntensity);                           
                        }
                    }
                }

            }

            switch (method)
            {               
                case IntensityWeightingType.Summed:
                    return IQuantitationChannelIntensitySum;
                case IntensityWeightingType.Average:
                    return GetRatio(IQuantitationChannelIntensitySum, (double)intensities.Count);
                case IntensityWeightingType.Median:                    
                    return GetMedian(intensities);
                default:
                    return 0;               
            }
        }

        public double GetOverallRatio(IQuantitationChannel numerator, IQuantitationChannel denominator, IntensityWeightingType method, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            double top = GetIQuantitationChannelIntensity(numerator, method, noiseBandCap, signalToNoiseThreshold);
            double bottom = GetIQuantitationChannelIntensity(denominator, method, noiseBandCap, signalToNoiseThreshold);

            return GetRatio(top, bottom);
        }

        public List<double> GetRatioList(IQuantitationChannel numerator, IQuantitationChannel denominator, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            List<double> log2Ratios = new List<double>();
            double peakNumerator = 0;
            double peakDenominator = 0;
            double ratio = 0;
            QuantifiedPeak numeratorPeak = null;
            QuantifiedPeak denominatorPeak = null;
            foreach (QuantifiedScan scan in QuantifiedScans)
            {
                for (int i = 0; i < QuantifiedScan.NumIsotopes; i++)
                {
                    if (scan.TryGetQuantifiedPeak(numerator, out numeratorPeak, i) && scan.TryGetQuantifiedPeak(denominator, out denominatorPeak, i))
                    {
                        peakNumerator = numeratorPeak.DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
                        peakDenominator = denominatorPeak.DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
                        ratio = GetRatio(peakNumerator, peakDenominator);
                        if (ratio != 0.0)
                        {
                            log2Ratios.Add(Math.Log(ratio, 2));
                        }
                    }                             
                }
            }

            return log2Ratios;
        }

        public double GetIndividualRatio(IQuantitationChannel numerator, IQuantitationChannel denominator, IntensityWeightingType method, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
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

        public double GetRatioVariation(IQuantitationChannel numerator, IQuantitationChannel denominator, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
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

        #region Static
        
        private static double GetRatio(double numerator, double denominator)
        {
            if (denominator.Equals(0.0))
            {
                return 0;
            }
            return numerator / denominator;
        }

        private static double GetMedian(IEnumerable<double> values)
        {
            double[] temp = values.ToArray();

            int count = temp.Length;

            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count == 1)
            {
                return temp[0];
            }
            else
            {
                Array.Sort(temp);

                if (count % 2 == 0)
                {
                    return (temp[(count / 2) - 1] + temp[count / 2]) / 2.0;
                }
                else
                {
                    return temp[count / 2];
                }
            }
        }

        /// <summary>
        /// Reduces a list of peptide spectral matches into distinct peptides
        /// </summary>
        /// <param name="psms"></param>
        /// <returns></returns>
        public static IList<QuantifiedPeptide> GenerateQuantifiedPeptides(IEnumerable<PeptideSpectralMatch> psms)
        {
            Dictionary<Peptide, QuantifiedPeptide> quantPeps = new  Dictionary<Peptide, QuantifiedPeptide>();
            QuantifiedPeptide quantPep;
            foreach (PeptideSpectralMatch psm in psms)
            {
                if (!quantPeps.TryGetValue(psm.Peptide, out quantPep))
                {
                    quantPep = new QuantifiedPeptide(psm.Peptide);
                    quantPeps.Add(psm.Peptide, quantPep);
                }
                quantPep.AddPeptideSpectralMatch(psm);
            }

            return quantPeps.Values.ToList();
        }

        #endregion
    }
}
