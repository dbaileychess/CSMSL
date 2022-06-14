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