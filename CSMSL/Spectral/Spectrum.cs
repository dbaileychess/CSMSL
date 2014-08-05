using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Spectral
{
    /// <summary>
    /// Represents the standard m/z spectrum, with intensity on the y-axis and m/z on the x-axis.
    /// </summary>
    [Serializable]
    public abstract class Spectrum<T, T2> : IEnumerable<T>, ISpectrum<T>
        where T : IPeak
        where T2 : Spectrum<T, T2>
    {
        /// <summary>
        /// The m/z of this spectrum in ascending order
        /// </summary>
        protected readonly double[] _masses;

        /// <summary>
        /// The intensity of this spectrum, linked to their m/z by index in the array
        /// </summary>
        protected readonly double[] _intensities;

        /// <summary>
        /// The number of peaks in this spectrum
        /// </summary>
        public int Count { get; protected set; }

        /// <summary>
        /// The first m/z of this spectrum
        /// </summary>
        public double FirstMz
        {
            get { return Count == 0 ? 0 : _masses[0]; }
        }

        /// <summary>
        /// The last m/z of this spectrum
        /// </summary>
        public double LastMZ
        {
            get { return Count == 0 ? 0 : _masses[Count - 1]; }
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
            if (intensities.Length != Count)
            {
                throw new ArgumentException("M/Z and Intensity arrays are not the same length");
            }

            if (shouldCopy)
            {
                _masses = new double[Count];
                _intensities = new double[Count];
                Buffer.BlockCopy(mz, 0, _masses, 0, sizeof(double) * Count);
                Buffer.BlockCopy(intensities, 0, _intensities, 0, sizeof(double) * Count);
            }
            else
            {
                _masses = mz;
                _intensities = intensities;
            }
        }

        protected Spectrum()
        {
            Count = 0;
            _masses = new double[0];
            _intensities = new double[0];
        }

        /// <summary>
        /// Initializes a new spectrum from another spectrum
        /// </summary>
        /// <param name="mzSpectrum">The spectrum to clone</param>
        protected Spectrum(MZSpectrum mzSpectrum)
            : this(mzSpectrum._masses, mzSpectrum._intensities)
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

            _masses = new double[count];
            _intensities = new double[count];
            Buffer.BlockCopy(mzintensities, 0, _masses, 0, sizeof(double) * count);
            Buffer.BlockCopy(mzintensities, sizeof(double) * length, _intensities, 0, sizeof(double) * count);
            Count = count;
        }

        protected Spectrum(byte[] mzintensities)
        {
            Count = mzintensities.Length / (sizeof(double) * 2);
            int size = sizeof(double) * Count;
            _masses = new double[Count];
            _intensities = new double[Count];
            Buffer.BlockCopy(mzintensities, 0, _masses, 0, size);
            Buffer.BlockCopy(mzintensities, size, _intensities, 0, size);
        }

        /// <summary>
        /// Gets the peak at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return GetPeak(index); }
        }

        /// <summary>
        /// Finds the largest intensity value in this spectrum
        /// </summary>
        /// <returns>The largest intensity value in this spectrum</returns>
        public virtual double GetBasePeakIntensity()
        {
            return Count == 0 ? 0 : _intensities.Max();
        }

        /// <summary>
        /// Gets the full m/z range of this spectrum
        /// </summary>
        /// <returns></returns>
        public virtual MzRange GetMzRange()
        {
            return new MzRange(FirstMz, LastMZ);
        }

        /// <summary>
        /// Gets a copy of the underlying m/z array
        /// </summary>
        /// <returns></returns>
        public virtual double[] GetMasses()
        {
            double[] masses = new double[Count];
            Buffer.BlockCopy(_masses, 0, masses, 0, sizeof(double) * Count);
            return masses;
        }

        /// <summary>
        /// Gets a copy of the underlying intensity array
        /// </summary>
        /// <returns></returns>
        public virtual double[] GetIntensities()
        {
            double[] intensities = new double[Count];
            Buffer.BlockCopy(_intensities, 0, intensities, 0, sizeof(double) * Count);
            return intensities;
        }

        /// <summary>
        /// Converts the spectrum into a multi-dimensional array of doubles
        /// </summary>
        /// <returns></returns>
        public virtual double[,] ToArray()
        {
            double[,] data = new double[2, Count];
            const int size = sizeof(double);
            Buffer.BlockCopy(_masses, 0, data, 0, size * Count);
            Buffer.BlockCopy(_intensities, 0, data, size * Count, size * Count);
            return data;
        }

        /// <summary>
        /// Calculates the total ion current of this spectrum
        /// </summary>
        /// <returns>The total ion current of this spectrum</returns>
        public virtual double GetTotalIonCurrent()
        {
            return Count == 0 ? 0 : _intensities.Sum();
        }

        /// <summary>
        /// Gets the m/z value at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual double GetMass(int index)
        {
            return _masses[index];
        }

        /// <summary>
        /// Gets the intensity value at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual double GetIntensity(int index)
        {
            return _intensities[index];
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

            int index = Array.BinarySearch(_masses, minMZ);
            if (index >= 0)
                return true;

            index = ~index;

            return (index < Count && _masses[index] <= maxMZ);
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

            while (index < Count && _masses[index] <= maxMZ)
            {
                intensity += _intensities[index++];
            }

            return intensity > 0.0;
        }

        public virtual T GetBasePeak()
        {
            int index = _intensities.MaxIndex();
            return GetPeak(index);
        }

        public virtual T GetClosestPeak(IRange<double> massRange)
        {
            double mean = (massRange.Maximum + massRange.Minimum) / 2.0;
            double width = massRange.Maximum - massRange.Minimum;
            return GetClosestPeak(mean, width);
        }

        public virtual T GetClosestPeak(double mean, double tolerance)
        {
            int index = GetClosestPeakIndex(mean, tolerance);

            return index >= 0 ? GetPeak(index) : default(T);
        }

        IPeak ISpectrum.GetClosestPeak(IRange<double> massRange)
        {
            return GetClosestPeak(massRange);
        }

        IPeak ISpectrum.GetClosestPeak(double mean, double tolerance)
        {
            return GetClosestPeak(mean, tolerance);
        }

        public virtual bool TryGetPeaks(IRange<double> rangeMZ, out List<T> peaks)
        {
            return TryGetPeaks(rangeMZ.Minimum, rangeMZ.Maximum, out peaks);
        }

        public virtual bool TryGetPeaks(double minMZ, double maxMZ, out List<T> peaks)
        {
            peaks = new List<T>();

            if (Count == 0)
                return false;

            int index = GetPeakIndex(minMZ);

            while (index < Count && _masses[index] <= maxMZ)
            {
                peaks.Add(GetPeak(index++));
            }

            return peaks.Count > 0;
        }

        public virtual string ToBase64String(bool zlibCompressed = false)
        {
            return Convert.ToBase64String(ToBytes(zlibCompressed));
        }

        #region Abstract

        public abstract T GetPeak(int index);

        public abstract byte[] ToBytes(bool zlibCompressed = false);

        public abstract T2 Extract(double minMZ, double maxMZ);

        #endregion Abstract

        #region Protected Methods

        protected int GetClosestPeakIndex(double mean, double tolerance)
        {
            if (Count == 0)
                return -1;

            int index = Array.BinarySearch(_masses, mean);

            if (index >= 0)
                return index;

            index = ~index;

            int indexm1 = index - 1;

            double minMZ = mean - tolerance;
            double maxMZ = mean + tolerance;
            if (index >= Count)
            {
                // only the indexm1 peak can be closer

                if (indexm1 >= 0 && _masses[indexm1] >= minMZ)
                {
                    return indexm1;
                }

                return -1;
            }
            if (index == 0)
            {
                // only the index can be closer
                if (_masses[index] <= maxMZ)
                {
                    return index;
                }

                return -1;
            }

            double p1 = _masses[indexm1];
            double p2 = _masses[index];

            if (p2 > maxMZ)
            {
                if (p1 >= minMZ)
                    return indexm1;
                return -1;
            }
            if (p1 >= minMZ)
            {
                if (mean - p1 > p2 - mean)
                    return index;
                return indexm1;
            }
            return index;
        }

        protected int GetPeakIndex(double mz)
        {
            int index = Array.BinarySearch(_masses, mz);

            if (index >= 0)
                return index;

            return ~index;
        }

        #endregion Protected Methods

        public override string ToString()
        {
            return string.Format("{0} (Peaks {1})", GetMzRange(), Count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetPeak(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
