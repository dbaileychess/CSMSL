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
    public abstract class Spectrum<TPeak, TSpectrum> : IEnumerable<TPeak>, ISpectrum<TPeak>, ISpectrum
        where TPeak : IPeak
        where TSpectrum : Spectrum<TPeak, TSpectrum>, ISpectrum
    {
        /// <summary>
        /// The m/z of this spectrum in ascending order
        /// </summary>
        protected double[] _masses;

        /// <summary>
        /// The intensity of this spectrum, linked to their m/z by index in the array
        /// </summary>
        protected double[] _intensities;

        /// <summary>
        /// The number of peaks in this spectrum
        /// </summary>
        public int Count { get; protected set; }

        /// <summary>
        /// The first m/z of this spectrum
        /// </summary>
        public double FirstMZ
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
            if (intensities.Length != Count)
            {
                throw new ArgumentException("M/Z and Intensity arrays are not the same length");
            }

            _masses = CopyData(mz, shouldCopy);
            _intensities = CopyData(intensities, shouldCopy);
        }

        protected Spectrum()
        {
            Count = 0;
            //_masses = new double[0];
            //_intensities = new double[0];
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
            return Count == 0 ? 0 : _intensities.Max();
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
            return CopyData(_masses);
        }

        /// <summary>
        /// Gets a copy of the underlying intensity array
        /// </summary>
        /// <returns></returns>
        public virtual double[] GetIntensities()
        {
            return CopyData(_intensities);
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

            return index < Count && _masses[index] <= maxMZ;
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
                intensity += _intensities[index++];

            return intensity > 0.0;
        }

        public virtual TPeak GetBasePeak()
        {
            return GetPeak(_intensities.MaxIndex());
        }

        public virtual TPeak GetClosestPeak(IRange<double> massRange)
        {
            double mean = (massRange.Maximum + massRange.Minimum) / 2.0;
            double width = massRange.Maximum - massRange.Minimum;
            return GetClosestPeak(mean, width);
        }

        public virtual TPeak GetClosestPeak(double mean, double tolerance)
        {
            int index = GetClosestPeakIndex(mean, tolerance);

            return index >= 0 ? GetPeak(index) : default(TPeak);
        }

        IPeak ISpectrum.GetClosestPeak(IRange<double> massRange)
        {
            return GetClosestPeak(massRange);
        }

        IPeak ISpectrum.GetClosestPeak(double mean, double tolerance)
        {
            return GetClosestPeak(mean, tolerance);
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

        public virtual byte[] ToBytes(bool zlibCompressed = false)
        {
            return ToBytes(zlibCompressed, _masses, _intensities);
        }

        /// <summary>
        /// Creates a clone of this spectrum with each mass transformed by some function
        /// </summary>
        /// <param name="convertor">The function to convert each mass by</param>
        /// <returns>A cloned spectrum with masses corrected</returns>
        public virtual TSpectrum CorrectMasses(Func<double,double> convertor)
        {
            TSpectrum newSpectrum = Clone();
            for (int i = 0; i < newSpectrum.Count; i++)
                newSpectrum._masses[i] = convertor(newSpectrum._masses[i]);
            return newSpectrum;
        }

        ISpectrum ISpectrum.Extract(double minMZ, double maxMZ)
        {
            return Extract(minMZ, maxMZ);
        }

        ISpectrum ISpectrum.Filter(IEnumerable<IRange<double>> mzRanges)
        {
            return Filter(mzRanges);
        }

        #region Abstract

        public abstract TPeak GetPeak(int index);
        
        public abstract TSpectrum Extract(double minMZ, double maxMZ);

        //public abstract T2 Filter(double minIntensity, double maxIntensity);

        public abstract TSpectrum Filter(IEnumerable<IRange<double>> mzRanges);

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
            Count = data.Length / (sizeof(double) * 2);
            int size = sizeof(double) * Count;
            double[] outArray = new double[Count];
            Buffer.BlockCopy(data, index*size, outArray, 0, size);
            return outArray;
        }

        protected byte[] ToBytes(bool zlibCompressed, params double[][] arrays)
        {
            int length = Count * sizeof(double);
            int arrayCount = arrays.Length;
            byte[] bytes = new byte[length * arrayCount];
            int i = 0;
            foreach (double[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, bytes, length * i++, length);
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
            if (!deepCopy)
            {
                return sourceArray;
            }
            int count = sourceArray.Length;
            TArray[] dstArray = new TArray[Count];
            Type type = typeof(TArray);
            Buffer.BlockCopy(sourceArray, 0, dstArray, 0, count * Marshal.SizeOf(type));
            return dstArray;
        }

        protected int GetClosestPeakIndex(double meanMZ, double tolerance)
        {
            if (Count == 0)
                return -1;

            int index = Array.BinarySearch(_masses, meanMZ);

            if (index >= 0)
                return index;

            index = ~index;

            int indexm1 = index - 1;

            double minMZ = meanMZ - tolerance;
            double maxMZ = meanMZ + tolerance;
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
                if (meanMZ - p1 > p2 - meanMZ)
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

        public IEnumerator<TPeak> GetEnumerator()
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
