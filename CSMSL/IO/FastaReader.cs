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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSMSL.Proteomics;


namespace CSMSL.IO
{
    public class FastaReader : IDisposable
    {
        public char[] Delimiters = { '>' };
        private string _fileName;
        private StreamReader _reader;

        public int LineNumber { get { return _lineNumber; } }

        public Stream BaseStream
        {
            get { return _reader.BaseStream; }
        }
       

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
                if (string.IsNullOrEmpty(line))
                    continue;
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

        private int _lineNumber;

        private IEnumerable<string> ReadNextLine()
        {
            _lineNumber = 0;
            
            while (_reader.Peek() >= 0 )
            {
                _lineNumber++;
               
                yield return _reader.ReadLine();
               
            }
            yield break;
        }

      
    }
}