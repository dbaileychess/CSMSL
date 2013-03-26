using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public interface ILabeledPeak: IPeak
    {
        double Noise { get; }
        int Charge { get; }

        double GetSignalToNoise();
        double GetDenormalizedIntensity(double injectionTime);
    }
}
