// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Protein.cs) is part of CSMSL.
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

using CSMSL.IO;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Proteomics
{
    public class Protein : AminoAcidPolymer
    {
        public Protein(string sequence, string description = "", bool isDecoy = false)
            : base(sequence)
        {
            Description = description;
            IsDecoy = isDecoy;
        }

        /// <summary>
        /// The description for the protein
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether this is a decoy protein or not
        /// </summary>
        public bool IsDecoy { get; set; }

        /// <summary>
        /// Converts this protein into the fasta format
        /// </summary>
        /// <returns></returns>
        public Fasta ToFasta()
        {
            return new Fasta(Sequence, Description);
        }

        #region Digestion

        /// <summary>
        /// Digests this protein into peptides.
        /// </summary>
        /// <param name="protease">The protease to digest with</param>
        /// <param name="maxMissedCleavages">The max number of missed cleavages generated, 0 means no missed cleavages</param>
        /// <param name="minLength">The minimum length (in amino acids) of the peptide</param>
        /// <param name="maxLength">The maximum length (in amino acids) of the peptide</param>
        /// <param name="initiatorMethonine"></param>
        /// <param name="includeModifications"></param>
        /// <param name="semiDigestion"></param>
        /// <returns>A list of digested peptides</returns>
        public virtual IEnumerable<Peptide> Digest(IProtease protease, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue, bool initiatorMethonine = true, bool includeModifications = false, bool semiDigestion = false)
        {
            return Digest(new[] {protease}, maxMissedCleavages, minLength, maxLength, initiatorMethonine, includeModifications, semiDigestion);
        }

        /// <summary>
        /// Digests this protein into peptides.
        /// </summary>
        /// <param name="proteases">The proteases to digest with</param>
        /// <param name="maxMissedCleavages">The max number of missed cleavages generated, 0 means no missed cleavages</param>
        /// <param name="minLength">The minimum length (in amino acids) of the peptide</param>
        /// <param name="maxLength">The maximum length (in amino acids) of the peptide</param>
        /// <param name="initiatorMethonine"></param>
        /// <param name="includeModifications"></param>
        /// <param name="semiDigestion"></param>
        /// <returns>A list of digested peptides</returns>
        public virtual IEnumerable<Peptide> Digest(IEnumerable<IProtease> proteases, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue, bool initiatorMethonine = true, bool includeModifications = false, bool semiDigestion = false)
        {
            return GetDigestionPoints(Sequence, proteases, maxMissedCleavages, minLength, maxLength, initiatorMethonine, semiDigestion).Select(points => new Peptide(this, points.Item1, points.Item2, includeModifications));
        }

        #endregion Digestion
    }
}