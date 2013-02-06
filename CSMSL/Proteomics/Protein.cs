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
using CSMSL.IO;
using System.Collections.Generic;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Protein : AminoAcidPolymer
    {
        private string _description;

        public Protein(string sequence)
            : this(sequence, string.Empty) { }

        public Protein(string sequence, string description)
            : base(sequence)
        {           
            _description = description;
        }
     
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Fasta ToFasta()
        {
            return new Fasta(this.Sequence, this.Description);
        }

        public List<Peptide> Digest(IProtease protease, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue)
        {
            return Digest(new IProtease[] { protease }, maxMissedCleavages, minLength, maxLength);
        }
               
        /// <summary>
        /// Digests this protein into peptides. Peptides are stored within the protein for easy access, this digestion overwrites and previously generated peptides.
        /// </summary>
        /// <param name="proteases">The proteases to digest with</param>
        /// <param name="maxMissedCleavages">The max number of missed cleavages generated, 0 means no missed cleavages</param>
        /// <param name="minLength">The minimum length (in amino acids) of the peptide</param>
        /// <param name="maxLength">The maximum length (in amino acids) of the peptide</param>
        /// <returns>A list of digested peptides</returns>
        public List<Peptide> Digest(IEnumerable<IProtease> proteases, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue)
        {
            if (maxMissedCleavages < 0)
            {
                throw new ArgumentOutOfRangeException("maxMissedCleavages", "The maximum number of missedcleavages must be >= 0");
            }

            //_childern.Clear();
            List<Peptide> peptides = new List<Peptide>();

            // Combine all the proteases digestion sites
            SortedSet<int> locations = new SortedSet<int>() { -1 };
            foreach (IProtease protease in proteases)
            {
                if (protease != null)
                {
                    locations.UnionWith(protease.GetDigestionSites(this.Sequence));
                }
            }
            locations.Add(Length - 1);          
           
            IList<int> indices = new List<int>(locations);
            //indices.Sort(); // most likely not needed if locations is a sorted set

            int indiciesCount = indices.Count;     
            for (int missed_cleavages = 0; missed_cleavages <= maxMissedCleavages; missed_cleavages++)
            {
                int max = indiciesCount - missed_cleavages - 1;
                for (int i = 0; i < max; i++)
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
                        peptides.Add(peptide);
                    }
                }
            }

            return peptides;
        }
    }
}