using CSMSL.Spectral;

namespace CSMSL.IO
{
    public interface IMSDataScan
    {
        int SpectrumNumber {get;}
        int MsnOrder {get;}
        double RetentionTime {get;}
        Polarity Polarity {get;}
        ISpectrum MassSpectrum {get;}
        MZAnalyzerType MzAnalyzer {get;}
        DoubleRange MzRange { get; }
    }

    public interface IMSDataScan<out TSpectrum> : IMSDataScan
        where TSpectrum : ISpectrum
    {
        new TSpectrum MassSpectrum { get; }
    }
}
