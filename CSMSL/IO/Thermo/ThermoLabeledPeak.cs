// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ThermoLabeledPeak.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

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
                return Intensity/Noise;
            }
        }

        public override string ToString()
        {
            string charge = "";
            if (Charge == 0)
            {
                charge = "?z";
            }
            else if (Charge > 0)
            {
                charge = "+" + Charge;
            }
            else
            {
                charge = "-" + Charge;
            }
            return string.Format("{0} {1} SN = {2:F2}", base.ToString(), charge, SN);
        }

        public ThermoLabeledPeak()
        {
        }

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
            return Intensity*injectionTime;
        }
    }
}