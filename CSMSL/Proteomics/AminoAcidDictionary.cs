using System.Collections.Generic;
using System.Xml;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public sealed class AminoAcidDictionary
    {
        private static readonly AminoAcidDictionary _instance = new AminoAcidDictionary();

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
        private Dictionary<string, AminoAcidResidue> _residuesSymbol;

        private Dictionary<char, AminoAcidResidue> _residuesLetter;
        
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
    }
}