using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CSMSL.Spectral;

namespace CSMSL.IO
{
    public class DtaReader : IDisposable
    {
        private static Regex dtaheaderRegex = new Regex(@"id=""(\d+)""\s*name=""(.+)""", RegexOptions.Compiled);

        private StreamReader _reader;

        public DtaReader(string filePath)
        {
            FilePath = filePath;

            _reader = new StreamReader(FilePath);
        }

        public string FilePath { get; set; }

        public void Close()
        {
            _reader.Close();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public IEnumerable<Dta> ReadNextDta()
        {
            List<double> mz = new List<double>();
            List<double> intensities = new List<double>();
            
            string name ="";
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
                    if(!first) {
                        yield return new Dta(name, id, precursorMass, precursorCharge, new Spectrum(mz.ToArray(), intensities.ToArray(), false)); 
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
                    string[] data= line.Trim().Split(' ');
                  
                    if(precursor) {
                        precursor = false;
                        precursorMass = double.Parse(data[0]);
                        precursorCharge = int.Parse(data[1]);
                    } else {
                        mz.Add(double.Parse(data[0]));
                        intensities.Add(double.Parse(data[1]));
                    }
                }
            }
       
            yield return new Dta(name, id, precursorMass, precursorCharge, new Spectrum(mz.ToArray(), intensities.ToArray(), false)); 
        }
    }
}
