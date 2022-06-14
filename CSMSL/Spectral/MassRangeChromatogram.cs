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