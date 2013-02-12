///////////////////////////////////////////////////////////////////////////
//  ChemicalModification.cs - A chemical that modifies a protein          /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace CSMSL.Chemistry
{
    public class ChemicalModification : ChemicalFormula
    {
        #region Reagents

        public static ChemicalModification iTRAQ4Plex = new ChemicalModification("C{13}3C4N{15}NOH12", "iTRAQ 4-plex");
        public static ChemicalModification iTRAQ8Plex = new ChemicalModification("C{13}7C7N{15}N3O3H24", "iTRAQ 8-plex");
        public static ChemicalModification TMT6plex = new ChemicalModification("C{13}4C8N{15}NO2H20", "TMT 6-plex");
        public static ChemicalModification CAM = new ChemicalModification("C2H3NO", "Carbamidomethyl");
        public static ChemicalModification PHOSPHO = new ChemicalModification("HPO3", "Phosphorylation");
        public static ChemicalModification OX = new ChemicalModification("O", "Oxidation");
        public static ChemicalModification ACETYL = new ChemicalModification("C2H2O", "Acetyl");

        #endregion

        private static Dictionary<string, ChemicalModification> _modifications;

        static ChemicalModification()
        {
            _modifications = new Dictionary<string, ChemicalModification>();
            iTRAQ4Plex = AddModification("C{13}3C4N{15}NOH12", "iTRAQ 4-plex");
            iTRAQ8Plex = AddModification("C{13}7C7N{15}N3O3H24", "iTRAQ 8-plex");
            TMT6plex = AddModification("C{13}4C8N{15}NO2H20", "TMT 6-plex");
            CAM = AddModification("C2H3NO", "Carbamidomethyl");
            PHOSPHO = AddModification("HPO3", "Phosphorylation");
            OX = AddModification("O", "Oxidation");
            ACETYL = AddModification("C2H2O", "Acetyl");
        }

        public static ChemicalModification AddModification(string chemicalFormula, string name)
        {
            ChemicalModification mod = new ChemicalModification(chemicalFormula, name);
            _modifications.Add(name, mod);
            return mod;
        }

        public static ChemicalModification GetModification(string name)
        {
            return _modifications[name];
        }

        public static bool TryGetModification(string name, out ChemicalModification mod)
        {
            return _modifications.TryGetValue(name, out mod);
        }
        
        private string _name;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    return base.ToString();
                return _name;
            }
        }

        public ChemicalModification(ChemicalFormula chemicalFormula, string name)
            : base(chemicalFormula) 
        {
            _name = name;
        }

        public ChemicalModification(ChemicalFormula chemicalFormula)
            : this(chemicalFormula, string.Empty) { }

        public ChemicalModification(string chemicalFormula)
            : this(chemicalFormula, string.Empty) { }

        public ChemicalModification(string chemicalFormula, string name)
            : base(chemicalFormula)
        {
            _name = name;
        }

        public ChemicalModification Append(ChemicalModification mod)
        {          
            return new ChemicalModification(this + mod, string.Format("{0} + {1}", this.Name, mod.Name));;
        }

        public override string ToString()
        {
            return Name;
        }

        internal static ChemicalModification MakeHeavy(Proteomics.IAminoAcid aminoAcidResidue)
        {
            ChemicalFormula formula = new ChemicalFormula();         
            Isotope c12 = Element.PeriodicTable["C"][12];
            Isotope n14 = Element.PeriodicTable["N"][14];
            Isotope c13 = Element.PeriodicTable["C"][13];
            Isotope n15 = Element.PeriodicTable["N"][15];
            int c12_count = aminoAcidResidue.ChemicalFormula.Count(c12);
            int n14_count = aminoAcidResidue.ChemicalFormula.Count(n14);
            formula.Add(c13, c12_count);
            formula.Add(n15, n14_count);
            formula.Remove(c12, c12_count);
            formula.Remove(n14, n14_count);
            return new ChemicalModification(formula.ToString(), "#");
        }
    }
}