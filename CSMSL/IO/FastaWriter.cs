using System;
using System.Collections.Generic;
using System.IO;
using CSMSL.Proteomics;

namespace CSMSL.IO
{
    public class FastaWriter : IDisposable
    {
        private int _charperline;
        private char _delimiter;
        private string _filename;
        private StreamWriter _writer;

        public FastaWriter(string filename)
            : this(filename, '>', 80) { }

        public FastaWriter(string filename, char delimiter)
            : this(filename, delimiter, 80) { }

        public FastaWriter(string filename, int charperline)
            : this(filename, '>', charperline) { }

        public FastaWriter(string filename, char delimiter, int charperline)
        {
            _filename = filename;
            _delimiter = delimiter;
            _charperline = charperline;
            _writer = new StreamWriter(filename);
        }

        public void Close()
        {
            _writer.Flush();
            _writer.Close();
        }

        public void Dispose()
        {
            Close();
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
            _writer.WriteLine("{0}{1}", _delimiter, description);
            for (int i = 0; i < sequence.Length; i += _charperline)
            {
                _writer.WriteLine(sequence.Substring(i, (i + _charperline < sequence.Length) ?
                    _charperline :
                    sequence.Length - i));
            }
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }
    }
}