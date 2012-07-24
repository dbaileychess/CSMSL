using System;
using System.Collections.Generic;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Protein : AminoAcidPolymer
    {
        private List<Peptide> _childern;

        public List<Peptide> Childern
        {
            get
            {
                return _childern;
            }
        }

        public Protein(string sequence)
            : base(sequence)
        {
            _childern = new List<Peptide>();
        }

        public List<Peptide> Digest(Protease protease, int? maxMissedCleavages, int? minLength, int? maxLength)
        {
            _childern.Clear();

            List<int> indices = new List<int>() { -1 };
            indices.AddRange(protease.GetDigestionSiteIndices(this));
            indices.Add(Length - 1);

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
                        Peptide peptide = new Peptide(this._residues.GetRange(begin, len), mods, this, begin);
                        _childern.Add(peptide);
                    }
                }
            }

            return _childern;
        }
    }
}