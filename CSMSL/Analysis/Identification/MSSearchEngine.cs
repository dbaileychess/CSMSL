using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Spectral;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.Identification
{
    public abstract class MSSearchEngine
    {
        public MassTolerance PrecursorMassTolerance { get; set; }

        public MassTolerance ProductMassTolerance { get; set; }

        public PeptideSpectralMatchScoreType DefaultPSMScoreType { get; set; }

        public abstract List<PeptideSpectralMatch> Search(IMassSpectrum massSpectrum);

        public abstract List<PeptideSpectralMatch> Search(IEnumerable<IMassSpectrum> massSpectra);

        private List<AminoAcidPolymer> _peptides;

        public void LoadPeptides(IEnumerable<AminoAcidPolymer> peptides)
        {
            _peptides = peptides.OrderBy(pep => pep.Mass.Monoisotopic).ToList();     
        }
               
    }
}
