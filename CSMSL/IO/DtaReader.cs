// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (DtaReader.cs) is part of CSMSL.
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