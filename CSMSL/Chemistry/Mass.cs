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
    public class Mass : IMass, IComparable<Mass>, IEquatable<Mass> 
    {
        /// <summary>
        /// The mass of all the isotopes (in unified atomic mass units)
        /// </summary>
        public double Monoisotopic { get; internal set; }

        /// <summary>
        /// The average mass of all the elements (in unified atomic mass units)
        /// </summary>
        public double Average { get; internal set;}   

        public Mass(IMass item)
            : this(item.Mass.Monoisotopic, item.Mass.Average) { }
               
        public Mass(double monoisotopic = 0, double average = 0)
        {
            Monoisotopic = monoisotopic;
            Average = average;
        }

        /// <summary>
        /// Adds the mass of another object to this
        /// </summary>
        /// <param name="item">The item which possesses a mass</param>
        public void Add(IMass item)
        {
            if (item == null)
                return;

            Monoisotopic += item.Mass.Monoisotopic;
            Average += item.Mass.Average;
        }

        /// <summary>
        /// Adds the mass of another object to this
        /// </summary>
        /// <param name="item">The item which possesses a mass</param>
        public void Add(Mass item)
        {
            if (item == null)
                return;

            Monoisotopic += item.Monoisotopic;
            Average += item.Average;
        }

        /// <summary>
        /// Removes the mass of another object to this
        /// </summary>
        /// <param name="item">The item which possesses a mass</param>
        public void Remove(Mass item)
        {
            if (item == null) 
                return;

            Monoisotopic -= item.Monoisotopic;
            Average -= item.Average;
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
            return Monoisotopic.ToString("G5");            
        }

        public int CompareTo(Mass other)
        {
            return Monoisotopic.CompareTo(other.Monoisotopic);
        }
                
        public bool Equals(Mass other)
        {
            return Monoisotopic.Equals(other.Monoisotopic) && Average.Equals(other.Average);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Mass;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Monoisotopic.GetHashCode() + Average.GetHashCode();
        }

        Mass IMass.Mass
        {
            get { return this; }
        }

        #region Static Methods

        public static Mass operator +(Mass left, IMass right)
        {
            return new Mass(left.Monoisotopic + right.Mass.Monoisotopic, left.Average + right.Mass.Average);
        }

        public static Mass operator -(Mass left, IMass right)
        {
            return new Mass(left.Monoisotopic - right.Mass.Monoisotopic, left.Average - right.Mass.Average);
        }

        public static Mass operator /(Mass left, int right)
        {
            return new Mass(left.Monoisotopic / right, left.Average / right);
        }

        public static Mass operator /(Mass left, double right)
        {
            return new Mass(left.Monoisotopic / right, left.Average / right);
        }

        public static Mass operator *(Mass left, int right)
        {
            return new Mass(left.Monoisotopic * right, left.Average * right);
        }

        public static Mass operator *(Mass left, double right)
        {
            return new Mass(left.Monoisotopic * right, left.Average * right);
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
            if (charge == 0)
                throw new DivideByZeroException("Charge cannot be zero");
            return mass / Math.Abs(charge) + Math.Sign(charge) * Constants.PROTON;
        }

        #endregion Static Methods




    }
}