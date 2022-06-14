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

namespace CSMSL.Chemistry
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMass
    {
        /// <summary>
        /// The monoisotopic mass of this object
        /// </summary>
        double MonoisotopicMass { get; }
    }

    public static class MassExtensions
    {
        /// <summary>
        /// The mass difference tolerance for having identical masses
        /// </summary>
        public const double MassEqualityEpsilon = 1e-10;

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

        /// <summary>
        /// Converts the object that has a m/z into a mass value based on the charge state
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="charge"></param>
        /// <param name="c13Isotope"></param>
        /// <returns></returns>
        public static double ToMass(this IMass mz, int charge, int c13Isotope = 0)
        {
            return Mass.MassFromMz(mz.MonoisotopicMass + c13Isotope*Constants.C13C12Difference, charge);
        }

        public static bool MassEquals(this double mass1, IMass mass2, double epsilon = MassEqualityEpsilon)
        {
            if (mass2 == null)
                return false;
            return Math.Abs(mass1 - mass2.MonoisotopicMass) < epsilon;
        }

        public static bool MassEquals(this double mass1, double mass2, double epsilon = MassEqualityEpsilon)
        {
            return Math.Abs(mass1 - mass2) < epsilon;
        }

        public static bool MassEquals(this IMass mass1, double mass2, double epsilon = MassEqualityEpsilon)
        {
            if (mass1 == null)
                return false;
            return Math.Abs(mass1.MonoisotopicMass - mass2) < epsilon;
        }

        public static bool MassEquals(this IMass mass1, IMass mass2, double epsilon = MassEqualityEpsilon)
        {
            if (mass1 == null || mass2 == null)
                return false;
            return Math.Abs(mass1.MonoisotopicMass - mass2.MonoisotopicMass) < epsilon;
        }

        public static int Compare(this IMass mass1, IMass mass2, double epsilon = MassEqualityEpsilon)
        {
            double difference = mass1.MonoisotopicMass - mass2.MonoisotopicMass;
            if (difference < -epsilon)
                return -1;
            return difference > epsilon ? 1 : 0;
        }
    }
}