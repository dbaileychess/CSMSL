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
    public class MZSpectrum : Spectrum<MZPeak>
    {
        public MZSpectrum()
            : base() { }

        public MZSpectrum(double[,] data)
            : base()
        {
            LoadData(data);
        }

        public MZSpectrum(double[] mzs, double[] intensities)
            : base()
        {
            LoadData(mzs, intensities);
        }

        public MZSpectrum(double[] mzs, float[] intensities)
            : base()
        {
            LoadData(mzs, intensities);
        }

        public MZSpectrum(IEnumerable<MZPeak> peaks)
            : base(peaks) { }

        private void LoadData(double[] mzs, double[] intensities)
        {
            if (mzs.Length != intensities.Length)
            {
                throw new FormatException("M/Z and Intensities arrays are not the same dimensions");
            }
            _count = mzs.Length;
            _tic = 0;
            _peaks = new MZPeak[_count];
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {
                float intensity = (float)intensities[i];
                _peaks[i] = new MZPeak(mzs[i], intensity);
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
            _peaks = new MZPeak[_count];
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {
                float intensity = intensities[i];
                _peaks[i] = new MZPeak(mzs[i], intensity);           
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
            _peaks = new MZPeak[_count];
            double maxInt = 0;
            for (int i = 0; i < _count; i++)
            {
                float intensity = (float)data[i, 1];
                _peaks[i] = new MZPeak(data[i, 0], intensity);
                _tic += intensity;
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    _basePeak = _peaks[i];
                }
            }
        }
    }   
    
}