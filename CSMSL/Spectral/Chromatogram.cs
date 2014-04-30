using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Spectral
{
    public class Chromatogram : IEnumerable<ChromatographicPeak>
    {
        protected readonly double[] _times;
        protected readonly double[] _intensities;

        public int Count { get; private set; }

        public double FirstTime { get { return _times[0]; } }
        public double LastTime { get { return _times[Count - 1]; } }

        private double _totalIonCurrent = -1;
        public double TotalIonCurrent {
            get
            {
                if (_totalIonCurrent < 0)
                {
                    _totalIonCurrent = _intensities.Sum();
                }
                return _totalIonCurrent;
            }            
        }

        private ChromatographicPeak _basePeak = null;
        public ChromatographicPeak BasePeak
        {
            get
            {
                if (_basePeak == null)
                {                   
                    _basePeak = GetPeak(_intensities.MaxIndex());
                }
                return _basePeak;
            }
        }

        public Chromatogram(double[] times, double[] intensities, bool shouldCopy = true)
        {
            Count = times.Length;

            if (shouldCopy)
            {
                _times = new double[Count];
                _intensities = new double[Count];
                System.Buffer.BlockCopy(times, 0, _times, 0, 8*Count);
                System.Buffer.BlockCopy(intensities, 0, _intensities, 0, 8*Count);
            }
            else
            {
                _times = times;
                _intensities = intensities;
            }
        }

        public Chromatogram(double[,] timeintensities)
        {
            Count = timeintensities.GetLength(1);
            _times = new double[Count];
            _intensities = new double[Count];
            System.Buffer.BlockCopy(timeintensities, 0, _times, 0, 8 * Count);
            System.Buffer.BlockCopy(timeintensities, 8 * Count, _intensities, 0, 8 * Count);         
        }

        private Chromatogram(byte[] timeintensities)
        {
            Count = timeintensities.Length / (sizeof(double) * 2);
            int size = sizeof(double) * Count;
            _times = new double[Count];
            _intensities = new double[Count];
            Buffer.BlockCopy(timeintensities, 0, _times, 0, size);
            Buffer.BlockCopy(timeintensities, size, _intensities, 0, size);     
        }

        public Chromatogram(Chromatogram other)
            : this(other._times, other._intensities) { }      

        public ChromatographicPeak GetPeak(int index)
        {
            return new ChromatographicPeak(_times[index], _intensities[index]);
        }

        public Range<double> GetTimeRange()
        {
            return new Range<double>(FirstTime, LastTime);
        }

        public double[] GetTimes()
        {
            return (double[])_times.Clone();
        }

        public double[] GetIntensities()
        {
            return (double[])_intensities.Clone();
        }

        public ChromatographicPeak GetApex(double mintime, double maxTime)
        {  
            int index = Array.BinarySearch(_times, mintime);
            if (index < 0)
                index = ~index;

            if (index >= Count)
            {
                return new ChromatographicPeak(_times[Count - 1], _intensities[Count - 1]);
            }

            double maxvalue = -1; // double.negative infinitiy?
            int apexIndex = index;
            while (index < Count && _times[index] <= maxTime)
            {
                double intensity = _intensities[index];
                if (intensity > maxvalue)
                {
                    apexIndex = index;
                    maxvalue = intensity;
                }
                index++;
            }

            return new ChromatographicPeak(_times[apexIndex], maxvalue);
        }

        public ChromatographicPeak GetApex(IRange<double> timeRange)
        {
            return GetApex(timeRange.Minimum, timeRange.Minimum);
        }

        public ChromatographicPeak GetApex()
        {
            return BasePeak;
        }

        public ChromatographicPeak FindNearestApex(double rt, int skipablePts = 1)
        {
            if (Count == 1)
                return GetPeak(0);

            int index = Array.BinarySearch(_times, rt);
            if (index < 0)
                index = ~index;

            if (index >= Count)
                index--;

            int bestApex = index;
            double apexValue = _intensities[bestApex];

            int i = index - 1;
            int count = 0;
            while (i >= 0)
            {
                if (_intensities[i] > apexValue)
                {
                    bestApex = i;
                    apexValue = _intensities[bestApex];
                    count = 0;
                }
                else
                {
                    count++;
                    if(count >= skipablePts) 
                        break;
                }
                i--;
            }
            
            i = index +1;
            count = 0;
            while (i < Count)
            {
                if (_intensities[i] > apexValue)
                {
                    bestApex = i;
                    apexValue = _intensities[bestApex];
                    count = 0;
                }
                else
                {
                    count++;
                    if (count >= skipablePts)
                        break;
                }
                i++;
            }

            return GetPeak(bestApex);
        }

        public DoubleRange GetPeakWidth(double time, double fraction, int upPts = 3, double upPrecent = 1.4)
        {
            int index = Array.BinarySearch(_times, time);
            if (index < 0)
                index = ~index;

            double maxTime = _times[index];
            double minTime = maxTime;
            double threshold = _intensities[index] * fraction;
            
            int count = 0;
            double localMin = _intensities[index];
            for (int i = index + 1; i < Count; i++)
            {
                double peakIntensity = _intensities[i];

                if (peakIntensity > localMin * upPrecent)
                {
                    // Going up
                    count++;
                    if (count > upPts)
                        break;
                    continue;
                }

               

                maxTime = _times[i];

                if (peakIntensity < localMin)
                    localMin = peakIntensity;

                count = 0;

                if (peakIntensity < threshold)
                    break;
            }

            //maxTime = Math.Min(maxTime, _times[Count - 1]);
          
            localMin = _intensities[index];
            count = 0;
            for (int i = index - 1; i >= 0; i--)              
            {
                double peakIntensity = _intensities[i];

                if (peakIntensity > localMin * upPrecent)
                {
                    // Going up
                    count++;
                    if (count > upPts)
                        break;
                    
                    continue;
                }

                minTime = _times[i];

                if(peakIntensity < localMin)
                    localMin = peakIntensity;

                count = 0;

                if (peakIntensity < threshold)
                    break;
            }
            
            //minTime = Math.Max(minTime,_times[0]);

            return new DoubleRange(minTime, maxTime);
        }

        public override string ToString()
        {
            return string.Format("Count = {0:N0} TIC = {1:G4} ({2})", Count, TotalIonCurrent);
        }        

        public Chromatogram Smooth(SmoothingType smoothing, int points)
        {
            switch (smoothing)
            {
                case SmoothingType.BoxCar:
                    return BoxCar(points);
                case SmoothingType.SavitzkyGolay:
                    return SavitzkyGolay(points);
                default:
                    return new Chromatogram(this);
            }
        }

        private Chromatogram SavitzkyGolay(int points)
        {
            throw new NotImplementedException();
        }

        private Chromatogram BoxCar(int points)
        {
            // Force to be odd
            points = points - (1 - points % 2);

            if (points <= 0 || points > Count)
                return new Chromatogram(this); // no smoothing
            
            int newCount = Count - points + 1;
          
            double[] times = new double[newCount];
            double[] intensities = new double[newCount];
           
            for (int i = 0; i < newCount; i++)
            {
                double time = 0;
                double intensity = 0;

                for (int j = i; j < i + points; j++)
                {
                    time += _times[j];
                    intensity += _intensities[j];
                }

                times[i] = time / points;
                intensities[i] = intensity / points;             
            }     

            return new Chromatogram(times, intensities, false);
        }

        public IEnumerator<ChromatographicPeak> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new ChromatographicPeak(_times[0], _intensities[0]);
            }
        }       

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public string ToBase64String(bool zlibCompressed = false)
        {
            return ConvertChromatogramToBase64String(this, zlibCompressed);
        }

        public byte[] ToBytes(bool zlibCompressed = false) {
            return ConvertChromatogramToBytes(this, zlibCompressed);
        }

        public static string ConvertChromatogramToBase64String(Chromatogram chromatogram, bool zlibCompressed = false)
        {
            int length = chromatogram.Count * sizeof(double);
            byte[] bytes = new byte[length * 2];
            Buffer.BlockCopy(chromatogram._times, 0, bytes, 0, length);
            Buffer.BlockCopy(chromatogram._intensities, 0, bytes, length, length);

            if (zlibCompressed)
            {
                bytes = bytes.Compress();
            }

            return Convert.ToBase64String(bytes);
        }

        public static Chromatogram ConvertBase64StringToChromatogram(string base64string, bool zlibCompressed = false)
        {
            byte[] bytes = Convert.FromBase64String(base64string);

            if (zlibCompressed)
            {
                bytes = bytes.Decompress();
            }

            return new Chromatogram(bytes);
        }

        public static Chromatogram ConvertBytesToChromatogram(byte[] bytes, bool zlibCompressed = false)
        {
            if (zlibCompressed)
            {
                bytes = bytes.Decompress();
            }

            return new Chromatogram(bytes);
        }

        public static byte[] ConvertChromatogramToBytes(Chromatogram chromatogram, bool zlibCompressed = false)
        {
            int length = chromatogram.Count * sizeof(double);
            byte[] bytes = new byte[length * 2];
            Buffer.BlockCopy(chromatogram._times, 0, bytes, 0, length);
            Buffer.BlockCopy(chromatogram._intensities, 0, bytes, length, length);

            if (zlibCompressed)
            {
                bytes = bytes.Compress();
            }

            return bytes;
        }

       
    }
}
