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

using System;
using System.Collections.Generic;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// A chemical formula that contains a common name.
    /// (e.g. H2O is often called Water)
    /// </summary>
    [Obsolete]
    public class NamedChemicalFormula : ChemicalFormula, IEquatable<NamedChemicalFormula>
    {
        #region Common Reagents / Modifications

        public static NamedChemicalFormula iTRAQ4Plex { get; private set; }
        public static NamedChemicalFormula iTRAQ8Plex { get; private set; }
        public static NamedChemicalFormula TMT6plex { get; private set; }
        public static NamedChemicalFormula TMT8plex { get; private set; }
        public static NamedChemicalFormula Carbamidomethyl { get; private set; }
        public static NamedChemicalFormula Phosphorylation { get; private set; }
        public static NamedChemicalFormula Oxidation { get; private set; }
        public static NamedChemicalFormula Acetyl { get; private set; }

        #endregion

        private static readonly Dictionary<string, NamedChemicalFormula> _modifications;

        static NamedChemicalFormula()
        {
            _modifications = new Dictionary<string, NamedChemicalFormula>(8);
            iTRAQ4Plex = AddModification("C{13}3C4N{15}NOH12", "iTRAQ 4-plex");
            iTRAQ8Plex = AddModification("C{13}7C7N{15}N3O3H24", "iTRAQ 8-plex");
            TMT6plex = AddModification("C{13}4C8N{15}NO2H20", "TMT 6-plex");
            TMT8plex = AddModification("C{13}4C8N{15}NO2H20", "TMT 8-plex");
            Carbamidomethyl = AddModification("C2H3NO", "Carbamidomethyl");
            Phosphorylation = AddModification("HPO3", "Phosphorylation");
            Oxidation = AddModification("O", "Oxidation");
            Acetyl = AddModification("C2H2O", "Acetyl");
        }

        /// <summary>
        /// Register a modification by its name
        /// </summary>
        /// <param name="chemicalFormula">The chemical formula of the modification</param>
        /// <param name="name">The name of the modification</param>
        /// <returns>The chemical modification that is registered for this name</returns>
        public static NamedChemicalFormula AddModification(string chemicalFormula, string name)
        {
            NamedChemicalFormula mod = new NamedChemicalFormula(chemicalFormula, name);
            _modifications.Add(name, mod);
            return mod;
        }

        public static NamedChemicalFormula AddModification(NamedChemicalFormula mod)
        {
            _modifications.Add(mod.Name, mod);
            return mod;
        }

        public static void ClearAllModifications()
        {
            _modifications.Clear();
        }

        /// <summary>
        /// Gets a registered modification by its name
        /// </summary>
        /// <param name="name">The name of the modification</param>
        /// <returns>The chemical modification that is registered for this name</returns>
        public static NamedChemicalFormula GetModification(string name)
        {
            return _modifications[name];
        }

        /// <summary>
        /// Tries to get a registered modification by its name
        /// </summary>
        /// <param name="name">The name of the modification</param>
        /// <param name="mod">The chemical modification that is registered for this name</param>
        /// <returns>True if the modification was found, false otherwise</returns>
        public static bool TryGetModification(string name, out NamedChemicalFormula mod)
        {
            return _modifications.TryGetValue(name, out mod);
        }
        

        private readonly string _name;

        /// <summary>
        /// Gets the name of the modification. If the name is null or empty
        /// it returns the chemical formula.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    return base.ToString();
                return _name;
            }
        }

        public NamedChemicalFormula(ChemicalFormula chemicalFormula, string name)
            : base(chemicalFormula) 
        {
            _name = name;
        }

        public NamedChemicalFormula(ChemicalFormula chemicalFormula)
            : this(chemicalFormula, string.Empty) { }

        public NamedChemicalFormula(string chemicalFormula)
            : this(chemicalFormula, string.Empty) { }

        public NamedChemicalFormula(string chemicalFormula, string name)
            : base(chemicalFormula)
        {
            _name = name;
        }

        public NamedChemicalFormula Append(NamedChemicalFormula mod)
        {          
            return new NamedChemicalFormula(this + mod, string.Format("{0} + {1}", Name, mod.Name));
        }

        public override string ToString()
        {
            return Name;
        }

        internal static NamedChemicalFormula MakeHeavy(Proteomics.IAminoAcid aminoAcidResidue)
        {
            ChemicalFormula formula = new ChemicalFormula();         
            Isotope c12 = Element.PeriodicTable["C"][12];
            Isotope n14 = Element.PeriodicTable["N"][14];
            Isotope c13 = Element.PeriodicTable["C"][13];
            Isotope n15 = Element.PeriodicTable["N"][15];
            int c12Count = aminoAcidResidue.ChemicalFormula.Count(c12);
            int n14Count = aminoAcidResidue.ChemicalFormula.Count(n14);
            formula.Add(c13, c12Count);
            formula.Add(n15, n14Count);
            formula.Remove(c12, c12Count);
            formula.Remove(n14, n14Count);
            return new NamedChemicalFormula(formula.ToString(), "#");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Name.GetHashCode();
        }

        public bool Equals(NamedChemicalFormula other)
        {
            if (base.Equals(other))
            {
                return Name.Equals(other.Name);
            }
            return false;
        }
    }
}