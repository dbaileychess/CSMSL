
namespace CSMSL.Spectral
{
    public interface ISpectrum : ISpectrum<MZPeak> { }

    public interface ISpectrum<out T> where T : IPeak
    {
        int Count { get; }
        double[] GetMasses();
        double[] GetIntensities();
        double GetBasePeakIntensity();
        double GetTotalIonCurrent();
        bool TryGetIntensities(double minMZ, double maxMZ, out double intensity);
        bool TryGetIntensities(IRange<double> rangeMZ, out double intensity);
        T GetClosestPeak(double minMZ, double maxMZ);
        T GetClosestPeak(IRange<double> rangeMZ);
        byte[] ToBytes(bool zlibCompressed);  
    }
}
