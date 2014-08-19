// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (CVParamater.cs) is part of CSMSL.
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

using System;
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

        public CVParamater(string label, string accession, string name, string value)
        {
            Label = label;
            Accession = accession;
            Name = name;
            Value = value;
        }

        public CVParamater(string parameter)
        {
            Match m = CvRegex.Match(parameter);
            if (!m.Success)
                throw new ArgumentException("Unable to parse this CV Parameter correctly: " + parameter);
            Label = m.Groups[1].Value.Trim();
            Accession = m.Groups[2].Value.Trim();
            Name = m.Groups[3].Value.Trim();
            Value = m.Groups[4].Value.Trim();
        }

        public override string ToString()
        {
            string name = Name;
            if (name.Contains(","))
            {
                name = "\"" + name + "\"";
            }
            return string.Format("[{0},{1},{2},{3}]", Label, Accession, name, Value);
        }

        public static implicit operator CVParamater(string parameter)
        {
            return new CVParamater(parameter);
        }
    }
}