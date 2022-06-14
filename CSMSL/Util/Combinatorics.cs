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

namespace CSMSL.Util
{
    public class Combinatorics
    {
        /// <summary>
        /// Calculates the exact cumulative binomial probability.
        /// </summary>
        /// <param name="s">the number of successes</param>
        /// <param name="p">the probability of success</param>
        /// <param name="N">the number of trials</param>
        /// <returns>the probability of obtaining at least as many successes by random chance</returns>
        public static double CumBinom(long s, double p, long N)
        {
            double prob = 0;
            while (s <= N)
            {
                prob += BinomCoefficient(N, s)*Math.Pow(p, s)*Math.Pow(1.0 - p, N - s);
                s++;
            }
            return prob;
        }

        /// <summary>
        /// Calculates the binomial coefficient (nCk) (N items, choose k)
        /// </summary>
        /// <param name="n">the number items</param>
        /// <param name="k">the number to choose</param>
        /// <returns>the binomial coefficient</returns>
        public static long BinomCoefficient(long n, long k)
        {
            if (k > n)
            {
                return 0;
            }
            if (k == 0 || n == k)
            {
                return 1;
            }
            if (k > n - k)
            {
                k = n - k;
            }

            if (n < MaxBinomCoefficientMemoization)
            {
                long cachedValue = BinomCoefficients[k, n];
                if (cachedValue > 0)
                    return cachedValue;
            }

            long N = n; // store N for memorization technique

            long c = 1;
            for (long i = 1; i <= k; i++)
            {
                c *= N--;
                c /= i;
            }

            if (n < MaxBinomCoefficientMemoization)
            {
                BinomCoefficients[k, n] = c;
            }

            return c;
        }

        private const int MaxBinomCoefficientMemoization = 50;
        private static readonly long[,] BinomCoefficients = new long[MaxBinomCoefficientMemoization, MaxBinomCoefficientMemoization];

        /// <summary>
        /// Returns the largest value v where v is less than a and Choose(v,b) is less than or equal to x
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static long LargestV(long a, long b, long x)
        {
            long v = a - 1;
            while (BinomCoefficient(v, b) > x) --v;
            return v;
        }

        /// <summary>
        /// Calculates the Ambiguity score (AScore)
        /// </summary>
        /// <param name="n1">The number of matches for the best scoring isoform</param>
        /// <param name="n2">The number of matches for the second best scoring isoform</param>
        /// <param name="p">The probability of randomly matching a peak</param>
        /// <param name="N">Total number of tries (usually # of site-determining fragments)</param>
        /// <returns></returns>
        public static double AScore(int n1, int n2, double p, int N)
        {
            // The difference of logs is the same as the log of the quotient log(a) - log(b) = log(a/b)
            return -10*Math.Log10(CumBinom(n1, p, N)/CumBinom(n2, p, N));
        }

        public static IEnumerable<T[]> Combinations<T>(T[] input, int level)
        {
            if (input == null || input.Length == 0 || level < 1 || level > input.Length)
                yield break;

            int n = input.Length;

            if (level == 1)
            {
                for (int i = 0; i < n; i++)
                {
                    yield return new[] { input[i] };
                }
            }
            else
            {
                for (int i = 0; i < n - level + 1; i++)
                {
                    var res = new T[level];
                    for (int t = i, c = 0; t < i + level - 1; t++, c++)
                        res[c] = input[t];
                    for (int j = i + level - 1; j < n; j++)
                    {
                        res[level - 1] = input[j];
                        yield return res;
                    }
                }
            }
        }
    }
}