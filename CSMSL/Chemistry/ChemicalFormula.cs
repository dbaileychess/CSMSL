///////////////////////////////////////////////////////////////////////////
//  ChemicalFormula.cs - A collection of elements in a single molecule    /
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
using System.Text;
using System.Text.RegularExpressions;

namespace CSMSL.Chemistry
{
    public class ChemicalFormula : IMass, IEquatable<ChemicalFormula>
    {
      
        /// <summary>
        /// A regular expression for matching chemical formulas such as: C2C{13}3H5NO5
        /// The first group is the only non-optional group and that handles the chemical symbol: H, He, etc..
        /// The second are optional, which handles alternative isotopes of elements: C{13} means carbon-13, while C is the common carbon-12
        /// The third group is optional and indicates if we are adding or subtracting the elements form the formula, C-2C{13}5 would mean first subtract 2 carbon-12 and then add 5 carbon-13
        /// The fourth group is optional and represents the number of isotopes to add, if not present it assumes 1: H2O means 2 Hydrogen and 1 Oxygen
        /// Modified from: http://stackoverflow.com/questions/4116786/parsing-a-chemical-formula-from-a-string-in-c
        /// </summary>
        private static readonly Regex _formulaRegex = new Regex(@"([A-Z][a-z]*)(?:\{([0-9]+)\})?(-)?([0-9]+)?", RegexOptions.Compiled);

        private static readonly int _uniqueIDCount = 10;
        private static readonly Regex _validateFormulaRegex = new Regex("^(" + _formulaRegex.ToString() + ")+$", RegexOptions.Compiled);
        private StringBuilder _chemicalFormulaSB;

        private bool _isDirty;

        private int[] _isotopes;

        private Mass _mass;

        private int _numberOfAtoms;

        /// <summary>
        /// Create an empty chemical formula
        /// </summary>
        public ChemicalFormula()
        {            
            _isotopes = new int[_uniqueIDCount];
            _isDirty = true;
        }

        /// <summary>
        /// Create an chemical formula from the given string representation
        /// </summary>
        /// <param name="chemicalFormula">The string representation of the chemical formula</param>
        public ChemicalFormula(string chemicalFormula)
            : this()
        {
            ParseString(chemicalFormula);
        }

        public ChemicalFormula(IChemicalFormula item)
            : this(item.ChemicalFormula) { }

        public ChemicalFormula(ChemicalFormula other)
        {
            if (other == null)
            {                
                // create a new blank chemical formula
                _isotopes = new int[_uniqueIDCount];
                _isDirty = true;
            }
            else
            {
                CopyFrom(other);               
            }
        }

        private void CopyFrom(ChemicalFormula other)
        {
            // Copy an existing chemical formula
            _isotopes = new int[other._isotopes.Length];
            _largestIsotopeID = other._largestIsotopeID;
            Array.Copy(other._isotopes, _isotopes, other._isotopes.Length);
            
            if (!(_isDirty = other._isDirty))
            {
                // old chemical formula is already clean, don't need to reclean
                _chemicalFormulaSB = new StringBuilder(other._chemicalFormulaSB.ToString());
                _numberOfAtoms = other._numberOfAtoms;
                _mass = new Mass(other._mass);
            }
        }

