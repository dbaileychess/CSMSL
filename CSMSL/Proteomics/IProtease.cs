// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (IProtease.cs) is part of CSMSL.
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