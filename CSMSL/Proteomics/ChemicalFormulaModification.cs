using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class ChemicalFormulaModification : Modification, IChemicalFormula
    {
        /// <summary>
        /// The Chemical Formula of this modifications
        /// </summary>
        public ChemicalFormula ChemicalFormula { get; private set; }

        public ChemicalFormulaModification(string chemicalFormula, string name = "", ModificationSites sites = ModificationSites.Any)
            : this(new ChemicalFormula(chemicalFormula), name, sites)
        {

        }

        public ChemicalFormulaModification(ChemicalFormula chemicalFormula, string name = "", ModificationSites sites = ModificationSites.Any)
            : base(chemicalFormula.MonoisotopicMass, name, sites)
        {
            ChemicalFormula = chemicalFormula;
        }
        
        public ChemicalFormulaModification(ChemicalFormulaModification other)
            : this(new ChemicalFormula(other.ChemicalFormula), other.Name, other.Sites)
        {

        }
    }
}
