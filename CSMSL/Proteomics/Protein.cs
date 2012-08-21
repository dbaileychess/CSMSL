///////////////////////////////////////////////////////////////////////////
//  Protein.cs - An intact amino acid polymer                             /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Protein : AminoAcidPolymer
    {
        private List<Peptide> _childern;

        private string _description;

        public Protein(string sequence)
            : this(sequence, string.Empty) { }

        public Protein(string sequence, string description)
            : base(sequence)
        {
            _childern = new List<Peptide>();
            _description = description;
        }

        public List<Peptide> Childern
        {
            get
            {
                return _childern;
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public List<Peptide> Digest(Protease protease, int? maxMissedCleavages, int? minLength, int? maxLength)
        {
            return Digest(new Protease[] { protease }, maxMissedCleavages, minLength, maxLength);
        }

        public void ClearChildern()
        {
            _childern.Clear();
        }

        /// <summary>
        /// Digests this protein into peptides. Peptides are stored within the protein for easy access, this digestion overwrites and previously generated peptides.
        /// </summary>
        /// <param name="proteases">The proteases to digest with</param>
        /// <param name="maxMissedCleavages">The max number of missed cleavages generated, 0 means no missed cleavages</param>
        /// <param name="minLength">The minimum length (in amino acids) of the peptide</param>
        /// <param name="maxLength">The maximum length (in amino acids) of the peptide</param>
        /// <returns>A list of digested peptides</returns>
        public List<Peptide> Digest(IEnumerable<Protease> proteases, int? maxMissedCleavages, int? minLength, int? maxLength)
        {
            _childern.Clear();

            HashSet<int> locations = new HashSet<int>() { -1 };
            foreach (Protease protease in proteases)
            {
                locations.UnionWith(protease.GetDigestionSiteIndices(this));
            }
            locations.Add(Length - 1);

            List<int> indices = new List<int>(locations);
            indices.Sort();

            int min = (minLength.HasValue) ? minLength.Value : 1;
            int max = (maxLength.HasValue) ? maxLength.Value : int.MaxValue;
            int max_missed = (maxMissedCleavages.HasValue) ? maxMissedCleavages.Value : 0;

            for (int missed_cleavages = 0; missed_cleavages <= max_missed; missed_cleavages++)
            {
                for (int i = 0; i < indices.Count - missed_cleavages - 1; i++)
                {
                    int len = indices[i + missed_cleavages + 1] - indices[i];
                    if (len >= minLength && len <= maxLength)
                    {
                        int begin = indices[i] + 1;
                        int end = begin + len + 1;
                        ChemicalModification[] mods = new ChemicalModification[len + 2];
                        Array.Copy(this._modifications, begin + 1, mods, 1, len);
                        mods[0] = (begin == 0) ? _modifications[0] : AminoAcidPolymer.DefaultNTerm;
                        mods[len + 1] = (end == _modifications.Length - 1) ? _modifications[end] : AminoAcidPolymer.DefaultCTerm;
                        Peptide peptide = new Peptide(this._residues.GetRange(begin, len), mods, this, begin + 1);
                        _childern.Add(peptide);
                    }
                }
            }

            return _childern;
        }
    }
}