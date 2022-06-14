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