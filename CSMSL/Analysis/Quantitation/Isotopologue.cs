using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class Isotopologue : NamedChemicalFormula, IQuantitationChannel
    {
        public Isotopologue(string chemicalFormula, string name)
            : base(chemicalFormula, name)
        {

        }

        public bool IsMS1Based
        {
            get { return true; }
        }

        public bool IsSequenceDependent
        {
            get { return true; }
        }
    }
}
