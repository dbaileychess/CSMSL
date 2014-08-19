// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ChemicalFormulaGenerator.cs) is part of CSMSL.
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

namespace CSMSL.Chemistry
{
    /// <summary>
    /// A chemical formula generator for constructing all possible chemical formulas within a mass range given
    /// restraints on the total number of isotopes.
    /// </summary>
    public class ChemicalFormulaGenerator
    {
        /// <summary>
        /// The maximum formula that can be generated.
        /// </summary>
        public ChemicalFormula MaximumFormula { get; private set; }

        /// <summary>
        /// The minimum formula that every generated formula has to contain
        /// </summary>
        public ChemicalFormula MinimumFormula { get; private set; }

        /// <summary>
        /// Create a empty generator, which will produce no formulas unless a constraint is added
        /// </summary>
        public ChemicalFormulaGenerator()
            : this(new ChemicalFormula(), new ChemicalFormula())
        {
        }

        /// <summary>
        /// Creates a generator with a maximum chemical formula allowed
        /// </summary>
        /// <param name="maximumChemicalFormula">The maximum chemical formula to generate</param>
        public ChemicalFormulaGenerator(ChemicalFormula maximumChemicalFormula)
            : this(new ChemicalFormula(), maximumChemicalFormula)
        {
        }

        /// <summary>
        /// Creates a generator with a maximum chemical formula allowed
        /// </summary>
        /// <param name="minimumChemicalFormula"></param>
        /// <param name="maximumChemicalFormula">The maximum chemical formula to generate</param>
        public ChemicalFormulaGenerator(ChemicalFormula minimumChemicalFormula, ChemicalFormula maximumChemicalFormula)
        {
            MinimumFormula = new ChemicalFormula(minimumChemicalFormula);
            MaximumFormula = new ChemicalFormula(maximumChemicalFormula);
        }

        public void AddConstraint(ChemicalFormula minimumChemicalFormula, ChemicalFormula maximumChemicalFormula)
        {
            MinimumFormula.Add(minimumChemicalFormula);
            MaximumFormula.Add(maximumChemicalFormula);
        }

        public void AddConstraint(Isotope isotope, int min, int max)
        {
            MinimumFormula.Add(isotope, min);
            MaximumFormula.Add(isotope, max);
        }

        public void AddConstraint(Isotope isotope, Range<int> range)
        {
            AddConstraint(isotope, range.Minimum, range.Maximum);
        }

        public void ClearConstraints()
        {
            MinimumFormula = new ChemicalFormula();
            MaximumFormula = new ChemicalFormula();
        }

        public void RemoveConstraint(Isotope isotope)
        {
            MinimumFormula.Remove(isotope);
            MaximumFormula.Remove(isotope);
        }

        /// <summary>
        /// Generate all formulas regardless of mass between the min and max formula
        /// </summary>
        /// <returns>A list of chemical formulas</returns>
        public IEnumerable<ChemicalFormula> AllFormulas()
        {
            return FromMass(0.0, int.MaxValue - 1);
        }

        public IEnumerable<ChemicalFormula> FromMass(IRange<double> massRange, int maxNumberOfResults = int.MaxValue, bool sort = true)
        {
            return FromMass(massRange.Minimum, massRange.Maximum, maxNumberOfResults, sort);
        }

        public IEnumerable<ChemicalFormula> FromMass(double lowMass, double highMass, int maxNumberOfResults = int.MaxValue, bool sort = true)
        {
            if (highMass <= lowMass)
            {
                throw new ArgumentException("The high mass must be greater than the low mass");
            }

            if (!MaximumFormula.Contains(MinimumFormula))
            {
                throw new ArgumentException("The maximum formula must include the minimum formula");
            }

            List<ChemicalFormula> returnFormulas = new List<ChemicalFormula>();

            // The minimum formula required for any return formulas

            double correctedLowMass = lowMass;
            double correctedHighMass = highMass;

            bool minFormulaExists = MinimumFormula.IsotopeCount != 0;
            int[] minValues = null;
            double minFormulaMass = 0;

            if (minFormulaExists)
            {
                minValues = MinimumFormula.GetIsotopes();
                minFormulaMass = MinimumFormula.MonoisotopicMass;

                correctedLowMass -= minFormulaMass;
                correctedHighMass -= minFormulaMass;

                // Add the minimum formula itself if it is within the bounds
            }

            // The maximum formula allowed, represented in number of isotopes
            int[] maxValues = MaximumFormula.GetIsotopes();

            // The current formula represented in isotopes
            int[] currentFormula = new int[maxValues.Length];

            // A list of all the isotopes masses
            double[] masses = new double[maxValues.Length];

            int length = maxValues.Length;

            for (int j = 0; j < length; j++)
            {
                if (minFormulaExists && j < minValues.Length)
                    maxValues[j] -= minValues[j];
                if (maxValues[j] == 0)
                    continue;
                masses[j] = PeriodicTable.GetIsotope(j).AtomicMass;
            }
            masses[0] = PeriodicTable.GetIsotope(0).AtomicMass;

            GenerateFormulaHelper(correctedLowMass, correctedHighMass, masses, maxValues, length - 1, currentFormula, returnFormulas);

            if (minFormulaExists)
            {
                foreach (ChemicalFormula formula in returnFormulas)
                {
                    formula.Add(MinimumFormula);
                }
                if (minFormulaMass >= lowMass && minFormulaMass <= highMass)
                {
                    returnFormulas.Add(new ChemicalFormula(MinimumFormula));
                }
            }

            if (!sort)
                return returnFormulas;
            double meanValue = (highMass + lowMass)/2.0;
            return returnFormulas.OrderBy(formula => Math.Abs(formula.MonoisotopicMass - meanValue)).Take(maxNumberOfResults);
        }

        private static void GenerateFormulaHelper(double lowMass, double highMass, double[] masses, int[] max, int index, int[] currentFormula, List<ChemicalFormula> formulas)
        {
            while (index > 0 && max[index] == 0)
            {
                index--;
            }
            if (index > 0)
            {
                int maxCount = Math.Min((int) Math.Ceiling(highMass/masses[index]), max[index]);
                for (int count = 0; count <= maxCount; count++)
                {
                    currentFormula[index] = count;
                    GenerateFormulaHelper(lowMass, highMass, masses, max, index - 1, currentFormula, formulas);
                }
            }
            else
            {
                double massAtIndex = masses[index];
                currentFormula[index] = 0;
                double currentMass = currentFormula.Zip(masses, (i, m) => i*m).Sum();
                int minCount = Math.Max((int) Math.Floor((lowMass - currentMass)/massAtIndex), 0);
                int value = (int) Math.Ceiling((highMass - currentMass)/massAtIndex);
                int maxCount = Math.Min(value, max[index]);
                for (int count = minCount; count <= maxCount; count++)
                {
                    currentMass += count*massAtIndex;
                    currentFormula[index] = count;

                    if (currentMass >= lowMass && currentMass <= highMass)
                    {
                        ChemicalFormula formula = new ChemicalFormula(currentFormula);
                        if (!formula.MassEquals(0.0))
                            formulas.Add(formula);
                    }
                    currentMass -= count*massAtIndex;
                }
            }
        }
    }
}