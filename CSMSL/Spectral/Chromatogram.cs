// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Chromatogram.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Spectral
{
    public class Chromatogram : Chromatogram<ChromatographicPeak>
    {
        public Chromatogram(double[] times, double[] intensities, bool shouldCopy = true)
            : base(times, intensities, shouldCopy)
        {
        }

        public Chromatogram(double[,] timeintensities)
            : base(timeintensities)
        {
        }

        public Chromatogram(Chromatogram other)
            : base(other)
        {
        }

        public Chromatogram(byte[] bytes)
            : base(bytes)
        {
        }

        public Chromatogram Smooth(SmoothingType smoothing, int points)
        {
            switch (smoothing)
            {
                case SmoothingType.BoxCar:
                    double[] newTimes = _times.BoxCarSmooth(points);
                    double[] newIntensities = _intensities.BoxCarSmooth(points);
                    return new Chromatogram(newTimes, newIntensities, false);
                case SmoothingType.SavitzkyGolay:
                    throw new NotImplementedException();
                default:
                    return new Chromatogram(this);
            }
        }

        public override ChromatographicPeak GetPeak(int index)
        {
            return new ChromatographicPeak(_times[index], _intensities[index]);
        }

        public override byte[] ToBytes(bool zlibCompressed = false)
        {
            int length = Count*sizeof (double);
            byte[] bytes = new byte[length*2];
            Buffer.BlockCopy(_times, 0, bytes, 0, length);
            Buffer.BlockCopy(_intensities, 0, bytes, length, length);

            if (zlibCompressed)
            {
                bytes = bytes.Compress();
            }

            return bytes;
        }
    }

    public abstract class Chromatogram<T> : IEnumerable<T> where T : IPeak
    {
        protected readonly double[] _times;
        protected readonly double[] _intensities;

        public int Count { get; private set; }

        public double FirstTime
        {
            get { return _times[0]; }
        }

        public double LastTime
        {
            get { return _times[Count - 1]; }
        }

        protected Chromatogram(double[] times, double[] intensities, bool shouldCopy = true)
        {
            Count = times.Length;

            if (shouldCopy)
            {
                _times = new double[Count];
                _intensities = new double[Count];
                Buffer.BlockCopy(times, 0, _times, 0, 8*Count);
                Buffer.BlockCopy(intensities, 0, _intensities, 0, 8*Count);
            }
            else
            {
                _times = times;
                _intensities = intensities;
            }
        }

        protected Chromatogram(double[,] timeintensities)
        {
            Count = timeintensities.GetLength(1);
            _times = new double[Count];
            _intensities = new double[Count];
            Buffer.BlockCopy(timeintensities, 0, _times, 0, 8*Count);
            Buffer.BlockCopy(timeintensities, 8*Count, _intensities, 0, 8*Count);
        }

        protected Chromatogram(byte[] timeintensities)
        {
            Count = timeintensities.Length/(sizeof (double)*2);
            int size = sizeof (double)*Count;
            _times = new double[Count];
            _intensities = new double[Count];
            Buffer.BlockCopy(timeintensities, 0, _times, 0, size);
            Buffer.BlockCopy(timeintensities, size, _intensities, 0, size);
        }

        protected Chromatogram(Chromatogram<T> other)
            : this(other._times, other._intensities)
        {
        }

        public abstract T GetPeak(int index);

        public abstract byte[] ToBytes(bool zlibCompressed);

        public Range<double> GetTimeRange()
        {
            return new Range<double>(FirstTime, LastTime);
        }

        public double[] GetTimes()
        {
            double[] times = new double[Count];
            Buffer.BlockCopy(_times, 0, times, 0, sizeof (double)*Count);
            return times;
        }

        public double[] GetIntensities()
        {
            double[] intensities = new double[Count];
            Buffer.BlockCopy(_intensities, 0, intensities, 0, sizeof (double)*Count);
            return intensities;
        }

        public double GetTime(int index)
        {
            return _times[index];
        }

        public double GetIntensity(int index)
        {
            return _intensities[index];
        }

        public virtual T GetApex(IRange<double> timeRange)
        {
            return GetApex(timeRange.Minimum, timeRange.Maximum);
        }

        public virtual T GetApex(double mintime, double maxTime)
        {
            int index = Array.BinarySearch(_times, mintime);
            if (index < 0)
                index = ~index;

            if (index >= Count)
            {
                return GetPeak(Count - 1);
            }

            double maxvalue = -1; // double.negative infinity?
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
            return GetPeak(apexIndex);
        }

        public virtual ChromatographicElutionProfile<T> GetElutionProfile(IRange<double> timeRange)
        {
            return GetElutionProfile(timeRange.Minimum, timeRange.Maximum);
        }

        public virtual ChromatographicElutionProfile<T> GetElutionProfile(double mintime, double maxTime)
        {
            int index = Array.BinarySearch(_times, mintime);
            if (index < 0)
                index = ~index;

            List<T> peaks = new List<T>();
            while (index < Count && _times[index] <= maxTime)
            {
                peaks.Add(GetPeak(index));
                index++;
            }
            return new ChromatographicElutionProfile<T>(peaks);
        }

        public virtual bool ContainsPeak(IRange<double> timeRange)
        {
            return ContainsPeak(timeRange.Minimum, timeRange.Maximum);
        }

        public virtual bool ContainsPeak(double mintime, double maxTime)
        {
            int index = Array.BinarySearch(_times, mintime);
            if (index < 0)
                index = ~index;

            if (index >= Count)
            {
                return false;
            }

            double startIntensity = _intensities[index];
            double maxIntensity = startIntensity;
            double endIntensity = startIntensity;
            while (index < Count && _times[index] <= maxTime)
            {
                double currentIntensity = _intensities[index];
                endIntensity = currentIntensity;
                if (currentIntensity > maxIntensity) {
                    maxIntensity = currentIntensity;
                }
                index++;
            }
            if (maxIntensity <= 0)
                return false;

            double maxEndPointIntensity = Math.Max(startIntensity, endIntensity);
            return maxIntensity >= 1.4 * maxEndPointIntensity;
        }
        
        public virtual T GetApex()
        {
            int index = _intensities.MaxIndex();
            return GetPeak(index);
        }

        public T FindNearestApex(double rt, int skipablePts = 1)
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
                    if (count >= skipablePts)
                        break;
                }
                i--;
            }

            i = index + 1;
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
        
        public DoubleRange GetPeakWidth(double time, double fraction = 0.1, int upPts = 3, double upPrecent = 1.4, double minValue = 0)
        {
            int index = Array.BinarySearch(_times, time);
            if (index < 0)
                index = ~index;

            if (index == _times.Length)
                index--;

            double maxTime = _times[index];
            double minTime = maxTime;
            double threshold = Math.Max(_intensities[index]*fraction, minValue);

            int count = 0;
            double localMin = _intensities[index];
            for (int i = index + 1; i < Count; i++)
            {
                double peakIntensity = _intensities[i];

                if (peakIntensity > localMin*upPrecent)
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

                if (peakIntensity <= threshold)
                    break;
            }

            //maxTime = Math.Min(maxTime, _times[Count - 1]);

            localMin = _intensities[index];
            count = 0;
            for (int i = index - 1; i >= 0; i--)
            {
                double peakIntensity = _intensities[i];

                if (peakIntensity > localMin*upPrecent)
                {
                    // Going up
                    count++;
                    if (count > upPts)
                        break;

                    continue;
                }

                minTime = _times[i];

                if (peakIntensity < localMin)
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
            return string.Format("Count = {0:N0} TIC = {1:G4}", Count, _intensities.Sum());
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetPeak(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}