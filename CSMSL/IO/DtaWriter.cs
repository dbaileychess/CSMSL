using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSMSL.IO
{
    public class DtaWriter : IDisposable
    {        
        
        public string FilePath { get; private set; }
        private readonly StreamWriter _writer;

        public DtaWriter(string filename)
        {
            FilePath = filename;
            _writer = new StreamWriter(filename);
        }

        public void Close()
        {
            _writer.Flush();
            _writer.Close();
        }

        void IDisposable.Dispose()
        {
            Close();
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

