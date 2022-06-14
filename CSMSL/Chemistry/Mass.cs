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