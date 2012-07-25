using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSMSL.Proteomics;

namespace CSMSL.IO
{
    public class Fasta
    {
        private string _description;
        private string _sequence;

        public Fasta(string sequence, string description)
        {
            _sequence = sequence;
            _description = description;
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
    }

    public class FastaReader : IDisposable
    {
        public char[] Delimiters = { '>' };
        private string _fileName;
        private StreamReader _reader;

        public FastaReader(string fileName)
        {
            _fileName = fileName;
            _reader = new StreamReader(fileName);
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public void Close()
        {
            _reader.Close();
        }

        public void Dispose()
        {
            Close();
        }

        public IEnumerable<Fasta> ReadNextFasta()
        {
            StringBuilder sequenceSB = new StringBuilder(50);
            StringBuilder headerSB = new StringBuilder(80);
            foreach (string line in ReadNextLine())
            {
                if (Array.IndexOf(Delimiters, line[0]) >= 0)
                {
                    if (sequenceSB.Length > 0)
                    {
                        yield return new Fasta(sequenceSB.ToString(), headerSB.ToString());
                        sequenceSB.Clear();
                        headerSB.Clear();
                    }
                    headerSB.Append(line.TrimStart(Delimiters));
                }
                else
                {
                    sequenceSB.Append(line);
                }
            }
            yield return new Fasta(sequenceSB.ToString(), headerSB.ToString());
            yield break;
        }

        public IEnumerable<Protein> ReadNextProtein()
        {
            foreach (Fasta f in ReadNextFasta())
            {
                yield return new Protein(f.Sequence, f.Description);
            }
            yield break;
        }

        public override string ToString()
        {
            return _fileName;
        }

        private IEnumerable<string> ReadNextLine()
        {
            while (_reader.Peek() >= 0)
                yield return _reader.ReadLine();
            yield break;
        }
    }
}