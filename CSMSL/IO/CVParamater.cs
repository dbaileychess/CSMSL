using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSMSL.IO
{
    public class CVParamater
    {
        private static readonly Regex CvRegex = new Regex(@"\[(.*),(.*),(.*),(.*)\]", RegexOptions.Compiled);

        public string Label { get; set; }
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public CVParamater(string parameter)
        {
            Match m = CvRegex.Match(parameter);
            if (!m.Success)
                return;
            Label = m.Groups[1].Value.Trim();
            Accession = m.Groups[2].Value.Trim();
            Name = m.Groups[3].Value.Trim();
            Value = m.Groups[4].Value.Trim();
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2},{3}]", Label, Accession, Name, Value);
        }

        public static implicit operator CVParamater(string parameter)
        {
            return new CVParamater(parameter);
        }

    }
}