        public Mass Mass
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _mass;
            }
        }

        public int NumberOfAtoms
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _numberOfAtoms;
            }
        }

        private int _uniqueElements;
        public int UniqueElements
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _uniqueElements;
            }
        }

        private int _uniqueIsotopes;
        public int UniqueIsotopes
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _uniqueIsotopes;
            }
        }
        
        public static implicit operator ChemicalFormula(string sequence)
        {
            return new ChemicalFormula(sequence);
        }

        public static bool IsValidChemicalFormula(string chemicalFormula)
        {
            return _validateFormulaRegex.IsMatch(chemicalFormula);
        }

        public static ChemicalFormula operator -(ChemicalFormula left, ChemicalFormula right)
        {
            ChemicalFormula newFormula = new ChemicalFormula(left);
            newFormula.Remove(right);
            return newFormula;
        }

        public static ChemicalFormula operator *(ChemicalFormula formula, int count)
        {
            ChemicalFormula newFormula = new ChemicalFormula(formula);
            for (int i = 0; i < newFormula._isotopes.Length; i++)
            {
                newFormula._isotopes[i] *= count;
            }
            newFormula._isDirty = true;
            return newFormula;
        }

        public static ChemicalFormula operator *(int count, ChemicalFormula formula)
        {
            return formula * count;
        }

        public static ChemicalFormula operator +(ChemicalFormula left, ChemicalFormula right)
        {
            ChemicalFormula newFormula = new ChemicalFormula(left);
            newFormula.Add(right);
            return newFormula;
        }

        public void Add(IChemicalFormula item)
        {
            Add(item.ChemicalFormula);
        }

        public void Add(ChemicalFormula formula)
        {
            if (formula == null) return;

            // Get the length of the formula to add
            int id = formula._largestIsotopeID;

            if (id > _largestIsotopeID)
            {
                _largestIsotopeID = id;
               
                if (id > _isotopes.Length)
                {
                    // Resize this formula array to match the size of the incoming one
                    Array.Resize(ref _isotopes, id + 1);
                }
            }          

            // Update each isotope
            for (int i = 0; i <= id; i++)
            {              
                _isotopes[i] += formula._isotopes[i];
            }

            _isDirty = true;
        }

        internal int _largestIsotopeID = 0;

        /// <summary>
        /// Add an isotope to this chemical formula
        /// </summary>
        /// <param name="isotope">The isotope to add</param>
        /// <param name="count">The number of the isotope to add</param>
        public void Add(Isotope isotope, int count)
        {
            if (isotope == null || count == 0)            
                return;            
           
            int id = isotope.UniqueID;

            if (id > _largestIsotopeID)
            {
                _largestIsotopeID = id;
                if (id > _isotopes.Length)
                {
                    // Isotope doesn't exist, resize array and set the count (faster than the += below)
                    Array.Resize(ref _isotopes, id + 1);
                    _isotopes[id] = count;
                    _isDirty = true;
                    return;
                }
            }
           
            _isotopes[id] += count;            
            _isDirty = true;
        }

        /// <summary>
        /// Remove all isotopes from this chemical formula to create an 'empty' chemical formula
        /// </summary>
        public void Clear()
        {
            Array.Clear(_isotopes, 0, _isotopes.Length);
            _isDirty = true;
        }

        /// <summary>
        /// Checks if the isotope is present in this chemical formula
        /// </summary>
        /// <param name="isotope">The isotope to look for</param>
        /// <returns>True if there is a non-negative number of the isotope in this formula</returns>
        public bool Contains(Isotope isotope)
        {
            return Count(isotope) != 0;           
        }

        public bool Contains(Element element)
        {
            return Count(element) != 0;
        }

        public bool Contains(string symbol)
        {
            return Count(symbol) != 0;
        }

        public bool Contains(string symbol, int atomicNumber)
        {
            return Count(symbol, atomicNumber) != 0;
        }
              
        ///// <summary>
        ///// Test for equality between two chemical formulas. Two formulas are equivalent if they have the exact same number and type of isotopes.
        ///// </summary>
        ///// <param name="other">The other chemical formula to compare with</param>
        ///// <returns>True if the chemical formulas are the same, false otherwise</returns>
        //public bool Equals(ChemicalFormula other)
        //{
        //    if (Object.ReferenceEquals(other, null)) return false;
        //    if (Object.ReferenceEquals(this, other)) return true;
        //    for (int i = 0; i < _isotopes.Length; i++)
        //    {
        //        if ((i > other._isotopes.Length && _isotopes[i] != 0) || _isotopes[i] != other._isotopes[i])
        //            return false;
        //    }
        //    return true;
        //}

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ChemicalFormula))
            {
                return false;
            }
            return this.Equals((ChemicalFormula)obj);
        }

        public virtual bool Equals(ChemicalFormula other)
        {
            if (Object.ReferenceEquals(this, other)) return true;
            if (this._largestIsotopeID != other._largestIsotopeID) return false;          

            for (int i = 0; i < this._largestIsotopeID; i++)
            {
                if (this._isotopes[i] != other._isotopes[i])
                    return false;
            }
            return true;
        }


        public void Remove(IChemicalFormula item)
        {
            Remove(item.ChemicalFormula);
        }

        public void Remove(ChemicalFormula formula)
        {
            if (formula == null) return;

            // Get the length of the formula to remove
            int id = formula._largestIsotopeID;

            // Resize this formula array to match the size of the incoming one
            if (id > _isotopes.Length)
            {
                Array.Resize(ref _isotopes, id + 1);
            }

            // Update each isotope
            for (int i = 0; i <= id; i++)
            {               
                _isotopes[i] -= formula._isotopes[i];
            }

            _isDirty = true;
        }

        public void Remove(Isotope isotope, int count)
        {
            Add(isotope, -count);
        }

        /// <summary>
        /// Return the number of given isotopes in this chemical fomrula
        /// </summary>
        /// <param name="isotope"></param>
        /// <returns></returns>
        public int Count(Isotope isotope)
        {          
            if (isotope == null || isotope.UniqueID > _isotopes.Length) 
                return 0;
            return _isotopes[isotope.UniqueID];
        }

        /// <summary>
        /// Count the number of isotopes from this element are
        /// present in this chemical formula
        /// </summary>
        /// <param name="element">The element to search for</param>
        /// <returns>The total number of all the element isotopes in this chemical formula</returns>
        public int Count(Element element)
        {
            if (element == null)
                return 0;
            int count = 0;
            foreach (Isotope isotope in element._isotopes.Values)
            {
                count += Count(isotope);
            }
            return count;
        }

        public int Count(string symbol)
        {

            Element element = Element.PeriodicTable[symbol];
            return Count(element);            
        }

        public int Count(string symbol, int atomicNumber)
        {
            Isotope isotope = Element.PeriodicTable[symbol][atomicNumber];
            return Count(isotope);
        }

        /// <summary>
        /// Completely removes a particular isotope from this chemical formula.
        /// </summary>
        /// <param name="isotope">The isotope to remove</param>
        /// <returns>True if the isotope was in the chemical formula and removed, false otherwise</returns>
        public bool Remove(Isotope isotope)
        {
            if (isotope == null || isotope.UniqueID > _isotopes.Length)
                return false;
         
            if (_isotopes[isotope.UniqueID] == 0)
            {
                return false;
            }
            _isotopes[isotope.UniqueID] = 0;
            return _isDirty = true;
        }

        public bool Remove(string symbol)
        {
            Element element = Element.PeriodicTable[symbol];
            return Remove(element);          
        }

        public bool Remove(Element element)
        {
            bool result = false;
            foreach (Isotope isotope in element._isotopes.Values)
            {
                result |= Remove(isotope);
            }
            return result;
        }

        public override string ToString()
        {
            if (_isDirty)
            {
                CleanUp();
            }
            return _chemicalFormulaSB.ToString();
        }

        /// <summary>
        /// Recalculate parameters of the chemical formula
        /// </summary>
        private void CleanUp()
        {
            _numberOfAtoms = 0;
            _uniqueIsotopes = 0;
            _mass = new Mass();
            if (_chemicalFormulaSB == null)
            {
                _chemicalFormulaSB = new StringBuilder(10);
            }
            else
            {
                _chemicalFormulaSB.Clear();
            }

            HashSet<Element> elements = new HashSet<Element>();

            for (int i = 0; i <= _largestIsotopeID; i++)
            {  
                int count = _isotopes[i];
                if (count == 0)
                    continue;

                Isotope isotope = Element.PeriodicTable[i];
                Element element = isotope.Element;
                elements.Add(element);

                _uniqueIsotopes++;

                _numberOfAtoms += count;

                _mass.Monoisotopic += count * isotope.AtomicMass;
             
                _mass.Average += count * element.AverageMass;

                _chemicalFormulaSB.Append(element.AtomicSymbol);

                if (!isotope.IsPrincipalIsotope)
                {
                    _chemicalFormulaSB.Append('{');
                    _chemicalFormulaSB.Append(isotope.MassNumber);
                    _chemicalFormulaSB.Append('}');
                }

                if (count != 1)
                {
                    _chemicalFormulaSB.Append(count);
                }
            }

            _uniqueElements = elements.Count;

            // Mark as clean
            _isDirty = false;
        }

        /// <summary>
        /// Parses a string representation of chemical formula and adds the elements
        /// to this chemical formula
        /// </summary>
        /// <param name="formula">the Chemical Formula to parse</param>
        private void ParseString(string formula)
        {
            if (string.IsNullOrEmpty(formula))         
                return;            

            if (!IsValidChemicalFormula(formula))
            {
                throw new FormatException("Input string for chemical formula was in an incorrect format");
            }

            Element element = null;
            foreach (Match match in _formulaRegex.Matches(formula))
            {
                string chemsym = match.Groups[1].Value;             // Group 1: Chemical Symbol
                if (Element.PeriodicTable.TryGetElement(chemsym, out element))
                {
                    Isotope isotope = match.Groups[2].Success ?     // Group 2 (optional): Isotope Mass Number
                        element[int.Parse(match.Groups[2].Value)] :
                        element.PrincipalIsotope;
                    int sign = match.Groups[3].Success ?            // Group 3 (optional): Negative Sign
                        -1 :
                        1;
                    int numofelem = match.Groups[4].Success ?       // Group 4 (optional): Number of Elements
                        int.Parse(match.Groups[4].Value) :
                        1;

                    Add(isotope, sign * numofelem);
                }
                else
                {
                    throw new ArgumentException(string.Format("Chemical Symbol {0} does not exist in the Periodic Table", chemsym));
                }
            }
        }
    }
}