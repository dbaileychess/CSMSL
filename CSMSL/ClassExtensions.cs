// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ClassExtensions.cs) is part of CSMSL.
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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CSMSL
{
    /// A collection of extension methods used in CSMSL
    public static class UtilExtension
    {
        /// <summary>
        /// Compares two doubles for equality based on their absolute difference being less
        /// than some specified tolerance.
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool FussyEquals(this double item1, double item2, double tolerance = 1e-10)
        {
            return Math.Abs(item1 - item2) < tolerance;
        }

        /// <summary>
        /// Compares two doubles for equality based on their absolute difference being less
        /// than some specified tolerance.
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool FussyEquals(this float item1, float item2, double tolerance = 1e-10)
        {
            return Math.Abs(item1 - item2) < tolerance;
        }
    }

    public static class CollectionExtension
    {
        public static double[] BoxCarSmooth(this double[] data, int points)
        {
            // Force to be odd
            points = points - (1 - points%2);

            int count = data.Length;

            if (points <= 0 || points > count)
            {
                return null;
            }

            int newCount = count - points + 1;

            double[] smoothedData = new double[newCount];

            for (int i = 0; i < newCount; i++)
            {
                double value = 0;

                for (int j = i; j < i + points; j++)
                {
                    value += data[j];
                }

                smoothedData[i] = value/points;
            }
            return smoothedData;
        }


        /// <summary>
        /// Checks if two collections are equivalent, regardless of the order of their contents
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

        /// <summary>
        /// Extracts a subarray from a larger array (similar to String.Substring)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Calculates the median value of a list of numerical values
        /// </summary>
        /// <param name="values">A list of double values</param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculates the standard deviation of a list of numerical values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double StdDev(this IList<double> values)
        {
            int length = values.Count;

            if (length == 0)
                return 0;

            double mean = values.Average();
            double stdDev = values.Sum(value => (value - mean)*(value - mean));
            return Math.Sqrt(stdDev/values.Count);
        }

        public static byte[] GetBytes(this double[] values)
        {
            if (values == null)
                return null;
            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        public static double[] GetDoubles(this byte[] bytes)
        {
            var result = new double[bytes.Length / sizeof(double)];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }

        public static string ToCsvString(this IEnumerable<string> values, char delimiter = ',')
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                if (value.Contains(delimiter))
                {
                    sb.Append("\"" + value + "\"");
                }
                else
                {
                    sb.Append(value);
                }
                sb.Append(',');
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
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
                int binnedValue = (int) ((value - min)/binSize); // (int)Math.Floor((value - min) / binSize);
                if (binnedValue == numberOfBins)
                    binnedValue--;
                bins[binnedValue]++;
            }
            return bins;
        }

        public static int[] Histogram(this IList<double> values, int numberOfBins, double min, double max, out double binSize)
        {
            double range = max - min;
            binSize = range/numberOfBins;
            int[] bins = new int[numberOfBins];

            foreach (double value in values)
            {
                if (value < min || value > max)
                    continue;
                int binnedValue = (int) ((value - min)/binSize);
                if (binnedValue == numberOfBins)
                    binnedValue--;
                bins[binnedValue]++;
            }
            return bins;
        }

        public static int MaxIndex<TSource>(this IEnumerable<TSource> items) where TSource : IComparable<TSource>
        {
            TSource maxItem;
            return MaxIndex(items, o => o, out maxItem);
        }

        public static int MaxIndex<TSource>(this IEnumerable<TSource> items, out TSource maxItem) where TSource : IComparable<TSource>
        {
            return MaxIndex(items, o => o, out maxItem);
        }

        /// <summary>
        /// Finds the index of the maximum value in a collection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="items">The collection of items</param>
        /// <returns>An index to the place of the maximum value in the collection</returns>
        public static int MaxIndex<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selectFunc) where TResult : IComparable<TResult>
        {
            TSource maxItem;
            return MaxIndex(items, selectFunc, out maxItem);
        }

        /// <summary>
        /// Finds the index of the maximum value in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The collection of items</param>
        /// <param name="maxValue">The maximum value in the collection</param>
        /// <returns>An index to the place of the maximum value in the collection</returns>
        public static int MaxIndex<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selectFunc, out TSource maxItem) where TResult : IComparable<TResult>
        {
            // From: http://stackoverflow.com/questions/462699/how-do-i-get-the-index-of-the-highest-value-in-an-array-using-linq
            int maxIndex = -1;
            TResult maxValue = default(TResult);
            maxItem = default(TSource);

            int index = 0;
            foreach (TSource item in items)
            {
                TResult value = selectFunc(item);

                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxItem = item;
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
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "length");
            if (list.Count - index < length)
                throw new ArgumentException();

            int lower = index;
            int upper = (index + length - 1);
            while (lower <= upper)
            {
                int adjustedIndex = lower + ((upper - lower) >> 1);
                int comparison = comparer.Compare(list[adjustedIndex], value);
                if (comparison == 0)
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
        /// <summary>
        /// Compresses a byte array using Gzip compression
        /// </summary>
        /// <param name="bytes">The byte array to compress</param>
        /// <returns>The compressed byte array</returns>
        public static byte[] Compress(this byte[] bytes)
        {
            var compressedStream = new MemoryStream();
            using (var stream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                new MemoryStream(bytes).CopyTo(stream);
            }
            return compressedStream.ToArray();
        }

        /// <summary>
        /// Decompresses a byte array using Gzip decompression
        /// </summary>
        /// <param name="bytes">The byte array to decompress</param>
        /// <returns>The decompressed byte array</returns>
        public static byte[] Decompress(this byte[] bytes)
        {
            var bigStreamOut = new MemoryStream();
            new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress).CopyTo(bigStreamOut);
            return bigStreamOut.ToArray();
        }

        /// <summary>
        /// Checks if the byte array is compressed using Gzip compression.
        /// </summary>
        /// <param name="bytes">The byte array to check for compression</param>
        /// <returns></returns>
        public static bool IsCompressed(this byte[] bytes)
        {
            // From http://stackoverflow.com/questions/19364497/how-to-tell-if-a-byte-array-is-gzipped
            return bytes.Length >= 2 && bytes[0] == 31 && bytes[1] == 139;
        }
    }

    public static class MatrixExtension
    {
        public static double[,] Copy(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] newMatrix = new double[rows, cols];
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
                if (big.FussyEquals(0.0))
                    throw new Exception("singular matrix");

                vv[i] = 1.0/big; //calculate scaling and save
            }
            //k is for columns start with the left look for the columns under the diagonal for the biggest value want to move the largest over diagonal
            for (k = 0; k < n; k++) //find the largest pivot element
            {
                big = 0.0;
                for (i = k; i < n; i++)
                {
                    temp = vv[i]*Math.Abs(m[i, k]);
                    if (temp > big)
                    {
                        big = temp;
                        imax = i;
                    }
                }

                if (k != imax) //do we need a row change
                {
                    for (j = 0; j < n; j++) // counter for the colums
                    {
                        temp = m[imax, j]; // change the rows
                        m[imax, j] = m[k, j];
                        m[k, j] = temp;
                    }
                    vv[imax] = vv[k];
                }
                index[k] = imax;

                for (i = k + 1; i < n; i++)
                {
                    temp = m[i, k] /= m[k, k]; //divide pilot element
                    for (j = k + 1; j < n; j++)
                        m[i, j] -= temp*m[k, j];
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
                    for (j = ii - 1; j < i; j++) sum -= luMatrix[i, j]*result[j];
                else if (sum.Equals(0.0))
                    ii = i + 1;
                result[i] = sum;
            }
            for (i = n - 1; i >= 0; i--)
            {
                sum = result[i];
                for (j = i + 1; j < n; j++) sum -= luMatrix[i, j]*result[j];
                result[i] = sum/luMatrix[i, i];
            }

            return result;
        }
    }
}