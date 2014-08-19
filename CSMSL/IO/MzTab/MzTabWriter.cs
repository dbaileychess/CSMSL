// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabWriter.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public class MzTabWriter : IDisposable
    {
        private readonly StreamWriter _writer;

        public string FilePath { get; private set; }
        public bool IsOpen { get; private set; }

        private MzTab.States _currentState;
        private MzTabMetaData _metaData;

        public MzTabWriter(string filePath)
        {
            FilePath = filePath;
            _writer = new StreamWriter(FilePath, false, MzTab.DefaultEncoding);
            IsOpen = true;
            _currentState = MzTab.States.None;
        }

        private void WriteData<T>(MzTabSection section, IEnumerable<T> data, bool includeCommentLine = false) where T : MzTabEntity
        {
            if ((_currentState & MzTab.States.MetaData) != MzTab.States.MetaData)
            {
                throw new ArgumentException("Unable to write the " + section + " section, incorrect location. The Meta Data section must come first.");
            }

            List<T> objects = data.ToList();

            if (objects.Count == 0)
                return;

            MzTab.LinePrefix headerPrefix = MzTab.LinePrefix.Comment;
            MzTab.LinePrefix prefix = MzTab.LinePrefix.Comment;

            switch (section)
            {
                case MzTabSection.SmallMolecule:
                    headerPrefix = MzTab.LinePrefix.SmallMoleculeTable;
                    prefix = MzTab.LinePrefix.SmallMoleculeData;
                    _currentState |= MzTab.States.SmallMoleculeData;
                    _currentState |= MzTab.States.SmallMoleculeHeader;
                    break;
                case MzTabSection.Peptide:
                    headerPrefix = MzTab.LinePrefix.PeptideTable;
                    prefix = MzTab.LinePrefix.PeptideData;
                    _currentState |= MzTab.States.PeptideData;
                    _currentState |= MzTab.States.PeptideHeader;
                    break;
                case MzTabSection.PSM:
                    headerPrefix = MzTab.LinePrefix.PsmTable;
                    prefix = MzTab.LinePrefix.PsmData;
                    _currentState |= MzTab.States.PsmData;
                    _currentState |= MzTab.States.PsmHeader;
                    break;
                case MzTabSection.Protein:
                    headerPrefix = MzTab.LinePrefix.ProteinTable;
                    prefix = MzTab.LinePrefix.ProteinData;
                    _currentState |= MzTab.States.ProteinData;
                    _currentState |= MzTab.States.ProteinHeader;
                    break;
            }

            // Write Header
            string[] header = MzTabEntity.GetHeader(objects).ToArray();
            WriteLine(headerPrefix, header);

            if (includeCommentLine)
            {
                WriteComment(string.Join("\t", header));
            }

            // Write table
            foreach (string[] values in objects.Select(datum => datum.GetStringValues(header).ToArray()))
            {
                WriteLine(prefix, values);
            }
        }

        #region Comment Section

        public void WriteComment(string comment)
        {
            WriteLine(MzTab.LinePrefix.Comment, comment);
        }

        #endregion

        #region MetaData

        public void WriteMetaData(MzTabMetaData metaData)
        {
            if (_currentState > MzTab.States.MetaData)
            {
                throw new ArgumentException("Unable to write Metadata, incorrect location. Only one Metadata section per file");
            }
            _currentState |= MzTab.States.MetaData;

            foreach (KeyValuePair<string, string> kvp in metaData.GetKeyValuePairs())
            {
                _writer.Write(MzTab.MetaDataLinePrefix);
                _writer.Write(MzTab.FieldSeparator);
                _writer.Write(kvp.Key);
                _writer.Write(MzTab.FieldSeparator);
                _writer.WriteLine(kvp.Value);
            }

            // Save meta data
            _metaData = metaData;
        }

        public void WriteMetaData(string key, object value)
        {
            if (_currentState > MzTab.States.MetaData)
            {
                throw new ArgumentException("Unable to write Metadata, incorrect location. Only one Metadata section per file");
            }
            _currentState |= MzTab.States.MetaData;

            WriteLine(MzTab.LinePrefix.MetaData, key, value);
        }

        #endregion

        public void WriteProteinData(IEnumerable<MzTabProtein> proteins, bool includeCommentLine = false)
        {
            WriteData(MzTabSection.Protein, proteins, includeCommentLine);
        }

        public void WritePsmData(IEnumerable<MzTabPSM> psms, bool includeCommentLine = false)
        {
            WriteData(MzTabSection.PSM, psms, includeCommentLine);
        }

        public void WritePeptideData(IEnumerable<MzTabPeptide> peptides, bool includeCommentLine = false)
        {
            WriteData(MzTabSection.Peptide, peptides, includeCommentLine);
        }

        public void WriteSmallMoleculeData(IEnumerable<MzTabSmallMolecule> smallMolecules, bool includeCommentLine = false)
        {
            WriteData(MzTabSection.SmallMolecule, smallMolecules, includeCommentLine);
        }

        /// <summary>
        /// Writes a blank line to the file, parsers should skip these lines
        /// </summary>
        public void WriteLine()
        {
            _writer.WriteLine();
        }

        /// <summary>
        /// Writes a line to output file as is, therefore it must be in the correct format or
        /// other parsers may not correctly read the line.
        /// </summary>
        /// <param name="line"></param>
        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }

        public void WriteLine(MzTab.LinePrefix prefix, string line)
        {
            _writer.Write(MzTab.GetLinePrefixText(prefix));
            _writer.Write(MzTab.FieldSeparator);
            _writer.WriteLine(line);
        }

        public void WriteLine(MzTab.LinePrefix prefix, params object[] data)
        {
            WriteLine(prefix, string.Join(MzTab.FieldSeparator.ToString(), data));
        }

        public void WriteLine(MzTab.LinePrefix prefix, params string[] data)
        {
            WriteLine(prefix, string.Join(MzTab.FieldSeparator.ToString(), data));
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
            IsOpen = false;
        }
    }
}