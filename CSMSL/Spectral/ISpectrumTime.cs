using System;
using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public interface ISpectrumTime : ISpectrumTime<MZPeak> { }

    public interface ISpectrumTime<out T> : ISpectrum<T> where T : IPeak
    {    
        /// <summary>
        /// The time associated with this spectrum
        /// </summary>
        double Time { get; }
    }

    public static class ISpectrumTimeExtension
    {
        public static Chromatogram GetExtractedIonChromatogram(this IEnumerable<ISpectrumTime> spectra, MzRange range)
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

        public static Chromatogram GetClosetsPeakChromatogram(this IEnumerable<ISpectrumTime> spectra, MzRange range)
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
