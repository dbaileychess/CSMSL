// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ThermoSpectrum.cs) is part of CSMSL.
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
using CSMSL.Spectral;
using System.Collections.Generic;

namespace CSMSL.IO.Thermo
{
    /// <summary>
    /// A high resolution spectra from a Thermo raw file
    /// </summary>
    [Serializable]
    public sealed class ThermoSpectrum : Spectrum<ThermoMzPeak, ThermoSpectrum>
    {
        /// <summary>
        /// An empty spectrum
        /// </summary>
        public static readonly ThermoSpectrum Empty = new ThermoSpectrum();

        private readonly double[] _noises;
        private readonly double[] _resolutions;
        private readonly int[] _charges;
        
        public bool IsHighResolution { get { return _charges != null; } }

        private ThermoSpectrum() {}

        internal ThermoSpectrum(double[,] peakData, int count)
            : base(peakData, count)
        {
            int arrayLength = peakData.GetLength(1);
            int depthLength = peakData.GetLength(0);
            if (depthLength <= 2) 
                return;
            _noises = new double[Count];
            _resolutions = new double[Count];
            var charges = new double[Count];

            Buffer.BlockCopy(peakData, sizeof (double)*arrayLength*(int) ThermoRawFile.RawLabelDataColumn.Resolution, _resolutions, 0, sizeof (double)*Count);
            Buffer.BlockCopy(peakData, sizeof (double)*arrayLength*(int) ThermoRawFile.RawLabelDataColumn.NoiseLevel, _noises, 0, sizeof (double)*Count);
            Buffer.BlockCopy(peakData, sizeof (double)*arrayLength*(int) ThermoRawFile.RawLabelDataColumn.Charge, charges, 0, sizeof (double)*Count);

            _charges = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                _charges[i] = (int) charges[i];
            }
        }

        public ThermoSpectrum(byte[] mzintensities)
        {
            Count = mzintensities.Length/(sizeof (double)*2);
            int size = sizeof (double)*Count;
            Masses = new double[Count];
            Intensities = new double[Count];
            Buffer.BlockCopy(mzintensities, 0, Masses, 0, size);
            Buffer.BlockCopy(mzintensities, size, Intensities, 0, size);
        }

        internal ThermoSpectrum(double[,] peakData)
            : this(peakData, peakData.GetLength(1))
        {
        }

        public ThermoSpectrum(double[] mz, double[] intensity, double[] noise, int[] charge, double[] resolutions, bool shouldCopy = true)
            : base(mz, intensity, shouldCopy)
        {
            _noises = CopyData(noise, shouldCopy);
            _resolutions = CopyData(resolutions, shouldCopy);
            _charges = CopyData(charge, shouldCopy);
        }
        
        public ThermoSpectrum(ThermoSpectrum thermoSpectrum)
            :this(thermoSpectrum.Masses, thermoSpectrum.Intensities,thermoSpectrum._noises, thermoSpectrum._charges,thermoSpectrum._resolutions)
        {
            
        }

        public double GetNoise(int index)
        {
            return IsHighResolution ? _noises[index] : 0;
        }

        public double GetSignalToNoise(int index)
        {
            if (!IsHighResolution)
                return 0;
            double noise = _noises[index];
            if (Math.Abs(noise) < 1e-25)
                return 0;
            return Intensities[index]/noise;
        }

        public int GetCharge(int index)
        {
            return IsHighResolution ? _charges[index] : 0;
        }

        public double GetResolution(int index)
        {
            return IsHighResolution ? _resolutions[index] : 0;
        }

        public double[] GetNoises()
        {
            return CopyData(_noises);
        }

        public double[] GetResolutions()
        {
            return CopyData(_resolutions);
        }

        public int[] GetCharges()
        {
            return CopyData(_charges);
        }

        public override ThermoMzPeak GetPeak(int index)
        {
            return IsHighResolution ? new ThermoMzPeak(Masses[index], Intensities[index], _charges[index], _noises[index], _resolutions[index]) 
                : new ThermoMzPeak(Masses[index], Intensities[index]);
        }

        public override byte[] ToBytes(bool zlibCompressed = false)
        {
            if (IsHighResolution)
            {
                double[] charges = new double[Count];
                for (int i = 0; i < Count; i++)
                    charges[i] = _charges[i];
                return ToBytes(zlibCompressed, Masses, Intensities, _resolutions, _noises, charges);
            }
            return base.ToBytes(zlibCompressed);
        }

