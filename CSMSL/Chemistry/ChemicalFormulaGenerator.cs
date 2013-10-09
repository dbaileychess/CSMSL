using System;
using System.Collections.Generic;
using System.Linq;
using Combinatorics.Collections;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// A chemical formula generator for constructing all possible chemical formulas within a mass range given
    /// restraints on the total number of isotopes.
    /// </summary>
    public class ChemicalFormulaGenerator
    {
        private ChemicalFormula _minFormula;
        private ChemicalFormula _maxFormula;

        /// <summary>
        /// Create a empty generator, which will produce no formulas unless a constraint is added
        /// </summary>
        public ChemicalFormulaGenerator()
        {
            _minFormula = new ChemicalFormula();
            _maxFormula = new ChemicalFormula();
        }

        /// <summary>
        /// Creates a generator with a maximum chemical formula allowed
        /// </summary>
        /// <param name="maximumChemicalFormula">The maximum chemical formula to generate</param>
        public ChemicalFormulaGenerator(ChemicalFormula maximumChemicalFormula)
        {
            _minFormula = new ChemicalFormula();
            _maxFormula = new ChemicalFormula(maximumChemicalFormula);
        }
        
        public void AddConstraint(ChemicalFormula minimumChemicalFormula, ChemicalFormula maximumChemicalFormula)
        {
            _minFormula.Add(minimumChemicalFormula);
            _maxFormula.Add(maximumChemicalFormula);
        }

        public void AddConstraint(Isotope isotope, int min, int max)
        {
            _minFormula.Add(isotope, min);
            _maxFormula.Add(isotope, max);
        }

        public void AddConstraint(Isotope isotope, Range<int> range) 
        {
            AddConstraint(isotope, range.Minimum, range.Maximum);
        }

        public void Clear()
        {
            _minFormula = new ChemicalFormula();
            _maxFormula = new ChemicalFormula();
        }

        public void RemoveConstraint(Isotope isotope)
        {
            _minFormula.Remove(isotope);
            _maxFormula.Remove(isotope);
        }

        public IEnumerable<ChemicalFormula> AllFormulas()
        {
            return FromMass(0, double.MaxValue);
        }

        public IEnumerable<ChemicalFormula> FromMass(IRange<double> range, int maxNumberOfResults = int.MaxValue)
        {
            return FromMass(range.Minimum, range.Maximum, maxNumberOfResults);
        }

        public IEnumerable<ChemicalFormula> FromMass(double lowMass, double highMass, int maxNumberOfResults = int.MaxValue)
        {
            List<ChemicalFormula> returnFormulas = new List<ChemicalFormula>();

            int[] minValues = _minFormula.GetIsotopes();
            double minFormulaMass = _minFormula.MonoisotopicMass;

            if (minFormulaMass >= lowMass && minFormulaMass <= highMass)
            {
                returnFormulas.Add(_minFormula);
            }

            int[] formulas = _maxFormula.GetIsotopes();
            int totalCombos = 1;
            int count = 0;
            for (int i = 0; i < minValues.Length; i++)
            {
                formulas[i] -= minValues[i];
                if (formulas[i] != 0)
                {
                    totalCombos *= (formulas[i] + 1);
                    count = i;
                }
            }
           
            int[] currentFormula = new int[formulas.Length];

            int combos = 0;
            totalCombos--;
            while (combos < totalCombos)
            {
                for (int i = 0; i <= count; i++)
                {
                    currentFormula[count - i]++;
                    if (currentFormula[count - i] > formulas[count - i])
                    {
                        currentFormula[count - i] = 0;
                    }
                    else
                    {
                        combos++;
                        ChemicalFormula formula = new ChemicalFormula(currentFormula);
                        double totalMass = formula.MonoisotopicMass + minFormulaMass;

                        if (totalMass >= lowMass && totalMass <= highMass)
                        {
                            formula.Add(_minFormula);
                            returnFormulas.Add(formula);
                        }
                        break;
                    }
                }
            }

            double meanValue = (highMass + lowMass) / 2.0;

            return returnFormulas.OrderBy(formula => Math.Abs(formula.MonoisotopicMass - meanValue)).Take(maxNumberOfResults);
        }
    }
}
