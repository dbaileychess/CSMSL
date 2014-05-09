using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Spectral;

namespace CSMSL.IO
{
    public class Dta
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double PrecursorMass { get; set; }
        public int PrecursorCharge { get; set; }
        public Spectrum Spectrum { get; set; }

        public Dta(string name, int id, double precursorMass, int precursorCharge, Spectrum spectrum)
        {
            Name = name;
            ID = ID;
            PrecursorMass = precursorMass;
            PrecursorCharge = precursorCharge;
            Spectrum = spectrum;
        }

        public string ToOutput(string precursormassFormat = "F5", string massFormat = "F4", string intensityFormat = "F2")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<dta id=\"{0}\" name=\"{1}\">", ID, Name);
            sb.AppendLine();
            sb.AppendFormat("{0} {1}", PrecursorMass.ToString(precursormassFormat), PrecursorCharge);
            sb.AppendLine();
            foreach (var peak in Spectrum)
            {
                sb.AppendFormat(" {0} {1}", peak.MZ.ToString(massFormat), peak.Intensity.ToString(intensityFormat));
                sb.AppendLine();
            }
            return sb.ToString();
        }
        
    }
}
