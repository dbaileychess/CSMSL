using System;
using System.Collections.Generic;
using System.Linq;
using CSMSL.Spectral;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// Calculates the isotopic distributions of molecules
    /// </summary>
    /// <remarks>
    /// C# version by Derek Bailey 2014
    /// 
    /// This is a port of software written in C++ and detailed in the following publication:
    /// 
    /// Molecular Isotopic Distribution Analysis (MIDAs) with Adjustable Mass Accuracy.
    /// Gelio Alves, Aleksy Y. Ogurtsov, and Yi-Kuo Yu
    /// J. Am. Soc. Mass Spectrom. (2014) 25:57-70
    /// DOI: 10.1007/s13361-013-0733-7
    /// 
    /// Please cite that publication if using these algorithms in your own publications.
    /// </remarks>
    public class IsotopicDistribution
    {
        public enum Normalization { Sum, BasePeak };

        readonly double _mwResolution;
        double _mergeFineResolution;
        double _fineResolution;
        readonly double _fineMinProb;
        
        public IsotopicDistribution(double fineResolution = 0.01, double minProbability = 1e-200, double molecularWeightResolution = 1e-12)
        {
            _fineResolution = fineResolution;
            _fineMinProb = minProbability;
            _mwResolution = molecularWeightResolution;
        }

        public Spectrum CalculateDistribuition(string chemicalFormula, Normalization normalization = Normalization.Sum)
        {
            return CalculateDistribuition(new ChemicalFormula(chemicalFormula), normalization);
        }

        public Spectrum CalculateDistribuition(IChemicalFormula obj, Normalization normalization = Normalization.Sum)
        {
            return CalculateDistribuition(obj.ChemicalFormula, normalization);
        }

        public Spectrum CalculateDistribuition(ChemicalFormula formula, Normalization normalization = Normalization.Sum)
        {
            double monoisotopicMass = formula.MonoisotopicMass;
            SetResolution(monoisotopicMass);

            List<List<Composition>> elementalComposition = new List<List<Composition>>();

            // Get all the unique elements of the formula
            foreach (Element element in formula.GetElements())
            {
                int count = formula.Count(element);
                List<Composition> isotopeComposition = new List<Composition>();
                foreach (Isotope isotope in element.Isotopes.Values.OrderBy(iso => iso.AtomicMass))
                {
                    double probability = isotope.RelativeAbundance;
                    if (probability <= 0) 
                        continue;

                    Composition c = new Composition {
                        Atoms = count, 
                        MolecularWeight = isotope.AtomicMass, 
                        Power = isotope.AtomicMass, 
                        Probability = isotope.RelativeAbundance};

                    isotopeComposition.Add(c);
                }
                elementalComposition.Add(isotopeComposition);
            }

            foreach (List<Composition> compositions in elementalComposition)
            {
                double sumProb = compositions.Sum(t => t.Probability);
                foreach (Composition composition in compositions)
                {
                    composition.Probability /= sumProb;
                    composition.LogProbability = Math.Log(composition.Probability);
                    composition.Power = Math.Floor(composition.MolecularWeight / _mwResolution + 0.5);
                }
            }

            return CalculateFineGrain(elementalComposition, normalization);
        }

        private void SetResolution(double monoisotopicMass)
        {
            double fineResolution = _fineResolution;

            if (fineResolution >= 1.0)
            {
                _mergeFineResolution = 1.0 - 0.022;
                fineResolution = 0.9;
            }
            else if (fineResolution <= 1e-4 && monoisotopicMass < 1e5)
            {
                fineResolution = 1e-4;
                _mergeFineResolution = fineResolution;
            }
            else if (fineResolution <= 1e-3 && monoisotopicMass < 1e6)
            {
                fineResolution = 1e-3;
                _mergeFineResolution = fineResolution;
            }
            else if (fineResolution <= 1e-2 && monoisotopicMass < 2e6)
            {
                fineResolution = 1e-2;
                _mergeFineResolution = fineResolution;
            }
            else
            {
                _mergeFineResolution = fineResolution;
                fineResolution = 1e-2;
            }

            _fineResolution = fineResolution / 2.0;
        }
        
        private Spectrum CalculateFineGrain(List<List<Composition>> elementalComposition, Normalization normalization)
        {
            List<Polynomial> fPolynomial = MultiplyFinePolynomial(elementalComposition);
            fPolynomial = MergeFinePolynomial(fPolynomial);

            // Convert polynomial to spectrum
            int count = fPolynomial.Count;
            double[] mz = new double[count];
            double[] intensities = new double[count];
            double totalProbability = 0;
            double basePeak = 0;
            int i = 0;
            foreach (Polynomial polynomial in fPolynomial)
            {
                totalProbability += polynomial.Probablity;
                if (polynomial.Probablity > basePeak)
                {
                    basePeak = polynomial.Probablity;
                }
                mz[i] = polynomial.Power * _mwResolution;
                intensities[i] = polynomial.Probablity;
                i++;
            }

            double normalizedValue = normalization == Normalization.Sum ? totalProbability : basePeak;

            // Normalize
            for (i = 0; i < count; i++)
            {
                intensities[i] /= normalizedValue;
            }
           
            return new Spectrum(mz, intensities, false);
        }

        private List<Polynomial> MergeFinePolynomial(List<Polynomial> tPolynomial)
        {
            // Sort by mass (i.e. power)
            tPolynomial.Sort((a, b) => a.Power.CompareTo(b.Power));

            int count = tPolynomial.Count;

            for (int k = 1; k <= 9; k++)
            {
                for (int i = 0; i < count; i++)
                {
                    double power = tPolynomial[i].Power;

                    if (power == 0)
                        continue;

                    double probability = tPolynomial[i].Probablity;
                    Polynomial tempPolynomial;
                    tempPolynomial.Power = power * probability;
                    tempPolynomial.Probablity = probability;

                    for (int j = i + 1; j < count; j++)
                    {
                        double value = Math.Abs(tPolynomial[i].Power * _mwResolution - tPolynomial[j].Power * _mwResolution);

                        double threshold = (k <= 8) ? k * _mergeFineResolution / 8 : _mergeFineResolution + _mergeFineResolution / 100;

                        // Combine terms if their mass difference (i.e. power difference) is less than some threshold
                        if (value <= threshold) 
                        {
                            tempPolynomial.Power = tempPolynomial.Power + tPolynomial[j].Power * tPolynomial[j].Probablity;
                            tempPolynomial.Probablity = tempPolynomial.Probablity + tPolynomial[j].Probablity;
                            tPolynomial[i] = new Polynomial { Power = tempPolynomial.Power / tempPolynomial.Probablity, Probablity = tempPolynomial.Probablity };
                            tPolynomial[j] = new Polynomial();
                        }
                        else
                        {
                            break;
                        }
                    }
                   
                    tPolynomial[i] = new Polynomial { Power = tempPolynomial.Power / tempPolynomial.Probablity, Probablity = tempPolynomial.Probablity };
                }
            }
        
            // return only non-zero terms
            return tPolynomial.Where(poly => poly.Power != 0).ToList();
        }

        private List<Polynomial> MultiplyFinePolynomial(List<List<Composition>> elementalComposition)
        {
            const int nc = 10;
            const int ncAddValue = 1;
            const int nAtoms = 200;
            double maxPolynomialSize = Math.Log(1e13);
            List<Polynomial> tPolynomial = new List<Polynomial>();
            
            int maxIsotope = 0;
            int n = 0;
            int k = 0;

            foreach (List<Composition> composition in elementalComposition)
            {
                if (composition.Count > 0)
                    n++;
                if (composition.Count > 10)
                    maxIsotope = 1;
            }

            List<List<Polynomial>> fPolynomial = new List<List<Polynomial>>();
            for (int i = 0; i < n; i++)
            {
                fPolynomial.Add(new List<Polynomial>());
            }

            if (maxIsotope == 0)
            {
                for (k = 0; k < n; k++)
                {
                    tPolynomial.Clear();

                    List<Composition> composition = elementalComposition[k];
                    int size = composition.Count;
                    int atoms = composition[0].Atoms;

                    int ncAdd = atoms < nAtoms ? 10 : ncAddValue;

                    if (size == 1)
                    {
                        double probability = composition[0].Probability;

                        int n1 = (int)(atoms * probability);

                        double prob = FactorLn(atoms) - FactorLn(n1) + n1 * composition[0].LogProbability;
                        prob = Math.Exp(prob);

                        fPolynomial[k].Add(new Polynomial { Power = n1 * composition[0].Power, Probablity = prob });
                    } 
                    else 
                    {
                        int[] means = new int[size];
                        int[] stds = new int[size];
                        int[] indices = new int[size];

                        double nPolynomialTerms = Math.Log(Math.Pow(2, size));
                        for (int i = 0; i < size; i++)
                        {
                            int n1 = (int)(elementalComposition[k][0].Atoms*elementalComposition[k][i].Probability);
                            int s1 = (int)Math.Ceiling(ncAdd + nc* Math.Sqrt(elementalComposition[k][i].Atoms * elementalComposition[k][i].Probability*(1.0-elementalComposition[k][i].Probability)));
                            nPolynomialTerms += Math.Log(n1 + s1);
                            
                            means[i] = n1;
                            stds[i] = s1;
                            indices[i] = n1 + s1;
                        }

                        if (nPolynomialTerms > maxPolynomialSize)
                        {
                            throw new NotImplementedException();
                        }

                        int[] mins = new int[means.Length - 1];
                        int[] maxs = new int[means.Length - 1];
                        indices = new int[means.Length - 1];
                        for (int i = 0; i < means.Length - 1; i++)
                        {
                            indices[i] = mins[i] = Math.Max(0, means[i] - stds[i]);
                            maxs[i] = means[i] + stds[i];
                        }

                        MultipleFinePolynomialRecursiveHelper(mins, maxs, indices, 0, fPolynomial[k], composition, atoms, _fineMinProb, means[means.Length - 1] + stds[stds.Length - 1]);
                    }
                }
            }

            tPolynomial = fPolynomial[0];

            if (k <= 1)
                return tPolynomial;

            List<Polynomial> fgidPolynomial = new List<Polynomial>();
            for (k = 1; k < n; k++)
            {
                MultiplyFineFinalPolynomial(tPolynomial, fPolynomial[k], fgidPolynomial);
            }

            return tPolynomial;
        }

        private void MultiplyFineFinalPolynomial(List<Polynomial> tPolynomial, List<Polynomial> fPolynomial, List<Polynomial> fgidPolynomial)
        {
            int i = tPolynomial.Count;
            int j = fPolynomial.Count;

            double deltaMass = (_fineResolution / _mwResolution);
            double minProbability = _fineMinProb;

            int index = 0;
            double minMass = tPolynomial[0].Power + fPolynomial[0].Power;
            double maxMass = tPolynomial[i - 1].Power + fPolynomial[j - 1].Power;

            int maxIndex = (int)(Math.Abs(maxMass - minMass) / deltaMass + 0.5);
            if (maxIndex >= fgidPolynomial.Count)
            {
                j = maxIndex - fgidPolynomial.Count;
                for (i = 0; i <= j; i++)
                {
                    fgidPolynomial.Add(new Polynomial());
                }
            }

            for (int t = 0; t < tPolynomial.Count; t++)
            {
                for (int f = 0; f < fPolynomial.Count; f++)
                {
                    double prob = tPolynomial[t].Probablity * fPolynomial[f].Probablity;
                    if (prob <= minProbability)
                        continue;

                    double power = tPolynomial[t].Power + fPolynomial[f].Power;
                    index = (int)(Math.Abs(power - minMass) / deltaMass + 0.5);

                    Polynomial tempPolynomial = fgidPolynomial[index];

                    fgidPolynomial[index] = new Polynomial { Power = tempPolynomial.Power + power * prob, Probablity = tempPolynomial.Probablity + prob };
                }
            }

            index = tPolynomial.Count;
            j = 0;
            for (i = 0; i < fgidPolynomial.Count; i++)
            {
                if (fgidPolynomial[i].Probablity != 0)
                {
                    if (j < index)
                    {
                        tPolynomial[j] = new Polynomial { Power = fgidPolynomial[i].Power / fgidPolynomial[i].Probablity, Probablity = fgidPolynomial[i].Probablity };
                        j++;
                    }
                    else
                    {
                        tPolynomial.Add(new Polynomial { Power = fgidPolynomial[i].Power / fgidPolynomial[i].Probablity, Probablity = fgidPolynomial[i].Probablity });
                    }
                }

                fgidPolynomial[i] = new Polynomial();
            }

            if (j < index)
            {
                tPolynomial.RemoveRange(j, tPolynomial.Count - j);
            }
        }


        private static void MultipleFinePolynomialRecursiveHelper(int[] mins, int[] maxs, int[] indices, int index, IList<Polynomial> fPolynomial, IList<Composition> elementalComposition, int atoms, double minProb, int maxValue)
        {
            for (indices[index] = mins[index]; indices[index] <= maxs[index]; indices[index]++)
            {
                if (index < mins.Length-1)
                {
                    MultipleFinePolynomialRecursiveHelper(mins, maxs, indices, index + 1, fPolynomial, elementalComposition, atoms, minProb, maxValue);
                }
                else
                {
                    int l = atoms - indices.Sum();
                    if(l < 0 || l > maxValue)
                        continue; 
                   
                    double prob = FactorLn(atoms) - FactorLn(l) + l * elementalComposition[elementalComposition.Count - 1].LogProbability;
                    double power = l * elementalComposition[elementalComposition.Count - 1].Power;
                    for (int i = 0; i <= elementalComposition.Count - 2; i++)
                    {
                        int indexValue = indices[i];
                        Composition tComposition = elementalComposition[i];
                        prob -= FactorLn(indexValue);
                        prob += indexValue * tComposition.LogProbability;
                        power += indexValue * tComposition.Power;
                    }

                    prob = Math.Exp(prob);
                    if (prob >= minProb)
                    {
                        Polynomial tPolynomial = new Polynomial { Probablity = prob, Power = power };
                        fPolynomial.Add(tPolynomial);
                    }
                    
                }
            }
        }
        
        private static readonly double[] factorLnArray = new double[50003];
        private static int _factorLnTop = 1;

        private static double FactorLn(int n)
        {
            if (n < 0)
                throw new ArgumentException("n must be zero or greater");
            
            if (n <= 1)
                return 0;

            if (n > 50000)
                return n * Math.Log(n) - n + 0.5 * Math.Log(6.28318530717959 * n) + 0.08333333333333 / n - 0.00333333333333 / (n * n * n);
            
            while (_factorLnTop <= n)
            {
                int j = _factorLnTop++;
                factorLnArray[j + 1] = factorLnArray[j] + Math.Log(_factorLnTop);
            }
            return factorLnArray[n];
        }

        private class Composition
        {
            public double Power;
            public double Probability;
            public double LogProbability;
            public double MolecularWeight;
            public int Atoms;
        }
       
        private struct Polynomial
        {
            public double Power;
            public double Probablity;

            public override string ToString()
            {
                return string.Format("{0} - {1}", Power, Probablity);
            }
        }
    }
}
;