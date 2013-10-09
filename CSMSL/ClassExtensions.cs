using System;
using System.Runtime.InteropServices.ComTypes;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using System.Collections.Generic;

namespace CSMSL
{
    public static class MassExtension
    {
        public static double ToMz(this IMass mass, int charge)
        {
            return Mass.MzFromMass(mass.MonoisotopicMass, charge);
        }

        public static bool MassEquals(this IMass mass1, IMass mass2)
        {
            return Math.Abs(mass1.MonoisotopicMass - mass2.MonoisotopicMass) < 0.00000000001;
        }
    }

    public static class ModificationSiteExtensions
    {
        public static ModificationSites Set(this ModificationSites sites, char aminoacid)
        {
            AminoAcid aa;
            if(AminoAcid.TryGetResidue(aminoacid, out aa))
            {
                sites |= aa.Site;
            }
            return sites;
        }
    }

    public static class ChemicalFormulaFilterExtension
    {
        [Flags]
        public enum FilterTypes
        {
            None = 0,
            Valence = 1,
            HydrogenCarbonRatio = 2,
            All = Int32.MaxValue,
        }

        public static IEnumerable<ChemicalFormula> GetValidatedMolecularFormulas(this IEnumerable<ChemicalFormula> formulas, FilterTypes filters)
        {
            bool useValence = filters.HasFlag(FilterTypes.Valence);
            bool useHydrogenCarbonRatio = filters.HasFlag(FilterTypes.HydrogenCarbonRatio);

            foreach (ChemicalFormula formula in formulas)
            {
                bool valid = false;
                int totalValence = 0;
                int maxValence = 0;
                int oddValences = 0;
                int atomCount = 0;
                int carbonCount = 0;
                int hydrogenCount = 0;
                int[] isotopes = formula.GetIsotopes();
                for (int i = 0; i < isotopes.Length; i++)
                {
                    int numAtoms = isotopes[i];
                    if (numAtoms > 0)
                    {
                        Isotope isotope = PeriodicTable.Instance[i];
                        if (useHydrogenCarbonRatio)
                        {
                            string symbol = isotope.AtomicSymbol;
                            if (symbol.Equals("H"))
                            {
                                hydrogenCount = numAtoms;
                            }
                            if (symbol.Equals("C"))
                            {
                                carbonCount = numAtoms;
                            }
                        }

                        if (useValence)
                        {
                            int numValenceElectrons = isotope.ValenceElectrons;
                            totalValence += numValenceElectrons * numAtoms;
                            atomCount += numAtoms;
                            if (numValenceElectrons > maxValence)
                            {
                                maxValence = numValenceElectrons;
                            }
                            if (numValenceElectrons % 2 != 0)
                            {
                                oddValences += numAtoms;
                            }
                        }
                    }
                }

                if (useValence)
                {
                    //Criteria
                    //i) The sum of valences or the total number of atoms having odd valences is even;
                    //ii) The sum of valences is greater than or equal to twice the
                    //maximum valence;
                    //iii) The sum of valences is greater than or equal to twice
                    //the number of atoms minus 1.
                    valid = ((totalValence % 2 == 0 || oddValences % 2 == 0) && (totalValence >= 2 * maxValence) && (totalValence >= ((2 * atomCount) - 1)));
                    if (!valid)
                    {
                        continue;
                    }
                }


                if (useHydrogenCarbonRatio)
                {
                    //Criteria
                    // H/C ratio should be in the following range 2.0 > H/C > 0.5
                    double HCRatio = (double)hydrogenCount / (double)carbonCount;
                    valid = (HCRatio > 0.5 && HCRatio < 2.0);
                }

                if (valid)
                {
                    yield return formula;
                }
            }
        }
    }
}


