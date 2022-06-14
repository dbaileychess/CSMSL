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

namespace CSMSL.Spectral
{
    public sealed class ChromatographicPeak : IPeak
    {
        public double Time { get; private set; }

        public double Intensity { get; private set; }

        public ChromatographicPeak(double time, double intensity)
        {
            Time = time;
            Intensity = intensity;
        }

        public override string ToString()
        {
            return string.Format("({0:G4}, {1:G4})", Time, Intensity);
        }

        public int CompareTo(double time)
        {
            return Time.CompareTo(time);
        }

        public int CompareTo(IPeak other)
        {
            return Time.CompareTo(other.X);
        }

        public int CompareTo(ChromatographicPeak other)
        {
            return Time.CompareTo(other.Time);
        }

        public int CompareTo(object other)
        {
            return 0;
        }

        double IPeak.X
        {
            get { return Time; }
        }

        double IPeak.Y
        {
            get { return Intensity; }
        }
    }
}