// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CSMSL.Spectral;

namespace CSMSL.IO.Thermo
{
    public class ThermoMzPeak : MZPeak
    {
        public int Charge { get; private set; }

        public double Noise { get; private set; }

        public double Resolution { get; private set; }

        public double SignalToNoise
        {
            get
            {
                if (Noise.Equals(0)) return float.NaN;
                return Intensity/Noise;
            }
        }

        public bool IsHighResolution { get { return Resolution > 0;} }

        public override string ToString()
        {
            return string.Format("{0} z = {1:+#;-#;?} SN = {2:F2}", base.ToString(), Charge, SignalToNoise);
        }

        public ThermoMzPeak()
        {
        }

        public ThermoMzPeak(double mz, double intensity, int charge = 0, double noise = 0.0, double resolution = 0.0)
            : base(mz, intensity)
        {
            Charge = charge;
            Noise = noise;
            Resolution = resolution;
        }

        public double GetSignalToNoise()
        {
            return SignalToNoise;
        }

        public double GetDenormalizedIntensity(double injectionTime)
        {
            return Intensity*injectionTime;
        }
    }
}