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

using System.Collections.Generic;

namespace CSMSL.Proteomics
{
    /// <summary>
    /// A proteolyic enzyme that cuts amino acids at specific residues.
    /// </summary>
    public interface IProtease
    {
        /// <summary>
        /// Finds the indicies of where this protease would cut in
        /// the given amino acid sequence
        /// </summary>
        /// <param name="aminoAcidSequence">The Amino Acid Polymer to cut</param>
        /// <returns>A set of the 1-based indicies to cut at</returns>
        IEnumerable<int> GetDigestionSites(string aminoAcidSequence);

        IEnumerable<int> GetDigestionSites(IAminoAcidSequence aminoAcidSequence);

        int MissedCleavages(string sequence);

        int MissedCleavages(IAminoAcidSequence aminoAcidSequence);
    }
}