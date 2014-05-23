using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Spectral
{
    /// <summary>
    /// Represents the standard m/z spectrum, with intensity on the y-axis and m/z on the x-axis.
    /// </summary>
    public class Spectrum : ISpectrum
    {
        /// <summary>
        /// An empty spectrum
        /// </summary>
        public static readonly Spectrum Empty = new Spectrum();

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
        public int Count { get; private set; }

        /// <summary>
        /// The first m/z of this spectrum
        /// </summary>
        public double FirstMz { get { return Count == 0 ? 0 : _masses[0]; } }

        /// <summary>
        /// The last m/z of this spectrum
        /// </summary>
        public double LastMZ { get { return Count == 0 ? 0 : _masses[Count - 1]; } }

     

        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="mz">The m/z's</param>
        /// <param name="intensities">The intensities</param>
        /// <param name="shouldCopy">Indicates whether the input arrays should be copied to new ones</param>
        public Spectrum(double[] mz, double[] intensities, bool shouldCopy = true)
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

        private Spectrum()
        {
            Count = 0;
            _masses = new double[0];
            _intensities = new double[0];
        }

        /// <summary>
        /// Initializes a new spectrum from another spectrum
        /// </summary>
        /// <param name="spectrum">The spectrum to clone</param>
        public Spectrum(Spectrum spectrum)
            : this(spectrum._masses, spectrum._intensities)
        {

        }

        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="mzintensities"></param>
        public Spectrum(double[,] mzintensities, int size = -1)
        {
            int length = mzintensities.GetLength(1);

            if (size <= 0)
                size = mzintensities.GetLength(1);
            
            _masses = new double[size];
            _intensities = new double[size];
            Buffer.BlockCopy(mzintensities, 0, _masses, 0, sizeof(double) * size);
            Buffer.BlockCopy(mzintensities, sizeof(double) * length, _intensities, 0, sizeof(double) * size);
            Count = size;
        }

        private Spectrum(byte[] mzintensities)
        {
            Count = mzintensities.Length / (sizeof(double) * 2);
            int size = sizeof(double) * Count;
            _masses = new double[Count];
            _intensities = new double[Count];
            Buffer.BlockCopy(mzintensities, 0, _masses, 0, size);
            Buffer.BlockCopy(mzintensities, size, _intensities, 0, size);     
        }

        /// <summary>
        /// Finds the largest intensity value in this spectrum
        /// </summary>
        /// <returns>The largest intensity value in this spectrum</returns>
        public double GetBasePeakIntensity()
        {
            return Count == 0 ? 0 : _intensities.Max();
        }

        public MzRange GetMzRange() 
        { 
            return new MzRange(FirstMz, LastMZ); 
        } 

        public virtual MZPeak GetBasePeak()
        {
            int index = _intensities.MaxIndex();
            return GetPeak(index);
        }
        
        public double[] GetMasses()
        {
            double[] masses = new double[Count];
            Buffer.BlockCopy(_masses, 0, masses, 0, sizeof(double) * Count);
            return masses;
        }

        public double[] GetIntensities()
        {
            double[] intensities = new double[Count];
            Buffer.BlockCopy(_intensities, 0, intensities, 0, sizeof(double) * Count);
            return intensities;
        }

        /// <summary>
        /// Calculates the total ion current of this spectrum
        /// </summary>
        /// <returns>The total ion current of ths spectrum</returns>
        public double GetTotalIonCurrent()
        {
            return Count == 0 ? 0 : _intensities.Sum();
        }

        /// <summary>
        /// Gets the m/z value at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMass(int index)
        {
            return _masses[index];
        }

        /// <summary>
        /// Gets the intensity value at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetIntensity(int index)
        {
            return _intensities[index];
        }

        public virtual MZPeak GetPeak(int index)
        {
            return new MZPeak(_masses[index], _intensities[index]);
        }

        public MZPeak GetClosestPeak(IRange<double> massRange)
        {
            double mean = (massRange.Maximum + massRange.Minimum) / 2.0;
            double width = massRange.Maximum - massRange.Minimum;
            return GetClosestPeak(mean, width);
        }

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

        public MZPeak GetClosestPeak(double mean, double tolerance)
        {
            int index = GetClosestPeakIndex(mean, tolerance);

            return index >= 0 ? GetPeak(index) : null;
        }

        public bool ContainsPeak(IRange<double> range)
        {
            return ContainsPeak(range.Minimum, range.Maximum);
        }

        public bool ContainsPeak()
        {
            return Count > 0;
        }

        public bool ContainsPeak(double minMZ, double maxMZ)
        {
            if (Count == 0)
                return false;

            int index = Array.BinarySearch(_masses, minMZ);
            if (index >= 0)
                return true;

            index = ~index;         

            return (index < Count && _masses[index] <= maxMZ);
        }

        public bool TryGetIntensities(IRange<double> rangeMZ, out double intensity)
        {
            return TryGetIntensities(rangeMZ.Minimum, rangeMZ.Maximum, out intensity);
        }

        public bool TryGetIntensities(double minMZ, double maxMZ, out double intensity)
        {
            intensity = 0;

            if (Count == 0)
                return false;

            int index = Array.BinarySearch(_masses, minMZ);
            if (index < 0)
                index = ~index;            
            
            while (index < Count && _masses[index] <= maxMZ)
            {
                intensity += _intensities[index++];              
            }

            return intensity > 0.0;
        }

        public bool TryGetPeaks(IRange<double> rangeMZ, out List<MZPeak> peaks)
        {
            return TryGetPeaks(rangeMZ.Minimum, rangeMZ.Maximum, out peaks);
        }

        public bool TryGetPeaks(double minMZ, double maxMZ, out List<MZPeak> peaks)
        {
            peaks = new List<MZPeak>();

            if (Count == 0)
                return false;

            int index = Array.BinarySearch(_masses, minMZ);
            if (index < 0)
                index = ~index;

            while (index < Count && _masses[index] <= maxMZ)
            {
                peaks.Add(GetPeak(index++));
            }

            return peaks.Count > 0;
        }

        public Spectrum Extract(double minMZ, double maxMZ)
        {
            if (Count == 0)
                return Empty;

            int index = Array.BinarySearch(_masses, minMZ);
            if (index < 0)
                index = ~index;

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
            return new Spectrum(mz, intensity, false);
        }

        /// <summary>
        /// Extracts a sub spectrum from this spectrum.
        /// Does not modify this spectrum.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public Spectrum Extract(IRange<double> range)
        {
            return Extract(range.Minimum, range.Maximum);
        }

        public Spectrum Filter(double minIntensity, double maxIntensity = double.MaxValue)
        {
            if (Count == 0)
                return Empty;

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
                return Empty;

            Array.Resize(ref mz, j);
            Array.Resize(ref intensities, j);
            return new Spectrum(mz, intensities, false);
        }

        public Spectrum Filter(IEnumerable<IRange<double>> rangesToRemove)
        {
            if (Count == 0)
                return Empty;

            int count = Count;

            // Peaks to remove
            HashSet<int> indiciesToRemove = new HashSet<int>();
            
            // Loop over each range to remove
            foreach (IRange<double> range in rangesToRemove)
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
                return Empty;

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
            return new Spectrum(mz, intensities, false);
        }

        public override string ToString()
        {
            return string.Format("{0} (Peaks {1})", GetMzRange(), Count);
        }
        
        public IEnumerator<MZPeak> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new MZPeak(_masses[i], _intensities[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] ToBytes(bool zlibCompressed = false)
        {
            return ConvertSpectrumToBytes(this, zlibCompressed);
        }

        #region Static

        public string ToBase64String(bool zlibCompressed = false)
        {
            return ConvertSpectrumToBase64String(this, zlibCompressed);
        }

        public static byte[] ConvertSpectrumToBytes(Spectrum spectrum, bool zlibCompressed = false)
        {
            int length = spectrum.Count * sizeof(double);
            byte[] bytes = new byte[length * 2];
            Buffer.BlockCopy(spectrum._masses, 0, bytes, 0, length);
            Buffer.BlockCopy(spectrum._intensities, 0, bytes, length, length);

            if (zlibCompressed)
            {
                bytes = bytes.Compress();
            }

            return bytes;
        }
        
        public static string ConvertSpectrumToBase64String(Spectrum spectrum, bool zlibCompressed = false)
        {           
            byte[] bytes = ConvertSpectrumToBytes(spectrum, zlibCompressed);
            return Convert.ToBase64String(bytes);
        }

        public static Spectrum ConvertBase64StringToSpectrum(string base64string, bool zlibCompressed = false)
        {
            byte[] bytes = Convert.FromBase64String(base64string);

            if (bytes.IsCompressed())
            {
                bytes = bytes.Decompress();    
            }

            return new Spectrum(bytes);          
        }

        public static Spectrum ConvertBytesToSpectrum(byte[] bytes, bool zlibCompressed = false)
        {
            if (zlibCompressed)
            {
                bytes = bytes.Decompress();
            }

            return new Spectrum(bytes);
        }

        #endregion

    }
}
