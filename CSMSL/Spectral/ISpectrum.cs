using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public interface ISpectrum : IEnumerable<MZPeak>
    {
        int Count { get; }
        double[] GetMasses();
        double[] GetIntensities();
        double GetBasePeakIntensity();
        double GetTotalIonCurrent();
        bool TryGetIntensities(double minMZ, double maxMZ, out double intensity);
        bool TryGetIntensities(IRange<double> rangeMZ, out double intensity);
        MZPeak GetClosestPeak(double minMZ, double maxMZ);
        MZPeak GetClosestPeak(IRange<double> rangeMZ);
        byte[] ToBytes(bool zlibCompressed);  
    }
}
