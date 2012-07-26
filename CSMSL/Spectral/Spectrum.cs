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
        internal double _basePeakMZ;
        internal int _count;
        internal double[] _intensities;
        internal double[] _mzs;
        internal Dictionary<double, T> _peaks;
        internal double _tic;

        public Spectrum(double[,] data)
        {
            LoadData(data);
        }

        public T BasePeak
        {
            get
            {
                return _peaks[_basePeakMZ];
            }
        }

        private void LoadData(double[,] data)
        {
            _count = data.GetLength(0);
            _mzs = new double[_count];
            _intensities = new double[_count];
            _peaks = new  Dictionary<double, T>(_count);
            _tic = 0;
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {                
                _mzs[i] = data[i, 0];
                double intensity = data[i, 1];
                _intensities[i] = intensity;
                _tic += intensity;
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeakMZ = _mzs[i];
                }
                T peak = new T();
                peak.MZ = _mzs[i];
                peak.Intensity = _intensities[i];
                _peaks.Add(peak.MZ, peak);
            }
            Array.Sort(_mzs, _intensities);
        }

        private T GetPeak(double mz)
        {
            return _peaks[mz];
        }

        public bool TryGetPeaks(out List<T> peaks, IRange<double> range)
        {
            return TryGetPeaks(out peaks, range.Minimum, range.Maximum);
        }

        public bool TryGetPeaks(out List<T> peaks, double min, double max)
        {
            int index = Array.BinarySearch(_mzs, min);
            if (index < 0)
                index = ~index;

            peaks = new List<T>();
            
            if (_mzs[index] > max) return false;

            do
            {
                T peak = GetPeak(_mzs[index]);
                peaks.Add(peak);
                index++;
            } while (index < _mzs.Length && _mzs[index] <= max);

            return true;
        }
    }
}