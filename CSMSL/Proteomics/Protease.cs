///////////////////////////////////////////////////////////////////////////
//  Protease.cs - An enzyme that cleaves amino acid polymers              /
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

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSMSL.Proteomics
{
    public class Protease
    {
        private Regex _cleavageRegex;

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Terminus Terminal;

        public Protease(string name, Terminus terminus, string cleavePattern)
        {
            _name = name;
            Terminal = terminus;
            _cleavageRegex = new Regex(cleavePattern, RegexOptions.Compiled);
        }

        public override string ToString()
        {
            return _name;
        }

        public List<int> GetDigestionSiteIndices(AminoAcidPolymer aminoacidpolymer)
        {
            return GetDigestionSiteIndices(aminoacidpolymer.Sequence);
        }

        public List<int> GetDigestionSiteIndices(string sequence)
        {
            List<int> indices = new List<int>();
            foreach (Match match in _cleavageRegex.Matches(sequence))
            {
                indices.Add(match.Groups["cleave"].Index - 1);
            }
            return indices;
        }
    }
}