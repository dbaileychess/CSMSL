using System;
using System.Collections.Generic;
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

        private static readonly Regex _validateFormulaRegex = new Regex("^(" + _formulaRegex.ToString() + ")+$", RegexOptions.Compiled);

        private StringBuilder _chemicalFormulaSB;

        private bool _isDirty;

        /// <summary>
        /// Internal dictionary of each isotope in the chemical formula. The key (string)
        /// represents the isotope name, and the value (int) represents the number of that isotope
        /// within this chemical formula.
        /// </summary>
        private Dictionary<Isotope, int> _isotopes;

        private Mass _mass;

        private int _numberOfAtoms;

        public ChemicalFormula()
        {
            _isotopes = new Dictionary<Isotope, int>();
            _chemicalFormulaSB = new StringBuilder(9); // Based off amino acid chemical formulas
            _isDirty = true;
        }

        public ChemicalFormula(string chemicalFormula)
            : this()
        {
            ParseString(chemicalFormula);
        }

        public ChemicalFormula(IChemicalFormula item)
            : this(item.ChemicalFormula)
        {
        }

        public ChemicalFormula(ChemicalFormula chemicalFormula)
        {
            if (chemicalFormula == null)
            {
                // create a new blank chemical formula
                _isotopes = new Dictionary<Isotope, int>();
                _chemicalFormulaSB = new StringBuilder(9); // Based off amino acid chemical formulas
                _isDirty = true;
            }
            else
            {
                // Copy an existing chemical formula
                _isotopes = new Dictionary<Isotope, int>(chemicalFormula._isotopes);
                if (!(_isDirty = chemicalFormula._isDirty))
                {
                    // old chemical formula is already clean, don't need to reclean
                    _chemicalFormulaSB = new StringBuilder(chemicalFormula._chemicalFormulaSB.ToString());
                    _numberOfAtoms = chemicalFormula._numberOfAtoms;
                    _mass = new Mass(chemicalFormula._mass);
                }
                else
                {
                    _chemicalFormulaSB = new StringBuilder(9); // Based off amino acid chemical formulas
                }
            }
        }

        public ChemicalFormula Clone()
        {
            return new ChemicalFormula(this);
        }

        public void Clear()
        {
            _isotopes.Clear();
            _isDirty = true;
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

        /// <summary>
        /// Number of unique elements in this chemical formula
        /// </summary>
        public int UniqueIsotopes
        {
            get
            {
                return _isotopes.Count;
            }
        }

        public static bool IsValidChemicalFormula(string chemicalFormula)
        {
            return _validateFormulaRegex.IsMatch(chemicalFormula);
        }

        public void Add(IChemicalFormula item)
        {
            Add(item.ChemicalFormula);
        }

        public void Add(ChemicalFormula formula)
        {
            if (formula == null) return;
            foreach (KeyValuePair<Isotope, int> kvp in formula._isotopes)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public void Add(Isotope isotope, int count)
        {
            if (count != 0)
            {
                int curValue = 0;
                if (_isotopes.TryGetValue(isotope, out curValue))
                {
                    int newValue = curValue + count;
                    if (newValue == 0)
                    {
                        _isotopes.Remove(isotope);
                    }
                    else
                    {
                        _isotopes[isotope] = newValue;
                    }
                }
                else
                {
                    _isotopes.Add(isotope, count);
                }
                _isDirty = true;
            }
        }

        public void Remove(IChemicalFormula item)
        {
            Remove(item.ChemicalFormula);
        }

        public void Remove(ChemicalFormula formula)
        {
            foreach (KeyValuePair<Isotope, int> kvp in formula._isotopes)
            {
                Remove(kvp.Key, kvp.Value);
            }
        }

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
            if (_isotopes.Remove(isotope))
            {
                return _isDirty = true;
            }
            return false;
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

        public bool ContainsIsotope(Isotope isotope)
        {
            return _isotopes.ContainsKey(isotope);
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
            if (_isotopes.Count != other._isotopes.Count) return false;
            int count = 0;
            foreach (KeyValuePair<Isotope, int> kvp in _isotopes)
            {
                if (!other._isotopes.TryGetValue(kvp.Key, out count)) return false;
                if (kvp.Value != count) return false;
            }
            return true;
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
            _mass = new Mass();
            _chemicalFormulaSB.Clear();
            foreach (KeyValuePair<Isotope, int> kvp in _isotopes)
            {
                int count = kvp.Value;
                Isotope isotope = kvp.Key;
                Element element = isotope.Element;

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

                // Can handle negative values of elements even if it doesn't make physical sense
                if (count < 0)
                {
                    _chemicalFormulaSB.Append("-");
                }

                // Append the number of elements if larger than 1
                if (count > 1)
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

        public static implicit operator ChemicalFormula(string sequence)
        {
            return new ChemicalFormula(sequence);
        }

        public static double[,] GetIsotopicDistribution(IChemicalFormula item)
        {
            return GetIsotopicDistribution(item.ChemicalFormula);
        }

        public static double[,] GetIsotopicDistribution(ChemicalFormula baseFormula)
        {
            double[,] data = new double[10, 2];
            double totalSum = 0;
            double value = 1;
            foreach (KeyValuePair<Isotope, int> kvp in baseFormula._isotopes)
            {
                value *= kvp.Key.RelativeAbundance * kvp.Value;
            }
            return data;
        }
    }
}