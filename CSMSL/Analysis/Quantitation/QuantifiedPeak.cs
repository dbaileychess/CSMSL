using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeak
    {
        public double Mz;
	    double Resolution;
        int Charge;
	    double Intensity;
	    double Noise;
        internal QuantifiedScan QuantScanParent
        {
            set;
            get;
        }
		
	    public QuantifiedPeak(double mz = 0, int charge = 0, double intensity = 0, double noise = 0, double resolution = 0)
	    {
		    Mz = mz;
            Charge = charge;
		    Intensity = intensity;
		    Noise = noise;
		    Resolution = resolution;
	    }
		
	    public double SignalToNoise
	    {
		    get
		    {
			    return Intensity / Noise;
		    }
	    }

        public double InjectionTime
        {
            get
            {
                if (QuantScanParent == null)
                {
                    // An injection time of 1 will not affect peak intensity de-normalization
                    return 1.0;
                }
                return QuantScanParent.InjectionTime;
            }
        }

        public double DenormalizedIntensity(bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        {
            if (!noiseBandCap)
            {
                if (SignalToNoise >= signalToNoiseThreshold)
                {
                    return Intensity * InjectionTime;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (SignalToNoise >= signalToNoiseThreshold)
                {
                    return Intensity * InjectionTime;
                }
                else
                {
                    return Noise * InjectionTime;
                }
            }
        }
    }
}
