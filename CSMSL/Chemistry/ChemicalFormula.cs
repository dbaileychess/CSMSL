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
        private static readonly Regex _formulaRegex = new Regex(@"([A-Z][a-z]*)(?:\{([0-9]+)\})?(-)?([0-9]+)?");

        private static readonly Regex _validateFormulaRegex = new Regex("^(" + _formulaRegex.ToString() + ")+$");

        private StringBuilder _chemicalFormulaSb;

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
            _chemicalFormulaSb = new StringBuilder(9); // Based off amino acid chemical formulas
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
                _isotopes = new Dictionary<Isotope, int>();
                _chemicalFormulaSb = new StringBuilder(9); // Based off amino acid chemical formulas
                _isDirty = true;
            }
            else
            {
                _isotopes = new Dictionary<Isotope, int>(chemicalFormula._isotopes);
                if (!(_isDirty = chemicalFormula._isDirty))
                {
                    _chemicalFormulaSb = new StringBuilder(chemicalFormula._chemicalFormulaSb.ToString());
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
            foreach (KeyValuePair<Isotope, int> kvp in formula._isotopes)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public void Add(Isotope isotope, int count)
        {
            if (count != 0)
            {
                int curVal = 0;
                if (_isotopes.TryGetValue(isotope, out curVal))
                {
                    _isotopes[isotope] = curVal + count; // Much quicker that isotopes[isotope] += count
                }
                else
                {
                    _isotopes.Add(isotope, count);
                }
                _isDirty = true;
            }
        }

        public bool ContainsIsotope(Isotope isotope)
        {
            return _isotopes.ContainsKey(isotope);
        }

        public bool Equals(ChemicalFormula other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return (_isotopes.GetHashCode() == other._isotopes.GetHashCode());
        }

        public override string ToString()
        {
            if (_isDirty)
            {
                CleanUp();
            }
            return _chemicalFormulaSb.ToString();
        }

        private void CleanUp()
        {
            _numberOfAtoms = 0;
            _mass = new Mass();
            _chemicalFormulaSb.Clear();
            foreach (KeyValuePair<Isotope, int> kvp in _isotopes)
            {
                int count = kvp.Value;
                Isotope isotope = kvp.Key;
                Element element = isotope.Element;

                _numberOfAtoms += count;

                _mass._mono += count * isotope.Mass;

                _mass._avg += count * element.AverageMass;

                _chemicalFormulaSb.Append(element.AtomicSymbol);

                if (!isotope.IsPrincipalIsotope)
                {
                    _chemicalFormulaSb.Append('{');
                    _chemicalFormulaSb.Append(isotope.MassNumber);
                    _chemicalFormulaSb.Append('}');
                }

                // Can handle negative values of elements even if it doesn't make physical sense
                if (count < 0)
                {
                    _chemicalFormulaSb.Append("-");
                }

                // Append the number of elements if larger than 1
                if (count > 1)
                {
                    _chemicalFormulaSb.Append(count);
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