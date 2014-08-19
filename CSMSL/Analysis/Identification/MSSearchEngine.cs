// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MSSearchEngine.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.Util.Collections;
using System.Collections.Generic;

namespace CSMSL.Analysis.Identification
{
    public abstract class MSSearchEngine
    {
        public Tolerance PrecursorMassTolerance { get; set; }

        public Tolerance ProductMassTolerance { get; set; }

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

        public abstract PeptideSpectralMatch Search(IMassSpectrum massSpectrum, Peptide peptide, FragmentTypes fragmentTypes, Tolerance productMassTolerance);

        public virtual SortedMaxSizedContainer<PeptideSpectralMatch> Search(IMassSpectrum massSpectrum, IEnumerable<Peptide> peptides)
        {
            return Search(massSpectrum, peptides, DefaultFragmentType, ProductMassTolerance);
        }

        public virtual SortedMaxSizedContainer<PeptideSpectralMatch> Search(IMassSpectrum massSpectrum, IEnumerable<Peptide> peptides, FragmentTypes fragmentTypes, Tolerance productMassTolerance)
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