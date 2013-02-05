///////////////////////////////////////////////////////////////////////////
//  Mass.cs - The monoisotopic and average mass of an object              /
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
    public class Mass : IComparable<Mass>
    {
        /// <summary>
        /// The mass of all the isotopes (in unified atomic mass units)
        /// </summary>
        public double Monoisotopic { get; internal set; }

        /// <summary>
        /// The average mass of all the elements (in unified atomic mass units)
        /// </summary>
        public double Average { get; internal set;}   

        /// <summary>
        /// Create a default mass with the Monoisotpic and Average mass of 0
        /// </summary>
        public Mass()
            : this(0, 0) { }

        public Mass(IMass item)
            : this(item.Mass.Monoisotopic, item.Mass.Average) { }

        public Mass(Mass item)
            : this(item.Monoisotopic, item.Average) { }

        public Mass(double monoisotopic, double average)
        {
            Monoisotopic = monoisotopic;
            Average = average;
        }

        /// <summary>
        /// Calculates the m/z for the monoisotopic mass
        /// </summary>
        /// <param name="charge">The charge state</param>
        /// <returns>The m/z for the moniosotopic mass at a given charge state</returns>
        public double ToMz(int charge)
        {
            return MzFromMass(Monoisotopic, charge);
        }

        public override string ToString()
        {
            return string.Format("{0:F5} ({1:F5})", Monoisotopic, Average);
        }

        public int CompareTo(Mass other)
        {
            return Monoisotopic.CompareTo(other.Monoisotopic);
        }

        #region Static Methods

        /// <summary>
        /// Calculates the mass of a given m/z and charge, assuming a proton is the charge donator
        /// </summary>
        /// <param name="mz">The given m/z</param>
        /// <param name="charge">The given charge</param>
        /// <returns>The mass</returns>
        public static double MassFromMz(double mz, int charge)
        {
            if (mz == 0) return 0;
            return Math.Abs(charge) * mz - charge * Constants.PROTON;
        }

        /// <summary>
        /// Calculates the m/z of a given mass and chargem assuming a proton is the charge donator
        /// </summary>
        /// <param name="mass">The given mass</param>
        /// <param name="charge">The given charge</param>
        /// <returns>The m/z</returns>
        public static double MzFromMass(double mass, int charge)
        {
            if (mass == 0 || charge == 0) return 0;
            return mass / Math.Abs(charge) + Math.Sign(charge) * Constants.PROTON;
        }

        /// <summary>
        /// Calculates the spacing of isotopes (C13 primarily) in m/z space at a given
        /// charge state
        /// </summary>
        /// <param name="charge">The charge state to calculate the spacing in</param>
        /// <returns>The distance (in Th) between successive isotopes</returns>
        public static double GetPeakSpacing(int charge)
        {
            return (Constants.CARBON13 - Constants.CARBON) / Math.Abs(charge);
        }

        #endregion Static Methods
    }
}