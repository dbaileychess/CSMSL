// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CSMSL.Chemistry
{
    /// <summary>
    /// Represents a single isotope of a chemical element. Contains a unique number
    /// of protons and neutrons compared to every other isotope.
    /// </summary>
    public sealed class Isotope
    {
        /// <summary>
        /// Create a new isotope
        /// </summary>
        /// <param name="parentElement">The parent element of the isotope</param>
        /// <param name="massNumber">The mass number of the isotope</param>
        /// <param name="atomicMass">The atomic mass of the isotope</param>
        /// <param name="abundance">The natural relative abundance of the isotope</param>
        internal Isotope(Element parentElement, int massNumber, double atomicMass, float abundance)
        {
            Element = parentElement;
            MassNumber = massNumber;
            AtomicMass = atomicMass;
            RelativeAbundance = abundance;
        }

        /// <summary>
        /// Unique numerical ID used to optimized chemical formula construction
        /// </summary>
        internal int UniqueId { get; set; }

        /// <summary>
        /// Is this the most abundant isotope of its parent element?
        /// </summary>
        internal bool IsPrincipalIsotope { get; set; }

        /// <summary>
        /// The atomic symbol for this isotope
        /// </summary>
        public string AtomicSymbol
        {
            get { return Element.AtomicSymbol; }
        }

        /// <summary>
        /// The number of valence electrons for this element
        /// </summary>
        public int ValenceElectrons
        {
            get { return Element.ValenceElectrons; }
        }

        /// <summary>
        /// The atomic number of the isotope's parent element (also the number of protons)
        /// </summary>
        public int AtomicNumber
        {
            get { return Element.AtomicNumber; }
        }

        /// <summary>
        /// The number of protons in this isotope
        /// </summary>
        public int Protons
        {
            get { return Element.AtomicNumber; }
        }

        /// <summary>
        /// The number of neutrons in this isotope
        /// </summary>
        public int Neutrons
        {
            get { return MassNumber - Element.AtomicNumber; }
        }

        /// <summary>
        /// The element this isotope is apart of (based on atomic number)
        /// </summary>
        public Element Element { get; private set; }

        /// <summary>
        /// The atomic mass of this isotope (in unified atomic mass units)
        /// </summary>
        public double AtomicMass { get; private set; }

        /// <summary>
        /// The total number of nucleons (protons and neutrons) in this isotope
        /// </summary>
        public int MassNumber { get; private set; }

        /// <summary>
        /// The relative natural abundance of this isotope in nature (on Earth)
        /// </summary>
        public float RelativeAbundance { get; private set; }

        /// <summary>
        /// Returns a textual representation of this isotope in the following format: H1 He4 O16
        /// </summary>
        /// <returns>The atomic symbol and mass number combined</returns>
        public override string ToString()
        {
            return string.Format("{0}{1:N0}", AtomicSymbol, MassNumber);
        }
    }
}