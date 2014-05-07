using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class ChemicalFormulaFragment : Fragment, IChemicalFormula
    {
        public ChemicalFormula ChemicalFormula { get; private set; }

        public ChemicalFormulaFragment(FragmentTypes type, int number, string chemicalFormula, AminoAcidPolymer parent, IEnumerable<IMass> mods = null, string descrip = null)
            :this(type, number, new ChemicalFormula(chemicalFormula), parent, mods, descrip)
        {

        }

        public ChemicalFormulaFragment(FragmentTypes type, int number, ChemicalFormula formula, AminoAcidPolymer parent, IEnumerable<IMass> mods = null, string descrip = null)
            : base(type, number, formula.MonoisotopicMass, parent, mods, descrip)
        {
            ChemicalFormula = new ChemicalFormula(formula);
        }
    }
}
