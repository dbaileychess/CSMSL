// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ChemicalFormula.cs) is part of CSMSL.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// A chemical / molecule consisting of multiple atoms.
    /// <remarks>This class is mutable</remarks>
    /// </summary>
    public sealed class ChemicalFormula : IEquatable<ChemicalFormula>, IChemicalFormula
    {
        /// <summary>
        /// A regular expression for matching chemical formulas such as: C2C{13}3H5NO5
        /// \s* (at end as well) allows for optional spacing among the elements, i.e. C2 C{13}3 H5 N O5
        /// The first group is the only non-optional group and that handles the chemical symbol: H, He, etc..
        /// The second group is optional, which handles alternative isotopes of elements: C{13} means carbon-13, while C is the common carbon-12
        /// The third group is optional and indicates if we are adding or subtracting the elements form the formula, C-2C{13}5 would mean first subtract 2 carbon-12 and then add 5 carbon-13
        /// The fourth group is optional and represents the number of isotopes to add, if not present it assumes 1: H2O means 2 Hydrogen and 1 Oxygen
        /// Modified from: http://stackoverflow.com/questions/4116786/parsing-a-chemical-formula-from-a-string-in-c
        /// </summary>
        private static readonly Regex FormulaRegex = new Regex(@"\s*([A-Z][a-z]*)(?:\{([0-9]+)\})?(-)?([0-9]+)?\s*", RegexOptions.Compiled);

        /// <summary>
        /// A wrapper for the formula regex that validates if a string is in the correct chemical formula format or not
        /// </summary>
        private static readonly Regex ValidateFormulaRegex = new Regex("^(" + FormulaRegex + ")+$", RegexOptions.Compiled);

        /// <summary>
        /// Determines if the chemical formula hill notation string is stored or calculated each time it is called.
        /// True means the notation is stored as a string in the chemical formula (quicker, but more memory)
        /// False means the notation is not stored as a string in the chemical formula (slower, no used memory)
        /// Changing this value will not automatically delete the interned notations of formulas.
        /// The default value is true.
        /// </summary>
        public static bool InternChemicalFormulaStrings = true;

        /// <summary>
        /// The default empty chemicalFormula
        /// </summary>
        public static readonly ChemicalFormula Empty = new ChemicalFormula();

        /// <summary>
        /// Indicates if the internal _isotope array has been modified, requiring necessary
        /// clean up code to be performed.
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Indicates if the Hill Notation string representation needs to be recalculated
        /// </summary>
        private bool _isFormulaDirty;

        /// <summary>
        /// Main data store, the isotopes.
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
        private int _largestIsotopeId;

        /// <summary>
        /// The average mass of the chemical formula
        /// </summary>
        private double _averageMass;

        /// <summary>
        /// The number of atoms in this chemical formula. Atoms represent individual isotopes
        /// </summary>
        private int _atomCount;

        /// <summary>
        /// The number of unique elements in this chemical formula.
        /// </summary>
        private int _elementCount;

        /// <summary>
        /// The number of unique isotopes in this chemical formula.
        /// </summary>
        private int _isotopeCount;

        /// <summary>
        /// The Hill Notation string for this chemical formula
        /// </summary>
        private string _chemicalFormulaString;

        #region Constructors

        /// <summary>
        /// Create an empty chemical formula
        /// </summary>
        public ChemicalFormula()
        {
            ChemicalFormulaConstructor();
        }

        /// <summary>
        /// Create an empty chemical formula with space for the largest ID
        /// </summary>
        private ChemicalFormula(int largestId)
        {
            ChemicalFormulaConstructor(largestId + 1);
        }

        internal ChemicalFormula(int[] uniqueIdCounts)
        {
            int count = uniqueIdCounts.Length;
            _isotopes = new int[count];
            MonoisotopicMass = 0;
            for (int i = 0; i < count; i++)
            {
                int isotopes = uniqueIdCounts[i];
                if (isotopes != 0)
                {
                    _isotopes[i] = isotopes;
                    MonoisotopicMass += isotopes*PeriodicTable.GetIsotope(i).AtomicMass;
                    _largestIsotopeId = i;
                }
            }
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
            : this(item.ChemicalFormula)
        {
        }

        /// <summary>
        /// Create a copy of a chemical formula from another chemical formula
        /// </summary>
        /// <param name="other">The chemical formula to copy</param>
        public ChemicalFormula(ChemicalFormula other)
        {
            if (other == null)
            {
                // create a new blank chemical formula
                ChemicalFormulaConstructor();

                return;
            }

            int length = other._isotopes.Length;
            ChemicalFormulaConstructor(length, other._largestIsotopeId);

            Array.Copy(other._isotopes, _isotopes, length);

            MonoisotopicMass = other.MonoisotopicMass;
        }

        /// <summary>
        /// Helper method for constructing new chemical formulas
        /// </summary>
        /// <param name="largestId"></param>
        /// <param name="larestIsotope"></param>
        private void ChemicalFormulaConstructor(int largestId = PeriodicTable.RecommendedId, int larestIsotope = 0)
        {
            _isotopes = new int[largestId];
            _largestIsotopeId = larestIsotope;
            MonoisotopicMass = 0;
            _isFormulaDirty = _isDirty = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the average mass of this chemical formula
        /// </summary>
        public double AverageMass
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _averageMass;
            }
        }

        /// <summary>
        /// Gets the monoisotopic mass of this chemical formula
        /// </summary>
        public double MonoisotopicMass { get; private set; }

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
                // Not storing, so always call the method
                if (!InternChemicalFormulaStrings)
                    return GetHillNotation();

                if (_isFormulaDirty)
                {
                    _chemicalFormulaString = GetHillNotation();

                    // Mark formula as clean
                    _isFormulaDirty = false;
                }
                return _chemicalFormulaString;
            }
        }

        #endregion Properties

        #region Add/Remove

        /// <summary>
        /// Replaces one isotope with another.
        /// Replacement happens on a 1 to 1 basis, i.e., if you remove 5 you will add 5
        /// </summary>
        /// <param name="isotopeToRemove">The isotope to remove</param>
        /// <param name="isotopToAdd">The isotope to add</param>
        public void Replace(Isotope isotopeToRemove, Isotope isotopToAdd)
        {
            int numberRemoved = Remove(isotopeToRemove);
            Add(isotopToAdd, numberRemoved);
        }

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
            int id = formula._largestIsotopeId;

            if (id > _largestIsotopeId)
            {
                _largestIsotopeId = id;

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
            if (_isotopes[_largestIsotopeId] == 0)
            {
                FindLargestIsotope();
            }

            MonoisotopicMass += formula.MonoisotopicMass;

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
                Isotope isotope = PeriodicTable.GetElement(symbol).PrincipalIsotope;
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

            MonoisotopicMass += isotope.AtomicMass*count;

            _isFormulaDirty = _isDirty = true;

            int id = isotope.UniqueId;

            if (id > _largestIsotopeId)
            {
                // Isotope doesn't exist, set the count (faster than the += below)
                _largestIsotopeId = id;

                if (id >= _isotopes.Length)
                {
                    // resize array if it is too small
                    Array.Resize(ref _isotopes, id + 1);
                }

                _isotopes[id] = count;

                return;
            }

            _isotopes[id] += count;

            // Force update of the largest isotope
            // if the largest isotope got cleared
            if (_isotopes[_largestIsotopeId] == 0)
            {
                FindLargestIsotope();
            }
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

            MonoisotopicMass -= formula.MonoisotopicMass;

            // Get the length of the formula to remove
            int id = formula._largestIsotopeId;

            // Resize this formula array to match the size of the incoming one
            if (id > _isotopes.Length)
            {
                _largestIsotopeId = id;
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
            if (_isotopes[_largestIsotopeId] == 0)
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
            Add(PeriodicTable.GetElement(symbol).PrincipalIsotope, -count);
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
        public int Remove(Isotope isotope)
        {
            if (isotope == null)
                return 0;

            int id = isotope.UniqueId;
            int count;

            if (id > _largestIsotopeId || (count = _isotopes[id]) == 0)
            {
                // isotope not contained or is already zero, do nothing and just return false
                return 0;
            }

            MonoisotopicMass -= isotope.AtomicMass*count;

            _isotopes[id] = 0;

            if (id == _largestIsotopeId)
            {
                // id is the largest, find the new largest
                FindLargestIsotope();
            }
            _isFormulaDirty = _isDirty = true;

            return count;
        }

        /// <summary>
        /// Remove all the isotopes of an chemical element represented by the symbol
        /// from this chemical formula
        /// </summary>
        /// <param name="symbol">The symbol of the chemical element to remove</param>
        /// <returns>True if the element was present and removed, false otherwise</returns>
        public int Remove(string symbol)
        {
            return Remove(PeriodicTable.GetElement(symbol));
        }

        /// <summary>
        /// Remove all the isotopes of an chemical element from this
        /// chemical formula
        /// </summary>
        /// <param name="element">The chemical element to remove</param>
        /// <returns>True if the element was present and removed, false otherwise</returns>
        public int Remove(Element element)
        {
            if (element == null)
                return 0;

            return element.Isotopes.Values.Sum(isotope => Remove(isotope));
        }

        /// <summary>
        /// Remove all isotopes from this chemical formula to create an 'empty' chemical formula
        /// </summary>
        public void Clear()
        {
            Array.Clear(_isotopes, 0, _isotopes.Length);
            _largestIsotopeId = 0;
            MonoisotopicMass = 0;
            _isFormulaDirty = _isDirty = true;
        }

        #endregion Add/Remove

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

        /// <summary>
        /// Checks if any isotope of the specified element is present in this chemical formula
        /// </summary>
        /// <param name="element">The element to look for</param>
        /// <returns>True if there is a non-zero number of the element in this formula</returns>
        public bool Contains(Element element)
        {
            return Count(element) != 0;
        }

        public bool Contains(string symbol)
        {
            return Count(symbol) != 0;
        }

        public bool Contains(ChemicalFormula formula)
        {
            return IsSuperSetOf(formula);
        }

        public bool IsSubSetOf(ChemicalFormula formula)
        {
            return formula != null && formula.IsSuperSetOf(this);
        }

        /// <summary>
        /// Checks whether this formula contains all the isotopes of the specified formula
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public bool IsSuperSetOf(ChemicalFormula formula)
        {
            if (formula == null)
                return false;

            int[] otherFormula = formula._isotopes;
            int[] thisFormula = _isotopes;

            int max = Math.Min(thisFormula.Length, otherFormula.Length);

            for (int i = 0; i < max; i++)
            {
                if (thisFormula[i] < otherFormula[i])
                    return false;
            }

            return true;
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
            if (isotope == null || isotope.UniqueId > _largestIsotopeId)
                return 0;
            return _isotopes[isotope.UniqueId];
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
            return element.Isotopes.Values.Sum(isotope => Count(isotope));
        }

        public int Count(string symbol)
        {
            Element element = PeriodicTable.GetElement(symbol);
            return Count(element);
        }

        public int Count(string symbol, int atomicNumber)
        {
            Isotope isotope = PeriodicTable.GetElement(symbol)[atomicNumber];
            return Count(isotope);
        }

        /// <summary>
        /// Gets the total number of neutrons in this chemical formula
        /// </summary>
        /// <returns></returns>
        public int GetNeutronCount()
        {
            int neutrons = 0;
            for (int i = 0; i <= _largestIsotopeId; i++)
            {
                if (_isotopes[i] == 0)
                    continue;

                neutrons += PeriodicTable.GetIsotope(i).Neutrons*_isotopes[i];
            }
            return neutrons;
        }

        /// <summary>
        /// Gets the total number of protons in this chemical formula
        /// </summary>
        /// <returns></returns>
        public int GetProtonCount()
        {
            int protons = 0;
            for (int i = 0; i <= _largestIsotopeId; i++)
            {
                if (_isotopes[i] == 0)
                    continue;

                protons += PeriodicTable.GetIsotope(i).Protons*_isotopes[i];
            }
            return protons;
        }

        /// <summary>
        /// Gets the ratio of the number of Carbon to Hydrogen in this chemical formula
        /// </summary>
        /// <returns></returns>
        public double GetCarbonHydrogenRatio()
        {
            int carbonCount = Count("C");

            if (carbonCount == 0)
                return 0;

            int hydrogenCount = Count("H");

            return hydrogenCount/(double) carbonCount;
        }

        #endregion Count/Contains

        public override int GetHashCode()
        {
            if (_isDirty)
                CleanUp();

            if (_isotopes == null)
                return 0;

            int hCode = 17;

            for (int i = 0; i <= _largestIsotopeId; i++)
            {
                unchecked
                {
                    hCode = hCode*23 + _isotopes[i];
                }
            }
            return hCode;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChemicalFormula);
        }

        public bool Equals(ChemicalFormula other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (_largestIsotopeId != other._largestIsotopeId) return false;

            for (int i = 0; i <= _largestIsotopeId; i++)
            {
                if (_isotopes[i] != other._isotopes[i])
                    return false;
            }
            return true;
        }

        public double[,] GetIsotopicDistribution(double resolution = 0.01, int numberOfIsotopes = int.MaxValue)
        {
            IsotopicDistribution id = new IsotopicDistribution(resolution);
            var spectrum = id.CalculateDistribuition(this, numberOfIsotopes, IsotopicDistribution.Normalization.BasePeak);
            return spectrum.ToArray();
        }

        /// <summary>
        /// Gets the unique elements in this chemical formula
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Element> GetElements()
        {
            HashSet<Element> elements = new HashSet<Element>();
            for (int i = 0; i <= _largestIsotopeId; i++)
            {
                if (_isotopes[i] != 0)
                {
                    elements.Add(PeriodicTable.GetIsotope(i).Element);
                }
            }
            return elements;
        }

        public override string ToString()
        {
            return Formula;
        }

        /// <summary>
        /// Returns the chemical formula with each element separated by the
        /// specified delimiter
        /// </summary>
        /// <param name="delimiter">The delimiter to separate elements by</param>
        /// <returns></returns>
        public string ToString(string delimiter)
        {
            return GetHillNotation(delimiter);
        }

        #region Private Methods

        private void FindLargestIsotope()
        {
            int index = _largestIsotopeId;
            while (index > 0)
            {
                if (_isotopes[index] != 0)
                    break;
                index--;
            }
            _largestIsotopeId = index;
        }

        /// <summary>
        /// Recalculate parameters of the chemical formula
        /// </summary>
        private void CleanUp()
        {
            int atomCount = 0;
            int isotopeCount = 0;
            double monoMass = 0.0;
            double avgMass = 0.0;

            HashSet<int> elements = new HashSet<int>();

            // Loop over every possible isotope in this formula
            for (int i = 0; i <= _largestIsotopeId; i++)
            {
                int count = _isotopes[i];

                // Skip zero isotopes
                if (count == 0)
                    continue;

                Isotope isotope = PeriodicTable.GetIsotope(i);
                Element element = isotope.Element;
                elements.Add(element.AtomicNumber);

                isotopeCount++;
                atomCount += count;

                monoMass += count*isotope.AtomicMass;
                avgMass += count*element.AverageMass;
            }

            // Set the instance variables to their new values
            _elementCount = elements.Count;
            MonoisotopicMass = monoMass;
            _averageMass = avgMass;
            _isotopeCount = isotopeCount;
            _atomCount = atomCount;

            // Mark as clean
            _isDirty = false;
        }

        /// <summary>
        /// Produces the Hill Notation of the chemical formula
        /// </summary>
        public string GetHillNotation(string delimiter = "")
        {
            string carbonPart = string.Empty;
            string hydrogenPart = string.Empty;
            List<string> otherParts = new List<string>();
            StringBuilder sb = new StringBuilder(4);

            bool nonNullDelimiter = !string.IsNullOrEmpty(delimiter);

            for (int i = 0; i <= _largestIsotopeId; i++)
            {
                int count = _isotopes[i];
                if (count == 0)
                    continue;

                Isotope isotope = PeriodicTable.GetIsotope(i);

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
                        if (nonNullDelimiter && !string.IsNullOrEmpty(carbonPart))
                            carbonPart += delimiter;
                        carbonPart += sb.ToString();
                        break;

                    case "D":
                    case "H":
                        if (nonNullDelimiter && !string.IsNullOrEmpty(hydrogenPart))
                            hydrogenPart += delimiter;
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
                if (!string.IsNullOrEmpty(hydrogenPart))
                    otherParts.Add(hydrogenPart);
                otherParts.Sort();
            }
            else
            {
                otherParts.Sort();

                if (!string.IsNullOrEmpty(hydrogenPart))
                    otherParts.Insert(0, hydrogenPart);

                otherParts.Insert(0, carbonPart);
            }

            return string.Join(delimiter, otherParts);
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

            foreach (Match match in FormulaRegex.Matches(formula))
            {
                string chemsym = match.Groups[1].Value; // Group 1: Chemical Symbol

                Element element;
                if (PeriodicTable.TryGetElement(chemsym, out element))
                {
                    Isotope isotope = element.PrincipalIsotope; // Start with the most abundant (principal) isotope

                    if (chemsym.Equals("D")) // Special case for Deuterium
                    {
                        isotope = element.Isotopes[2];
                    }
                    else if (match.Groups[2].Success) // Group 2 (optional): Isotope Mass Number
                    {
                        isotope = element[int.Parse(match.Groups[2].Value)];
                    }

                    int sign = match.Groups[3].Success ? // Group 3 (optional): Negative Sign
                        -1 :
                        1;

                    int numofelem = match.Groups[4].Success ? // Group 4 (optional): Number of Elements
                        int.Parse(match.Groups[4].Value) :
                        1;

                    Add(isotope, sign*numofelem);
                }
                else
                {
                    throw new ArgumentException(string.Format("The chemical Symbol '{0}' does not exist in the Periodic Table", chemsym));
                }
            }
        }

        #endregion Private Methods

        #region Internal

        /// <summary>
        /// Get the internal isotope array for this chemical formula as a deep copy.
        /// </summary>
        /// <returns>The isotopes that make up this chemical formula</returns>
        internal int[] GetIsotopes()
        {
            int[] isotopes = new int[_largestIsotopeId + 1];
            Array.Copy(_isotopes, isotopes, _largestIsotopeId + 1);
            return isotopes;
        }

        #endregion Internal

        #region Statics

        public static implicit operator ChemicalFormula(string sequence)
        {
            return new ChemicalFormula(sequence);
        }

        public static implicit operator String(ChemicalFormula sequence)
        {
            return sequence.ToString();
        }

        public static bool IsValidChemicalFormula(string chemicalFormula)
        {
            return ValidateFormulaRegex.IsMatch(chemicalFormula);
        }

        public static ChemicalFormula operator -(ChemicalFormula left, IChemicalFormula right)
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

            int id = formula._largestIsotopeId;
            ChemicalFormula newFormula = new ChemicalFormula(formula);
            for (int i = 0; i <= id; i++)
            {
                newFormula._isotopes[i] *= count;
            }
            newFormula.MonoisotopicMass = formula.MonoisotopicMass*count;
            newFormula._isDirty = true;
            newFormula._isFormulaDirty = true;
            return newFormula;
        }

        public static ChemicalFormula operator *(int count, ChemicalFormula formula)
        {
            return formula*count;
        }

        public static ChemicalFormula operator +(ChemicalFormula left, IChemicalFormula right)
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
            int largestId = 0;
            int[] isotopes = new int[PeriodicTable.BiggestIsotopeNumber];
            double mass = 0;
            foreach (IChemicalFormula iformula in formulas)
            {
                if (iformula == null)
                    continue;

                ChemicalFormula formula = iformula.ChemicalFormula;
                if (formula == null)
                    continue;

                mass += formula.MonoisotopicMass;

                int length = formula._largestIsotopeId;

                if (length > largestId)
                {
                    largestId = length;
                }

                int[] otherIsotopes = formula._isotopes;
                for (int i = 0; i <= length; i++)
                {
                    isotopes[i] += otherIsotopes[i];
                }
            }

            ChemicalFormula returnFormula = new ChemicalFormula(largestId);
            Array.Copy(isotopes, returnFormula._isotopes, largestId + 1);

            // Force update of the largest isotope
            // if the largest isotope got cleared
            if (returnFormula._isotopes[largestId] == 0)
            {
                returnFormula.FindLargestIsotope();
            }

            returnFormula.MonoisotopicMass = mass;
            returnFormula._isDirty = true;
            returnFormula._isFormulaDirty = true;

            return returnFormula;
        }

        #endregion Statics

        #region IChemicalFormula

        ChemicalFormula IChemicalFormula.ChemicalFormula
        {
            get { return this; }
        }

        #endregion IChemicalFormula
    }
}