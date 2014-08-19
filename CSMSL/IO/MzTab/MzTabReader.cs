// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabReader.cs) is part of CSMSL.
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
using System.Data;
using System.IO;
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public sealed class MzTabReader : IDisposable
    {
        #region Public Properties

        public string FilePath { get; private set; }
        public bool IsOpen { get; private set; }

        public MzTabMetaData MetaData { get; private set; }

        #endregion

        private StreamReader _reader;
        private MzTab.States _currentState;
        private readonly DataSet _dataSet;

        #region Constructors

        public MzTabReader(string filePath, bool ignoreComments = true)
        {
            IsOpen = false;
            FilePath = filePath;
            MetaData = new MzTabMetaData();
            _ignoreComments = ignoreComments;
            _dataSet = new DataSet(FilePath) {CaseSensitive = MzTab.CaseSensitive};
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens up the mzTab file and parses all the information into memory
        /// </summary>
        public void Open()
        {
            var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _reader = new StreamReader(stream, MzTab.DefaultEncoding, true);
            LoadData();
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
            IsOpen = false;
        }

        public string[] GetColumns(MzTabSection section)
        {
            switch (section)
            {
                case MzTabSection.PSM:
                    return _psmDataTable != null ? _psmDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray() : null;
                case MzTabSection.Peptide:
                    return _peptideDataTable != null ? _peptideDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray() : null;
                case MzTabSection.Protein:
                    return _proteinDataTable != null ? _proteinDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray() : null;
                case MzTabSection.SmallMolecule:
                    return _smallMoleculeDataTable != null ? _smallMoleculeDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray() : null;
                default:
                    return null;
            }
        }

        public bool ContainsColumn(MzTabSection section, string columnName)
        {
            switch (section)
            {
                case MzTabSection.PSM:
                    return _psmDataTable != null && _psmDataTable.Columns.Contains(columnName);
                case MzTabSection.Peptide:
                    return _peptideDataTable != null && _peptideDataTable.Columns.Contains(columnName);
                case MzTabSection.Protein:
                    return _proteinDataTable != null && _proteinDataTable.Columns.Contains(columnName);
                case MzTabSection.SmallMolecule:
                    return _smallMoleculeDataTable != null && _smallMoleculeDataTable.Columns.Contains(columnName);
                default:
                    return false;
            }
        }

        public string GetData(MzTabSection section, int index, string columnName)
        {
            switch (section)
            {
                case MzTabSection.PSM:
                    return _psmDataTable != null ? (string) _psmDataTable.Rows[index][columnName] : null;
                case MzTabSection.Peptide:
                    return _peptideDataTable != null ? (string) _peptideDataTable.Rows[index][columnName] : null;
                case MzTabSection.Protein:
                    return _proteinDataTable != null ? (string) _proteinDataTable.Rows[index][columnName] : null;
                case MzTabSection.SmallMolecule:
                    return _smallMoleculeDataTable != null ? (string) _smallMoleculeDataTable.Rows[index][columnName] : null;
                default:
                    return null;
            }
        }

        public string[] GetData(MzTabSection section, int index)
        {
            switch (section)
            {
                case MzTabSection.PSM:
                    return _psmDataTable != null ? (string[]) _psmDataTable.Rows[index].ItemArray : null;
                case MzTabSection.Peptide:
                    return _peptideDataTable != null ? (string[]) _peptideDataTable.Rows[index].ItemArray : null;
                case MzTabSection.Protein:
                    return _proteinDataTable != null ? (string[]) _proteinDataTable.Rows[index].ItemArray : null;
                case MzTabSection.SmallMolecule:
                    return _smallMoleculeDataTable != null ? (string[]) _smallMoleculeDataTable.Rows[index].ItemArray : null;
                default:
                    return null;
            }
        }

        public string this[MzTabSection section, int index, string columnName]
        {
            get { return GetData(section, index, columnName); }
        }

        public string[] this[MzTabSection section, int index]
        {
            get { return GetData(section, index); }
        }

        #endregion

        #region Private Methods

        private void LoadData()
        {
            int lineNumber = 0;
            while (!_reader.EndOfStream)
            {
                lineNumber++;

                // Read the next line
                string line = _reader.ReadLine();

                // Empty lines are ignored
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Split the line into different parts
                string[] data = line.Split(MzTab.FieldSeparator);

                // Get the line prefix of the current line
                string linePrefix = data[0];

                // Jump to the method that handles each of the different line prefixes
                switch (linePrefix)
                {
                        // Comments
                    case MzTab.CommentLinePrefix:
                        ReadComment(data, lineNumber);
                        break;

                        // MetaData
                    case MzTab.MetaDataLinePrefix:
                        ReadMetaData(data, lineNumber);
                        break;

                        // Table Headers
                    case MzTab.ProteinTableLinePrefix:
                        _proteinDataTable = _dataSet.Tables.Add(Enum.GetName(typeof (MzTabSection), MzTabSection.Protein));
                        ReadTableDefinition(MzTab.States.ProteinHeader, data, _proteinDataTable);
                        break;
                    case MzTab.PeptideTableLinePrefix:
                        _peptideDataTable = _dataSet.Tables.Add(Enum.GetName(typeof (MzTabSection), MzTabSection.Peptide));
                        ReadTableDefinition(MzTab.States.PeptideHeader, data, _peptideDataTable);
                        break;
                    case MzTab.PsmTableLinePrefix:
                        _psmDataTable = _dataSet.Tables.Add(Enum.GetName(typeof (MzTabSection), MzTabSection.PSM));
                        ReadTableDefinition(MzTab.States.PsmHeader, data, _psmDataTable);
                        break;
                    case MzTab.SmallMoleculeTableLinePrefix:
                        _smallMoleculeDataTable = _dataSet.Tables.Add(Enum.GetName(typeof (MzTabSection), MzTabSection.SmallMolecule));
                        ReadTableDefinition(MzTab.States.SmallMoleculeHeader, data, _smallMoleculeDataTable);
                        break;

                        // Table Data
                    case MzTab.ProteinDataLinePrefix:
                        ReadDataTable(MzTab.States.ProteinData, data, _proteinDataTable);
                        break;
                    case MzTab.PeptideDataLinePrefix:
                        ReadDataTable(MzTab.States.PeptideData, data, _peptideDataTable);
                        break;
                    case MzTab.PsmDataLinePrefix:
                        ReadDataTable(MzTab.States.PsmData, data, _psmDataTable);
                        break;
                    case MzTab.SmallMoleculeDataLinePrefix:
                        ReadDataTable(MzTab.States.SmallMoleculeData, data, _smallMoleculeDataTable);
                        break;

                        // If we got here, the line prefix is not valid
                    default:
                        CheckError(line, lineNumber);
                        break;
                }
            }
            IsOpen = true;
        }

        private void CheckError(string line, int lineNumber)
        {
            Console.Error.WriteLine(line);
            throw new ArgumentException("Unable to correctly parse line #" + lineNumber);
        }

        private void ReadTableDefinition(MzTab.States headerState, string[] data, DataTable table)
        {
            if ((_currentState & MzTab.States.MetaData) != MzTab.States.MetaData)
            {
                throw new ArgumentException("The MetaData section MUST occur before the " + table.TableName + " Section. Invalid input file");
            }

            if ((_currentState & headerState) == headerState)
            {
                throw new ArgumentException("The " + table.TableName + " Table Header has already been parsed once, only one  " + table.TableName + " section is allowed per mzTab file.");
            }

            // Set the we have entered the current state
            _currentState |= headerState;

            int i = 1;
            while (i < data.Length && !string.IsNullOrWhiteSpace(data[i]))
            {
                table.Columns.Add(data[i].Trim());
                i++;
            }
        }

        private void ReadDataTable(MzTab.States dataState, object[] data, DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentException("No header information loaded for " + dataState + ", unable to parse data");
            }

            // Set the we have entered the current state
            _currentState |= dataState;

            // Add the row to the Protein data table
            table.Rows.Add(data.SubArray(1, table.Columns.Count));
        }

        private IEnumerable<T> GetObjects<T>(MzTabSection section) where T : MzTabEntity, new()
        {
            DataTable table = null;

            switch (section)
            {
                case MzTabSection.PSM:
                    table = _psmDataTable;
                    break;
                case MzTabSection.Peptide:
                    table = _peptideDataTable;
                    break;
                case MzTabSection.SmallMolecule:
                    table = _smallMoleculeDataTable;
                    break;
                case MzTabSection.Protein:
                    table = _proteinDataTable;
                    break;
            }

            if (table == null || table.Rows.Count == 0)
                yield break;

            string[] columns = GetColumns(section);
            int count = columns.Length;

            foreach (DataRow row in table.Rows)
            {
                var item = new T();
                for (int i = 0; i < count; i++)
                {
                    item.SetValue(columns[i], (string) row.ItemArray[i]);
                }
                yield return item;
            }
        }

        #endregion

        #region Peptide Section

        private DataTable _peptideDataTable;

        public bool ContainsPeptides
        {
            get { return _peptideDataTable != null && _peptideDataTable.Rows.Count > 0; }
        }

        public int NumberOfPeptides
        {
            get { return (ContainsPeptides) ? _peptideDataTable.Rows.Count : 0; }
        }

        public IEnumerable<MzTabPeptide> GetPeptides()
        {
            return GetObjects<MzTabPeptide>(MzTabSection.Peptide);
        }

        #endregion

        #region Small Molecule Section

        private DataTable _smallMoleculeDataTable;

        public bool ContainsSmallMolecules
        {
            get { return _smallMoleculeDataTable != null && _smallMoleculeDataTable.Rows.Count > 0; }
        }

        public int NumberOfSmallMolecules
        {
            get { return (ContainsSmallMolecules) ? _smallMoleculeDataTable.Rows.Count : 0; }
        }

        public IEnumerable<MzTabSmallMolecule> GetSmallMolecules()
        {
            return GetObjects<MzTabSmallMolecule>(MzTabSection.SmallMolecule);
        }

        #endregion

        #region PSM Section

        private DataTable _psmDataTable;

        public bool ContainsPsms
        {
            get { return _psmDataTable != null && _psmDataTable.Rows.Count > 0; }
        }

        public int NumberOfPsms
        {
            get { return (ContainsPsms) ? _psmDataTable.Rows.Count : 0; }
        }

        public IEnumerable<MzTabPSM> GetPsms()
        {
            return GetObjects<MzTabPSM>(MzTabSection.PSM);
        }

        #endregion

        #region Protein Section

        private DataTable _proteinDataTable;

        public bool ContainsProteins
        {
            get { return _proteinDataTable != null && _proteinDataTable.Rows.Count > 0; }
        }

        public int NumberOfProteins
        {
            get { return (ContainsProteins) ? _proteinDataTable.Rows.Count : 0; }
        }

        public IEnumerable<MzTabProtein> GetProteins()
        {
            return GetObjects<MzTabProtein>(MzTabSection.Protein);
        }

        #endregion

        #region Comment Section

        private DataTable _commentsDataTable;
        private readonly bool _ignoreComments;

        public bool ContainsComments
        {
            get { return _commentsDataTable != null; }
        }

        private void ReadComment(string[] data, int lineNumber)
        {
            // Do nothing with the comment if we aren't storing them
            if (_ignoreComments)
                return;

            // Create the comment table if it doesn't exist
            if (_commentsDataTable == null)
            {
                _commentsDataTable = _dataSet.Tables.Add("Comments");
                _commentsDataTable.Columns.Add("lineNumber", typeof (int));
                _commentsDataTable.Columns.Add("comment");
            }

            // TODO comments can contain tabs, so figure out how to read all the fields
            // The comment should be the second thing in the data array
            string comment = data[1];

            _commentsDataTable.Rows.Add(lineNumber, comment);
        }

        #endregion

        #region MetaData Section

        private void ReadMetaData(string[] data, int lineNumber)
        {
            // Set that we have enter in Metadata section
            _currentState |= MzTab.States.MetaData;

            // Grab the key-value pair, which should correspond to index 1 and 2, respectively
            string key = data[1];
            string value = data[2];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("No key was specified in the metadata section at line #" + lineNumber);
            }

            MetaData.SetValue(key, value);
        }

        #endregion
    }
}