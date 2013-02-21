using CSMSL.Spectral;

namespace CSMSL.IO.Thermo
{
    public class ThermoLabeledPeak : MZPeak, ILabeledPeak
    {
        private int _charge;

        public int Charge
        {
            get { return _charge; }
        }

        private double _noise;

        public double Noise
        {
            get { return _noise; }
        }
                
        public double SN
        {
            get
            {
                if (Noise.Equals(0)) return float.NaN;
                return Intensity / Noise;
            }
        }

        public override string ToString()
        {
            string charge = "";
            if(Charge == 0) {
                charge = "?z";
            } else if (Charge > 0) {
                charge = "+"+Charge;
            } else {
                charge = "-"+Charge;
            }
            return string.Format("{0} {1} SN = {2:F2}",base.ToString(), charge, SN);
        }

        public ThermoLabeledPeak(double mz, double intensity, int charge, double noise)
            : base(mz, intensity)
        {
            _charge = charge;
            _noise = noise;
        }

        public double GetSignalToNoise()
        {
            return SN;
        }

        public double GetDenormalizedIntensity(double injectionTime)
        {
            return Intensity * injectionTime;
        }
    }
}