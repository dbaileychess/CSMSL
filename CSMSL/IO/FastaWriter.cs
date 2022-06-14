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

namespace CSMSL.IO
{
    public sealed class FastaWriter : IDisposable
    {
        public int CharactersPerLine { get; private set; }

        public char Delimiter { get; private set; }

        public string FilePath { get; private set; }

        private readonly StreamWriter _writer;

        public FastaWriter(string filename, char delimiter = '>', int charperline = 80)
        {
            FilePath = filename;
            Delimiter = delimiter;
            CharactersPerLine = charperline;
            _writer = new StreamWriter(filename) {AutoFlush = true};
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }

        public void Write(IEnumerable<Protein> proteins)
        {
            foreach (Protein protein in proteins)
            {
                Write(protein);
            }
        }

        public void Write(Protein protein)
        {
            Write(protein.Sequence, protein.Description);
        }

        public void Write(IEnumerable<Fasta> fastas)
        {
            foreach (Fasta fasta in fastas)
            {
                Write(fasta.Sequence, fasta.Description);
            }
        }

        public void Write(Fasta fasta)
        {
            Write(fasta.Sequence, fasta.Description);
        }

        public void Write(string sequence, string description)
        {
            _writer.WriteLine("{0}{1}", Delimiter, description);
            for (int i = 0; i < sequence.Length; i += CharactersPerLine)
            {
                _writer.WriteLine(sequence.Substring(i, (i + CharactersPerLine < sequence.Length) ?
                    CharactersPerLine :
                    sequence.Length - i));
            }
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }
    }
}