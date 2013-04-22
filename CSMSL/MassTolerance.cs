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
        private MassToleranceType _type;
        private double _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public MassTolerance(MassToleranceType type, double value)
        {
            _type = type;
            _value = value;
        }

        public MassTolerance(MassToleranceType type, double experimental, double theoretical)
            : this(type, GetTolerance(experimental, theoretical, type)) { }

        public MassToleranceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public static double GetTolerance(double experimental, double theoretical, MassToleranceType type)
        {
            switch (type)
            {
                case MassToleranceType.MMU:
                    return (experimental - theoretical) * 1000.0;
                case MassToleranceType.PPM:
                    return (experimental - theoretical) / theoretical * 1000000.0;
                case MassToleranceType.DA:
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
            return string.Format("{0:f4} {1}", _value, Enum.GetName(typeof(MassToleranceType), _type));
        }
    }
}