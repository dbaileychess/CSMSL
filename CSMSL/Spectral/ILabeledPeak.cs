
using System;

namespace CSMSL.Spectral
{
    [Obsolete]
    public interface ILabeledPeak: IPeak
    {
        double Noise { get; }
        int Charge { get; }

        double GetSignalToNoise();
        double GetDenormalizedIntensity(double injectionTime);
    }
}
