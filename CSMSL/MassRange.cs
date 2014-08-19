// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MassRange.cs) is part of CSMSL.
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

namespace CSMSL
{
    public class MassRange : DoubleRange
    {
        public MassRange()
        {
        }

        public MassRange(double minMass, double maxMass)
            : base(minMass, maxMass)
        {
        }

        public MassRange(double meanMass, Tolerance toleranceWidth)
            : base(meanMass, toleranceWidth)
        {
        }

        public override string ToString()
        {
            return ToString("G9");
        }

        public override string ToString(string format)
        {
            return string.Format("{0} - {1} Da", Minimum.ToString(format), Maximum.ToString(format));
        }

        #region Static

        public new static MassRange FromPPM(double mean, double ppmTolerance)
        {
            return new MassRange(mean, new Tolerance(ToleranceUnit.PPM, ppmTolerance));
        }

        #endregion Static
    }
}