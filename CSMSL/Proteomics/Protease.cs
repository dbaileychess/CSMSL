using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSMSL.Proteomics
{
    public class Protease
    {
        private static readonly AminoAcidDictionary AMINO_ACIDS = AminoAcidDictionary.Instance;

        public Dictionary<char, List<KeyValuePair<char,int>>> CleaveAt;
   
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

        public Protease(string name, bool isCTerminal)
        {
            _name = name;
            _isCTerminal = isCTerminal;
            CleaveAt = new Dictionary<char, List<KeyValuePair<char, int>>>();          
        }
                  
        public List<int> GetDigestionSiteIndices(AminoAcidPolymer aminoacidpolymer)
        {
            List<int> indices = new List<int>();

            List<KeyValuePair<char, int>> exceptions = null;
            for (int i = 0; i < aminoacidpolymer.Length; i++)
            {
                char c = aminoacidpolymer[i].Letter;
                if (CleaveAt.TryGetValue(c, out exceptions))
                {
                    if (exceptions == null || exceptions.Count < 1)
                    {
                        indices.Add(IsCTerminal ? i : i - 1);
                    }
                    else
                    {
                        bool cleave = true;
                        foreach (KeyValuePair<char, int> exception in exceptions)
                        {
                            int j = i + exception.Value;
                            if (j < aminoacidpolymer.Length)
                            {
                                char c2 = aminoacidpolymer[j].Letter;
                                if(c2 == exception.Key)
                                {
                                    cleave = false;
                                    break;                                   
                                }
                            }                            
                        }
                        if (cleave)
                        {
                            indices.Add(IsCTerminal ? i : i - 1);
                        }
                    }     
                }               
            }
            return indices;
        }
    }


}