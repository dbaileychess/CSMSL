using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;

namespace CSMSL.IO.OMSSA
{
    public class OmssaCsvReader
    {
        public string FilePath { get; private set; }

        private CsvReader _reader;

        public OmssaCsvReader(string filePath)
        {
            FilePath = filePath;
            _reader = new CsvReader(new StreamReader(filePath));
        }

        public IEnumerable<OmssaPeptideSpectralMatch> Read() {
            return _reader.GetRecords<OmssaPeptideSpectralMatch>();
        }

    }
}
