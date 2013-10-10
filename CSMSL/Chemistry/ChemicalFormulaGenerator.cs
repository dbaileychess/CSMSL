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

        private static void GenerateFormulaHelper(double lowMass, double highMass, int length, double[] mass, int[] max, int index, int[] currentFormula, List<ChemicalFormula> formulas)
        {
            //int maxCount = Math.Min((int)Math.Ceiling(highMass / mass[index]), max[index]);

            //for (int count = 0; count <= maxCount; count++)
            //{
            //    currentFormula[index] = count;
            //    if (index < length - 1)
            //    {
            //        GenerateFormulaHelper(lowMass, highMass, length, mass, max, index + 1, currentFormula, formulas);
            //    }
            //    else
            //    {
            //        var formula = new ChemicalFormula(currentFormula);
            //        //if(formula.MonoisotopicMass < lowMass)
            //        //    continue;
            //        //if(formula.MonoisotopicMass > highMass)
            //        //    return;
            //        if (formula.MonoisotopicMass >= lowMass && formula.MonoisotopicMass <= highMass)
            //        {
            //            formulas.Add(formula);
            //        }
            //    }
            //}
            if (index < length - 1)
            {
                if (max[index] == 0)
                {
                    GenerateFormulaHelper(lowMass, highMass, length, mass, max, index + 1, currentFormula, formulas);
                }
                else
                {

                    int maxCount = Math.Min((int) Math.Ceiling(highMass/mass[index]), max[index]);
                    for (int count = 0; count <= maxCount; count++)
                    {
                        currentFormula[index] = count;
                        GenerateFormulaHelper(lowMass, highMass, length, mass, max, index + 1, currentFormula, formulas);
                    }
                }
            }
            else
            {
                currentFormula[index] = 0;
                double currentMass = new ChemicalFormula(currentFormula).MonoisotopicMass;
                int minCount = Math.Max((int)Math.Floor((lowMass - currentMass) / mass[index]), 0);
                int maxCount = Math.Min((int)Math.Ceiling((highMass - currentMass) / mass[index]), max[index]);
                for (int count = minCount; count <= maxCount; count++)
                {
                    currentFormula[index] = count;
                    var formula = new ChemicalFormula(currentFormula);
                    if (formula.MonoisotopicMass >= lowMass && formula.MonoisotopicMass <= highMass)
                    {
                        formulas.Add(formula);
                    }
                }
            }
        }

        public IEnumerable<ChemicalFormula> FromMass(double lowMass, double highMass, int maxNumberOfResults = int.MaxValue)
        {
            List<ChemicalFormula> returnFormulas = new List<ChemicalFormula>();

            // The minimum formula required for any return formulas
            int[] minValues = _minFormula.GetIsotopes();
            double minFormulaMass = _minFormula.MonoisotopicMass;

            double correctedLowMass = lowMass - minFormulaMass;
            double correctedHighMass = highMass - minFormulaMass;

            // Add the minimum formula itself if it is within the bounds
            if (minFormulaMass >= lowMass && minFormulaMass <= highMass)
            {
                returnFormulas.Add(_minFormula);
            }
           
            int totalCombos = 1;
            int count = 0;

            // The maximum formula allowed, represented in number of isotopes
            int[] maxValues = _maxFormula.GetIsotopes();

            // The current formula represented in isotopes
            int[] currentFormula = new int[maxValues.Length];

            // A list of all the isotopes masses
            double[] mass = new double[maxValues.Length];

            int length = maxValues.Length;

            for (int j = 0; j < length; j++)
            {
                if(j < minValues.Length)
                    maxValues[j] -= minValues[j];
                if (maxValues[j] == 0)
                    continue;
                mass[j] = PeriodicTable.Instance[j].AtomicMass;
            }

            GenerateFormulaHelper(lowMass, highMass, length, mass, maxValues, 0, new int[maxValues.Length], returnFormulas);
            
            if (_minFormula.ElementCount > 0)
            {
                foreach (ChemicalFormula formula in returnFormulas)
                {
                    formula.Add(_minFormula);
                }
            }

            double meanValue = (highMass + lowMass) / 2.0;

            return returnFormulas.OrderBy(formula => Math.Abs(formula.MonoisotopicMass - meanValue)).Take(maxNumberOfResults);
        }

    }
}
