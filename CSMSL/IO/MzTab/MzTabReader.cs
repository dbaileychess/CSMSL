using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;

namespace CSMSL.IO.MzTab
{
    public sealed class MzTabReader : IDisposable
    {
        public string FilePath { get; private set; }
        public bool IsOpen { get; private set; }

        #region Comment Section

        private DataTable _commentsDataTable;
        private readonly bool _ignoreComments;
        public bool ContainsComments { get { return _commentsDataTable != null; } }

        #endregion

        #region PSM Section

        private DataTable _psmDataTable;
        public bool ContainsPsms { get { return _psmDataTable != null; } }
        public int NumberOfPsms { get { return (ContainsPsms) ? _psmDataTable.Rows.Count : 0; } }

        #endregion

        #region Modifications

        private DataTable _modDataTable;

        #endregion

        private StreamReader _reader;
       
        private Dictionary<int, string> _msRunLocations;
        private Dictionary<int, string> _fixedModifications;
        private Dictionary<int, string> _variableModifications; 
        
        public MzTab.MzTabMode Mode { get; private set; }
        public MzTab.MzTabType Type { get; private set; }
        public string Version { get; private set; }
        public string Description { get; private set; }

        public MzTabReader(string filePath, bool ignoreComments = true)
        {
            IsOpen = false;
            FilePath = filePath;
            _ignoreComments = ignoreComments;
            _dataSet = new DataSet(FilePath) {CaseSensitive = MzTab.CaseSensitive};
            _metaDataTable = _dataSet.Tables.Add("MetaData");
            _metaDataTable.Columns.Add("key");
            _metaDataTable.Columns.Add("value");
            _modDataTable = _dataSet.Tables.Add("Modifications");
            _modDataTable.Columns.Add("key");
            _modDataTable.Columns.Add("value");
            _modDataTable.Columns.Add("isFixed");
        }

        public void Open()
        {
            var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _reader = new StreamReader(stream, MzTab.DefaultEncoding, true);
            IsOpen = true;
            Read();
        }
        
        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
            IsOpen = false;
        }

        private MzTab.States _currentState;

        private readonly DataSet _dataSet;
     
        private readonly DataTable _metaDataTable;
     

