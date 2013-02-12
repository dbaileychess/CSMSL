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

using System.Collections.ObjectModel;
using System.Collections.Generic;
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

        private static Dictionary<string, Protease> _proteases;

        static Protease()
        {
            _proteases = new Dictionary<string, Protease>(12);

            Trypsin = AddProtease("Trypsin", Terminus.C, @"[K|R](?'cleave')(?!P)");
            TrypsinNoProlineRule = AddProtease("Trypsin No Proline Rule", Terminus.C, @"[K|R](?'cleave')");
            GluC = AddProtease("GluC", Terminus.C, @"E(?'cleave')");
            LysN = AddProtease("LysN", Terminus.N, @"(?'cleave')K");
            ArgC = AddProtease("ArgC", Terminus.C, @"R(?'cleave')");
            Chymotrypsin = AddProtease("Chymotrypsin", Terminus.C, @"[Y|W|F|L](?'cleave')(?!P)");
            LysC = AddProtease("LysC", Terminus.C, @"K(?'cleave')");
            CNBr = AddProtease("CNBr", Terminus.C, @"M(?'cleave')");
            AspN = AddProtease("AspN", Terminus.N, @"(?'cleave')[B|D]");
            Thermolysin = AddProtease("Thermolysin", Terminus.N, @"(?<![D|E])(?'cleave')[A|F|I|L|M|V]");
            None = AddProtease("None", Terminus.C, @"[A-Z](?'cleave')");
        }

        public static IEnumerable<Protease> GetAllProteases()
        {
            return _proteases.Values;
        }

        public static Protease GetProtease(string name)
        {
            return _proteases[name];
        }

        public static bool TryGetProtease(string name, out Protease protease)
        {
            return _proteases.TryGetValue(name, out protease);
        }

        /// <summary>
        /// Adds a protease to the singleton dictionary of proteases
        /// </summary>
        /// <param name="protease">The protease to add</param>
        public static Protease AddProtease(string name, Terminus terminus, string cleavePattern)
        {
            Protease protease = new Protease(name, terminus, cleavePattern);
            _proteases.Add(protease.Name, protease);
            return protease;
        }        

        private Regex _cleavageRegex;

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Terminus Terminal { get; private set; }

        public string CleavagePattern { get { return _cleavageRegex.ToString(); } }

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

        public ReadOnlyCollection<int> GetDigestionSites(AminoAcidPolymer aminoacidpolymer)
        {
            return GetDigestionSites(aminoacidpolymer.Sequence);
        }

        public ReadOnlyCollection<int> GetDigestionSites(string sequence)
        {
            List<int> sites = new List<int>();            
            foreach (Match match in _cleavageRegex.Matches(sequence))
            {
                sites.Add(match.Groups["cleave"].Index - 1);
            }
            return new ReadOnlyCollection<int>(sites);
        }
    }
}