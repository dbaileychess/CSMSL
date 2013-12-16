using System;
using System.Linq;
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

    public static class UtilExtension
    {
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
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
            All = int.MaxValue,
        }

        public static IEnumerable<ChemicalFormula> Validate(this IEnumerable<ChemicalFormula> formulas, FilterTypes filters = FilterTypes.All)
        {
            bool useValence = filters.HasFlag(FilterTypes.Valence);
            bool useHydrogenCarbonRatio = filters.HasFlag(FilterTypes.HydrogenCarbonRatio);

            foreach (ChemicalFormula formula in formulas)
            {
                if (useHydrogenCarbonRatio)
                {
                    double ratio = formula.GetCarbonHydrogenRatio();
               
                    if (ratio < 0.5 || ratio > 2.0)
                        continue;
                }

                if (useValence)
                {
                    int totalValence = 0;
                    int maxValence = 0;
                    int oddValences = 0;
                    int atomCount = 0;
                    int[] isotopes = formula.GetIsotopes();
                    for (int i = 0; i < isotopes.Length; i++)
                    {
                        int numAtoms = isotopes[i];
                        if (numAtoms != 0) 
                            continue;
                        Isotope isotope = PeriodicTable.Instance[i];
                          
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
                    if (!((totalValence % 2 == 0 || oddValences % 2 == 0) && (totalValence >= 2 * maxValence) && (totalValence >= ((2 * atomCount) - 1))))
                    {
                        continue;
                    }
                }
                
                yield return formula;
            }
        }
    }
}


