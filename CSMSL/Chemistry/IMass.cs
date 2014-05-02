///////////////////////////////////////////////////////////////////////////
//  IMass.cs - An object that contains a mass                             /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System;

namespace CSMSL.Chemistry
{
    public interface IMass
    {
        /// <summary>
        /// The monoisotopic mass of this object
        /// </summary>
        double MonoisotopicMass { get; }
    }

    public static class IMassExtensions
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
            return Mass.MzFromMass(mass.MonoisotopicMass + c13Isotope * Constants.C13C12Difference, charge);
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
}