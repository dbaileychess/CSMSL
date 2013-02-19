using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Spectral;

namespace CSMSL.Analysis.Identification
{
    public class PeptideSpectralMatch : IFalseDiscovery<double>, IMassSpectrum
    {
        public AminoAcidPolymer Peptide { get; private set; }

        public MSDataScan Spectrum { get; private set; }
        
        /// <summary>
        /// The score of the match between the peptide and spectrum.
        /// </summary>
        public double Score { get; private set; }

        public PeptideSpectralMatchScoreType ScoreType { get; private set; }

        public bool IsDecoy { get; private set; }
        
        double IFalseDiscovery<double>.FDRScoreMetric
        {
            get { return Score; }
        }

        MassSpectrum IMassSpectrum.MassSpectrum
        {
            get { return Spectrum.MassSpectrum; }
        }
    }
}
