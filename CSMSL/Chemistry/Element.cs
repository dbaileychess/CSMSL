// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Element.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// Represents a single chemical element. Elements comprises of multiple
    /// isotopes, with the element mass being a weighted average of all the
    /// isotopes atomic masses weighted by their natural relative abundance.
    /// </summary>
    public sealed class Element
    {
        /// <summary>
        /// The element's isotopes stored based on their atomic number
        /// </summary>
        internal Dictionary<int, Isotope> Isotopes;

        /// <summary>
        /// Gets an isotope of this element based on its atomic number
        /// </summary>
        /// <param name="atomicNumber">The atomic number of the isotope to get</param>
        /// <returns>The isotope with the supplied atomic number</returns>
        public Isotope this[int atomicNumber]
        {
            get { return Isotopes[atomicNumber]; }
        }

        /// <summary>
        /// Create a new element
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <param name="symbol">The symbol of the element</param>
        /// <param name="atomicNumber">The atomic number of the element</param>
        /// <param name="valenceElectrons">The number of valence electrons of the element</param>
        internal Element(string name, string symbol, int atomicNumber, int valenceElectrons)
        {
            Name = name;
            AtomicSymbol = symbol;
            AtomicNumber = atomicNumber;
            ValenceElectrons = valenceElectrons;
            AverageMass = 0;
            TotalAbundance = 0;
            Isotopes = new Dictionary<int, Isotope>();
        }

        /// <summary>
        /// The atomic number of this element (also the number of protons)
        /// </summary>
        public int AtomicNumber { get; private set; }

        /// <summary>
        /// The atomic symbol of this element
        /// </summary>
        public string AtomicSymbol { get; private set; }

        /// <summary>
        /// The average mass of all this element's isotopes weighted by their
        /// relative natural abundance (in unified atomic mass units)
        /// </summary>
        public double AverageMass { get; private set; }

        /// <summary>
        /// The total abundance of all this isotopes (should be nearly one, any deviation
        /// is due to the lack of precision in the raw NIST data)
        /// </summary>
        public double TotalAbundance { get; private set; }

        /// <summary>
        /// The total number of valence electrons for this element
        /// </summary>
        public int ValenceElectrons { get; private set; }

        /// <summary>
        /// The most abundant (principal) isotope of this element
        /// </summary>
        public Isotope PrincipalIsotope { get; private set; }

        /// <summary>
        /// The name of this element
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The number of isotopes this element comprises of (only isotopes with
        /// natural relative abundances > 0% are considered)
        /// </summary>
        public int IsotopeCount
        {
            get { return Isotopes.Count; }
        }

        /// <summary>
        /// Returns a textual representation of this element in the following format: Hydrogen (H) Helium (He)
        /// </summary>
        /// <returns>The name and atomic symbol</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, AtomicSymbol);
        }

        /// <summary>
        /// The sum of the weighted isotope masses
        /// </summary>
        private double _totalMass;

        /// <summary>
        /// Add an isotope to this element
        /// </summary>
        /// <param name="atomicNumber">The atomic number of the isotope</param>
        /// <param name="atomicMass">The atomic mass of the isotope </param>
        /// <param name="abundance">The natural relative abundance of the isotope</param>
        /// <returns>The created isotopes that is added to this element</returns>
        internal Isotope AddIsotope(int atomicNumber, double atomicMass, float abundance)
        {
            var isotope = new Isotope(this, atomicNumber, atomicMass, abundance);
            if (Isotopes.ContainsKey(atomicNumber))
                return isotope;
            Isotopes.Add(atomicNumber, isotope);
            TotalAbundance += abundance;
            _totalMass += abundance*atomicMass;
            AverageMass = _totalMass/TotalAbundance;
            if (PrincipalIsotope != null && !(abundance > PrincipalIsotope.RelativeAbundance))
                return isotope;
            if (PrincipalIsotope != null)
            {
                PrincipalIsotope.IsPrincipalIsotope = false;
            }
            PrincipalIsotope = isotope;
            PrincipalIsotope.IsPrincipalIsotope = true;
            return isotope;
        }
    }
}