using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Spectral;
using CSMSL.Proteomics;
using CSMSL.Chemistry;
using CSMSL.Util.Collections;

namespace CSMSL.Analysis.Identification
{
    public abstract class MSSearchEngine
    {
        public MassTolerance PrecursorMassTolerance { get; set; }

        public MassTolerance ProductMassTolerance { get; set; }

        public int MaxMatchesPerSpectrum { get; set; }

        public int MaxProductIonChargeState { get; set; }

        public int MaxVariableModificationsPerPeptide { get; set; }

        public PeptideSpectralMatchScoreType DefaultPSMScoreType { get; protected set; }

        public abstract SortedMaxSizedContainer<PeptideSpectralMatch> Search(IMassSpectrum massSpectrum);

        public abstract SortedMaxSizedContainer<PeptideSpectralMatch> Search(IEnumerable<IMassSpectrum> massSpectra);

        public abstract SortedMaxSizedContainer<PeptideSpectralMatch> Search(AminoAcidPolymer peptide);

        public abstract SortedMaxSizedContainer<PeptideSpectralMatch> Search(IEnumerable<AminoAcidPolymer> peptides);

        private List<AminoAcidPolymer> _peptides;
        private List<IMassSpectrum> _spectrum;

        public void LoadPeptides(IEnumerable<AminoAcidPolymer> peptides)
        {
            _peptides = peptides.OrderBy(pep => pep.Mass.MonoisotopicMass).ToList();     
        }

        public void AddVariableModification(IChemicalFormula modification, char residue)
        {

        }

        public void AddVariableModification(IChemicalFormula modificaiton, IAminoAcid aminoAcid)
        {

        }

        public void AddVariableModification(IChemicalFormula modificaiton, Terminus terminus)
        {
            
        }

        public void AddVariableModification(IChemicalFormula modificaiton, ModificationSites sites)
        {

        }

        public void ClearVariableModifications()
        {

        }

        public MSSearchEngine()
        {
            MaxMatchesPerSpectrum = 10;
        }
               
    }
}
