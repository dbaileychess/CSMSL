using CSMSL.Spectral;

namespace CSMSL.IO.Thermo
{
    public class ThermoLabeledPeak : MZPeak
    {
        public int Charge { get; private set; }

        public double Noise { get; private set; }

        public double Resolution { get; private set; }

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

        public ThermoLabeledPeak() { }

        public ThermoLabeledPeak(double mz, double intensity, int charge, double noise, double resolution)
            : base(mz, intensity)
        {
            Charge = charge;
            Noise = noise;
            Resolution = resolution;
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