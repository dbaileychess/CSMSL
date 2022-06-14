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
using System.Globalization;
using System.Text.RegularExpressions;

namespace CSMSL
{
    /// <summary>
    /// The tolerance, or error, of two points
    /// </summary>
    public class Tolerance
    {
        /// <summary>
        /// A regex for parsing a string representation of a tolerance
        /// <para>
        /// i.e., "10 PPM", "-+10 PPM", "5 DA", "±10 MMU", etc...
        /// </para>
        /// </summary>
        private static readonly Regex StringRegex = new Regex(@"(\+-|-\+|±)?\s*([\d.]+)\s*(PPM|DA|MMU)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Creates a new tolerance given a unit, value, and whether the tolerance is ±
        /// </summary>
        /// <param name="unit">The units for this tolerance</param>
        /// <param name="value">The numerical value of the tolerance</param>
        /// <param name="type">Whether the tolerance is full or half width</param>
        public Tolerance(ToleranceUnit unit, double value, ToleranceType type = ToleranceType.PlusAndMinus)
        {
            Unit = unit;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Creates a new tolerance given a unit, two points (one experimental and one theoretical), and whether the tolerance is ±
        /// </summary>
        /// <param name="unit">The units for this tolerance</param>
        /// <param name="experimental">The experimental value</param>
        /// <param name="theoretical">The theoretical value</param>
        /// <param name="type">Whether the tolerance is full or half width</param>
        public Tolerance(ToleranceUnit unit, double experimental, double theoretical, ToleranceType type = ToleranceType.PlusAndMinus)
            : this(unit, GetTolerance(experimental, theoretical, unit), type)
        {
        }

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
            Value = double.Parse(m.Groups[2].Value, CultureInfo.CurrentCulture);
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

        /// <summary>
        /// Gets the range of values encompassed by this tolerance
        /// </summary>
        /// <param name="mean">The mean value</param>
        /// <returns></returns>
        public DoubleRange GetRange(double mean)
        {
            double value = Value*((Type == ToleranceType.PlusAndMinus) ? 2 : 1);

            double tol;
            switch (Unit)
            {
                case ToleranceUnit.MMU:
                    tol = value/2000.0;
                    break;

                case ToleranceUnit.PPM:
                    tol = value*mean/2e6;
                    break;

                default:
                    tol = value/2.0;
                    break;
            }
            return new DoubleRange(mean - tol, mean + tol);
        }

        /// <summary>
        /// Gets the minimum value that is still within this tolerance
        /// </summary>
        /// <param name="mean"></param>
        /// <returns></returns>
        public double GetMinimumValue(double mean)
        {
            double value = Value*((Type == ToleranceType.PlusAndMinus) ? 2 : 1);

            switch (Unit)
            {
                case ToleranceUnit.MMU:
                    return mean - value/2000.0;

                case ToleranceUnit.PPM:
                    return mean*(1 - (value/2e6));

                default:
                    return mean - value/2.0;
            }
        }

        /// <summary>
        /// Gets the maximum value that is still within this tolerance
        /// </summary>
        /// <param name="mean"></param>
        /// <returns></returns>
        public double GetMaximumValue(double mean)
        {
            double value = Value*((Type == ToleranceType.PlusAndMinus) ? 2 : 1);

            switch (Unit)
            {
                case ToleranceUnit.MMU:
                    return mean + value/2000.0;

                case ToleranceUnit.PPM:
                    return mean*(1 + (value/2e6));

                default:
                    return mean + value/2.0;
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
            double tolerance = Math.Abs(GetTolerance(experimental, theoretical, Unit));
            double value = (Type == ToleranceType.PlusAndMinus) ? Value : Value/2;
            return tolerance <= value;
        }

        public override string ToString()
        {
            return string.Format("{0}{1:f4} {2}", (Type == ToleranceType.PlusAndMinus) ? "±" : "", Value, Enum.GetName(typeof (ToleranceUnit), Unit));
        }

        #region Static

        public static double GetTolerance(double experimental, double theoretical, ToleranceUnit type)
        {
            switch (type)
            {
                case ToleranceUnit.MMU:
                    return (experimental - theoretical)*1000.0;

                case ToleranceUnit.PPM:
                    return (experimental - theoretical)/theoretical*1e6;

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
            nominalMassOffset = (int) Math.Round(massError/difference);
            double massOffset = nominalMassOffset*difference;
            adjustedObservedMass = observedMass - massOffset;
            return new Tolerance(type, adjustedObservedMass, theoreticalMass);
        }

        #endregion Static
    }
}