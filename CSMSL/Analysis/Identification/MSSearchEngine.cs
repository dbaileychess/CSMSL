using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Spectral;
using CSMSL.Proteomics;
using CSMSL.Util.Collections;

namespace CSMSL.Analysis.Identification
{
    public abstract class MSSearchEngine
    {
        public MassTolerance PrecursorMassTolerance { get; set; }

        public MassTolerance ProductMassTolerance { get; set; }

        public int MaxMatchesPerSpectrum { get; set; }

        public PeptideSpectralMatchScoreType DefaultPSMScoreType { get; set; }

        public abstract SortedMaxSizedContainer<PeptideSpectralMatch> Search(IMassSpectrum massSpectrum);

        public abstract SortedMaxSizedContainer<PeptideSpectralMatch> Search(IEnumerable<IMassSpectrum> massSpectra);

        private List<AminoAcidPolymer> _peptides;

        public void LoadPeptides(IEnumerable<AminoAcidPolymer> peptides)
        {
            _peptides = peptides.OrderBy(pep => pep.Mass.Monoisotopic).ToList();     
        }
               
    }
}
