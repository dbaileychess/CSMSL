using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSMSL.Chemistry
{
    public class ChemicalFormulaGenerator
    {

        private int[] minValues;
        private int[] maxValues;
        private int maxLength;
        private int maxUniqueID = 0;

        public ChemicalFormulaGenerator(ChemicalFormula baseChemicalFormula)
        {
            this.maxValues = baseChemicalFormula.GetIsotopes();
            this.minValues = new int[this.maxValues.Length];
            this.maxLength = maxValues.Length;
        }

        public void AddConstraint(ChemicalFormula minimumChemicalFormula, ChemicalFormula maximumChemicalFormula)
        {
            int[] newMins = minimumChemicalFormula.GetIsotopes();
            int[] newMaxs = maximumChemicalFormula.GetIsotopes();
            if (minValues.Length < newMins.Length)
            {
                Array.Resize(ref minValues, maxUniqueID);
                Array.Resize(ref maxValues, maxUniqueID);
                maxUniqueID = newMins.Length - 1;
            }
            for (int i = 0; i < newMins.Length; i++)
            {
                minValues[i] = newMins[i];
                maxValues[i] = newMaxs[i];
            }
        }

        public void AddConstraint(Isotope isotope, int min, int max)
        {
            if (maxUniqueID < isotope.UniqueId)
            {
                maxUniqueID = isotope.UniqueId;
                Array.Resize(ref minValues, maxUniqueID);
                Array.Resize(ref maxValues, maxUniqueID);
            }
        }

        public void AddConstraint(Isotope isotope, Range<int> range) 
        {
            AddConstraint(isotope, range.Minimum, range.Maximum);
        }

        public void RemoveConstraint(Isotope isotope)
        {
            if (minValues.Length > isotope.UniqueId)
            {
                minValues[isotope.UniqueId] = 0;
                maxValues[isotope.UniqueId] = 0;
            }
        }

        public IEnumerable<ChemicalFormula> FromMass(int maxNumberOfResults = int.MaxValue)
        {
            return FromMass(0, double.MaxValue, maxNumberOfResults);
        }

        public IEnumerable<ChemicalFormula> FromMass(MassRange range, int maxNumberOfResults = int.MaxValue)
        {
            return FromMass(range.Minimum, range.Maximum, maxNumberOfResults);
        }

        public IEnumerable<ChemicalFormula> FromMass(double lowMass, double highMass, int maxNumberOfResults = int.MaxValue)
        {
            List<ChemicalFormula> returnFormulas = new List<ChemicalFormula>();
            int maxValue = minValues.Length;
            while (!minValues.SequenceEqual(maxValues))
            {
                for (int i = 1; i <= maxValue; i++)
                {
                    minValues[maxValue - i]++;
                    if (minValues[maxValue - i] > maxValues[maxValue - i])
                    {
                        minValues[maxValue - i] = 0;
                    }
                    else
                    {
                        ChemicalFormula formula = new ChemicalFormula(minValues);
                        if (formula.MonoisotopicMass < highMass && formula.MonoisotopicMass > lowMass)
                        {
                            returnFormulas.Add(formula);
                        }
                        break;
                    }
                }
            }
            Dictionary<string, int> valences = new Dictionary<string, int>();
            valences.Add("H", 1);
            valences.Add("C", 4);
            valences.Add("F", 7);
            valences.Add("O", 6);
            valences.Add("N", 5);

            //i) The sum of valences or the total number of atoms having odd valences is even;
            //ii) The sum of valences is greater than or equal to twice the
            //maximum valence;
            //iii) The sum of valences is greater than or equal to twice
            //the number of atoms minus 1.
            // H/C ratio should be in the following range 2.0 > H/C > 0.5

            List<ChemicalFormula> validFormulas = new List<ChemicalFormula>();
            foreach (ChemicalFormula form in returnFormulas)
            {
                int totalValence = 0;
                int maxValence = 0;
                int oddValences = 0;
                int atomCount = 0;
                int carbonCount = 0;
                int hydrogenCount = 0;
                int[] isotopes = form.GetIsotopes();
                for (int i = 0; i < isotopes.Length; i++)
                {
                    if (isotopes[i] > 0)
                    {
                        string symbol = PeriodicTable.Instance[i].AtomicSymbol;
                        totalValence += valences[symbol] * isotopes[i];
                        atomCount += isotopes[i];
                        if (symbol.Equals("H"))
                        {
                            hydrogenCount = isotopes[i];
                        }
                        if (symbol.Equals("C"))
                        {
                            carbonCount = isotopes[i];
                        }
                        if (valences[symbol] > maxValence)
                        {
                            maxValence = valences[symbol];
                        }
                        if (valences[symbol] % 2 != 0)
                        {
                            oddValences += isotopes[i];
                        }
                    }
                }
                if (totalValence % 2 == 0 || oddValences % 2 == 0)
                {
                    if (totalValence >= 2 * maxValence)
                    {
                        if (totalValence >= ((2 * atomCount) - 1))
                        {
                            double HCRatio = (double)hydrogenCount / (double)carbonCount;
                            if (HCRatio > 0.5 && HCRatio < 2.0)
                            {
                                validFormulas.Add(form);
                            }
                        }
                    }
                }
            }
            return validFormulas; 
        }
    }
}
