// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (IMass.cs) is part of CSMSL.
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