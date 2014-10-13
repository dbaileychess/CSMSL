using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CSMSL.Spectral
{
    public class ChromatographicElutionProfile<T> where T : IPeak
    {
        public T StartPeak
        {
            get { return _peaks[0]; }
        }

        public T EndPeak
        {
            get { return _peaks[Count - 1]; }
        }

        private readonly int _maxPeakIndex = 0;
        public T MaxPeak
        {
            get { return _peaks[_maxPeakIndex]; }
        }

        public DoubleRange TimeRange {get; private set;}

        public int Count {get; private set;}

        public double SummedArea { get; private set; }

        private readonly T[] _peaks;

        public ChromatographicElutionProfile(ICollection<T> peaks)
        {
            Count = peaks.Count;
            if (Count == 0)
            {
                return;
            }
            _peaks = peaks.ToArray();
       
            _maxPeakIndex = _peaks.MaxIndex(p => p.Y);
            SummedArea = _peaks.Sum(p => p.Y);
            TimeRange = new DoubleRange(_peaks[0].X, _peaks[Count - 1].X);
        }

        public double TrapezoidalArea()
        {
            double area = 0;
            for (int i = 0; i < Count - 1; i++)
            {
                T peak1 = _peaks[i];
                T peak2 = _peaks[i+1];
                area += (peak2.X - peak1.X) * (peak2.Y + peak1.Y);
            }
            return area / 2.0;
        }





    }
}
