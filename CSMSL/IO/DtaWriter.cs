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
using System.Collections.Generic;
using System.IO;

namespace CSMSL.IO
{
    public sealed class DtaWriter : IDisposable
    {
        public string FilePath { get; private set; }

        private readonly StreamWriter _writer;

        public DtaWriter(string filename)
        {
            FilePath = filename;
            _writer = new StreamWriter(filename) {AutoFlush = true};
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }

        public void Write(Dta dta)
        {
            _writer.WriteLine(dta.ToOutput());
        }

        public void Write(IEnumerable<Dta> dtas)
        {
            foreach (Dta dta in dtas)
            {
                _writer.WriteLine(dta.ToOutput());
            }
        }
    }
}