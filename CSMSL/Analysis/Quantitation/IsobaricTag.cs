using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class IsobaricTag : NamedChemicalFormula, IChemicalFormula, IQuantitationChannel 
    {       
        public IsobaricTag(string chemicalFormula, string name)
            : base(chemicalFormula, name)
        {
         
        }

        public IsobaricTag(ChemicalFormula chemicalFormula, string name)
            : base(chemicalFormula, name)
        {

        }
         
        public bool IsMS1Based
        {
	        get { return false; }
        }
    }
}
