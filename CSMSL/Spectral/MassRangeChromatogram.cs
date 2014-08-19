// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MassRangeChromatogram.cs) is part of CSMSL.
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

namespace CSMSL.Spectral
{
    public class MassRangeChromatogram : Chromatogram
    {
        public DoubleRange Range { get; private set; }

        public MassRangeChromatogram(double[] times, double[] intensities, DoubleRange range)
            : base(times, intensities)
        {
            Range = range;
        }

        public MassRangeChromatogram(double[,] timeintensities, DoubleRange range)
            : base(timeintensities)
        {
            Range = range;
        }

        public MassRangeChromatogram(MassRangeChromatogram chromatogram)
            : base(chromatogram)
        {
            Range = chromatogram.Range;
        }

        public MassRangeChromatogram(Chromatogram chromatogram, DoubleRange range)
            : base(chromatogram)
        {
            Range = range;
        }

        public new MassRangeChromatogram Smooth(SmoothingType smoothing, int points)
        {
            Chromatogram chrom = base.Smooth(smoothing, points);
            return new MassRangeChromatogram(chrom, Range);
        }
    }
}