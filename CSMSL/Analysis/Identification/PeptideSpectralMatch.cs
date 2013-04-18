using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Spectral;

namespace CSMSL.Analysis.Identification
{
    public class PeptideSpectralMatch : IFalseDiscovery<double>, IMassSpectrum, IEquatable<MSDataScan>
    {
        public virtual Peptide Peptide { get; set; }

        public virtual MsnDataScan Spectrum { get; set; }

        public virtual int SpectrumNumber { get; set; }

        public virtual string FileName { get; set; }

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
       
    }
}
