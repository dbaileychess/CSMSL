// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (Spectrum.cs) is part of CSMSL.
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

using System;
using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public class MZSpectrum : Spectrum<MZPeak, MZSpectrum>
    {
        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="mz">The m/z's</param>
        /// <param name="intensities">The intensities</param>
        /// <param name="shouldCopy">Indicates whether the input arrays should be copied to new ones</param>
        public MZSpectrum(double[] mz, double[] intensities, bool shouldCopy = true)
            : base(mz, intensities, shouldCopy)
        {
        }

        /// <summary>
        /// Initializes a new spectrum from another spectrum
        /// </summary>
        /// <param name="mzSpectrum">The spectrum to clone</param>
        public MZSpectrum(MZSpectrum mzSpectrum)
            : this(mzSpectrum._masses, mzSpectrum._intensities)
        {
        }

        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="mzintensities"></param>
        public MZSpectrum(double[,] mzintensities)
            : this(mzintensities, mzintensities.GetLength(1))
        {
        }

        public MZSpectrum(double[,] mzintensities, int count)
            : base(mzintensities, count) { }
      
        public MZSpectrum(byte[] mzintensities)
        {
            _masses = FromBytes(mzintensities, 0);
            _intensities = FromBytes(mzintensities, 1);
            Count = mzintensities.Length / (sizeof(double) * 2);
            int size = sizeof(double) * Count;
            _masses = new double[Count];
            _intensities = new double[Count];
            Buffer.BlockCopy(mzintensities, 0, _masses, 0, size);
            Buffer.BlockCopy(mzintensities, size, _intensities, 0, size);
        }

        private MZSpectrum()
        {
        }

        /// <summary>
        /// An empty spectrum
        /// </summary>
        public static readonly MZSpectrum Empty = new MZSpectrum();

        public override MZPeak GetPeak(int index)
        {
            return new MZPeak(_masses[index], _intensities[index]);
        }

        public override MZSpectrum Extract(double minMZ, double maxMZ)
        {
            if (Count == 0)
                return Empty;

            int index = GetPeakIndex(minMZ);

            int count = Count;
            double[] mz = new double[count];
            double[] intensity = new double[count];
            int j = 0;

            while (index < Count && _masses[index] <= maxMZ)
            {
                mz[j] = _masses[index];
                intensity[j] = _intensities[index];
                index++;
                j++;
            }

            if (j == 0)
                return Empty;

            Array.Resize(ref mz, j);
            Array.Resize(ref intensity, j);
            return new MZSpectrum(mz, intensity, false);
        }

        public override MZSpectrum Clone()
        {
            return new MZSpectrum(this);
        }

        /// <summary>
        /// Extracts a sub spectrum from this spectrum.
        /// Does not modify this spectrum.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public MZSpectrum Extract(IRange<double> range)
        {
            return Extract(range.Minimum, range.Maximum);
        }

        public MZSpectrum Filter(double minIntensity, double maxIntensity = double.MaxValue)
        {
            if (Count == 0)
                return new MZSpectrum();

            int count = Count;
            double[] mz = new double[count];
            double[] intensities = new double[count];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                double intensity = _intensities[i];
                if (intensity >= minIntensity && intensity < maxIntensity)
                {
                    mz[j] = _masses[i];
                    intensities[j] = intensity;
                    j++;
                }
            }

            if (j == 0)
                return new MZSpectrum();

            if (j != count)
            {
                Array.Resize(ref mz, j);
                Array.Resize(ref intensities, j);
            }

            return new MZSpectrum(mz, intensities, false);
        }

        public override MZSpectrum Filter(IEnumerable<IRange<double>> mzRanges)
        {
            if (Count == 0)
                return new MZSpectrum();

            int count = Count;

            // Peaks to remove
            HashSet<int> indiciesToRemove = new HashSet<int>();

            // Loop over each range to remove
            foreach (IRange<double> range in mzRanges)
            {
                double min = range.Minimum;
                double max = range.Maximum;

                int index = Array.BinarySearch(_masses, min);
                if (index < 0)
                    index = ~index;

                while (index < count && _masses[index] <= max)
                {
                    indiciesToRemove.Add(index);
                    index++;
                }
            }

            // The size of the cleaned spectrum
            int cleanCount = count - indiciesToRemove.Count;

            if (cleanCount == 0)
                return new MZSpectrum();

            // Create the storage for the cleaned spectrum
            double[] mz = new double[cleanCount];
            double[] intensities = new double[cleanCount];

            // Transfer peaks from the old spectrum to the new one
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                if (indiciesToRemove.Contains(i))
                    continue;
                mz[j] = _masses[i];
                intensities[j] = _intensities[i];
                j++;
            }

            // Return a new spectrum, don't bother recopying the arrays
            return new MZSpectrum(mz, intensities, false);
        }
        
    }
    
}