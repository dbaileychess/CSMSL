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

        protected List<Peptide> Peptides; 

        /// <summary>
        /// Gets or sets the maximum number of peptide spectral matches to record per spectrum
        /// </summary>
        public int MaxMatchesPerSpectrum { get; set; }

        public int MaxProductIonChargeState { get; set; }

        public int MaxVariableModificationsPerPeptide { get; set; }

        public PeptideSpectralMatchScoreType DefaultPsmScoreType { get; protected set; }

        public FragmentTypes DefaultFragmentType { get; protected set; }

        public virtual PeptideSpectralMatch Search(IMassSpectrum massSpectrum, Peptide peptide)
        {
            return Search(massSpectrum, peptide, DefaultFragmentType, ProductMassTolerance);
        }

        public abstract PeptideSpectralMatch Search(IMassSpectrum massSpectrum, Peptide peptide, FragmentTypes fragmentTypes, MassTolerance productMassTolerance);

        public virtual SortedMaxSizedContainer<PeptideSpectralMatch> Search(IMassSpectrum massSpectrum, IEnumerable<Peptide> peptides)
        {
            return Search(massSpectrum, peptides, DefaultFragmentType, ProductMassTolerance);
        }

        public virtual SortedMaxSizedContainer<PeptideSpectralMatch> Search(IMassSpectrum massSpectrum, IEnumerable<Peptide> peptides, FragmentTypes fragmentTypes, MassTolerance productMassTolerance)
        {
            SortedMaxSizedContainer<PeptideSpectralMatch> results = new SortedMaxSizedContainer<PeptideSpectralMatch>(MaxMatchesPerSpectrum);

            foreach (var peptide in peptides)
            {
                results.Add(Search(massSpectrum, peptide, fragmentTypes, productMassTolerance));
            }

            return results;
        }
      
        public void LoadPeptides(IEnumerable<AminoAcidPolymer> peptides)
        {
        }

        public void AddVariableModification(IMass modification, char residue)
        {

        }

        public void AddVariableModification(IMass modificaiton, IAminoAcid aminoAcid)
        {

        }

        public void AddVariableModification(IMass modificaiton, Terminus terminus)
        {
            
        }

        public void AddVariableModification(IMass modificaiton, ModificationSites sites)
        {

        }

        public void ClearVariableModifications()
        {

        }

        protected MSSearchEngine()
        {
            MaxMatchesPerSpectrum = 10;
        }
               
    }
}
