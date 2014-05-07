using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.Quantitation
{
    public class IsobaricTag : ChemicalFormulaModification, IQuantitationChannel 
    {   
        private readonly ChemicalFormula _reporterFormula;

        private readonly ChemicalFormula _balanceFormula;

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
                return _reporterFormula.MonoisotopicMass;
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

        double IQuantitationChannel.ReporterMass
        {
            get { return _reporterFormula.MonoisotopicMass; }
        }
    }
}
