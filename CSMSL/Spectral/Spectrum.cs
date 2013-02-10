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
using System.Linq;

namespace CSMSL.Spectral
{
    public class Spectrum : Spectrum<Peak>
    {
        public Spectrum()
            : base() { }

        public Spectrum(double[,] data)
            : base()
        {
            LoadData(data);
        }

        public Spectrum(double[] mzs, double[] intensities)
            : base()
        {
            LoadData(mzs, intensities);
        }

        public Spectrum(double[] mzs, float[] intensities)
            : base()
        {
            LoadData(mzs, intensities);
        }

        public Spectrum(IEnumerable<Peak> peaks)
            : base(peaks) { }

        private void LoadData(double[] mzs, double[] intensities)
        {
            if (mzs.Length != intensities.Length)
            {
                throw new FormatException("M/Z and Intensities arrays are not the same dimensions");
            }
            _count = mzs.Length;
            _tic = 0;
            _peaks = new Peak[_count];
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {
                float intensity = (float)intensities[i];
                _peaks[i] = new Peak(mzs[i], intensity);
                _tic += intensity;
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeak = _peaks[i];
                }
            }
        }

        private void LoadData(double[] mzs, float[] intensities)
        {
            if (mzs.Length != intensities.Length)
            {
                throw new FormatException("M/Z and Intensities arrays are not the same dimensions");
            }
            _count = mzs.Length;
            _tic = 0;
            _peaks = new Peak[_count];
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {
                float intensity = intensities[i];
                _peaks[i] = new Peak(mzs[i], intensity);
                _tic += intensity;
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeak = _peaks[i];
                }
            }
        }

        private void LoadData(double[,] data)
        {
            _count = data.GetLength(0);
            _tic = 0;
            _peaks = new Peak[_count];
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {
                float intensity = (float)data[i, 1];
                _peaks[i] = new Peak(data[i, 0], intensity);
                _tic += intensity;
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeak = _peaks[i];
                }
            }
        }
    }

    public class Spectrum<T> : IDisposable, IEnumerable<T> where T : Peak
    {
        protected T _basePeak;
        protected int _count;
        protected T[] _peaks;
        protected float _tic;

        protected Spectrum() { }

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

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public float TIC
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
            _count = peaks.Count();
            _peaks = new T[_count];
            _tic = 0;

            double maxInt = 0;
            int i = 0;
            float intensity;
            foreach (T peak in peaks)
            {
                _tic += intensity = peak.Intensity;
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