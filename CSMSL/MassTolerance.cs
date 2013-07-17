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

namespace CSMSL
{
    public class MassTolerance
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public MassTolerance(MassToleranceType type, double value)
        {
            Type = type;
            Value = value;
        }

        public MassTolerance(MassToleranceType type, double experimental, double theoretical)
            : this(type, GetTolerance(experimental, theoretical, type)) { }

        public MassToleranceType Type { get; set; }

        public double Value { get; set; }

        public MassRange GetMassRange(double mass)
        {
            double tol;
            switch (Type)
            {
                case MassToleranceType.MMU:
                    tol = Value/2000.0;
                    break;
                case MassToleranceType.PPM:
                    tol = Value * mass / 2e6;
                    break;
                default:
                    tol = Value / 2.0;
                    break;
            }
            return new MassRange(mass - tol, mass + tol);
        }

        public double GetMinimumValue(double mass)
        {
            switch (Type)
            {
                case MassToleranceType.MMU:
                    return mass - Value / 2000.0;
                case MassToleranceType.PPM:
                    return mass * (1 - (Value / 2e6));
                default:
                    return mass - Value / 2.0;
            }
        }

        public double GetMaximumValue(double mass)
        {
            switch (Type)
            {
                case MassToleranceType.MMU:
                    return mass + Value / 2000.0;
                case MassToleranceType.PPM:
                    return mass * (1 + (Value / 2e6));
                default:
                    return mass + Value / 2.0;
            }
        }

        public static double GetTolerance(double experimental, double theoretical, MassToleranceType type)
        {
            switch (type)
            {
                case MassToleranceType.MMU:
                    return (experimental - theoretical) * 1000.0;
                case MassToleranceType.PPM:
                    return (experimental - theoretical) / theoretical * 1000000.0;
                default:
                    return experimental - theoretical;
            }
        }
        
        public static MassTolerance FromPPM(double value)
        {
            return new MassTolerance(MassToleranceType.PPM, value);
        }

        public static MassTolerance FromDA(double value)
        {
            return new MassTolerance(MassToleranceType.DA, value);
        }

        public override string ToString()
        {
            return string.Format("{0:f4} {1}", Value, Enum.GetName(typeof(MassToleranceType), Type));
        }
    }
}