using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using CSMSL.Spectral;
using CSMSL.Analysis.ExperimentalDesign;
using CSMSL.IO;
using CSMSL.Chemistry;
using CSMSL;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeptide
    {
        public static double RTWindow = 0.5;
        public static double ResolutionMin = 100000;
       
        public HashSet<PeptideSpectralMatch> PSMs;
        public HashSet<MSDataScan> QuantifiedScans;
        public Peptide Peptide { get; private set; }

        private Dictionary<ExperimentalCondition, QuantifiedPeakSet> _quantities;

        public QuantifiedPeptide(Peptide peptide)
        {
            Peptide = peptide;
            PSMs = new HashSet<PeptideSpectralMatch>();
            QuantifiedScans = new HashSet<MSDataScan>();
            _quantities = new Dictionary<ExperimentalCondition, QuantifiedPeakSet>();
        }

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

        public QuantifiedPeakSet this[ExperimentalCondition condition]
        {
            get
            {
                return _quantities[condition];
            }
        }

        public PeptideSpectralMatch BestPSM { get; private set;}              

        public override string ToString()
        {
            return string.Format("{0} ({1:N0} PSMs)", Peptide, PsmCount);
        }

        public void AddPeptideSpectralMatch(PeptideSpectralMatch psm)
        {
            if (psm == null)
            {
                throw new ArgumentNullException("null psm");
            }

            if (PSMs.Add(psm))
            {
                // Check for new best PSM
                if (BestPSM != null)
                {
                    if(psm.CompareTo(BestPSM) < 0)
                        BestPSM = psm;
                }
                else
                {
                    BestPSM = psm;
                }          
            }     
        }

        public void AddQuantifiedScan(MSDataScan quantScan)
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
            //bool psmFound = false;  
            //foreach(PeptideSpectralMatch psm in PSMs)         
            //{             
            //    if (quantScan.DataScan.MsnOrder == 2)
            //    {
            //        if (((MSDataScan)psm.Spectrum).Equals(quantScan.DataScan))
            //        {
            //            psmFound = true;
            //            break;
            //        }                   
            //    }
            //    else
            //    {
            //        if (((int)psm.Spectrum.PrecursorCharge).Equals(quantScan.Charge))
            //        {
            //            psmFound = true;
            //            break;
            //        }                  
            //    }
            //}

            //if (!psmFound)
            //{
            //    throw new ArgumentOutOfRangeException("psm not found");
            //}
            
            //quantScan.QuantifiedPeptideParent = this;
            QuantifiedScans.Add(quantScan);
        }
        
        public double GetIQuantitationChannelIntensity(IQuantitationChannel IQuantitationChannel, IntensityWeightingType method, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            double IQuantitationChannelIntensitySum = 0;
            QuantifiedPeak peak = null;
            List<double> intensities = new List<double>();
            double IQuantitationChannelIntensity = 0;         

            //foreach (QuantifiedScan scan in QuantifiedScans)
            //{
            //    for (int i = 0; i < QuantifiedScan.NumIsotopes; i++)
            //    {                                          
            //        if (scan.TryGetQuantifiedPeak(IQuantitationChannel, out peak, i))
            //        {
            //            if (peak.SignalToNoise >= signalToNoiseThreshold || noiseBandCap)
            //            {
            //                IQuantitationChannelIntensity = peak.DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
            //                IQuantitationChannelIntensitySum += IQuantitationChannelIntensity;
            //                intensities.Add(IQuantitationChannelIntensity);                           
            //            }
            //        }
            //    }

            //}

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

        public double GetRatio(ExperimentalCondition numerator, ExperimentalCondition denominator, Func<QuantifiedPeak, bool> predicate)
        {
            QuantifiedPeakSet num = _quantities[numerator].Filter(predicate);
            QuantifiedPeakSet den = _quantities[denominator].Filter(predicate);
            return num.Intensity / den.Intensity;
        }

        public IEnumerable<double> GetRatioList(ExperimentalCondition numerator, ExperimentalCondition denominator, Func<QuantifiedPeak, QuantifiedPeak, bool> predicate)
        {
            QuantifiedPeakSet num = _quantities[numerator];
            QuantifiedPeakSet den = _quantities[denominator];
            foreach (QuantifiedPeak num_peak in num.GetPeaks())
            {
                foreach (QuantifiedPeak den_peak in den.GetPeaks())
                {
                    if (predicate(num_peak, den_peak))
                    {
                        yield return num_peak.Intensity / den_peak.Intensity;
                    }
                }
            }

            yield break;
        }

        public List<double> GetRatioList(IQuantitationChannel numerator, IQuantitationChannel denominator, bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            List<double> log2Ratios = new List<double>();
            double peakNumerator = 0;
            double peakDenominator = 0;
            double ratio = 0;
            QuantifiedPeak numeratorPeak = null;
            QuantifiedPeak denominatorPeak = null;
            //foreach (QuantifiedScan scan in QuantifiedScans)
            //{
            //    for (int i = 0; i < QuantifiedScan.NumIsotopes; i++)
            //    {
            //        if (scan.TryGetQuantifiedPeak(numerator, out numeratorPeak, i) && scan.TryGetQuantifiedPeak(denominator, out denominatorPeak, i))
            //        {
            //            peakNumerator = numeratorPeak.DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
            //            peakDenominator = denominatorPeak.DenormalizedIntensity(noiseBandCap, signalToNoiseThreshold);
            //            ratio = GetRatio(peakNumerator, peakDenominator);
            //            if (ratio != 0.0)
            //            {
            //                log2Ratios.Add(Math.Log(ratio, 2));
            //            }
            //        }                             
            //    }
            //}

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

        private Dictionary<int, Dictionary<string,List<PeptideSpectralMatch>>> SortPSMsByChargeAndDataFile()
        {
            Dictionary<int, Dictionary<string, List<PeptideSpectralMatch>>> sortedPSMs = new Dictionary<int, Dictionary<string,List<PeptideSpectralMatch>>>();

            foreach (PeptideSpectralMatch psm in PSMs)
            {
                Dictionary<string, List<PeptideSpectralMatch>> files = null;
                List<PeptideSpectralMatch> psms = null;

                // Check for existing charge state & file name
                if (sortedPSMs.TryGetValue(psm.Charge, out files))
                {
                    // Add PSM to existing charge state & file name list
                    if (files.TryGetValue(psm.FileName, out psms))
                    {
                        psms.Add(psm);
                    }
                    // Add new file name
                    else
                    {
                        psms = new List<PeptideSpectralMatch>();
                        psms.Add(psm);
                        files = new Dictionary<string, List<PeptideSpectralMatch>>();
                        files.Add(psm.FileName, psms);
                    }
                }
                // Add new charge state & file name
                else
                {
                    psms = new List<PeptideSpectralMatch>();
                    psms.Add(psm);
                    files = new Dictionary<string, List<PeptideSpectralMatch>>();
                    files.Add(psm.FileName, psms);
                    sortedPSMs.Add(psm.Charge, files);
                }                
            }

            return sortedPSMs;
        }

        private List<MSDataScan> FindQuantScans(MSDataFile dataFile, List<PeptideSpectralMatch> psms)
        {
            double minTime = CalculateRTRangeMin(psms);
            double maxTime = CalculateRTRangeMax(psms);
            double firstTime = dataFile[dataFile.FirstSpectrumNumber].RetentionTime;
            double lastTime = dataFile[dataFile.LastSpectrumNumber].RetentionTime;

            if (minTime < firstTime)
            {
                minTime = firstTime;
            }

            if (maxTime > lastTime)
            {
                maxTime = lastTime;
            }

            Range<double> rTRange = new Range<double>(minTime, maxTime);
            List<MSDataScan> scans = FindScansInRange(dataFile, rTRange);
            return scans;
        }

        private double CalculateRTRangeMin(List<PeptideSpectralMatch> psms)
        {
            double minTime;
            int numPSMs = psms.Count;

            List<PeptideSpectralMatch> sortedPSMs = psms.OrderBy(psm => psm.SpectrumNumber).ToList();
            minTime = sortedPSMs[0].Spectrum.RetentionTime - RTWindow;

            return minTime;
        }

        private double CalculateRTRangeMax(List<PeptideSpectralMatch> psms)
        {
            double maxTime;
            int numPSMs = psms.Count;

            List<PeptideSpectralMatch> sortedPSMs = psms.OrderBy(psm => psm.SpectrumNumber).ToList();
            maxTime = sortedPSMs[numPSMs - 1].Spectrum.RetentionTime + RTWindow;

            return maxTime;
        }

        private List<MSDataScan> FindScansInRange(MSDataFile dataFile, Range<double> rTRange, int msn = 1, double resolution = 100000)
        {
            List<MSDataScan> scans = new List<MSDataScan>();

            foreach (MSDataScan dataScan in dataFile.Where(scan => scan.MsnOrder == msn && scan.Resolution >= resolution && scan.RetentionTime >= rTRange.Minimum && scan.RetentionTime <= rTRange.Maximum))
            {
                scans.Add(dataScan);
            }

            return scans;
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
             
        public static IEnumerable<QuantifiedPeptide> GroupPeptideSpectralMatches(IEnumerable<PeptideSpectralMatch> psms)
        {
            // Group psms into peptides
            Dictionary<Peptide, QuantifiedPeptide> quantPeps = new Dictionary<Peptide, QuantifiedPeptide>();
            {
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
            }
            return quantPeps.Values;
        }

        /// <summary>
        /// Reduces a list of peptide spectral matches into distinct peptides
        /// </summary>
        /// <param name="psms"></param>
        /// <returns></returns>
        public static void Quantify(IEnumerable<QuantifiedPeptide> quantifiedPeptides, IEnumerable<ExperimentalCondition> conditions)
        {          
            foreach (QuantifiedPeptide quantPep in quantifiedPeptides)
            {
                HashSet<QuantitationChannelSet> channels = QuantitationChannelSet.GetQuantChannelModifications(quantPep.Peptide);
                bool sequenceIndependent = channels.Any(channel => !channel.IsSequenceDependent);

                if (sequenceIndependent)
                {
                    quantPep.SequenceIndependentQuantify(conditions);
                }
                else
                {
                    SequenceDependentQuantify(quantPep);
                }
                // MS1-based quant
                //Dictionary<int, Dictionary<string, List<PeptideSpectralMatch>>> sortedPSMs = quantPep.SortPSMsByChargeAndDataFile();

                //foreach (KeyValuePair<int, Dictionary<string, List<PeptideSpectralMatch>>> chargeState in sortedPSMs)
                //{
                //    foreach (KeyValuePair<string, List<PeptideSpectralMatch>> fileName in chargeState.Value)
                //    {
                //        MSDataFile msDataFile = fileName.Value[0].Spectrum.ParentFile;
                //    
                //        foreach (MSDataScan scan in quantPep.FindQuantScans(msDataFile, fileName.Value))
                //        {
                //            quantPep.QuantifiedScans.Add(new QuantifiedScan(scan, chargeState.Key));
                //        }
                //    }
                //}
             
                
            }

            //return quantPeps.Values.ToList();
        }

        private static void SequenceDependentQuantify(QuantifiedPeptide quantPep)
        {

        }

        private static ExperimentalCondition GetCondition(Peptide peptide, IEnumerable<ExperimentalCondition> conditions)
        {
              HashSet<IQuantitationChannel> quantChannels = new HashSet<IQuantitationChannel>(peptide.Modifications.OfType<IQuantitationChannel>());
              foreach (ExperimentalCondition condition in conditions)
              {
                  if (quantChannels.SetEquals(condition))
                      return condition;              
              }
            return null;

        }

        private void SequenceIndependentQuantify(IEnumerable<ExperimentalCondition> conditions)
        {
            QuantifiedPeakSet peakSet;
            List<MZPeak> peaks;

            // MS2-based quant.
            foreach (PeptideSpectralMatch psm in PSMs)
            {
                MassSpectrum spectrum = psm.Spectrum.MassSpectrum;
                int charge = psm.Charge;             

                foreach (Peptide peptide in QuantitationChannelSet.GetUniquePeptides(psm.Peptide))
                {
                    ExperimentalCondition condition = GetCondition(peptide, conditions);
                             
                    Mass mass = GetReporterMass(peptide, condition);
                    double mz = mass.ToMz(1);
                    if (spectrum.TryGetPeaks(MassRange.FromDa(mz, 0.05), out peaks))
                    {
                        MZPeak peak = peaks.OrderBy(p => p.Intensity).ToArray()[0];
                        QuantifiedPeak qpeak = new QuantifiedPeak(psm.Spectrum, peak.MZ, 1, peak.Intensity);
                     
                        if (!_quantities.TryGetValue(condition, out peakSet))
                        {
                            peakSet = new QuantifiedPeakSet();
                            _quantities.Add(condition, peakSet);
                        }
                        peakSet.Add(qpeak); 
                        //quantScan.AddQuant(quantChannels.ToArray()[0], qpeak, 0);
                    }
                    else
                    {
                        
                    }
                    
                }
                QuantifiedScans.Add(psm.Spectrum);
            }
        }

        private static Mass GetReporterMass(Peptide peptide, IEnumerable<IQuantitationChannel> channels)
        {            
            foreach (IQuantitationChannel channel in channels)
            {
                if (!channel.IsSequenceDependent)
                {
                    return channel.ReporterMass;
                }               
            }
            return peptide.Mass;
        }
      

        public static void PopulateQuantifiedScans(IEnumerable<QuantifiedPeptide> quantPeps)
        {
            foreach (QuantifiedPeptide pep in quantPeps)
            {

            }
        }

        #endregion
    }
}
