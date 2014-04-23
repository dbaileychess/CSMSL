using System;
using System.Collections.Generic;
using System.Linq;
using CSMSL;

namespace CSMSL.Spectral
{
    public class Spectrum : ISpectrum
    {
        private readonly double[] _masses;
        private readonly double[] _intensities;

        public int Count { get; private set; }

        public double FirstMz { get { return _masses[0]; } }
        public double LastMZ { get { return _masses[Count - 1]; } }

        public Spectrum(double[] mz, double[] intensities)
            : this(mz, intensities, true) { }    

        public Spectrum(double[] mz, double[] intensities, bool shouldCopy = true)
        {
            Count = mz.Length;
           
            if (shouldCopy)
            {
                _masses = new double[Count];
                _intensities = new double[Count];
                System.Buffer.BlockCopy(mz, 0, _masses, 0, 8 * Count);
                System.Buffer.BlockCopy(intensities, 0, _intensities, 0, 8 * Count);
            }
            else
            {
                _masses = mz;
                _intensities = intensities;
            }

        }

        public Spectrum(Spectrum spectrum)
            : this(spectrum._masses, spectrum._intensities)
        {

        }

        public Spectrum(double[,] mzintensities)
        {
            Count = mzintensities.GetLength(1);           
            _masses = new double[Count];
            _intensities = new double[Count];
            Buffer.BlockCopy(mzintensities, 0, _masses, 0, 8 * Count);
            Buffer.BlockCopy(mzintensities, 8 * Count, _intensities, 0, 8 * Count);         
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

        public double GetBasePeakIntensity()
        {
            return _intensities.Max();
        }

        public MZPeak GetBasePeak()
        {
            int index = _intensities.MaxIndex();
            return GetPeak(index);
        }

        public double[] GetMasses()
        {
            return (double[])_masses.Clone();
        }

        public double[] GetIntensities()
        {
            return (double[])_intensities.Clone();
        }

        public double GetTotalIonCurrent()
        {
            return _intensities.Sum();
        }

        public double GetMass(int index)
        {
            return _masses[index];
        }

        public double GetIntensity(int index)
        {
            return _intensities[index];
        }

        public MZPeak GetPeak(int index)
        {
            return new MZPeak(_masses[index], _intensities[index]);
        }

        public MZPeak GetClosestPeak(IRange<double> massRange)
        {
            double mean = (massRange.Maximum + massRange.Minimum) / 2.0;
            double width = massRange.Maximum - massRange.Minimum;
            return GetClosestPeak(mean, width);
        } 

        public MZPeak GetClosestPeak(double mean, double tolerance)
        {
            if (Count == 0)
                return null;

            int index = Array.BinarySearch(_masses, mean);

            if (index >= 0)
                return GetPeak(index);

            index = ~index;

            int indexm1 = index - 1;

            double minMZ = mean - tolerance;
            double maxMZ = mean + tolerance;
            if (index >= Count)
            {
                // only the indexm1 peak can be closer

                if (indexm1 >= 0 && _masses[indexm1] >= minMZ)
                {
                    return GetPeak(indexm1);
                }

                return null;
            }
            else if (index == 0)
            {
                // only the index can be closer
                if (_masses[index] <= maxMZ)
                {
                    return GetPeak(index);
                }

                return null;
            }

            double p1 = _masses[indexm1];
            double p2 = _masses[index];

            if (p2 > maxMZ)
            {
                if (p1 >= minMZ)
                    return GetPeak(indexm1);
                return null;
            }
            if (p1 >= minMZ)
            {
                if (mean - p1 > p2 - mean)
                    return GetPeak(index);
                return GetPeak(indexm1);
            }
            return GetPeak(index);
        }

        public bool ContainsPeak(IRange<double> range)
        {
            return ContainsPeak(range.Minimum, range.Maximum);
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

        public Spectrum Extract(IRange<double> range)
        {
            if (Count == 0)
                return null;

            int index = Array.BinarySearch(_masses, range.Minimum);
            if (index < 0)
                index = ~index;

            int count = Count;
            double[] mz = new double[count];
            double[] intensity = new double[count];
            int j = 0;

            while (index < Count && _masses[index] <= range.Maximum)
            {
                mz[j] = _masses[index];
                intensity[j] = _intensities[index];
                index++;
                j++;
            }

            if (j <= 0) 
                return null;
            Array.Resize(ref mz, j);
            Array.Resize(ref intensity, j);
            return new Spectrum(mz, intensity, false);
        }

        public Spectrum Filter(double minIntensity, double maxIntensity = double.MaxValue)
        {
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
            Array.Resize(ref mz, j);
            Array.Resize(ref intensities, j);
            return new Spectrum(mz, intensities, false);
        }

        public override string ToString()
        {
            return string.Format("{0:G0} Peaks", Count);
        }
        
        public IEnumerator<MZPeak> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new MZPeak(_masses[i], _intensities[i]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public byte[] ToBytes(bool zlibCompressed = false)
        {
            return ConvertSpectrumToBytes(this, zlibCompressed);
        }

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

            if (zlibCompressed)
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
       
    }
}
