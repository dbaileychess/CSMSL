// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Dta.cs) is part of CSMSL.
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