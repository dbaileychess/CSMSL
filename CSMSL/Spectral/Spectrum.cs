///////////////////////////////////////////////////////////////////////////
//  Spectrum.cs - A collection of peaks                                   /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public class Spectrum : Spectrum<Peak>
    {
        public Spectrum()
            : base() { }

        public Spectrum(double[,] data)
            : base(data) { }

        public Spectrum(double[] mzs, double[] intensities)
            : base(mzs, intensities) { }

        public Spectrum(double[] mzs, float[] intensities)
            : base(mzs, intensities) { }

        public Spectrum(IEnumerable<Peak> peaks)
            : base(peaks) { }
    }

    public class Spectrum<T> : IDisposable where T : Peak
    {
        internal T _basePeak;
        internal int _count;
        internal T[] _peaks;
        internal double _tic;        

        public Spectrum()
        {

        }

        public Spectrum(double[,] data)
        {
            LoadData(data);
        }

        public Spectrum(double[] mzs, double[] intensities)
        {
            LoadData(mzs, intensities);
        }

        public Spectrum(double[] mzs, float[] intensities)
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
            int index = Array.BinarySearch(_peaks, new Peak(minMZ, 0f));
            if (index < 0)
                index = ~index;

            peaks = new List<T>();
            T peak; 
            if (index >= _peaks.Length || (peak = _peaks[index]).MZ > maxMZ) return false;
                   
            do
            {
                peaks.Add(peak);
                index++;
            } while (index < _peaks.Length && (peak = _peaks[index]).MZ <= maxMZ);

            return true;
        }

        private void LoadData(IEnumerable<T> peaks)
        {

            //_count = peaks.
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
                    _basePeak = i;
                }
            }
           // Array.Sort(_peaks);
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
                temppeak.Intensity = (float)intensities[i];
                _tic += temppeak.Intensity;
                if (temppeak.Intensity > maxInt)
                {
                    maxInt = temppeak.Intensity;
                    _basePeak = i;
                }
                _peaks[i] = temppeak;
            }
            Array.Sort(_peaks);
        }

        private void LoadData(double[] mzs, float[] intensities)
        {
            if (mzs.Length != intensities.Length)
            {
                throw new FormatException("M/Z and Intensities arrays are not the same dimensions");
            }
            _count = mzs.Length;
            _mzs = mzs;
            _intensities = intensities;
            _tic = 0;
            double maxInt = 0;
            double intensity = 0;
            for (int i = 0; i < _count; i++)
            {
                _tic += intensity = _intensities[i];
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeak = i;
                }
            }
        }

        private void LoadData(double[,] data)
        {
            _count = data.GetLength(0);                 
            _tic = 0;
            double maxInt = 0;
            double intensity = 0;
            T tempPeak;
            for (int i = 0; i < _count; i++)
            {                
                tempPeak = new T(
                _tic += intensity = _intensities[i] = (float)data[i, 1];
                _mzs[i] = data[i, 0];
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeak = i;
                }              
            }     
        }

        public override string ToString()
        {
            return string.Format("{0:G0} Peaks", Count);
        }

        public void Dispose()
        {
            if(_peaks != null)
                Array.Clear(_peaks, 0, _peaks.Length);
            _peaks = null;
        }
    }  

}