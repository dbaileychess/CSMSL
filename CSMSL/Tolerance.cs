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
        /// <param name="unit"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public Tolerance(ToleranceUnit unit, double value, ToleranceType type = ToleranceType.PlusAndMinus)
        {
            Unit = unit;
            Value = value;
            Type = type;
        }

        public Tolerance(ToleranceUnit unit, double experimental, double theoretical, ToleranceType toleranceType = ToleranceType.PlusAndMinus)
            : this(unit, GetTolerance(experimental, theoretical, unit), toleranceType) { }

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
            Type = m.Groups[1].Success ? ToleranceType.PlusAndMinus : ToleranceType.FullWidth;
            Value = double.Parse(m.Groups[2].Value);
            ToleranceUnit type;
            Enum.TryParse(m.Groups[3].Value, true, out type);
            Unit = type;
        }

        /// <summary>
        /// The tolerance unit type
        /// </summary>
        public ToleranceUnit Unit { get; set; }

        /// <summary>
        /// The value of the tolerance
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Indicates if this tolerance is ± or not
        /// </summary>
        public ToleranceType Type { get; set; }

        public DoubleRange GetRange(double mass)
        {
            double tol;
            switch (Unit)
            {
                case ToleranceUnit.MMU:
                    tol = Value/2000.0;
                    break;
                case ToleranceUnit.PPM:
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
            switch (Unit)
            {
                case ToleranceUnit.MMU:
                    return mass - Value / 2000.0;
                case ToleranceUnit.PPM:
                    return mass * (1 - (Value / 2e6));
                default:
                    return mass - Value / 2.0;
            }
        }

        public double GetMaximumValue(double mass)
        {
            switch (Unit)
            {
                case ToleranceUnit.MMU:
                    return mass + Value / 2000.0;
                case ToleranceUnit.PPM:
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
            double tolerance = GetTolerance(experimental, theoretical, Unit);
            return Math.Abs(tolerance) <= Value;
        }

        public override string ToString()
        {
            return string.Format("{0}{1:f4} {2}", (Type == ToleranceType.PlusAndMinus) ? "±" : "", Value, Enum.GetName(typeof(ToleranceUnit), Unit));
        }

        #region Static
        
        public static double GetTolerance(double experimental, double theoretical, ToleranceUnit type)
        {
            switch (type)
            {
                case ToleranceUnit.MMU:
                    return (experimental - theoretical) * 1000.0;
                case ToleranceUnit.PPM:
                    return (experimental - theoretical) / theoretical * 1e6;
                default:
                    return experimental - theoretical;
            }
        }
        
        public static Tolerance FromPPM(double value, ToleranceType toleranceType = ToleranceType.PlusAndMinus)
        {
            return new Tolerance(ToleranceUnit.PPM, value, toleranceType);
        }

        public static Tolerance FromDA(double value, ToleranceType toleranceType = ToleranceType.PlusAndMinus)
        {
            return new Tolerance(ToleranceUnit.DA, value, toleranceType);
        }

        public static Tolerance FromMMU(double value, ToleranceType toleranceType = ToleranceType.PlusAndMinus)
        {
            return new Tolerance(ToleranceUnit.MMU, value, toleranceType);
        }

        public static Tolerance CalculatePrecursorMassError(double theoreticalMass, double observedMass, out int nominalMassOffset, out double adjustedObservedMass, double difference = Constants.C13C12Difference,
           ToleranceUnit type = ToleranceUnit.PPM)
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