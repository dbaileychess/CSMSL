// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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