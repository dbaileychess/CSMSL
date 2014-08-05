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

namespace CSMSL.Spectral
{
    public interface ISpectrum
    {
        /// <summary>
        /// The number of peaks in the spectrum
        /// </summary>
        int Count { get; }

        double[] GetMasses();

        double[] GetIntensities();

        double GetBasePeakIntensity();

        double GetTotalIonCurrent();

        bool TryGetIntensities(double minMZ, double maxMZ, out double intensity);

        bool TryGetIntensities(IRange<double> rangeMZ, out double intensity);

        byte[] ToBytes(bool zlibCompressed);

        IPeak GetClosestPeak(double minMZ, double maxMZ);

        IPeak GetClosestPeak(IRange<double> rangeMZ);
    }

    public interface ISpectrum<out T> : ISpectrum where T : IPeak
    {
        new T GetClosestPeak(double minMZ, double maxMZ);

        new T GetClosestPeak(IRange<double> rangeMZ);
    }

}