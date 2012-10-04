using CSMSL.Spectral;

namespace CSMSL.IO.Thermo
{
    public class ThermoLabeledPeak : Peak, IPeak
    {
        private short _charge;

        public short Charge
        {
            get { return _charge; }
        }

        private float _noise;

        public float Noise
        {
            get { return _noise; }
        }
                
        public float SN
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

        public ThermoLabeledPeak(double mz, float intensity, short charge, float noise)
            : base(mz, intensity)
        {
            _charge = charge;
            _noise = noise;
        }
    }
}