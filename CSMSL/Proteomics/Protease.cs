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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSMSL.Proteomics
{
    public class Protease: IProtease
    {        
        public static Protease Trypsin {get; private set;}
        public static Protease TrypsinNoProlineRule { get; private set; }
        public static Protease GluC { get; private set; }
        public static Protease LysN { get; private set; }
        public static Protease ArgC { get; private set; }
        public static Protease Chymotrypsin { get; private set; }
        public static Protease LysC { get; private set; }
        public static Protease CNBr { get; private set; }
        public static Protease AspN { get; private set; }
        public static Protease Thermolysin { get; private set; }
        public static Protease None { get; private set; }

        private static readonly Dictionary<string, Protease> Proteases;

        static Protease()
        {
            Proteases = new Dictionary<string, Protease>(12);

            Trypsin = AddProtease("Trypsin", Terminus.C,"KR","P");
            TrypsinNoProlineRule = AddProtease("Trypsin No Proline Rule", Terminus.C, "KR");
            GluC = AddProtease("GluC", Terminus.C, "E");
            LysN = AddProtease("LysN", Terminus.N,"K");
            ArgC = AddProtease("ArgC", Terminus.C, "R");
            Chymotrypsin = AddProtease("Chymotrypsin", Terminus.C, "YWFL","P");
            LysC = AddProtease("LysC", Terminus.C, "K");
            CNBr = AddProtease("CNBr", Terminus.C, "M");
            AspN = AddProtease("AspN", Terminus.N, "BD");
            Thermolysin = AddProtease("Thermolysin", Terminus.N, "AFILMV","DE");
            None = AddProtease("None", Terminus.C,"ACDEFGHIKLMNPQRSTVWY");
        }            

        public static IEnumerable<Protease> GetAllProteases()
        {
            return Proteases.Values;
        }

        public static Protease GetProtease(string name)
        {
            return Proteases[name];
        }

        public static bool TryGetProtease(string name, out Protease protease)
        {
            return Proteases.TryGetValue(name, out protease);
        }
        
        public static Protease AddProtease(string name, Terminus terminus, string cut, string noCut = "")
        {
            Protease protease = new Protease(name, terminus, cut, noCut);
            Proteases.Add(protease.Name, protease);
            return protease;
        }        

        private readonly Regex _cleavageRegex;

        public string Cut { get; private set; }
        public string NoCut { get; private set; }

        public string Name { get; set; }

        public Terminus Terminal { get; private set; }

        public string CleavagePattern { get { return _cleavageRegex.ToString(); } }

        public Protease(string name, Terminus terminus, string cut, string nocut = "", string cleavePattern = "")
        {
            Name = name;
            Terminal = terminus;
            Cut = cut;
            NoCut = nocut;

            if (string.IsNullOrEmpty(cleavePattern))
            {

                StringBuilder constructedRegex = new StringBuilder();

                // Add grouping [ ] if more than one residue
                string cutregex = (Cut.Length > 1) ? "[" + Cut + "]" : Cut;
                string nocutregex = (NoCut.Length > 1) ? "[" + NoCut + "]" : NoCut;

                if (terminus == Terminus.N)
                {
                    // Apply negative look-a-head for N-terminal no cuts
                    if (!string.IsNullOrEmpty(nocut))
                    {
                        constructedRegex.Append("(?<!" + nocutregex + ")");
                    }
                    constructedRegex.Append("(?'cut')");
                    constructedRegex.Append(cutregex);
                }
                else
                {
                    constructedRegex.Append(cutregex);
                    constructedRegex.Append("(?'cut')");
                    if (!string.IsNullOrEmpty(nocut))
                    {
                        constructedRegex.Append("(?!" + nocutregex + ")");
                    }
                }


                _cleavageRegex = new Regex(constructedRegex.ToString(), RegexOptions.Compiled);
            }
            else
            {
                _cleavageRegex = new Regex(cleavePattern, RegexOptions.Compiled);
            }          
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<int> GetDigestionSites(IAminoAcidSequence aminoacidpolymer)
        {
            return GetDigestionSites(aminoacidpolymer.Sequence);
        }

        public IEnumerable<int> GetDigestionSites(string sequence)
        {
            return (from Match match in _cleavageRegex.Matches(sequence) select match.Groups["cut"].Index - 1);
        }
    }
}