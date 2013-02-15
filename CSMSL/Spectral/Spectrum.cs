using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public class Spectrum<T> : IEnumerable<T> where T : IPeak
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
            TryGetPeaks(range.Minimum, range.Maximum, out peaks);
            return peaks;
        }

        public List<T> GetPeaks(double minMZ, double maxMZ)
        {
            List<T> peaks = null;
            TryGetPeaks(minMZ, maxMZ, out peaks);
            return peaks;
        }

        public bool TryGetPeaks(IRange<double> range, out List<T> peaks)
        {
            return TryGetPeaks(range.Minimum, range.Maximum, out peaks);
        }

        public bool TryGetPeaks(double minX, double maxX, out List<T> peaks)
        {
            int index = Array.BinarySearch(_peaks, minX);
            if (index < 0)
                index = ~index;

            peaks = new List<T>();
            T peak;
            if (index >= _peaks.Length || (peak = _peaks[index]).X > maxX) 
                return false;

            do
            {
                peaks.Add(peak);
                index++;
            } while (index < _peaks.Length && (peak = _peaks[index]).X <= maxX);

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
                _tic += intensity = peak.Y;
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
            _count = 0;
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
