﻿// Copyright 2022 Derek J. Bailey
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

namespace CSMSL
{
    public class DoubleRange : Range<double>
    {
        /// <summary>
        /// Creates a range from 0 to 0
        /// </summary>
        public DoubleRange()
            : base(0, 0)
        {
        }

        /// <summary>
        /// Creates a range from the minimum to maximum values
        /// </summary>
        /// <param name="minimum">The minimum value of the range</param>
        /// <param name="maximum">The maximum value of the range</param>
        public DoubleRange(double minimum, double maximum)
            : base(minimum, maximum)
        {
        }

        /// <summary>
        /// Creates a range from another double range. This is the
        /// clone constructor.
        /// </summary>
        /// <param name="range">The other range to copy</param>
        public DoubleRange(IRange<double> range)
            : base(range.Minimum, range.Maximum)
        {
        }

        /// <summary>
        /// Creates a range around some mean value with a specified tolerance.
        /// <para>
        /// i.e. 10 ppm at 500 would give you 499.9975 - 500.0025
        /// which has a width of 0.005. Converting back to ppm
        /// (1e6) *0.005 / 500 = 10 ppm.
        /// </para>
        /// <para>
        /// The difference from the mean value to an boundary is exactly
        /// half the tolerance you specified
        /// </para>
        /// </summary>
        /// <param name="mean">The mean value for the range</param>
        /// <param name="tolerance">The tolerance range</param>
        public DoubleRange(double mean, Tolerance tolerance)
        {
            SetTolerance(mean, tolerance);
        }

        private void SetTolerance(double mean, Tolerance tolerance)
        {
            if (tolerance == null)
            {
                Minimum = Maximum = mean;
                return;
            }

            double value = Math.Abs(tolerance.Value);

            if (tolerance.Type == ToleranceType.PlusAndMinus)
                value *= 2;

            switch (tolerance.Unit)
            {
                default:
                    Minimum = mean - value / 2.0;
                    Maximum = mean + value / 2.0;
                    break;

                case ToleranceUnit.MMU:
                    Minimum = mean - value / 2000.0;
                    Maximum = mean + value / 2000.0;
                    break;

                case ToleranceUnit.PPM:
                    Minimum = mean * (1 - (value / 2e6));
                    Maximum = mean * (1 + (value / 2e6));
                    break;
            }
        }

        /// <summary>
        /// The mean value of this range:
        /// (Max + Min) / 2
        /// </summary>
        public double Mean
        {
            get { return (Maximum + Minimum) / 2.0; }
        }

        /// <summary>
        /// The width of this range:
        /// (Max - Min)
        /// </summary>
        public double Width
        {
            get { return Maximum - Minimum; }
        }

        /// <summary>
        /// Calculates the ppm tolerance value for this range:
        /// 1e6 * Width / Mean;
        /// </summary>
        /// <returns>The ppm</returns>
        public double ToPPM()
        {
            return 1e6 * Width / Mean;
        }

        public double OverlapFraction(DoubleRange otherRange)
        {
            DoubleRange shorter, longer;
            if (Width < otherRange.Width)
            {
                shorter = this;
                longer = otherRange;
            }
            else
            {
                shorter = otherRange;
                longer = this;
            }

            double coveredWidth = 0;
            if (shorter.Minimum > longer.Minimum)
            {
                if (shorter.Minimum < longer.Maximum)
                {
                    coveredWidth = Math.Min(longer.Maximum, shorter.Maximum) - shorter.Minimum;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (shorter.Maximum < longer.Minimum)
                {
                    coveredWidth = Math.Min(shorter.Maximum, longer.Maximum) - shorter.Maximum;
                }
                else
                {
                    return 0;
                }
            }
            return coveredWidth / shorter.Width;
        }

        /// <summary>
        /// Returns a string representation of this range at the given numerical format
        /// </summary>
        /// <param name="format">The format to display the double values</param>
        /// <returns></returns>
        public virtual string ToString(string format)
        {
            return string.Format("[{0} - {1}]", Minimum.ToString(format), Maximum.ToString(format));
        }

        #region Static

        public static DoubleRange FromPPM(double mean, double ppmTolerance)
        {
            return new DoubleRange(mean, new Tolerance(ToleranceUnit.PPM, ppmTolerance));
        }

        public static DoubleRange FromDa(double mean, double daTolerance)
        {
            return new DoubleRange(mean, new Tolerance(ToleranceUnit.DA, daTolerance));
        }

        #endregion Static
    }
}