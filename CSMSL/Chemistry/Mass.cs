// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Mass.cs) is part of CSMSL.
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
    public struct Mass : IMass, IComparable<Mass>, IEquatable<Mass>
    {
        /// <summary>
        /// The mass of all the isotopes (in unified atomic mass units)
        /// </summary>
        public double MonoisotopicMass { get; private set; }

        public Mass(IMass item)
            : this(item.MonoisotopicMass)
        {
        }

        public Mass(double monoisotopic = 0) : this()
        {
            MonoisotopicMass = monoisotopic;
        }

        public override string ToString()
        {
            return ToString("G5");
        }

        public string ToString(string format)
        {
            return MonoisotopicMass.ToString(format);
        }

        public int CompareTo(Mass other)
        {
            return this.Compare(other);
        }

        public bool Equals(Mass other)
        {
            return MonoisotopicMass.MassEquals(other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Mass))
                return false;
            Mass mass = (Mass) obj;
            return Equals(mass);
        }

        public override int GetHashCode()
        {
            return MonoisotopicMass.GetHashCode();
        }

        #region Static Methods

        public static Mass operator +(Mass left, IMass right)
        {
            return new Mass(left.MonoisotopicMass + right.MonoisotopicMass);
        }

        public static Mass operator -(Mass left, IMass right)
        {
            return new Mass(left.MonoisotopicMass - right.MonoisotopicMass);
        }

        public static Mass operator /(Mass left, int right)
        {
            return new Mass(left.MonoisotopicMass/right);
        }

        public static Mass operator /(Mass left, double right)
        {
            return new Mass(left.MonoisotopicMass/right);
        }

        public static Mass operator *(Mass left, int right)
        {
            return new Mass(left.MonoisotopicMass*right);
        }

        public static Mass operator *(Mass left, double right)
        {
            return new Mass(left.MonoisotopicMass*right);
        }

        /// <summary>
        /// Calculates the mass of a given m/z and charge, assuming a proton is the charge donator
        /// </summary>
        /// <param name="mz">The given m/z</param>
        /// <param name="charge">The given charge</param>
        /// <returns>The mass</returns>
        public static double MassFromMz(double mz, int charge)
        {
            if (charge == 0)
                throw new DivideByZeroException("Charge cannot be zero");
            return Math.Abs(charge)*mz - charge*Constants.Proton;
        }

        /// <summary>
        /// Calculates the m/z of a given mass and chargem assuming a proton is the charge donator
        /// </summary>
        /// <param name="mass">The given mass</param>
        /// <param name="charge">The given charge</param>
        /// <returns>The m/z</returns>
        public static double MzFromMass(double mass, int charge)
        {
            if (charge == 0)
                throw new DivideByZeroException("Charge cannot be zero");
            return mass/Math.Abs(charge) + Math.Sign(charge)*Constants.Proton;
        }

        #endregion Static Methods
    }
}