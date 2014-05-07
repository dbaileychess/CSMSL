using System;
using System.Linq;
using CSMSL.Chemistry;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using CSMSL.Spectral;

namespace CSMSL
{
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
                double intensity;          
            
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
                throw new ArgumentNullException("list");
            if (index < 0 || length < 0)
                throw new ArgumentOutOfRangeException( (index < 0) ? "index" : "length" );
            if (list.Count - index < length)
                throw new ArgumentException();

            int lower = index;
            int upper = (index + length -1);
            while(lower <= upper) {
                int adjustedIndex = lower + ((upper - lower) >> 1);
                int comparison = comparer.Compare(list[adjustedIndex], value);
                if(comparison == 0)
                    return adjustedIndex;
                if (comparison < 0)
                    lower = adjustedIndex + 1;
                else 
                    upper = adjustedIndex - 1;
            }
            return ~lower;
        }

        public static int BinarySearch<T>(this IList<T> list, T value, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException("list");
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

        public static double[,] LUDecompose(this double[,] matrix, out int[] index)
        {
            double[,] m = matrix.Copy();
            int n = m.GetLength(0);

            index = new int[m.Length];

            int i, imax = 0, j, k;
            double big, temp;
            double[] vv = new double[n];

            //preChecks
            if (m.GetLength(0) != m.GetLength(1))
                throw new Exception("matrix dimension problem only use square matrices");

            //for each row find the absolute value of the greatest cell and store in vv
            for (i = 0; i < n; i++)
            {
                big = 0.0;
                for (j = 0; j < n; j++)
                    if ((temp = Math.Abs(m[i, j])) > big) big = temp;
                if (big.MassEquals(0.0))
                    throw new Exception("singular matrix");

                vv[i] = 1.0 / big; //calculate scaling and save
            }
            //k is for colums start with the left look for the columns under the diagonal for the biggest value want to move the largest over diagonal
            for (k = 0; k < n; k++)//find the largest pivot element 
            {
                big = 0.0;
                for (i = k; i < n; i++)
                {
                    temp = vv[i] * Math.Abs(m[i, k]);
                    if (temp > big)
                    {
                        big = temp;
                        imax = i;
                    }
                }

                if (k != imax)//do we need a row change 
                {
                    for (j = 0; j < n; j++)// counter for the colums
                    {
                        temp = m[imax, j];// change the rows
                        m[imax, j] = m[k, j];
                        m[k, j] = temp;
                    }
                    vv[imax] = vv[k];
                }
                index[k] = imax;

                for (i = k + 1; i < n; i++)
                {
                    temp = m[i, k] /= m[k, k];//divide pilot element
                    for (j = k + 1; j < n; j++)
                        m[i, j] -= temp * m[k, j];
                }
            }
            return m;
        }

        public static double[] Solve(this double[,] luMatrix, double[] b, int[] index)
        {
            if (b.Length != luMatrix.GetLength(0) || b.Length != luMatrix.GetLength(0))
                throw new Exception("vector dimension problem");

            double[] result = new double[b.Length];

            int n = luMatrix.GetLength(0);
            int i, ii = 0;
            int j;
            double sum;
            for (i = 0; i < n; i++) result[i] = b[i];
            for (i = 0; i < n; i++)
            {
                int ip = index[i];
                sum = result[ip];
                result[ip] = result[i];
                if (ii != 0)
                    for (j = ii - 1; j < i; j++) sum -= luMatrix[i, j] * result[j];
                else if (sum.Equals(0.0))
                    ii = i + 1;
                result[i] = sum;
            }
            for (i = n - 1; i >= 0; i--)
            {
                sum = result[i];
                for (j = i + 1; j < n; j++) sum -= luMatrix[i, j] * result[j];
                result[i] = sum / luMatrix[i, i];
            }

            return result;
        }
    }
    
}


