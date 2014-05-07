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
    public class Tolerance
    {
        private static readonly Regex _fromString = new Regex(@"([\d.-]+)\s(PPM|DA|MMU)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Tolerance(ToleranceType type, double value)
        {
            Type = type;
            Value = value;
        }

        public Tolerance(ToleranceType type, double experimental, double theoretical)
            : this(type, GetTolerance(experimental, theoretical, type)) { }

        public Tolerance(string s)
        {
            Match m = _fromString.Match(s);
            if (!m.Success)
                throw new ArgumentException("Input string is not in the correct format: " + s); 
            Value = double.Parse(m.Groups[1].Value);
            ToleranceType type;
            Enum.TryParse(m.Groups[2].Value, true, out type);
            Type = type;
        }

        public ToleranceType Type { get; set; }

        public double Value { get; set; }

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

        public bool Within(double experimental, double theoretical)
        {
            double tolerance = GetTolerance(experimental, theoretical, Type);
            return Math.Abs(tolerance) <= Value;
        }

        public static double GetTolerance(double experimental, double theoretical, ToleranceType type)
        {
            switch (type)
            {
                case ToleranceType.MMU:
                    return (experimental - theoretical) * 1000.0;
                case ToleranceType.PPM:
                    return (experimental - theoretical) / theoretical * 1000000.0;
                default:
                    return experimental - theoretical;
            }
        }
        
        public static Tolerance FromPPM(double value)
        {
            return new Tolerance(ToleranceType.PPM, value);
        }

        public static Tolerance FromDA(double value)
        {
            return new Tolerance(ToleranceType.DA, value);
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

        //public static Tolerance CalculatePrecursorMassError(double theoreticalMZ, double observedMZ, int charge, out int nominalMassOffset, out double experimentalNeutralMass, double difference = Constants.Carbon13 - Constants.Carbon, ToleranceType type = ToleranceType.PPM)
        //{
        //    return CalculatePrecursorMassError(Mass.MassFromMz(theoreticalMZ, charge),
        //        Mass.MassFromMz(observedMZ, charge), out nominalMassOffset, out experimentalNeutralMass, difference, type);
        //}

        public override string ToString()
        {
            return string.Format("{0:f4} {1}", Value, Enum.GetName(typeof(ToleranceType), Type));
        }
    }
}