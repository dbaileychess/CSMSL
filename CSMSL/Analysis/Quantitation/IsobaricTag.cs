using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class IsobaricTag : NamedChemicalFormula, IChemicalFormula, IQuantitationChannel 
    {   
        private ChemicalFormula _reporterFormula;

        private ChemicalFormula _balanceFormula;

        public ChemicalFormula totalFormula
        {
            get
            {
                return _reporterFormula + _balanceFormula;
            }
        }

        public double reporterMass
        {
            get
            {
                return _reporterFormula.Mass.Monoisotopic;
            }
        }
        
        public IsobaricTag(string chemicalFormula, string name)
            : base(chemicalFormula, name)
        {
         
        }

        public IsobaricTag(ChemicalFormula chemicalFormula, string name)
            : base(chemicalFormula, name)
        {

        }

        public IsobaricTag(string reporterFormula, string balanceFormula, string name)
            : base(reporterFormula + balanceFormula, name)
        {
            _reporterFormula = new ChemicalFormula(reporterFormula);
            _balanceFormula = new ChemicalFormula(balanceFormula);
        }
         
        public bool IsMS1Based
        {
	        get { return false; }
        }


        public bool IsSequenceDependent
        {
            get { return false; }
        }

        Mass IQuantitationChannel.ReporterMass
        {
            get { return _reporterFormula.Mass; }
        }
    }
}
