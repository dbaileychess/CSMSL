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

        private bool _isCTerminal;

        public bool IsCTerminal
        {
            get { return _isCTerminal; }
            set { _isCTerminal = value; }
        }

        public bool IsNTerminal
        {
            get { return !_isCTerminal; }
        }

        public Protease(string name, bool isCTerminal, string cleavePattern)
        {
            _name = name;
            _isCTerminal = isCTerminal;             
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