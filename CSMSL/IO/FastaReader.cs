///////////////////////////////////////////////////////////////////////////
//  FastaReader.cs - Reads a text-based file using the fasta format       /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace CSMSL.IO
{
    public class FastaReader : IDisposable
    {
        public char[] Delimiters = { '>' };
        private readonly StreamReader _reader;

        public int LineNumber { get; private set; }

        public Stream BaseStream
        {
            get { return _reader.BaseStream; }
        }

        public FastaReader(string fileName)
        {
            FileName = fileName;
            LineNumber = 0;
            _reader = new StreamReader(fileName);
        }

        public string FileName { get; set; }

        public void Close()
        {
            _reader.Close();
            LineNumber = 0;
        }

        public void Dispose()
        {
            Close();
        }

        public IEnumerable<Fasta> ReadNextFasta()
        {
            StringBuilder sequenceSb = new StringBuilder(50);
            StringBuilder headerSb = new StringBuilder(80);
            foreach (string line in ReadNextLine().Where(line => !string.IsNullOrEmpty(line)))
            {
                if (Array.IndexOf(Delimiters, line[0]) >= 0)
                {
                    if (sequenceSb.Length > 0)
                    {
                        yield return new Fasta(sequenceSb.ToString(), headerSb.ToString());
                        sequenceSb.Clear();
                        headerSb.Clear();
                    }
                    headerSb.Append(line.TrimStart(Delimiters));
                }
                else
                {
                    sequenceSb.Append(line);
                }
            }
            yield return new Fasta(sequenceSb.ToString(), headerSb.ToString());
        }


        public IEnumerable<Protein> ReadNextProtein()
        {
            return ReadNextFasta().Select(f => new Protein(f.Sequence, f.Description));
        }

        public override string ToString()
        {
            return FileName;
        }

        private IEnumerable<string> ReadNextLine()
        {
            while (_reader.Peek() >= 0)
            {
                LineNumber++;
               
                yield return _reader.ReadLine();
            }
        }
    }
}