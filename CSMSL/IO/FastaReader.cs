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

using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSMSL.IO
{
    public sealed class FastaReader : IDisposable
    {
        private readonly StreamReader _reader;

        public char Delimiter { get; private set; }

        public FastaReader(string fileName, char delimiter = '>')
        {
            FileName = fileName;
            Delimiter = delimiter;
            _reader = new StreamReader(fileName);
        }

        public string FileName { get; set; }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }

        public IEnumerable<Fasta> ReadNextFasta()
        {
            StringBuilder sequenceSb = new StringBuilder(500);
            StringBuilder headerSb = new StringBuilder(80);

            while (!_reader.EndOfStream)
            {
                string line = _reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line[0] == Delimiter)
                {
                    if (sequenceSb.Length > 0)
                    {
                        yield return new Fasta(sequenceSb.ToString().TrimEnd('*'), headerSb.ToString());
                        sequenceSb.Clear();
                        headerSb.Clear();
                    }
                    headerSb.Append(line.TrimStart(Delimiter));
                }
                else
                {
                    sequenceSb.Append(line);
                }
            }
            if (sequenceSb.Length > 0)
                yield return new Fasta(sequenceSb.ToString().TrimEnd('*'), headerSb.ToString());
        }

        public IEnumerable<Protein> ReadNextProtein()
        {
            return ReadNextFasta().Select(f => new Protein(f.Sequence, f.Description));
        }

        public override string ToString()
        {
            return FileName;
        }

        public static int NumberOfEntries(string filePath, char delimiter = '>')
        {
            int entries = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] == delimiter)
                        entries++;
                }
            }
            return entries;
        }
    }
}