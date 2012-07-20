using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Proteomics
{
    public class Protein : AminoAcidPolymer
    {
        public List<Peptide> Childern;

        public Protein(string sequence)
            : base(sequence)
        {
            Childern = new List<Peptide>();
        }
    }
}
