using System;
using System.Linq;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;
using CSMSL.Spectral;

namespace CSMSL
{
    public static class MassExtension
    {
        /// <summary>
        /// Converts the object that has a mass into a m/z value based on the charge state
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="charge"></param>
        /// <param name="c13Isotope"></param>
        /// <returns></returns>
        public static double ToMz(this IMass mass, int charge, int c13Isotope = 0)
        {
            return Mass.MzFromMass(mass.MonoisotopicMass + c13Isotope*Constants.C13C12Difference, charge);
        }

        public static bool MassEquals(this IMass mass1, IMass mass2, double precision = 0.00000000001)
        {
            return Math.Abs(mass1.MonoisotopicMass - mass2.MonoisotopicMass) < precision;
        }

        public static int Compare(this IMass mass1, IMass mass2, double precision = 0.00000000001)
        {
            double difference = mass1.MonoisotopicMass - mass2.MonoisotopicMass;
            if (difference < -precision)
                return -1;
            if (difference > precision)
                return 1;
            return 0;
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

    public static class SpectrumExtension
    {
        public static Chromatogram GetExtractedIonChromatogram(this IEnumerable<ISpectrumTime> spectra, MzRange range)
        {
            if (range == null)
            {
                throw new ArgumentException("A range must be declared for a m/z range chromatogram");
            }

            List<double> times = new List<double>();
            List<double> intensities = new List<double>();

            foreach (ISpectrumTime spectrum in spectra)
            {
                double intensity = 0;            
            
                spectrum.TryGetIntensities(range, out intensity);
                times.Add(spectrum.Time);
                intensities.Add(intensity);
            }

            return new MassRangeChromatogram(times.ToArray(), intensities.ToArray(), range);
        }

        public static Chromatogram GetClosetsPeakChromatogram(this IEnumerable<ISpectrumTime> spectra, MzRange range)
        {
            if (range == null)
            {
                throw new ArgumentException("A range must be declared for a m/z range chromatogram");
            }

            List<double> times = new List<double>();
            List<double> intensities = new List<double>();

            foreach (ISpectrumTime spectrum in spectra)
            {
                times.Add(spectrum.Time);
                var peak = spectrum.GetClosestPeak(range);               
                intensities.Add((peak != null) ? peak.Intensity : 0);                
            }

            return new MassRangeChromatogram(times.ToArray(), intensities.ToArray(), range);
        }
    }

    public static class UtilExtension
    {
        /// <summary>
        /// Checks if two collections are equivalent, regardless of order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
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

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

    public static class ListExtension
    {
        public static double Median(this List<double> values)
        {
            int length = values.Count;

            if (length == 0)
                return 0;

            values.Sort();

            int mid = length/2;
            if (length%2 != 0)
            {
                return values[mid];
            }

            return (values[mid] + values[mid - 1])/2.0;

        }

        public static double StdDev(this IList<double> values)
        {
            int length = values.Count;

            if (length == 0)
                return 0;

            double mean = values.Average();
            double stdDev = values.Sum(value => (value - mean)*(value - mean));
            return Math.Sqrt(stdDev / values.Count);
        }

        public static int[] Histogram(this IList<double> values, int numberOfBins, out double min, out double max, out double binSize)
        {
            max = values.Max();
            min = values.Min();
            double range = max - min;
            binSize = range/numberOfBins;
            int[] bins = new int[numberOfBins];

            foreach (double value in values)
            {
                int binnedValue = (int)((value - min)/binSize);
                if (binnedValue == numberOfBins)
                    binnedValue--;
                bins[binnedValue]++;
            }
            return bins;
        }

        public static int[] Histogram(this IList<double> values, int numberOfBins, double min, double max, out double binSize)
        {
            double range = max - min;
            binSize = range / numberOfBins;
            int[] bins = new int[numberOfBins];

            foreach (double value in values)
            {
                int binnedValue = (int)((value - min) / binSize);
                if (binnedValue == numberOfBins)
                    binnedValue--;
                bins[binnedValue]++;
            }
            return bins;
        } 

        public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            // From: http://stackoverflow.com/questions/462699/how-do-i-get-the-index-of-the-highest-value-in-an-array-using-linq
            int maxIndex = -1;
            T maxValue = default(T);

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

        public static int BinarySearch<T>(this IList<T> list, int index, int length, T value, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException("List");
            else if (index < 0 || length < 0)
                throw new ArgumentOutOfRangeException( (index < 0) ? "index" : "length" );
            else if (list.Count - index < length)
                throw new ArgumentException();

            int lower = index;
            int upper = (index + length -1);
            while(lower <= upper) {
                int adjustedIndex = lower + ((upper - lower) >> 1);
                int comparison = comparer.Compare(list[adjustedIndex], value);
                if(comparison == 0)
                    return adjustedIndex;
                else if (comparison < 0)
                    lower = adjustedIndex + 1;
                else 
                    upper = adjustedIndex - 1;
            }
            return ~lower;
        }

        public static int BinarySearch<T>(this IList<T> list, T value, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException("List");
            return list.BinarySearch(0, list.Count, value, comparer);
        }

        public static int BinarySearch<T>(this IList<T> list, T value) where T : IComparer<T>
        {
            return list.BinarySearch(value, Comparer<T>.Default);
        }
       
    }

    public static class ByteArrayExtension
    {
        public static byte[] Compress(this byte[] bytes)
        {
            var compressedStream = new MemoryStream();
            using (var stream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                new MemoryStream(bytes).CopyTo(stream);
            }
            return compressedStream.ToArray();
        }

        public static byte[] Decompress(this byte[] bytes)
        {
            var bigStreamOut = new MemoryStream();
            new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress).CopyTo(bigStreamOut);
            return bigStreamOut.ToArray();
        }

        /// <summary>
        /// Checks if the byte array is gzipped. 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool IsCompressed(this byte[] bytes)
        {
            // From http://stackoverflow.com/questions/19364497/how-to-tell-if-a-byte-array-is-gzipped
            return bytes.Length >= 2 && bytes[0] == 31 && bytes[1] == 139;
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

    public static class MatrixExtension
    {
        public static double[,] Copy(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] newMatrix = new double[rows,cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    newMatrix[r, c] = matrix[r, c];
                }
            }
            return newMatrix;
        }
    }
}


