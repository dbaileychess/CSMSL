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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CSMSL.IO
{
    public sealed class DtaReader : IDisposable
    {
        private static Regex dtaheaderRegex = new Regex(@"id=""(\d+)""\s*name=""(.+)""", RegexOptions.Compiled);

        private StreamReader _reader;

        public DtaReader(string filePath)
        {
            FilePath = filePath;

            _reader = new StreamReader(FilePath);
        }

        public string FilePath { get; set; }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }

        public IEnumerable<Dta> ReadNextDta()
        {
            List<double> mz = new List<double>();
            List<double> intensities = new List<double>();

            string name = "";
            int id = -1;
            bool first = true;
            bool precursor = false;
            double precursorMass = 0;
            int precursorCharge = 0;
            while (!_reader.EndOfStream)
            {
                string line = _reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("<dta"))
                {
                    if (!first)
                    {
                        yield return new Dta(name, id, precursorMass, precursorCharge, new MZSpectrum(mz.ToArray(), intensities.ToArray(), false));
                    }

                    first = false;
                    var match = dtaheaderRegex.Match(line);
                    id = int.Parse(match.Groups[1].Value);
                    name = match.Groups[2].Value;
                    mz.Clear();
                    intensities.Clear();
                    precursor = true;
                }
                else
                {
                    string[] data = line.Trim().Split(' ');

                    if (precursor)
                    {
                        precursor = false;
                        precursorMass = double.Parse(data[0]);
                        precursorCharge = int.Parse(data[1]);
                    }
                    else
                    {
                        mz.Add(double.Parse(data[0]));
                        intensities.Add(double.Parse(data[1]));
                    }
                }
            }

            yield return new Dta(name, id, precursorMass, precursorCharge, new MZSpectrum(mz.ToArray(), intensities.ToArray(), false));
        }
    }
}