        private void Read()
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
                    case MzTab.CommentLinePrefix:
                        ReadComment(data, lineNumber);
                        break;
                    case MzTab.MetaDataLinePrefix:
                        ReadMetaData(data, lineNumber);
                        break;
                    case MzTab.ProteinTableLinePrefix:
                        ReadProteinTable(data, lineNumber);
                        break;
                    case MzTab.ProteinDataLinePrefix:
                        ReadProteinData(data, lineNumber);
                        break;
                    case MzTab.PeptideDataLinePrefix:
                        ReadPeptideData(data, lineNumber);
                        break;
                    case MzTab.PeptideTableLinePrefix:
                        ReadPeptideTable(data, lineNumber);
                        break;
                    case MzTab.PsmTableLinePrefix:
                        ReadPSMHeader(data, lineNumber);
                        break;
                    case MzTab.PsmDataLinePrefix:
                        ReadPSMData(data, lineNumber);
                        break;
                    case MzTab.SmallMoleculeTableLinePrefix:
                        ReadSmallMoleculeTable(data, lineNumber);
                        break;
                    case MzTab.SmallMoleculeDataLinePrefix:
                        ReadSmallMoleculeData(data, lineNumber);
                        break;
                    // If we got here, the line prefix is not valid
                    default:
                        CheckError(line, lineNumber);
                        break;
                }
            }
        }

        private void ReadPeptideTable(string[] data, int lineNumber)
        {
            throw new NotImplementedException();
        }

        private void ReadPeptideData(string[] data, int lineNumber)
        {
            throw new NotImplementedException();
        }

        private void ReadSmallMoleculeData(string[] data, int lineNumber)
        {
            throw new NotImplementedException();
        }

        private void ReadSmallMoleculeTable(string[] data, int lineNumber)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, int> _psmHeaderIndices;

        private void ReadPSMHeader(string[] data, int lineNumber)
        {
            if ((_currentState & MzTab.States.PsmHeader) == MzTab.States.PsmHeader)
            {
                throw new ArgumentException("Already parsed one PSM header line, mzTab files only support one PSM section per file");
            }

            // set that we enter in PSM Header section
            _currentState |= MzTab.States.PsmHeader;
            
            // Add the PSM table to the dataset
            _psmDataTable = _dataSet.Tables.Add("PSM");
            
            int i = 1;
            while (i < data.Length && !string.IsNullOrWhiteSpace(data[i]))
            {
                _psmDataTable.Columns.Add(data[i].Trim());
                i++;
            }

            // Add validation?
        }
   
        private void ReadPSMData(string[] data, int lineNumber)
        {
            if ((_currentState & MzTab.States.PsmHeader) != MzTab.States.PsmHeader)
            {
                throw new ArgumentException("No PSM header information loaded, unable to parse PSM data");
            }
            
            // Set that we have entered the PSM data section
            _currentState |= MzTab.States.PsmData;
            
            // Add the row to the PSM data table
            _psmDataTable.Rows.Add(data.SubArray(1, data.Length - 1));

            // Loop over each column up to the maximum defined by the PSM header
            //for (int i = 1; i < _maxPSMIndex; i++)
            //{
            //    string key = _psmHeader[i].Trim();
            //    string value = data[i];


            //    row[key] = value;
            //    continue;
            //    switch (key)
            //    {
            //        case MzTab.PSMSequenceField:
            //            psm.Sequence = value.ToUpper();
            //            break;
            //        case MzTab.PSMIdField:
            //            psm.ID = int.Parse(value);
            //            break;
            //        case MzTab.PSMAcessionField:
            //            psm.Accession = value;
            //            break;
            //        case MzTab.PSMUniqueField:
            //            psm.Unique = value == "1";
            //            break;
            //        case MzTab.PSMDatabaseField:
            //            psm.Database = value;
            //            break;
            //        case MzTab.PSMDatabaseVersionField:
            //            psm.DatabaseVersion = value;
            //            break;
            //        case MzTab.PSMSearchEngineField:
            //            psm.SearchEngines = value;
            //            break;
            //        case MzTab.PSMRelibailityField:
            //            psm.Reliability = int.Parse(value);
            //            break;
            //        case MzTab.PSMModificationsField:
            //            psm.Modifications = value;
            //            break;
            //        case MzTab.PSMRetentionTimeField:
            //            psm.RetentionTime = double.Parse(value);
            //            break;
            //        case MzTab.PSMChargeField:
            //            psm.Charge = int.Parse(value);
            //            break;
            //        case MzTab.PSMExperimentalMZField:
            //            psm.ExperimentalMZ = double.Parse(value);
            //            break;
            //        case MzTab.PSMTheoreticalMZField:
            //            psm.TheoreticalMZ = double.Parse(value);
            //            break;
            //        case MzTab.PSMUriField:
            //            psm.URI = value;
            //            break;
            //        case MzTab.PSMSpectraReferenceField:
            //            psm.SpectraReference = value;
            //            break;
            //        case MzTab.PSMPreviousAminoAcidField:
            //            psm.PreviousAminoAcid = value[0];
            //            break;
            //        case MzTab.PSMFollowingAminoAcidField:
            //            psm.FollowingAminoAcid = value[0];
            //            break;
            //        case MzTab.PSMStartResidueField:
            //            psm.StartResiduePosition = int.Parse(value);
            //            break;
            //        case MzTab.PSMEndResidueField:
            //            psm.EndResiduePosition = int.Parse(value);
            //            break;
            //        default:
            //            if (key.StartsWith("opt_"))
            //            {
            //                psm.AddOptionalField(key, value);
            //            }
            //            else if (key.StartsWith(MzTab.PSMSearchEngineScoreField))
            //            {
            //                int index = key.Contains('[') ? MzTab.GetIndex(key) : 1;
            //                psm.SetEngineScore(index, value);
            //            }
            //            else
            //            {
            //                throw new Exception("The PSM header " + key + " is not a valid header");
            //            }
            //            break;
            //    }
            //}

            //if (_psms == null)
            //    _psms = new List<MzTabPSM>();

            //_psms.Add(psm);
        }

        //private List<MzTabPSM> _psms; 

        private void ReadProteinData(string[] data, int lineNumber)
        {
            if ((_currentState & MzTab.States.ProteinHeader) != MzTab.States.ProteinHeader)
            {
                throw new ArgumentException("No protein header information loaded, unable to parse protein data");
            }

            // set that we enter in Protein Data section
            _currentState |= MzTab.States.ProteinData;
        }

        private void ReadProteinTable(string[] data, int lineNumber)
        {
            // set that we enter in Protein Header section
            _currentState |= MzTab.States.ProteinHeader;
        }
        
        private void ReadMetaData(string[] data, int lineNumber)
        {
            // Set that we have enter in Metadata section
            _currentState |= MzTab.States.MetaData;
            
            // Grab the key-value pair, which should correspond to index 1 and 2, respectively
            string key = data[1];
            string value = data[2];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("No key was specified in the metadata section at line #" + lineNumber);
            }

            // Handle mandatory metadata keys
            switch (key)
            {
                case MzTab.MDVersionField:
                    Version = value;
                    break;
                case MzTab.MDModeField:
                    Mode = (MzTab.MzTabMode)Enum.Parse(typeof(MzTab.MzTabMode), value, true);
                    break;
                case MzTab.MDTypeField:
                    Type = (MzTab.MzTabType)Enum.Parse(typeof(MzTab.MzTabType), value, true);
                    break;
                case MzTab.MDDescriptionField:
                    Description = value;
                    break;
                case MzTab.MDMsRunLocationField:
                    if (_msRunLocations == null)
                        _msRunLocations = new Dictionary<int, string>();
                    _msRunLocations.Add(1, value);
                    break;
                case MzTab.MDFixedModField:
                    if (_fixedModifications == null)
                        _fixedModifications = new Dictionary<int, string>();
                    _fixedModifications.Add(1, value);
                    break;
                case MzTab.MDVariableModField:
                     if (_variableModifications == null)
                         _variableModifications = new Dictionary<int, string>();
                     _variableModifications.Add(1, value);
                    break;
            }

            // Add the data to the MetaData table
            _metaDataTable.Rows.Add(key, value);
        }

        private void ReadComment(string[] data, int lineNumber)
        {
            // Do nothing with the comment if we aren't storing them
            if (_ignoreComments)
                return;

            if (_commentsDataTable == null)
            {
                _commentsDataTable = _dataSet.Tables.Add("Comments");
                _commentsDataTable.Columns.Add("lineNumber", typeof(int));
                _commentsDataTable.Columns.Add("comment");
            }

            // The comment should be the second thing in the data array
            string comment = data[1];

            _commentsDataTable.Rows.Add(lineNumber, comment);
        }

        private void CheckError(string line, int lineNumber)
        {
            throw new ArgumentException("Unable to correctly parse line #" + lineNumber);
        }
        
        public bool ContainsPsmField(string field)
        {
            return _psmDataTable.Columns.Contains(field);
        }

        public string GetPSMValue(string key, int psmIndex)
        {
            return _psmDataTable.Rows[psmIndex][key] as string;
        }

        //public string this[MzTab.LinePrefix prefix, int index, string field]
        //{
        //    get
        //    {
        //        return _psmDataTable.Rows[index][field] as string;
        //    }
        //}

        //public object[] this[MzTab.LinePrefix prefix, int index]
        //{
        //    get
        //    {
        //        return _psmDataTable.Rows[index].ItemArray;
        //    }
        //}
       
        public IEnumerable<MzTabPSM> GetPsms()
        {
            if (_psmDataTable == null)
                yield break;

            bool containsReliability = _psmDataTable.Columns.Contains(MzTab.PSMRelibailityField);
            bool containsUri = _psmDataTable.Columns.Contains(MzTab.PSMUriField);

        
            foreach (DataRow row in _psmDataTable.Rows)
            {
                MzTabPSM psm = new MzTabPSM
                {
                    Sequence = (string)row[MzTab.PSMSequenceField],
                    ID = int.Parse((string)row[MzTab.PSMIdField])
                };

                if(containsReliability)
                    psm.Reliability = int.Parse((string)row[MzTab.PSMRelibailityField]);

                if(containsUri)
                    psm.URI = (string)row[MzTab.PSMUriField];
                
                yield return psm;
            }
        }
    }
}