        public override double[,] ToArray()
        {
            double[,] data = new double[5, Count];
            const int size = sizeof (double);
            int bytesToCopy = size*Count;
            Buffer.BlockCopy(Masses, 0, data, 0, bytesToCopy);
            Buffer.BlockCopy(Intensities, 0, data, bytesToCopy, bytesToCopy);
            Buffer.BlockCopy(_resolutions, 0, data, (int) ThermoRawFile.RawLabelDataColumn.Resolution*bytesToCopy, bytesToCopy);
            Buffer.BlockCopy(_noises, 0, data, (int) ThermoRawFile.RawLabelDataColumn.NoiseLevel*bytesToCopy, bytesToCopy);

            double[] charges = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                charges[i] = _charges[i];
            }
            Buffer.BlockCopy(charges, 0, data, (int) ThermoRawFile.RawLabelDataColumn.Charge*bytesToCopy, bytesToCopy);
            return data;
        }
        
        public override ThermoSpectrum Extract(double minMZ, double maxMZ)
        {
            if (Count == 0)
                return Empty;

            int index = GetPeakIndex(minMZ);
            int index2 = GetPeakIndex(maxMZ);

            int count = 1 + index2 - index;
            double[] mz = new double[count];
            double[] intensity = new double[count];
            int[] charges = new int[count];
            double[] noises = new double[count];
            double[] resolutions = new double[count];
            int j = 0;
            
            while (index < Count && Masses[index] <= maxMZ)
            {
                mz[j] = Masses[index];
                intensity[j] = Intensities[index];
                charges[j] = _charges[index];
                noises[j] = _noises[index];
                resolutions[j] = _resolutions[index];
                index++;
                j++;
            }

            if (j == 0)
                return Empty;

            Array.Resize(ref mz, j);
            Array.Resize(ref intensity, j);
            Array.Resize(ref charges, j);
            Array.Resize(ref noises, j);
            Array.Resize(ref resolutions, j);
            return new ThermoSpectrum(mz, intensity, noises, charges, resolutions, false);
        }

        public MZSpectrum ToMZSpectrum()
        {
            return new MZSpectrum(Masses, Intensities);
        }

        public override ThermoSpectrum Clone()
        {
            return new ThermoSpectrum(this);
        }

        public bool TryGetSignalToNoise(IRange<double> rangeMZ, out double signalToNoise)
        {
            return TryGetSignalToNoise(rangeMZ.Minimum, rangeMZ.Maximum, out signalToNoise);
        }

        public bool TryGetSignalToNoise(double minMZ, double maxMZ, out double signalToNoise)
        {
            signalToNoise = 0;

            if (Count == 0)
                return false;

            int index = GetPeakIndex(minMZ);

            while (index < Count && Masses[index] <= maxMZ)
                signalToNoise += GetSignalToNoise(index++);

            return signalToNoise > 0.0;
        }
        
        public override ThermoSpectrum FilterByIntensity(double minIntensity = 0, double maxIntensity = double.MaxValue)
        {
            if (Count == 0)
                return new ThermoSpectrum();

            int count = Count;
            double[] mz = new double[count];
            double[] intensities = new double[count];
            double[] resolutions = new double[count];
            double[] noises = new double[count];
            int[] charges = new int[count];

            int j = 0;
            for (int i = 0; i < count; i++)
            {
                double intensity = Intensities[i];
                if (intensity >= minIntensity && intensity < maxIntensity)
                {
                    mz[j] = Masses[i];
                    intensities[j] = intensity;
                    resolutions[j] = _resolutions[i];
                    charges[j] = _charges[i];
                    noises[j] = _noises[i];
                    j++;
                }
            }

            if (j == 0)
                return new ThermoSpectrum();

            if (j != count)
            {
                Array.Resize(ref mz, j);
                Array.Resize(ref intensities, j);
                Array.Resize(ref resolutions, j);
                Array.Resize(ref noises, j);
                Array.Resize(ref charges, j);
            }

            return new ThermoSpectrum(mz, intensities, noises, charges, resolutions, false);
        }


        public override ThermoSpectrum FilterByMZ(IEnumerable<IRange<double>> mzRanges)
        {
            throw new NotImplementedException();
            //if (Count == 0)
            //    return new ThermoSpectrum();

            //int count = Count;

            //// Peaks to remove
            //HashSet<int> indiciesToRemove = new HashSet<int>();

            //// Loop over each range to remove
            //foreach (IRange<double> range in mzRanges)
            //{
            //    double min = range.Minimum;
            //    double max = range.Maximum;

            //    int index = Array.BinarySearch(_masses, min);
            //    if (index < 0)
            //        index = ~index;

            //    while (index < count && _masses[index] <= max)
            //    {
            //        indiciesToRemove.Add(index);
            //        index++;
            //    }
            //}

            //// The size of the cleaned spectrum
            //int cleanCount = count - indiciesToRemove.Count;

            //if (cleanCount == 0)
            //    return new ThermoSpectrum();

            //// Create the storage for the cleaned spectrum
            //double[] mz = new double[cleanCount];
            //double[] intensities = new double[cleanCount];

            //// Transfer peaks from the old spectrum to the new one
            //int j = 0;
            //for (int i = 0; i < count; i++)
            //{
            //    if (indiciesToRemove.Contains(i))
            //        continue;
            //    mz[j] = _masses[i];
            //    intensities[j] = _intensities[i];
            //    j++;
            //}

            //// Return a new spectrum, don't bother recopying the arrays
            //return new ThermoSpectrum(mz, intensities, false);
        }

        public override ThermoSpectrum FilterByMZ(double minMZ, double maxMZ)
        {
            throw new NotImplementedException();
            //if (Count == 0)
            //    return Empty;

            //int count = Count;

            //// Peaks to remove
            //HashSet<int> indiciesToRemove = new HashSet<int>();

            //int index = GetPeakIndex(minMZ);
          
            //while (index < count && Masses[index] <= maxMZ)
            //{
            //    indiciesToRemove.Add(index);
            //    index++;
            //}


            //// The size of the cleaned spectrum
            //int cleanCount = count - indiciesToRemove.Count;

            //if (cleanCount == 0)
            //    return Empty;

            //// Create the storage for the cleaned spectrum
            //double[] mz = new double[cleanCount];
            //double[] intensities = new double[cleanCount];

            //// Transfer peaks from the old spectrum to the new one
            //int j = 0;
            //for (int i = 0; i < count; i++)
            //{
            //    if (indiciesToRemove.Contains(i))
            //        continue;
            //    mz[j] = Masses[i];
            //    intensities[j] = Intensities[i];
            //    j++;
            //}

            //// Return a new spectrum, don't bother recopying the arrays
            //return new ThermoSpectrum(mz, intensities);
        }
    }
}