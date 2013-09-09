using System;

namespace CSMSL.Util
{
    public class Combinatorics
    {
        /// <summary>
        /// Calculates the exact cumulative binomial probability.
        /// </summary>
        /// <param name="n">the number of successes</param>
        /// <param name="p">the probability of success</param>
        /// <param name="N">the number of trials</param>
        /// <returns>the probabilty of obtaining at least as many successes by random chance</returns>
        public static double CumBinom(long n, double p, long N)
        {
            double prob = 0;

            for (long k = n; k <= N; k++)
            {
                prob += BinomCoefficient(N, k) * Math.Pow(p, k) * Math.Pow(1.0 - p, N - k);
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
            if (k > n) { return 0; }
            if (n == k) { return 1; }
            if (k > n - k) { k = n - k; }
            long c = 1;
            for (long i = 0; i < k; i++)
            {
                c *= (n - i);
                c /= (i + 1);
            }
            return c;
        }

        /// <summary>
        /// Returns the larget value v where v < a and Choose(v,b) <= x
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
            // log(a) - log(b) = log(a/b)
            return -10 * Math.Log10(CumBinom(n1, p, N) / CumBinom(n2, p, N));
        }
    }
}