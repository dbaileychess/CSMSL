using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Spectral
{

    public class Spectrum<T> where T : IPeak
    {
        protected T[] Peaks;       

        protected Spectrum() 
        {
            Count = 0;         
        }

        public Spectrum(IEnumerable<T> peaks)            
            : this()
        {
            LoadPeaks(peaks);
        }

        public T FirstPeak
        {
            get { return Count == 0 ? default(T) : Peaks[0]; }
        }

        public T LastPeak
        {
            get { return Count == 0 ? default(T) : Peaks[Count-1]; }
        }

        public int Count { get; protected set; }

        public bool ContainsPeak()
        {
            return Count > 0;
        }

        public bool ContainsPeak(IRange<double> range)
        {
            return ContainsPeak(range.Minimum, range.Maximum);
        }

        public bool ContainsPeak(double min, double max)
        {
            if (Count == 0)
                return false;

            int index = Array.BinarySearch(Peaks, min);
            if (index < 0)
                index = ~index;

            return (index < Count && Peaks[index].X <= max);
        }

        public Spectrum ToReadOnlySpectrum()
        {
            double[] mz = new double[Count];
            double[] intensities = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                mz[i] = Peaks[i].X;
                intensities[i] = Peaks[i].Y;
            }
            return new Spectrum(mz, intensities, false);
        }

        public virtual Spectrum<T> Clone()
        {
            return new Spectrum<T>(Peaks);
        }
        
        public List<T> GetPeaks(IRange<double> range)
        {
            if (range == null)
                return new List<T>();
            return GetPeaks(range.Minimum, range.Maximum);
        }

        public List<T> GetPeaks(double minMZ, double maxMZ)
        {
            if (Count == 0)
                return null;

            int index = Array.BinarySearch(Peaks, minMZ);
            if (index < 0)
                index = ~index;
            
            T peak;
            if (index >= Count || (peak = Peaks[index]).X > maxMZ)
                return null;

            var peaks = new List<T>();
            do
            {
                peaks.Add(peak);
                index++;
            } while (index < Count && (peak = Peaks[index]).X <= maxMZ);

            return peaks;
        }

        public bool TryGetPeaks(IRange<double> range, out List<T> peaks)
        {
            return TryGetPeaks(range.Minimum, range.Maximum, out peaks);
        }

        public bool TryGetPeaks(double minX, double maxX, out List<T> peaks)
        {
            peaks = GetPeaks(minX, maxX);
            return peaks != null;
        }

        public T GetClosestPeak(DoubleRange massRange)
        {
            return GetClosestPeak(massRange.Mean, massRange.Width);
        }

        public T GetClosestPeak(double mean, Tolerance tolerance)
        {
            return GetClosestPeak(mean, tolerance.GetRange(mean).Width);
        }

        public T GetClosestPeak(double mean, double tolerance)
        {
            if (Count == 0)
                return default(T);
            
            int index = Array.BinarySearch(Peaks, mean);

            if (index >= 0)
                return Peaks[index];
     
            index = ~index;

            int indexm1 = index - 1;

            double minMZ = mean - tolerance;
            double maxMZ = mean + tolerance;
            if (index >= Count)
            {
                // only the indexm1 peak can be closer

                if (indexm1 >= 0 && Peaks[indexm1].X >= minMZ)
                {
                    return Peaks[indexm1];
                }

                return default(T);
            } 
            if (index == 0)
            {
                // only the index can be closer
                if (Peaks[index].X <= maxMZ)
                {
                    return Peaks[index];
                }

                return default(T);
            }

            
            double p1 = Peaks[indexm1].X;
            double p2 = Peaks[index].X;
            if (p2 > maxMZ)
            {
                if (p1 >= minMZ)
                    return Peaks[indexm1];
                return default(T);
            }
            if (p1 >= minMZ)
            {
                if (mean - p1 > p2 - mean)
                    return Peaks[index];
                return Peaks[indexm1];
            }
            return Peaks[index];
        }
        
        private void LoadPeaks(IEnumerable<T> peaks)
        {
            Peaks = peaks.ToArray();
            Count = Peaks.Length;       
        }
        
        public override string ToString()
        {
            return string.Format("{0:G0} Peaks", Count);
        }
    }
}
