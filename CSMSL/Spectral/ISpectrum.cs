// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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