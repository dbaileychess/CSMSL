// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (ISpectrum.cs) is part of CSMSL.
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

using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public interface ISpectrum
    {
        /// <summary>
        /// The number of peaks in the spectrum
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The first m/z of the spectrum
        /// </summary>
        double FirstMZ { get; }

        /// <summary>
        /// The last m/z of the spectrum
        /// </summary>
        double LastMZ { get; }

        double TotalIonCurrent { get; }

        /// <summary>
        /// Gets an array of m/z values for this spectrum
        /// </summary>
        /// <returns></returns>
        double[] GetMasses();

        /// <summary>
        /// Gets an array of intenisty values for this spectrumm, ordered by m/z value
        /// </summary>
        /// <returns></returns>
        double[] GetIntensities();

        /// <summary>
        /// Get the intensity of the most intense peak in this spectrum
        /// </summary>
        /// <returns></returns>
        double GetBasePeakIntensity();

        /// <summary>
        /// Get the sum of the intensities for this spectrum
        /// </summary>
        /// <returns></returns>
        double GetTotalIonCurrent();

        bool TryGetIntensities(double minMZ, double maxMZ, out double intensity);

        bool TryGetIntensities(IRange<double> rangeMZ, out double intensity);

        byte[] ToBytes(bool zlibCompressed);
        
        IPeak GetClosestPeak(double minMZ, double maxMZ);

        IPeak GetClosestPeak(IRange<double> rangeMZ);

        ISpectrum Extract(double minMZ, double maxMZ);

        ISpectrum Filter(IEnumerable<IRange<double>> mzRanges);
    }

    public interface ISpectrum<out T> : ISpectrum, IEnumerable<T> 
        where T : IPeak
    {
        new T GetClosestPeak(double minMZ, double maxMZ);

        new T GetClosestPeak(IRange<double> rangeMZ);
   }

}