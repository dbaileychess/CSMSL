///////////////////////////////////////////////////////////////////////////
//  Tolerance.cs - A measure of the difference between two values         /
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
using System.Text.RegularExpressions;

namespace CSMSL
{

    /// <summary>
    /// The tolerance, or error, of two points
    /// </summary>
    public class Tolerance
    {

        private static readonly Regex StringRegex = new Regex(@"(\+-|-\+|±)?\s*([\d.]+)\s*(PPM|DA|MMU)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Creates a new tolerance given a type, value, and whether the tolerance is ±
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="plusAndMinus"></param>
        public Tolerance(ToleranceType type, double value, bool plusAndMinus = true)
        {
            Type = type;
            Value = value;
            PlusAndMinus = plusAndMinus;
        }

        public Tolerance(ToleranceType type, double experimental, double theoretical, bool plusAndMinus = true)
            : this(type, GetTolerance(experimental, theoretical, type), plusAndMinus) { }

        /// <summary>
        /// Calculates a tolerance from the string representation
        /// <para>
        /// i.e., "10 PPM", "-+10 PPM", "5 DA", "±10 MMU", etc...
        /// </para>
        /// </summary>
        /// <param name="s"></param>
        public Tolerance(string s)
        {
            Match m = StringRegex.Match(s);
            if (!m.Success)
                throw new ArgumentException("Input string is not in the correct format: " + s);
            PlusAndMinus = m.Groups[1].Success;
            Value = double.Parse(m.Groups[2].Value);
            ToleranceType type;
            Enum.TryParse(m.Groups[3].Value, true, out type);
            Type = type;
        }

        /// <summary>
        /// The tolerance unit type
        /// </summary>
        public ToleranceType Type { get; set; }

        /// <summary>
        /// The value of the tolerance
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Indicates if this tolerance is ± or not
        /// </summary>
        public bool PlusAndMinus { get; set; }

        public DoubleRange GetRange(double mass)
        {
            double tol;
            switch (Type)
            {
                case ToleranceType.MMU:
                    tol = Value/2000.0;
                    break;
                case ToleranceType.PPM:
                    tol = Value * mass / 2e6;
                    break;
                default:
                    tol = Value / 2.0;
                    break;
            }
            return new DoubleRange(mass - tol, mass + tol);
        }

        public double GetMinimumValue(double mass)
        {
            switch (Type)
            {
                case ToleranceType.MMU:
                    return mass - Value / 2000.0;
                case ToleranceType.PPM:
                    return mass * (1 - (Value / 2e6));
                default:
                    return mass - Value / 2.0;
            }
        }

        public double GetMaximumValue(double mass)
        {
            switch (Type)
            {
                case ToleranceType.MMU:
                    return mass + Value / 2000.0;
                case ToleranceType.PPM:
                    return mass * (1 + (Value / 2e6));
                default:
                    return mass + Value / 2.0;
            }
        }

        /// <summary>
        /// Indicates if the two values provided are within this tolerance
        /// </summary>
        /// <param name="experimental">The experimental value</param>
        /// <param name="theoretical">The theoretical value</param>
        /// <returns>Returns true if the value is within this tolerance  </returns>
        public bool Within(double experimental, double theoretical)
        {
            double tolerance = GetTolerance(experimental, theoretical, Type);
            return Math.Abs(tolerance) <= Value;
        }

        public override string ToString()
        {
            return string.Format("{0}{1:f4} {2}", (PlusAndMinus) ? "±" : "", Value, Enum.GetName(typeof(ToleranceType), Type));
        }

        #region Static
        
        public static double GetTolerance(double experimental, double theoretical, ToleranceType type)
        {
            switch (type)
            {
                case ToleranceType.MMU:
                    return (experimental - theoretical) * 1000.0;
                case ToleranceType.PPM:
                    return (experimental - theoretical) / theoretical * 1e6;
                default:
                    return experimental - theoretical;
            }
        }
        
        public static Tolerance FromPPM(double value, bool plusAndMinus = true)
        {
            return new Tolerance(ToleranceType.PPM, value, plusAndMinus);
        }

        public static Tolerance FromDA(double value, bool plusAndMinus = true)
        {
            return new Tolerance(ToleranceType.DA, value, plusAndMinus);
        }

        public static Tolerance FromMMU(double value, bool plusAndMinus = true)
        {
            return new Tolerance(ToleranceType.MMU, value, plusAndMinus);
        }

        public static Tolerance CalculatePrecursorMassError(double theoreticalMass, double observedMass, out int nominalMassOffset, out double adjustedObservedMass, double difference = Constants.C13C12Difference,
           ToleranceType type = ToleranceType.PPM)
        {
            double massError = observedMass - theoreticalMass;
            nominalMassOffset = (int)Math.Round(massError / difference);
            double massOffset = nominalMassOffset * difference;
            adjustedObservedMass = observedMass - massOffset;
            return new Tolerance(type, adjustedObservedMass, theoreticalMass);
        }

        #endregion
    }
}