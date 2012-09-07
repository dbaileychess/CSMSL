///////////////////////////////////////////////////////////////////////////
//  AminoAcidDictionary.cs - A collection of amino acid residues          /
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
using System.Xml;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public sealed class AminoAcidDictionary : IEnumerable<AminoAcidResidue>
    {
        private static readonly AminoAcidDictionary _instance = new AminoAcidDictionary();
        private static readonly PeriodicTable PERIODIC_TABLE = PeriodicTable.Instance;

        /// <summary>
        /// The single instance of the Amino Acid Dictionary created by the program (singleton pattern)
        /// </summary>
        public static AminoAcidDictionary Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// The internal dictionary housing all the elements, keyed by their unique atomic symbol
        /// </summary>
        internal Dictionary<string, AminoAcidResidue> _residuesSymbol;

        internal Dictionary<char, AminoAcidResidue> _residuesLetter;

        public AminoAcidResidue this[string symbol]
        {
            get
            {
                return _residuesSymbol[symbol];
            }
        }

        public AminoAcidResidue this[char letter]
        {
            get
            {
                return _residuesLetter[letter];
            }
        }

        static AminoAcidDictionary() { }

        /// <summary>
        /// Creates a default periodictable with the supplied elements in "Resources/Elements.xml"
        /// </summary>
        private AminoAcidDictionary()
        {
            _residuesSymbol = new Dictionary<string, AminoAcidResidue>(42);
            _residuesLetter = new Dictionary<char, AminoAcidResidue>(21);
            LoadResidues("Resources/AminoAcids.xml");
        }

        /// <summary>
        /// Load a xml file containing amino acid residues into this dictionary
        /// </summary>
        /// <param name="residueListXML">The xml file containing the data</param>
        public void LoadResidues(string residueListXML)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(residueListXML);
            XmlNode periodictableNode = doc.SelectSingleNode("/AminoAcids");
            foreach (XmlNode elementNode in periodictableNode.SelectNodes("AminoAcid"))
            {
                string name = elementNode.SelectSingleNode("Name").InnerText;
                char _char = char.Parse(elementNode.SelectSingleNode("Char").InnerText);
                string symbol = elementNode.SelectSingleNode("Symbol").InnerText;
                string chemicalFormula = elementNode.SelectSingleNode("ChemicalFormula").InnerText;
                AminoAcidResidue residue = new AminoAcidResidue(name, _char, symbol, new ChemicalFormula(chemicalFormula));
                AddResidue(residue);
            }
        }

        public bool TryGetResidue(char letter, out AminoAcidResidue residue)
        {
            return _residuesLetter.TryGetValue(letter, out residue);
        }

        public bool TryGetResidue(string symbol, out AminoAcidResidue residue)
        {
            return _residuesSymbol.TryGetValue(symbol, out residue);
        }

        /// <summary>
        /// Adds, or overwrites, an amino acid residue to the global dictionary. It will key the amino acid by its
        /// 1-letter abbreviation, 3-letter symbol, and name, overwriting any keys that already exist.
        /// </summary>
        /// <param name="aminoAcid">The amino acid residue to add to the dictionary</param>
        public void AddResidue(AminoAcidResidue aminoAcid)
        {
            if (_residuesLetter.ContainsKey(aminoAcid.Letter))
            {
                _residuesLetter[aminoAcid.Letter] = aminoAcid;
            }
            else
            {
                _residuesLetter.Add(aminoAcid.Letter, aminoAcid);
            }
            if (_residuesSymbol.ContainsKey(aminoAcid.Symbol))
            {
                _residuesSymbol[aminoAcid.Symbol] = aminoAcid;
            }
            else
            {
                _residuesSymbol.Add(aminoAcid.Symbol, aminoAcid);
            }

            if (_residuesSymbol.ContainsKey(aminoAcid.Name))
            {
                _residuesSymbol[aminoAcid.Name] = aminoAcid;
            }
            else
            {
                _residuesSymbol.Add(aminoAcid.Name, aminoAcid);
            }
        }

        public IEnumerator<AminoAcidResidue> GetEnumerator()
        {
            return _residuesLetter.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _residuesLetter.Values.GetEnumerator();
        }
    }
}