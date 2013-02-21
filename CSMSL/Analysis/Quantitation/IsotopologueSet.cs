using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class IsotopologueSet: IChemicalFormula
    {
        public string Name { get; set; }

        public List<Isotopologue> _isotopologues;

        public ChemicalFormula ChemicalFormula { get; private set; }

        public Mass Mass
        {
            get { return ChemicalFormula.Mass; }
        }

        public IsotopologueSet(string name)
        {
            Name = name;
            _isotopologues = new List<Isotopologue>();
            ChemicalFormula = new ChemicalFormula();
        }

        public void AddIsotopologue(Isotopologue isotopologue)
        {
            _isotopologues.Add(isotopologue);
            ChemicalFormula = isotopologue;
        }

        public override string ToString()
        {
            return Name;
        }



    }
}
