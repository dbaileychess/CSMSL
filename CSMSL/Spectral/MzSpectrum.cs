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
            : this(mzSpectrum.Masses, mzSpectrum.Intensities)
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
            : base(mzintensities, count)
        {
        }

        public MZSpectrum(byte[] mzintensities)
        {
            Masses = FromBytes(mzintensities, 0);
            Intensities = FromBytes(mzintensities, 1);
            Count = mzintensities.Length/(sizeof (double)*2);
            int size = sizeof (double)*Count;
            Masses = new double[Count];
            Intensities = new double[Count];
            Buffer.BlockCopy(mzintensities, 0, Masses, 0, size);
            Buffer.BlockCopy(mzintensities, size, Intensities, 0, size);
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
            return new MZPeak(Masses[index], Intensities[index]);
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

            while (index < Count && Masses[index] <= maxMZ)
            {
                mz[j] = Masses[index];
                intensity[j] = Intensities[index];
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

        public override MZSpectrum FilterByIntensity(double minIntensity = 0, double maxIntensity = double.MaxValue)
        {
            if (Count == 0)
                return Empty;

            int count = Count;
            double[] mz = new double[count];
            double[] intensities = new double[count];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                double intensity = Intensities[i];
                if (intensity >= minIntensity && intensity < maxIntensity)
                {
                    mz[j] = Masses[i];
                    intensities[j] = intensity;
                    j++;
                }
            }

            if (j == 0)
                return Empty;

            if (j != count)
            {
                Array.Resize(ref mz, j);
                Array.Resize(ref intensities, j);
            }

            return new MZSpectrum(mz, intensities, false);
        }

        public override MZSpectrum FilterByMZ(IEnumerable<IRange<double>> mzRanges)
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

                int index = Array.BinarySearch(Masses, min);
                if (index < 0)
                    index = ~index;

                while (index < count && Masses[index] <= max)
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
                mz[j] = Masses[i];
                intensities[j] = Intensities[i];
                j++;
            }

            // Return a new spectrum, don't bother recopying the arrays
            return new MZSpectrum(mz, intensities, false);
        }

        public override MZSpectrum FilterByMZ(double minMZ, double maxMZ)
        {
            if (Count == 0)
                return new MZSpectrum();

            int count = Count;

            // Peaks to remove
            HashSet<int> indiciesToRemove = new HashSet<int>();

            int index = Array.BinarySearch(Masses, minMZ);
            if (index < 0)
                index = ~index;

            while (index < count && Masses[index] <= maxMZ)
            {
                indiciesToRemove.Add(index);
                index++;
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
                mz[j] = Masses[i];
                intensities[j] = Intensities[i];
                j++;
            }

            // Return a new spectrum, don't bother recopying the arrays
            return new MZSpectrum(mz, intensities, false);
        }
    }
}