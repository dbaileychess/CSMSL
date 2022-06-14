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
using System.Text;

namespace CSMSL.IO
{
    public class Dta
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public double PrecursorMass { get; set; }

        public int PrecursorCharge { get; set; }

        public MZSpectrum MzSpectrum { get; set; }

        public Dta(string name, int id, double precursorMass, int precursorCharge, MZSpectrum mzSpectrum)
        {
            Name = name;
            ID = id;
            PrecursorMass = precursorMass;
            PrecursorCharge = precursorCharge;
            MzSpectrum = mzSpectrum;
        }

        public string ToOutput(string precursormassFormat = "F5", string massFormat = "F4", string intensityFormat = "F2")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<dta id=\"{0}\" name=\"{1}\">", ID, Name);
            sb.AppendLine();
            sb.AppendFormat("{0} {1}", PrecursorMass.ToString(precursormassFormat), PrecursorCharge);
            sb.AppendLine();
            foreach (var peak in MzSpectrum)
            {
                sb.AppendFormat(" {0} {1}", peak.MZ.ToString(massFormat), peak.Intensity.ToString(intensityFormat));
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}