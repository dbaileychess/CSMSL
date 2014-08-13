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
    public sealed class ThermoSpectrum : Spectrum<ThermoLabeledPeak, ThermoSpectrum>
    {
        /// <summary>
        /// An empty spectrum
        /// </summary>
        public static readonly ThermoSpectrum Empty = new ThermoSpectrum();

        private readonly double[] _noises;
        private readonly double[] _resolutions;
        private readonly int[] _charges;

        private ThermoSpectrum()
        {
            _noises = new double[0];
            _resolutions = new double[0];
            _charges = new int[0];
        }

        internal ThermoSpectrum(double[,] peakData, int count)
            : base(peakData, count)
        {
            int arrayLength = peakData.GetLength(1);
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
            :this(thermoSpectrum._masses, thermoSpectrum._intensities,thermoSpectrum._noises,
            thermoSpectrum._charges,thermoSpectrum._resolutions)
        {
            
        }

        public double GetNoise(int index)
        {
            return _noises[index];
        }

        public double GetSignalToNoise(int index)
        {
            double noise = _noises[index];
            if (noise == 0)
                return 0;
            return _intensities[index]/noise;
        }

        public int GetCharge(int index)
        {
            return _charges[index];
        }

        public double GetResolution(int index)
        {
            return _resolutions[index];
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

        public override ThermoLabeledPeak GetPeak(int index)
        {
            return new ThermoLabeledPeak(_masses[index], _intensities[index], _charges[index], _noises[index], _resolutions[index]);
        }

        public override byte[] ToBytes(bool zlibCompressed = false)
        {
            double[] charges = new double[Count];
            for (int i = 0; i < Count; i++)
                charges[i] = _charges[i];
            return ToBytes(zlibCompressed, _masses, _intensities, _resolutions, _noises, charges);
        }

        public override double[,] ToArray()
        {
            double[,] data = new double[5, Count];
            const int size = sizeof (double);
            int bytesToCopy = size*Count;
            Buffer.BlockCopy(_masses, 0, data, 0, bytesToCopy);
            Buffer.BlockCopy(_intensities, 0, data, bytesToCopy, bytesToCopy);
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

            int index = Array.BinarySearch(_masses, minMZ);
            if (index < 0)
                index = ~index;

            int count = Count;
            double[] mz = new double[count];
            double[] intensity = new double[count];
            int[] charges = new int[count];
            double[] noises = new double[count];
            double[] resolutions = new double[count];
            int j = 0;

            while (index < Count && _masses[index] <= maxMZ)
            {
                mz[j] = _masses[index];
                intensity[j] = _intensities[index];
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

        public override ThermoSpectrum Clone()
        {
            return new ThermoSpectrum(this);
        }

        public override ThermoSpectrum Filter(IEnumerable<IRange<double>> mzRanges)
        {
            throw new NotImplementedException();
        }
    }
}