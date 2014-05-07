using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class ChemicalFormulaFragment : Fragment, IChemicalFormula
    {
        public ChemicalFormula ChemicalFormula { get; private set; }

        public ChemicalFormulaFragment(FragmentTypes type, int number, string chemicalFormula, AminoAcidPolymer parent)
            :this(type, number, new ChemicalFormula(chemicalFormula), parent) { }
   
        public ChemicalFormulaFragment(FragmentTypes type, int number, ChemicalFormula formula, AminoAcidPolymer parent)
            : base(type, number, formula.MonoisotopicMass, parent)
        {
            ChemicalFormula = new ChemicalFormula(formula);
        }
    }
}
