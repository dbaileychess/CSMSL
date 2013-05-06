using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Spectral;

namespace CSMSL.Analysis.Identification
{
    public class PeptideSpectralMatch : IFalseDiscovery<double>, IMassSpectrum, IEquatable<MSDataScan>, IComparable<PeptideSpectralMatch>
    {
        public virtual Peptide Peptide { get; set; }

        public virtual MsnDataScan Spectrum { get; set; }

        public virtual int Charge { get; set; }

        public virtual int SpectrumNumber { get; set; }     

        public virtual string FileName { get; set; }

        public PeptideSpectralMatch(PeptideSpectralMatchScoreType type = PeptideSpectralMatchScoreType.LowerIsBetter)
        {
            ScoreType = type;
        }

        /// <summary>
        /// The score of the match between the peptide and spectrum.
        /// </summary>
        public virtual double Score { get; set; }

        public virtual PeptideSpectralMatchScoreType ScoreType { get; set; }

        public virtual bool IsDecoy { get;  set; }
        
        double IFalseDiscovery<double>.FDRScoreMetric
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
            return string.Format("{0} (SN: {1} Score: {2:G3} {3})", Peptide, SpectrumNumber, Score, Enum.GetName(typeof(PeptideSpectralMatchScoreType), ScoreType));
        }

        public bool Equals(MSDataScan other)
        {
            return this.Spectrum.Equals(other);
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
            return Score.CompareTo(other.Score) * Math.Sign((int)ScoreType);
        }
    }
}
