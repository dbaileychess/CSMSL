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
    public interface ISpectrum : IEnumerable<MZPeak>
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

        /// <summary>
        /// The total ion current of the spectrum
        /// </summary>
        double TotalIonCurrent { get; }

        /// <summary>
        /// Gets the m/z at a particular index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double GetMass(int index);

        /// <summary>
        /// Gets the intensity at a particular index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double GetIntensity(int index);

        /// <summary>
        /// Gets an array of m/z values for this spectrum
        /// </summary>
        /// <returns></returns>
        double[] GetMasses();

        /// <summary>
        /// Gets an array of intensity values for this spectrum, ordered by m/z value
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

        bool ContainsPeak(double minMZ, double maxMZ);

        bool ContainsPeak(IRange<double> range);
        
        bool ContainsPeak();

        double[,] ToArray();

        MZPeak GetPeak(int index);

        MZPeak GetClosestPeak(double minMZ, double maxMZ);

        MZPeak GetClosestPeak(IRange<double> rangeMZ);

        ISpectrum Extract(IRange<double> mzRange);

        ISpectrum Extract(double minMZ, double maxMZ);

        ISpectrum FilterByMZ(IEnumerable<IRange<double>> mzRanges);

        ISpectrum FilterByMZ(IRange<double> mzRange);

        ISpectrum FilterByMZ(double minMZ, double maxMZ);

        ISpectrum FilterByIntensity(double minIntensity, double maxIntensity);

        ISpectrum FilterByIntensity(IRange<double> intenistyRange);
    }

    public interface ISpectrum<out TPeak> : ISpectrum
        where TPeak : IPeak
    {
        new TPeak GetPeak(int index);

        new TPeak GetClosestPeak(double minMZ, double maxMZ);

        new TPeak GetClosestPeak(IRange<double> rangeMZ);

        new ISpectrum<TPeak> Extract(IRange<double> mzRange);

        new ISpectrum<TPeak> Extract(double minMZ, double maxMZ);

        new ISpectrum<TPeak> FilterByMZ(IEnumerable<IRange<double>> mzRanges);

        new ISpectrum<TPeak> FilterByMZ(IRange<double> mzRange);

        new ISpectrum<TPeak> FilterByMZ(double minMZ, double maxMZ);

        new ISpectrum<TPeak> FilterByIntensity(double minIntensity, double maxIntensity);

        new ISpectrum<TPeak> FilterByIntensity(IRange<double> intenistyRange);
   }

}