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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CSMSL.Spectral
{
    /// <summary>
    /// Represents the standard m/z spectrum, with intensity on the y-axis and m/z on the x-axis.
    /// </summary>
    [Serializable]
    public abstract class Spectrum<TPeak, TSpectrum> : ISpectrum<TPeak>
        where TPeak : MZPeak
        where TSpectrum : Spectrum<TPeak, TSpectrum>
    {
        /// <summary>
        /// The m/z of this spectrum in ascending order
        /// </summary>
        protected double[] Masses;

        /// <summary>
        /// The intensity of this spectrum, linked to their m/z by index in the array
        /// </summary>
        protected double[] Intensities;

        /// <summary>
        /// The number of peaks in this spectrum
        /// </summary>
        public int Count { get; protected set; }

        /// <summary>
        /// The first m/z of this spectrum
        /// </summary>
        public double FirstMZ
        {
            get { return Count == 0 ? 0 : Masses[0]; }
        }

        /// <summary>
        /// The last m/z of this spectrum
        /// </summary>
        public double LastMZ
        {
            get { return Count == 0 ? 0 : Masses[Count - 1]; }
        }

        /// <summary>
        /// The total ion current of this spectrum
        /// </summary>
        public double TotalIonCurrent
        {
            get { return Count == 0 ? 0 : GetTotalIonCurrent(); }
        }

        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="mz">The m/z's</param>
        /// <param name="intensities">The intensities</param>
        /// <param name="shouldCopy">Indicates whether the input arrays should be copied to new ones</param>
        protected Spectrum(double[] mz, double[] intensities, bool shouldCopy = true)
        {
            Count = mz.Length;
            Masses = CopyData(mz, shouldCopy);
            Intensities = CopyData(intensities, shouldCopy);
        }

        protected Spectrum()
        {
            Count = 0;
        }

        /// <summary>
        /// Initializes a new spectrum from another spectrum
        /// </summary>
        /// <param name="spectrum">The spectrum to clone</param>
        protected Spectrum(ISpectrum spectrum)
            : this(spectrum.GetMasses(), spectrum.GetIntensities())
        {
        }

        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="mzintensities"></param>
        protected Spectrum(double[,] mzintensities)
            : this(mzintensities, mzintensities.GetLength(1))
        {
        }

        protected Spectrum(double[,] mzintensities, int count)
        {
            int length = mzintensities.GetLength(1);

            Masses = new double[count];
            Intensities = new double[count];
            Buffer.BlockCopy(mzintensities, 0, Masses, 0, sizeof (double)*count);
            Buffer.BlockCopy(mzintensities, sizeof (double)*length, Intensities, 0, sizeof (double)*count);
            Count = count;
        }

        protected Spectrum(byte[] mzintensities)
        {
            Count = mzintensities.Length/(sizeof (double)*2);
            int size = sizeof (double)*Count;
            Masses = new double[Count];
            Intensities = new double[Count];
            Buffer.BlockCopy(mzintensities, 0, Masses, 0, size);
            Buffer.BlockCopy(mzintensities, size, Intensities, 0, size);
        }

        /// <summary>
        /// Gets the peak at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TPeak this[int index]
        {
            get { return GetPeak(index); }
        }

        /// <summary>
        /// Finds the largest intensity value in this spectrum
        /// </summary>
        /// <returns>The largest intensity value in this spectrum</returns>
        public virtual double GetBasePeakIntensity()
        {
            return Count == 0 ? 0 : Intensities.Max();
        }

        /// <summary>
        /// Gets the full m/z range of this spectrum
        /// </summary>
        /// <returns></returns>
        public virtual MzRange GetMzRange()
        {
            return new MzRange(FirstMZ, LastMZ);
        }

        /// <summary>
        /// Gets a copy of the underlying m/z array
        /// </summary>
        /// <returns></returns>
        public virtual double[] GetMasses()
        {
            return CopyData(Masses);
        }

        /// <summary>
        /// Gets a copy of the underlying intensity array
        /// </summary>
        /// <returns></returns>
        public virtual double[] GetIntensities()
        {
            return CopyData(Intensities);
        }

        /// <summary>
        /// Converts the spectrum into a multi-dimensional array of doubles
        /// </summary>
        /// <returns></returns>
        public virtual double[,] ToArray()
        {
            double[,] data = new double[2, Count];
            const int size = sizeof (double);
            Buffer.BlockCopy(Masses, 0, data, 0, size*Count);
            Buffer.BlockCopy(Intensities, 0, data, size*Count, size*Count);
            return data;
        }

        /// <summary>
        /// Calculates the total ion current of this spectrum
        /// </summary>
        /// <returns>The total ion current of this spectrum</returns>
        public virtual double GetTotalIonCurrent()
        {
            return Count == 0 ? 0 : Intensities.Sum();
        }

        /// <summary>
        /// Gets the m/z value at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual double GetMass(int index)
        {
            return Masses[index];
        }

        /// <summary>
        /// Gets the intensity value at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual double GetIntensity(int index)
        {
            return Intensities[index];
        }

        /// <summary>
        /// Checks if this spectrum contains any peaks
        /// </summary>
        /// <returns></returns>
        public virtual bool ContainsPeak()
        {
            return Count > 0;
        }

        /// <summary>
        /// Checks if this spectrum contains any peaks within the range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool ContainsPeak(IRange<double> range)
        {
            return ContainsPeak(range.Minimum, range.Maximum);
        }

        /// <summary>
        /// Checks if this spectrum contains any peaks within the range
        /// </summary>
        /// <param name="minMZ">The minimum m/z (inclusive)</param>
        /// <param name="maxMZ">The maximum m/z (inclusive)</param>
        /// <returns></returns>
        public virtual bool ContainsPeak(double minMZ, double maxMZ)
        {
            if (Count == 0)
                return false;

            int index = Array.BinarySearch(Masses, minMZ);
            if (index >= 0)
                return true;

            index = ~index;

            return index < Count && Masses[index] <= maxMZ;
        }

        public virtual bool TryGetIntensities(IRange<double> rangeMZ, out double intensity)
        {
            return TryGetIntensities(rangeMZ.Minimum, rangeMZ.Maximum, out intensity);
        }

        public virtual bool TryGetIntensities(double minMZ, double maxMZ, out double intensity)
        {
            intensity = 0;

            if (Count == 0)
                return false;

            int index = GetPeakIndex(minMZ);

            while (index < Count && Masses[index] <= maxMZ)
                intensity += Intensities[index++];

            return intensity > 0.0;
        }

        public virtual TPeak GetBasePeak()
        {
            return GetPeak(Intensities.MaxIndex());
        }

        public virtual TPeak GetClosestPeak(IRange<double> massRange)
        {
            double mean = (massRange.Maximum + massRange.Minimum)/2.0;
            double width = massRange.Maximum - massRange.Minimum;
            return GetClosestPeak(mean, width);
        }

        public virtual TPeak GetClosestPeak(double mean, double tolerance)
        {
            int index = GetClosestPeakIndex(mean, tolerance);

            return index >= 0 ? GetPeak(index) : default(TPeak);
        }

        public virtual bool TryGetPeaks(IRange<double> rangeMZ, out List<TPeak> peaks)
        {
            return TryGetPeaks(rangeMZ.Minimum, rangeMZ.Maximum, out peaks);
        }

        public virtual bool TryGetPeaks(double minMZ, double maxMZ, out List<TPeak> peaks)
        {
            peaks = new List<TPeak>();

            if (Count == 0)
                return false;

            int index = GetPeakIndex(minMZ);

            while (index < Count && Masses[index] <= maxMZ)
            {
                peaks.Add(GetPeak(index++));
            }

            return peaks.Count > 0;
        }

        public virtual string ToBase64String(bool zlibCompressed = false)
        {
            return Convert.ToBase64String(ToBytes(zlibCompressed));
        }

        public virtual byte[] ToBytes(bool zlibCompressed = false)
        {
            return ToBytes(zlibCompressed, Masses, Intensities);
        }

        /// <summary>
        /// Creates a clone of this spectrum with each mass transformed by some function
        /// </summary>
        /// <param name="convertor">The function to convert each mass by</param>
        /// <returns>A cloned spectrum with masses corrected</returns>
        public virtual TSpectrum CorrectMasses(Func<double, double> convertor)
        {
            TSpectrum newSpectrum = Clone();
            for (int i = 0; i < newSpectrum.Count; i++)
                newSpectrum.Masses[i] = convertor(newSpectrum.Masses[i]);
            return newSpectrum;
        }

        public virtual TSpectrum Extract(IRange<double> mzRange)
        {
            return Extract(mzRange.Minimum, mzRange.Maximum);
        }

        public virtual TSpectrum FilterByMZ(IRange<double> mzRange)
        {
            return FilterByMZ(mzRange.Minimum, mzRange.Maximum);
        }

        public virtual TSpectrum FilterByIntensity(IRange<double> intensityRange)
        {
            return FilterByIntensity(intensityRange.Minimum, intensityRange.Maximum);
        }

        #region ISpectrum

        MZPeak ISpectrum.GetClosestPeak(IRange<double> massRange)
        {
            return GetClosestPeak(massRange);
        }

        MZPeak ISpectrum.GetClosestPeak(double mean, double tolerance)
        {
            return GetClosestPeak(mean, tolerance);
        }

        ISpectrum ISpectrum.Extract(double minMZ, double maxMZ)
        {
            return Extract(minMZ, maxMZ);
        }

        ISpectrum ISpectrum.Extract(IRange<double> mzRange)
        {
            return Extract(mzRange.Minimum, mzRange.Maximum);
        }

        ISpectrum ISpectrum.FilterByMZ(IEnumerable<IRange<double>> mzRanges)
        {
            return FilterByMZ(mzRanges);
        }

        ISpectrum ISpectrum.FilterByMZ(IRange<double> mzRange)
        {
            return FilterByMZ(mzRange.Minimum, mzRange.Maximum);
        }

        ISpectrum ISpectrum.FilterByMZ(double minMZ, double maxMZ)
        {
            return FilterByMZ(minMZ, maxMZ);
        }

        ISpectrum ISpectrum.FilterByIntensity(double minIntensity, double maxIntensity)
        {
            return FilterByIntensity(minIntensity, maxIntensity);
        }

        ISpectrum ISpectrum.FilterByIntensity(IRange<double> intenistyRange)
        {
            return FilterByIntensity(intenistyRange.Minimum, intenistyRange.Maximum);
        }

        #endregion

        #region Abstract

        public abstract TPeak GetPeak(int index);

        public abstract TSpectrum Extract(double minMZ, double maxMZ);
        public abstract TSpectrum FilterByMZ(IEnumerable<IRange<double>> mzRanges);
        public abstract TSpectrum FilterByMZ(double minMZ, double maxMZ);
        public abstract TSpectrum FilterByIntensity(double minIntensity = 0, double maxIntensity = double.MaxValue);

        /// <summary>
        /// Returns a new deep clone of this spectrum.
        /// </summary>
        /// <returns></returns>
        public abstract TSpectrum Clone();

        #endregion Abstract

        #region Protected Methods

        protected double[] FromBytes(byte[] data, int index)
        {
            if (data.IsCompressed())
                data = data.Decompress();
            Count = data.Length/(sizeof (double)*2);
            int size = sizeof (double)*Count;
            double[] outArray = new double[Count];
            Buffer.BlockCopy(data, index*size, outArray, 0, size);
            return outArray;
        }

        protected byte[] ToBytes(bool zlibCompressed, params double[][] arrays)
        {
            int length = Count*sizeof (double);
            int arrayCount = arrays.Length;
            byte[] bytes = new byte[length*arrayCount];
            int i = 0;
            foreach (double[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, bytes, length*i++, length);
            }

            if (zlibCompressed)
            {
                bytes = bytes.Compress();
            }

            return bytes;
        }

        /// <summary>
        /// Copies the source array to the destination array
        /// </summary>
        /// <typeparam name="TArray"></typeparam>
        /// <param name="sourceArray">The source array to copy from</param>
        /// <param name="deepCopy">If true, a new array will be generate, else references are copied</param>
        protected TArray[] CopyData<TArray>(TArray[] sourceArray, bool deepCopy = true) where TArray : struct
        {
            if (sourceArray == null)
                return null;
            if (sourceArray.Length != Count)
                throw new ArgumentException("Mismatched array size");
            if (!deepCopy)
            {
                return sourceArray;
            }
            int count = sourceArray.Length;
            TArray[] dstArray = new TArray[Count];
            Type type = typeof (TArray);
            Buffer.BlockCopy(sourceArray, 0, dstArray, 0, count*Marshal.SizeOf(type));
            return dstArray;
        }

        protected int GetClosestPeakIndex(double meanMZ, double tolerance)
        {
            if (Count == 0)
                return -1;

            int index = Array.BinarySearch(Masses, meanMZ);

            if (index >= 0)
                return index;

            index = ~index;

            int indexm1 = index - 1;

            double minMZ = meanMZ - tolerance;
            double maxMZ = meanMZ + tolerance;
            if (index >= Count)
            {
                // only the indexm1 peak can be closer

                if (indexm1 >= 0 && Masses[indexm1] >= minMZ)
                {
                    return indexm1;
                }

                return -1;
            }
            if (index == 0)
            {
                // only the index can be closer
                if (Masses[index] <= maxMZ)
                {
                    return index;
                }

                return -1;
            }

            double p1 = Masses[indexm1];
            double p2 = Masses[index];

            if (p2 > maxMZ)
            {
                if (p1 >= minMZ)
                    return indexm1;
                return -1;
            }
            if (p1 >= minMZ)
            {
                if (meanMZ - p1 > p2 - meanMZ)
                    return index;
                return indexm1;
            }
            return index;
        }

        protected int GetPeakIndex(double mz)
        {
            int index = Array.BinarySearch(Masses, mz);

            if (index >= 0)
                return index;

            return ~index;
        }

        #endregion Protected Methods

        public override string ToString()
        {
            return string.Format("{0} (Peaks {1})", GetMzRange(), Count);
        }

        public IEnumerator<TPeak> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetPeak(i);
            }
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.Extract(IRange<double> mzRange)
        {
            return Extract(mzRange);
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.Extract(double minMZ, double maxMZ)
        {
            return Extract(minMZ, maxMZ);
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.FilterByMZ(IEnumerable<IRange<double>> mzRanges)
        {
            return FilterByMZ(mzRanges);
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.FilterByMZ(IRange<double> mzRange)
        {
            return FilterByMZ(mzRange);
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.FilterByMZ(double minMZ, double maxMZ)
        {
            return FilterByMZ(minMZ, maxMZ);
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.FilterByIntensity(double minIntensity, double maxIntensity)
        {
            return FilterByIntensity(minIntensity, maxIntensity);
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.FilterByIntensity(IRange<double> intenistyRange)
        {
            return FilterByIntensity(intenistyRange);
        }

        MZPeak ISpectrum.GetPeak(int index)
        {
            return GetPeak(index);
        }

        IEnumerator<MZPeak> IEnumerable<MZPeak>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}