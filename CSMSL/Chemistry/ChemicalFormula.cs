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
using System.Text;
using System.Text.RegularExpressions;

namespace CSMSL.Chemistry
{
    public class ChemicalFormula : IMass, IEquatable<ChemicalFormula>
    {
        public static readonly PeriodicTable PERIODIC_TABLE = PeriodicTable.Instance;

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

        public ChemicalFormula()
        {
            // create a new blank chemical formula
            _isotopes = new int[_uniqueIDCount];
            _isDirty = true;
        }

        public ChemicalFormula(string chemicalFormula)
            : this()
        {
            ParseString(chemicalFormula);
        }

        public ChemicalFormula(IChemicalFormula item)
            : this(item.ChemicalFormula) { }

        public ChemicalFormula(ChemicalFormula chemicalFormula)
        {
            if (chemicalFormula == null)
            {
                // create a new blank chemical formula
                _isotopes = new int[_uniqueIDCount];
                _isDirty = true;
            }
            else
            {
                // Copy an existing chemical formula
                _isotopes = new int[chemicalFormula._isotopes.Length];
                Array.Copy(chemicalFormula._isotopes, _isotopes, chemicalFormula._isotopes.Length);
                if (!(_isDirty = chemicalFormula._isDirty))
                {
                    // old chemical formula is already clean, don't need to reclean
                    _chemicalFormulaSB = new StringBuilder(chemicalFormula._chemicalFormulaSB.ToString());
                    _numberOfAtoms = chemicalFormula._numberOfAtoms;
                    _mass = new Mass(chemicalFormula._mass);
                }
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

        public static double[,] GetIsotopicDistribution(IChemicalFormula item)
        {
            return GetIsotopicDistribution(item.ChemicalFormula);
        }

        public static double[,] GetIsotopicDistribution(ChemicalFormula baseFormula)
        {
            double[,] data = new double[10, 2];
            //double value = 1;
            //foreach (KeyValuePair<Isotope, int> kvp in baseFormula._isotopes)
            //{
            //    value *= kvp.Key.RelativeAbundance * kvp.Value;
            //}
            return data;
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
            ChemicalFormula newFormula = formula.Clone();
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
            int length = formula._isotopes.Length;

            // Resize this formula array to match the size of the incoming one
            if (length > _isotopes.Length)
            {
                Array.Resize(ref _isotopes, length);
            }

            // Update each isotope
            for (int i = 0; i < length; i++)
            {
                if (formula._isotopes[i] != 0)
                    _isotopes[i] += formula._isotopes[i];
            }

            _isDirty = true;
        }

        public void Add(Isotope isotope, int count)
        {
            if (count == 0)
            {
                return;
            }
           
            int id = isotope.UniqueID;
            if (id > _isotopes.Length)
            {
                // Isotope doesn't exist, resize array and set the count
                Array.Resize(ref _isotopes, id + 1);
                _isotopes[id] = count;
            }
            else
            {
                _isotopes[id] += count;
            }
            _isDirty = true;
        }

        public void Clear()
        {
            Array.Clear(_isotopes, 0, _isotopes.Length);
            _isDirty = true;
        }

        public ChemicalFormula Clone()
        {
            return new ChemicalFormula(this);
        }

        public bool ContainsIsotope(Isotope isotope)
        {
            if (isotope.UniqueID > _isotopes.Length) return false;
            return _isotopes[isotope.UniqueID] != 0;
        }

        /// <summary>
        /// Test for equality between two chemical formulas. Two formulas are equivalent if they have the exact same number and type of isotopes.
        /// </summary>
        /// <param name="other">The other chemical formula to compare with</param>
        /// <returns>True if the chemical formulas are the same, false otherwise</returns>
        public bool Equals(ChemicalFormula other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            for (int i = 0; i < _isotopes.Length; i++)
            {
                if ((i > other._isotopes.Length && _isotopes[i] != 0) || _isotopes[i] != other._isotopes[i])
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

            // Get the length of the formula to add
            int length = formula._isotopes.Length;

            // Resize this formula array to match the size of the incoming one
            if (length > _isotopes.Length)
            {
                Array.Resize(ref _isotopes, length);
            }

            // Update each isotope
            for (int i = 0; i < length; i++)
            {
                if (formula._isotopes[i] != 0)
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
            int id = isotope.UniqueID;
            if (id > _isotopes.Length) return 0;
            return _isotopes[id];
        }

        /// <summary>
        /// Completely removes a particular isotope from this chemical formula.
        /// </summary>
        /// <param name="isotope">The isotope to remove</param>
        /// <returns>True if the isotope was in the chemical formula and removed, false otherwise</returns>
        public bool Remove(Isotope isotope)
        {
            int id = isotope.UniqueID;
            if (id > _isotopes.Length) return false;
            if (_isotopes[id] == 0)
            {
                return false;
            }
            _isotopes[id] = 0;
            return _isDirty = true;
        }

        public bool Remove(string symbol)
        {
            Element element;
            if (PERIODIC_TABLE.TryGetElement(symbol, out element))
            {
                return Remove(element);
            }
            else
            {
                return false;
            }
        }

        public bool Remove(Element element)
        {
            bool result = false;
            foreach (Isotope isotope in element)
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

            for (int i = 0; i < _isotopes.Length; i++)
            {
                if (_isotopes[i] == 0) continue;
                int count = _isotopes[i];

                Isotope isotope = PERIODIC_TABLE[i];
                Element element = isotope.Element;

                _uniqueIsotopes++;

                _numberOfAtoms += count;

                _mass._mono += count * isotope.Mass;

                _mass._avg += count * element.AverageMass;

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
            {
                return;
            }

            if (!IsValidChemicalFormula(formula))
            {
                throw new FormatException("Input string for chemical formula was in an incorrect format");
            }

            Element element = null;
            foreach (Match match in _formulaRegex.Matches(formula))
            {
                string chemsym = match.Groups[1].Value;             // Group 1: Chemical Symbol
                if (PERIODIC_TABLE.TryGetElement(chemsym, out element))
                {
                    Isotope isotope = match.Groups[2].Success ?     // Group 2 (optional): Isotope Mass Number
                        element[int.Parse(match.Groups[2].Value)] :
                        element.Principal;
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