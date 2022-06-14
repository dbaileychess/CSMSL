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

using System;
using System.Collections.Generic;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// An object that has a chemical formula
    /// </summary>
    public interface IChemicalFormula : IMass
    {
        /// <summary>
        /// The chemical formula of this object
        /// </summary>
        ChemicalFormula ChemicalFormula { get; }
    }

    public static class ChemicalFormulaExtensions
    {
        [Flags]
        public enum FilterTypes
        {
            None = 0,
            Valence = 1,
            HydrogenCarbonRatio = 2,
            All = 3,
        }

        public static IEnumerable<IChemicalFormula> Validate(this IEnumerable<IChemicalFormula> formulas, FilterTypes filters = FilterTypes.All)
        {
            bool useValence = filters.HasFlag(FilterTypes.Valence);
            bool useHydrogenCarbonRatio = filters.HasFlag(FilterTypes.HydrogenCarbonRatio);

            foreach (IChemicalFormula formula in formulas)
            {
                if (useHydrogenCarbonRatio)
                {
                    double ratio = formula.ChemicalFormula.GetCarbonHydrogenRatio();

                    if (ratio < 0.5 || ratio > 2.0)
                        continue;
                }

                if (useValence)
                {
                    int totalValence = 0;
                    int maxValence = 0;
                    int oddValences = 0;
                    int atomCount = 0;
                    int[] isotopes = formula.ChemicalFormula.GetIsotopes();
                    for (int i = 0; i < isotopes.Length; i++)
                    {
                        int numAtoms = isotopes[i];
                        if (numAtoms != 0)
                            continue;
                        Isotope isotope = PeriodicTable.GetIsotope(i);

                        int numValenceElectrons = isotope.ValenceElectrons;
                        totalValence += numValenceElectrons*numAtoms;
                        atomCount += numAtoms;
                        if (numValenceElectrons > maxValence)
                        {
                            maxValence = numValenceElectrons;
                        }
                        if (numValenceElectrons%2 != 0)
                        {
                            oddValences += numAtoms;
                        }
                    }
                    if (!((totalValence%2 == 0 || oddValences%2 == 0) && (totalValence >= 2*maxValence) && (totalValence >= ((2*atomCount) - 1))))
                    {
                        continue;
                    }
                }

                yield return formula;
            }
        }
    }
}