using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Proteomics
{
    public class ProteinGroup : IEnumerable<Protein>
    {
        private List<Protein> _proteins;
        private List<Peptide> _peptide;

        public ProteinGroup()
        {
            _proteins = new List<Protein>();
            _peptide = new List<Peptide>();
        }

        public int Count { get { return _proteins.Count; } }

        public void Add(Protein protein)
        {
            if(protein == null)
                return;
            _proteins.Add(protein);
        }
        public void Add(Peptide peptide)
        {
            if (peptide == null)
                return;
            _peptide.Add(peptide);
        }

        public IEnumerator<Protein> GetEnumerator()
        {
            return _proteins.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _proteins.GetEnumerator();
        }

        #region Statics

        private static Dictionary<Peptide, ProteinGroup> _pepToProts;

        public static void SetProteins(IEnumerable<Protein> proteins, IProtease protease, int maxMissed = 3, int minLength = 5, int maxLength = 35)
        {
            _pepToProts = new Dictionary<Peptide, ProteinGroup>();
            ProteinGroup pg;
            foreach (Protein protein in proteins)
            {
                foreach (Peptide peptide in protein.Digest(protease, maxMissed, minLength, maxLength))
                {
                    if (!_pepToProts.TryGetValue(peptide, out pg))
                    {
                        pg = new ProteinGroup();
                        _pepToProts.Add(peptide, pg);
                    }
                    pg.Add(protein);
                    pg.Add(peptide);
                }
            }
        }
        
        public static ProteinGroup Group(Peptide peptide)
        {
            ProteinGroup pg;
            if (_pepToProts.TryGetValue(peptide, out pg))
            {
                return pg;
            }
            return null;           
        }


        #endregion

      
    }
}
