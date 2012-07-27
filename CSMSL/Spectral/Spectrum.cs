using System;
using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public class Spectrum : Spectrum<Peak>
    {
        public Spectrum(double[,] data)
            : base(data) { }
    }

    public class Spectrum<T> where T : IPeak, new()
    {
        internal T _basePeak;
        internal int _count;
        internal T[] _peaks;
        internal double _tic;

        public Spectrum(double[,] data)
        {
            LoadData(data);
        }

        public Spectrum(double[] mzs, double[] intensities)
        {
            LoadData(mzs, intensities);
        }

        public Spectrum(IEnumerable<T> peaks)
        {
            LoadData(peaks);
        }

        public T BasePeak
        {
            get
            {
                return _basePeak;
            }
        }

        public double Count
        {
            get
            {
                return _count;
            }
        }

        public double TIC
        {
            get
            {
                return _tic;
            }
        }

        public List<T> GetPeaks(IRange<double> range)
        {
            List<T> peaks = null;
            TryGetPeaks(out peaks, range.Minimum, range.Maximum);
            return peaks;
        }

        public List<T> GetPeaks(double minMZ, double maxMZ)
        {
            List<T> peaks = null;
            TryGetPeaks(out peaks, minMZ, maxMZ);
            return peaks;
        }

        public bool TryGetPeaks(out List<T> peaks, IRange<double> range)
        {
            return TryGetPeaks(out peaks, range.Minimum, range.Maximum);
        }

        public bool TryGetPeaks(out List<T> peaks, double minMZ, double maxMZ)
        {
            T temppeak = new T();
            temppeak.MZ = minMZ;
            int index = Array.BinarySearch(_peaks, temppeak);
            if (index < 0)
                index = ~index;

            peaks = new List<T>();

            if ((temppeak = _peaks[index]).MZ > maxMZ) return false;

            do
            {
                peaks.Add(temppeak);
                index++;
            } while (index < _peaks.Length && (temppeak = _peaks[index]).MZ <= maxMZ);

            return true;
        }

        private void LoadData(IEnumerable<T> peaks)
        {
            List<T> temppeaks = new List<T>(peaks);
            _count = temppeaks.Count;
            _peaks = new T[_count];
            _tic = 0;
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {
                T temppeak = temppeaks[i];
                _peaks[i] = temppeak;
                _tic += temppeak.Intensity;
                if (temppeak.Intensity > maxInt)
                {
                    maxInt = temppeak.Intensity;
                    _basePeak = temppeak;
                }
            }
            Array.Sort(_peaks);
        }

        private void LoadData(double[] mzs, double[] intensities)
        {
            if (mzs.Length != intensities.Length)
            {
                throw new FormatException("M/Z and Intensities arrays are not the same dimensions");
            }
            _count = mzs.Length;
            _peaks = new T[_count];
            _tic = 0;
            double maxInt = 0;
            T temppeak;
            for (int i = 0; i < _count; i++)
            {
                temppeak = new T();
                temppeak.MZ = mzs[i];
                temppeak.Intensity = intensities[i];
                _tic += temppeak.Intensity;
                if (temppeak.Intensity > maxInt)
                {
                    maxInt = temppeak.Intensity;
                    _basePeak = temppeak;
                }
                _peaks[i] = temppeak;
            }
            Array.Sort(_peaks);
        }

        private void LoadData(double[,] data)
        {
            _count = data.GetLength(0);
            _peaks = new T[_count];
            _tic = 0;
            double maxInt = 0;
            T temppeak;
            for (int i = 0; i < _count; i++)
            {
                temppeak = new T();
                temppeak.MZ = data[i, 0];
                temppeak.Intensity = data[i, 1];
                _tic += temppeak.Intensity;
                if (temppeak.Intensity > maxInt)
                {
                    maxInt = temppeak.Intensity;
                    _basePeak = temppeak;
                }
                _peaks[i] = temppeak;
            }
            Array.Sort(_peaks);
        }
    }
}