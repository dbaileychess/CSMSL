using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public class Spectrum<T> : IDisposable, IEnumerable<T> where T : IPeak
    {
        protected T _basePeak;
        protected int _count;
        protected T[] _peaks;       
        protected double _tic;

        protected Spectrum() { }

        public Spectrum(IEnumerable<T> peaks)
        {
            LoadPeaks(peaks);
        }

        public T BasePeak
        {
            get
            {
                return _basePeak;
            }
        }

        public int Count
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

        public bool TryGetPeaks(out List<T> peaks, double minX, double maxX)
        {
            int index = Array.BinarySearch(_peaks, minX);
            if (index < 0)
                index = ~index;

            peaks = new List<T>();
            T peak;
            if (index >= _peaks.Length || (peak = _peaks[index]).GetX() > maxX) return false;

            do
            {
                peaks.Add(peak);
                index++;
            } while (index < _peaks.Length && (peak = _peaks[index]).GetX() <= maxX);

            return true;
        }

        private void LoadPeaks(IEnumerable<T> peaks)
        {
            _count = peaks.Count();
            _peaks = new T[_count];           
            _tic = 0;

            double maxInt = 0;
            int i = 0;
            double intensity;
            foreach (T peak in peaks)
            {
                _tic += intensity = peak.GetY();
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeak = peak;
                }               
                _peaks[i++] = peak;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:G0} Peaks", Count);
        }

        public void Clear()
        {
            if (_peaks != null)
                Array.Clear(_peaks, 0, _peaks.Length);
        }

        public void Dispose()
        {
            if (_peaks != null)
                Array.Clear(_peaks, 0, _peaks.Length);
            _peaks = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _peaks.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _peaks.GetEnumerator();
        }
    }
}
