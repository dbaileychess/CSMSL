using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using System;
using System.Collections.Generic;

namespace CSMSL.Analysis.Identification
{
    public class PeptideSpectralMatch : IFalseDiscovery<double>, IMassSpectrum, IEquatable<MSDataScan>,
        IComparable<PeptideSpectralMatch>, IMass
    {
        public virtual Peptide Peptide { get; set; }

        public virtual MsnDataScan Spectrum { get; set; }

        public virtual int Charge { get; set; }

        public virtual int SpectrumNumber { get; set; }

        public virtual string FileName { get; set; }

        /// <summary>
        /// Theoretical Precursor M/Z
        /// </summary>
        public virtual double PrecursorMz
        {
            get { return Peptide.ToMz(Charge); }
        }

        public virtual double MonoisotopicMass
        {
            get
            {
                return Peptide.MonoisotopicMass;
            }
        }

        public virtual double IsolationMz { get; set; }

        public virtual MassTolerance PrecursorMassError { get; set; }

        private Dictionary<string, string> _extraData;

        public PeptideSpectralMatch(PeptideSpectralMatchScoreType type = PeptideSpectralMatchScoreType.LowerIsBetter)
        {
            ScoreType = type;
        }

        /// <summary>
        /// The score of the match between the peptide and spectrum.
        /// </summary>
        public virtual double Score { get; set; }

        public PeptideSpectralMatchScoreType ScoreType { get; set; }

        public virtual bool IsDecoy { get; set; }

        double IFalseDiscovery<double>.FdrScoreMetric
        {
            get { return Score; }
        }

        MassSpectrum IMassSpectrum.MassSpectrum
        {
            get
            {
                if (Spectrum == null)
                    return null;
                return Spectrum.MassSpectrum;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (SN: {1} Score: {2:G3} {3})", Peptide, SpectrumNumber, Score,
                Enum.GetName(typeof (PeptideSpectralMatchScoreType), ScoreType));
        }

        public bool Equals(MSDataScan other)
        {
            return Spectrum.Equals(other);
        }

        /// <summary>
        /// Compares two PSM based on their score. 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PeptideSpectralMatch other)
        {
            if (other == null)
                return 1;

            if (!ScoreType.Equals(other.ScoreType))
            {
                throw new ArgumentException("Cannot compare peptide spectral matches with different score types");
            }

            // The sign of the scoretype enum indicates how they should be compared
            return Score.CompareTo(other.Score)*Math.Sign((int) ScoreType);
        }

        public string this[string name]
        {
            get { return _extraData[name]; }
        }

        public void AddExtraData(string name, string value)
        {
            if (_extraData == null)
                _extraData = new Dictionary<string, string>();
            _extraData[name] = value;
        }
    }
}
