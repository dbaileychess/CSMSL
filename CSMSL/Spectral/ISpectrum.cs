
namespace CSMSL.Spectral
{
    public interface ISpectrum
    {
        int Count { get; }
        double[] GetMasses();
        double[] GetIntensities();
        double GetBasePeakIntensity();
        double GetTotalIonCurrent();
        bool TryGetIntensities(double minMZ, double maxMZ, out double intensity);
        bool TryGetIntensities(IRange<double> rangeMZ, out double intensity);
        IPeak GetClosestPeak(double minMZ, double maxMZ);
        IPeak GetClosestPeak(IRange<double> rangeMZ);
        byte[] ToBytes(bool zlibCompressed);  
    }
}
