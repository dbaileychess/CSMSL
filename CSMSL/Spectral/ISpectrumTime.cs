// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ISpectrumTime.cs) is part of CSMSL.
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
using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public interface ISpectrumTime : ISpectrum
    {
        /// <summary>
        /// The time associated with this spectrum
        /// </summary>
        double Time { get; }
    }

    public static class ISpectrumTimeExtension
    {
        public static Chromatogram GetExtractedIonChromatogram(this IEnumerable<ISpectrumTime> spectra, DoubleRange range)
        {
            if (range == null)
            {
                throw new ArgumentException("A range must be declared for a m/z range chromatogram");
            }

            List<double> times = new List<double>();
            List<double> intensities = new List<double>();

            foreach (ISpectrumTime spectrum in spectra)
            {
                double intensity;
                spectrum.TryGetIntensities(range, out intensity);
                times.Add(spectrum.Time);
                intensities.Add(intensity);
            }

            return new MassRangeChromatogram(times.ToArray(), intensities.ToArray(), range);
        }

        public static Chromatogram GetClosetsPeakChromatogram(this IEnumerable<ISpectrumTime> spectra, DoubleRange range)
        {
            if (range == null)
            {
                throw new ArgumentException("A range must be declared for a m/z range chromatogram");
            }

            List<double> times = new List<double>();
            List<double> intensities = new List<double>();

            foreach (ISpectrumTime spectrum in spectra)
            {
                times.Add(spectrum.Time);
                var peak = spectrum.GetClosestPeak(range);
                intensities.Add((peak != null) ? peak.Intensity : 0);
            }

            return new MassRangeChromatogram(times.ToArray(), intensities.ToArray(), range);
        }
    }
}