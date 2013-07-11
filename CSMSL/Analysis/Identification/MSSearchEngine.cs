using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.Util.Collections;
using System.Collections.Generic;

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


        public void LoadPeptides(IEnumerable<AminoAcidPolymer> peptides)
        {
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
