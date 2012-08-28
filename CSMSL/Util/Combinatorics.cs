using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Util
{
    public class Combinatorics
    {

        public static long Choose(long n, long k)
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
    }
}
