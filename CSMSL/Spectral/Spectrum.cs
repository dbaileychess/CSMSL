using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public class Spectrum<T> where T : IPeak
    {
        protected int _count;
        protected T[] _peaks;       

        protected Spectrum() 
        {
            _count = 0;         
        }

        public Spectrum(IEnumerable<T> peaks)            
            : this()
        {
            LoadPeaks(peaks);
        }
        
        public int Count
        {
            get
            {
                return _count;
            }
        }
        
        public List<T> GetPeaks(IRange<double> range)
        {
            if (range == null)
                return new List<T>();
            return GetPeaks(range.Minimum, range.Maximum);
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
            _peaks = peaks.ToArray();
            _count = _peaks.Length;       
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
       
    }
}
