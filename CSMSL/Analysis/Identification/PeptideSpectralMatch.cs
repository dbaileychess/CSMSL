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
            get { return Spectrum.MassSpectrum; }
        }

        public bool Equals(MSDataScan other)
        {
            return this.Spectrum.Equals(other);
        }
    }
}
