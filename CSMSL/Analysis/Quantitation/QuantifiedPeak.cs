using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeak
    {
        static double SignalToNoiseThreshold = 3.0;
        public double ExpMz;
	    double TheoMz;
	    double Resolution;
        int Charge;
	    double Intensity;
	    double Noise;
        internal QuantifiedScan QuantScanParent
        {
            set;
            get;
        }
		
	    public QuantifiedPeak(double exp = 0, int charge = 0, double intensity = 0, double noise = 0, double resolution = 0)
	    {
		    ExpMz = exp;
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

        public double DenormalizedIntensity(bool NoiseBandCap = false)
        {
            if (!NoiseBandCap)
            {
                return Intensity * InjectionTime;
            }
            else
            {
                if (SignalToNoise >= SignalToNoiseThreshold)
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
