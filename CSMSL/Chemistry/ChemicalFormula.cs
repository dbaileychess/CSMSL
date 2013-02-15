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
    /// <summary>
    /// A chemical / molecule consisting of multiple atoms.
    /// </summary>
    public class ChemicalFormula : IMass, IEquatable<ChemicalFormula>, IChemicalFormula
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

        /// <summary>
        /// A wrapper for the formula regex that validates if a string is in the correct chemical formula format or not
        /// </summary>
        private static readonly Regex _validateFormulaRegex = new Regex("^(" + _formulaRegex.ToString() + ")+$", RegexOptions.Compiled);
                         
        /// <summary>
        /// Indicates if the internal _isotope array has been modified, requiring necessary
        /// clean up code to be performed.
        /// </summary>
        private bool _isDirty;          

        /// <summary>
        /// Inidicates if the Hill Notation string representation needs to be recalculated
        /// </summary>
        private bool _isFormulaDirty;

        /// <summary>
        /// Main data store the isotopes. 
        /// <remarks>Acts as a dictionary where each isotope's UniqueID
        /// is the key (index) of this array. The array is front loaded to provide the most
        /// common elements first (C H N O P) to reduce memory footprint and provide quick
        /// addition/subtraction of formulas.</remarks>
        /// </summary>
        private int[] _isotopes;
        
        /// <summary>
        /// The index pointer to the largest isotope UniqueID currently contained in
        /// int[] _isotopes 
        /// </summary>
        private int _largestIsotopeID;

        private Mass _mass;
        private int _atomCount;
        private int _elementCount;
        private int _isotopeCount;
        private string _chemicalFormulaString;

        #region Constructors

        /// <summary>
        /// Create an empty chemical formula
        /// </summary>
        public ChemicalFormula()
        {
            _isotopes = new int[PeriodicTable.RecommendedID];
            _largestIsotopeID = 0;
            _isFormulaDirty = _isDirty = true;
        }

        /// <summary>
        /// Create an empty chemical formula with space for the largest ID
        /// </summary>
        private ChemicalFormula(int largestID)
        {
            _isotopes = new int[largestID + 1];
            _largestIsotopeID = 0;
            _isFormulaDirty = _isDirty = true;
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

        /// <summary>
        /// Create an chemical formula from an item that contains a chemical formula
        /// </summary>
        /// <param name="item">The item of which a new chemical formula will be made from</param>
        public ChemicalFormula(IChemicalFormula item)
            : this(item.ChemicalFormula) { }

        /// <summary>
        /// Create a copy of a chemical formula from another chemical formula
        /// </summary>
        /// <param name="other">The chemical formula to copy</param>
        public ChemicalFormula(ChemicalFormula other)
        {
            if (other == null)
            {                
                // create a new blank chemical formula
                _isotopes = new int[PeriodicTable.RecommendedID];
                _largestIsotopeID = 0;                 
            }
            else
            {
                int length = other._isotopes.Length;
                _isotopes = new int[length];
                _largestIsotopeID = other._largestIsotopeID;
                Array.Copy(other._isotopes, _isotopes, length);                      
            }
            _isDirty = _isFormulaDirty = true;           
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the mass of this chemical formula
        /// </summary>
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

        /// <summary>
        /// Gets the number of atoms in this chemical formula
        /// </summary>
        public int AtomCount
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _atomCount;
            }
        }
        
        /// <summary>
        /// Gets the number of unique chemical elements in this chemical formula
        /// </summary>
        public int ElementCount
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _elementCount;
            }
        }

        /// <summary>
        /// Gets the number of unique chemical isotopes in this chemical formula
        /// </summary>
        public int IsotopeCount
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _isotopeCount;
            }
        }


        /// <summary>
        /// Gets the string representation (Hill Notation) of this chemical formula
        /// </summary>
        public string Formula
        {
            get
            {
                if (_isFormulaDirty)
                    CleanUpFormula();
                return _chemicalFormulaString;
            }
        }

        #endregion

        #region Add/Remove

        /// <summary>
        /// Add a chemical formula containing object to this chemical formula
        /// </summary>
        /// <param name="item">The object that contains a chemical formula</param>
        public void Add(IChemicalFormula item)
        {
            if (item == null)
                return;
            Add(item.ChemicalFormula);
        }

        /// <summary>
        /// Add a chemical formula to this chemical formula.
        /// </summary>
        /// <param name="formula">The chemical formula to add to this</param>
        public void Add(ChemicalFormula formula)
        {
            if (formula == null)
                return;

            // Get the length of the formula to add
            int id = formula._largestIsotopeID;

            if (id > _largestIsotopeID)
            {
                _largestIsotopeID = id;
               
                if (id >= _isotopes.Length)
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

            // Force update of the largest isotope
            // if the largest isotope got cleared
            if (_isotopes[_largestIsotopeID] == 0)
            {
                FindLargestIsotope();
            }

            _isFormulaDirty = _isDirty = true;
        }

        /// <summary>
        /// Add the principal isotope of the element to this chemical formula
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <param name="count">The number of the element to add</param>
        public void Add(Element element, int count)
        {
            if (element == null)
                return;
            Add(element.PrincipalIsotope, count);
        }

        /// <summary>
        /// Add the principal isotope of the element to this chemical formula
        /// given its chemical symbol
        /// </summary>
        /// <param name="symbol">The chemical symbol of the element to add</param>
        /// <param name="count">The number of the element to add</param>
        public void Add(string symbol, int count)
        {
            try
            {
                Isotope isotope = Element.PeriodicTable[symbol].PrincipalIsotope;
                Add(isotope, count);
            }
            catch (KeyNotFoundException e)
            {
                throw new KeyNotFoundException(string.Format("The element symbol '{0}' is not found in the periodic table", symbol), e);
            }
        }
               
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
                if (id >= _isotopes.Length)
                {
                    // Isotope doesn't exist, resize array and set the count (faster than the += below)
                    Array.Resize(ref _isotopes, id + 1);
                    _isotopes[id] = count;
                    _isFormulaDirty = _isDirty = true;                  
                    return;
                }
            }
           
            _isotopes[id] += count;

            // Force update of the largest isotope
            // if the largest isotope got cleared
            if (_isotopes[_largestIsotopeID] == 0)
            {
                FindLargestIsotope();
            }

            _isFormulaDirty = _isDirty = true;
        }

        /// <summary>
        /// Remove a chemical formula containing object from this chemical formula
        /// </summary>
        /// <param name="item">The object that contains a chemical formula</param>
        public void Remove(IChemicalFormula item)
        {
            if (item == null)
                return;
            Remove(item.ChemicalFormula);
        }

        /// <summary>
        /// Remove a chemical formula from this chemical formula
        /// </summary>
        /// <param name="formula">The chemical formula to remove</param>
        public void Remove(ChemicalFormula formula)
        {
            if (formula == null) return;

            // Get the length of the formula to remove
            int id = formula._largestIsotopeID;

            // Resize this formula array to match the size of the incoming one
            if (id > _isotopes.Length)
            {
                _largestIsotopeID = id;
                if (id >= _isotopes.Length)
                {
                    Array.Resize(ref _isotopes, id + 1);
                }
            }

            // Update each isotope
            for (int i = 0; i <= id; i++)
            {
                _isotopes[i] -= formula._isotopes[i];
            }

            // Force update of the largest isotope
            // if the largest isotope got cleared
            if (_isotopes[_largestIsotopeID] == 0)
            {
                FindLargestIsotope();
            }

            _isFormulaDirty = _isDirty = true;
        }

        /// <summary>
        /// Remove the principal isotope of the element represented by the symbol
        /// from this chemical formula
        /// </summary>
        /// <param name="symbol">The symbol of the chemical element to remove</param>
        /// <param name="count">The number of isotopes to remove</param>
        public void Remove(string symbol, int count)
        {
            Add(Element.PeriodicTable[symbol].PrincipalIsotope, -count);
        }

        /// <summary>
        /// Remove a isotope from this chemical formula
        /// </summary>
        /// <param name="isotope">The isotope to remove</param>
        /// <param name="count">The number of isotopes to remove</param>
        public void Remove(Isotope isotope, int count)
        {
            Add(isotope, -count);
        }

        /// <summary>
        /// Completely removes a particular isotope from this chemical formula.
        /// </summary>
        /// <param name="isotope">The isotope to remove</param>
        /// <returns>True if the isotope was in the chemical formula and removed, false otherwise</returns>
        public bool Remove(Isotope isotope)
        {
            if (isotope == null)
                return false;

            int id = isotope.UniqueID;

            if (id > _largestIsotopeID)
            {
                // id not contained, just return false
                return false;
            }
            else if (id == _largestIsotopeID)
            {
                // id is the largest, set it to 0 and find the new largest
                _isotopes[id] = 0;
                FindLargestIsotope();
            }
            else if (_isotopes[id] == 0)
            {
                return false;               
            }
            else
            {
                _isotopes[id] = 0;
            }

            return _isFormulaDirty = _isDirty = true;
        }

        /// <summary>
        /// Remove all the isotopes of an chemical element represented by the symbol
        /// from this chemical formula
        /// </summary>
        /// <param name="symbol">The symbol of the chemical element to remove</param>
        /// <returns>True if the element was present and removed, false otherwise</returns>
        public bool Remove(string symbol)
        {
            Element element = Element.PeriodicTable[symbol];
            return Remove(element);
        }
        
        /// <summary>
        /// Remove all the isotopes of an chemical element from this
        /// chemical formula
        /// </summary>
        /// <param name="element">The chemical element to remove</param>
        /// <returns>True if the element was present and removed, false otherwise</returns>
        public bool Remove(Element element)
        {
            if (element == null)
                return false;
            bool result = false;
            foreach (Isotope isotope in element._isotopes.Values)
            {
                result |= Remove(isotope);
            }
            return result;
        }

        /// <summary>
        /// Remove all isotopes from this chemical formula to create an 'empty' chemical formula
        /// </summary>
        public void Clear()
        {
            Array.Clear(_isotopes, 0, _isotopes.Length);
            _largestIsotopeID = 0;
            _isFormulaDirty = _isDirty = true;  
        }

        #endregion 

        #region Count/Contains

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

        /// <summary>
        /// Return the number of given isotopes in this chemical fomrula
        /// </summary>
        /// <param name="isotope"></param>
        /// <returns></returns>
        public int Count(Isotope isotope)
        {
            if (isotope == null || isotope.UniqueID > _largestIsotopeID)
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

        public int GetNeutronCount()
        {
            int neutrons = 0;
            for (int i = 0; i < _largestIsotopeID; i++)
            {
                if (_isotopes[i] == 0)
                    continue;

                neutrons += Element.PeriodicTable[i].Neutrons * _isotopes[i];
            }
            return neutrons;
        }

        public int GetProtonCount()
        {
            int protons = 0;
            for (int i = 0; i < _largestIsotopeID; i++)
            {
                if (_isotopes[i] == 0)
                    continue;

                protons += Element.PeriodicTable[i].Protons * _isotopes[i];
            }
            return protons;
        }

        #endregion

        public override int GetHashCode()
        {
            int hCode = 7;
            for (int i = 0; i < _largestIsotopeID; i++)
            {
                hCode += _isotopes[i] << (i + 1);
            }
            return hCode;
        }

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
               
        public override string ToString()
        {
            return Formula;
        }

        #region Private Methods
                 
        private void FindLargestIsotope()
        {
            int index = _largestIsotopeID;
            while (index > 0)
            {
                if (_isotopes[index] != 0)
                    break;
                index--;
            }
            _largestIsotopeID = index;
        }

        /// <summary>
        /// Recalculate parameters of the chemical formula
        /// </summary>
        private void CleanUp()
        {
            _atomCount = 0;
            _isotopeCount = 0;
            _mass = new Mass();                     
           
            HashSet<int> elements = new HashSet<int>();         

            for (int i = 0; i <= _largestIsotopeID; i++)
            {  
                int count = _isotopes[i];
                if (count == 0)
                    continue;
       
                Isotope isotope = Element.PeriodicTable[i];
                Element element = isotope.Element;
                elements.Add(element.AtomicNumber);

                _isotopeCount++;

                _atomCount += count;

                _mass.Monoisotopic += count * isotope.AtomicMass;
             
                _mass.Average += count * element.AverageMass;                
            }

            _elementCount = elements.Count;
                   
            // Mark as clean
            _isDirty = false;
        }
                
        private void CleanUpFormula()
        {
            string carbonPart = "";
            string hydrogenPart = "";
            List<string> otherParts = new List<string>();
            StringBuilder sb = new StringBuilder(4);
            
            for (int i = 0; i <= _largestIsotopeID; i++)
            {
                int count = _isotopes[i];
                if (count == 0)
                    continue;

                Isotope isotope = Element.PeriodicTable[i];
                Element element = isotope.Element;              

                sb.Clear();
                sb.Append(isotope.AtomicSymbol);

                if (!isotope.IsPrincipalIsotope)
                {
                    sb.Append('{');
                    sb.Append(isotope.MassNumber);
                    sb.Append('}');
                }

                if (count != 1)
                {
                    sb.Append(count);
                }

                switch (isotope.AtomicSymbol)
                {
                    case "C":
                        carbonPart += sb.ToString();
                        break;
                    case "D":
                    case "H":
                        hydrogenPart += sb.ToString();
                        break;
                    default:
                        otherParts.Add(sb.ToString());
                        break;
                }
            }
          
            if (string.IsNullOrEmpty(carbonPart))
            {
                // No carbons, so just add the hydrogen to the list and sort alphabetically
                otherParts.Add(hydrogenPart);
                otherParts.Sort();
            }
            else
            {
                otherParts.Sort();
                otherParts.Insert(0, hydrogenPart);
                otherParts.Insert(0, carbonPart);
            }

            _chemicalFormulaString = string.Join("", otherParts);

            // Mark as clean
            _isFormulaDirty = false;
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

        #endregion

        #region Statics

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
            if (left == null)
            {
                if (right == null)
                    return null;
                return new ChemicalFormula(right);
            }

            ChemicalFormula newFormula = new ChemicalFormula(left);
            newFormula.Remove(right);
            return newFormula;
        }

        public static ChemicalFormula operator *(ChemicalFormula formula, int count)
        {
            if (count == 0)
                return new ChemicalFormula();

            if (formula == null)
                return null;

            int id = formula._largestIsotopeID;
            ChemicalFormula newFormula = new ChemicalFormula(formula);
            for (int i = 0; i < id; i++)
            {
                newFormula._isotopes[i] *= count;
            }
            
            newFormula._isDirty = true;
            newFormula._isFormulaDirty = true;
            return newFormula;
        }

        public static ChemicalFormula operator *(int count, ChemicalFormula formula)
        {
            return formula * count;
        }

        public static ChemicalFormula operator +(ChemicalFormula left, ChemicalFormula right)
        {
            if (left == null)
            {
                if (right == null)
                    return null;
                return new ChemicalFormula(right);
            }

            ChemicalFormula newFormula = new ChemicalFormula(left);
            newFormula.Add(right);
            return newFormula;
        }

        public static ChemicalFormula Combine(IEnumerable<IChemicalFormula> formulas)
        {
            int largestID = 0;
            int[] isotopes = new int[300];
            foreach (IChemicalFormula iformula in formulas)
            {             
                if (iformula == null)
                    continue;

                ChemicalFormula formula = iformula.ChemicalFormula;
                if (formula == null)
                    continue;

                int length = formula._largestIsotopeID;

                if (length > largestID)
                {
                    largestID = length;
                }

                int[] otherIsotopes = formula._isotopes;
                for (int i = 0; i <= length; i++)
                {
                    isotopes[i] += otherIsotopes[i];
                }
            }

            ChemicalFormula returnFormula = new ChemicalFormula(largestID);
            Array.Copy(isotopes, returnFormula._isotopes, largestID + 1);
           
            // Force update of the largest isotope
            // if the largest isotope got cleared
            if (returnFormula._isotopes[largestID] == 0)
            {
                returnFormula.FindLargestIsotope();
            }

            returnFormula._isDirty = true;
            returnFormula._isFormulaDirty = true;

            return returnFormula;
        }

        #endregion

        ChemicalFormula IChemicalFormula.ChemicalFormula
        {
            get { return this; }
        }
    }
}