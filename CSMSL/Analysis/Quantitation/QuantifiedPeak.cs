using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Spectral;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeak : MZPeak
    {
        public double? Resolution { get; set; }
        public int? Charge { get; set; }
        public double? Noise { get; set; }
        public string Name { get; set; }
        public int IsotopeNumber { get; set; }
        public MSDataScan Spectrum { get; set; }  

        public QuantifiedPeak(MSDataScan spectrum = null, double mz = 0, int charge = 0, double intensity = 0, double noise = 1, double resolution = 0)
            : base (mz, intensity)
	    {
            Spectrum = spectrum;
            IsotopeNumber = 0;
            //Noise = noise;
            //Resolution = resolution;
	    }
		
	    public double? SignalToNoise
	    {
		    get
		    {
                if (Noise.HasValue)
                {
                    return Intensity / Noise.Value;
                }
                return null;
		    }
	    }

        public double DenomalizedIntensity
        {
            get { return Intensity * InjectionTime; }
        }

        public double InjectionTime
        {
            get
            {
                return Spectrum.InjectionTime;
            }
        }

        //public double DenormalizedIntensity(bool noiseBandCap = false, double signalToNoiseThreshold = 3.0)
        //{
        //    if (!noiseBandCap)
        //    {
        //        if (SignalToNoise >= signalToNoiseThreshold)
        //        {
        //            return Intensity * InjectionTime;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        if (SignalToNoise >= signalToNoiseThreshold)
        //        {
        //            return Intensity * InjectionTime;
        //        }
        //        else
        //        {
        //            if (Noise.HasValue)
        //            {
        //                return Noise.Value * InjectionTime;
        //            }
        //            return InjectionTime;
        //        }
        //    }
        //}

     
    }
}
