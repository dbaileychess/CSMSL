using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CSMSL.IO.MzTab
{
    public class MzTabWriter : IDisposable
    {
        private readonly StreamWriter _writer;

        public string FilePath { get; private set; }
        public bool IsOpen { get; private set; }
        
        private MzTab.States _currentState;
   
        public MzTabWriter(string filePath)
        {
            FilePath = filePath;
            _writer = new StreamWriter(FilePath, false, MzTab.DefaultEncoding);
            IsOpen = true;
            _currentState = MzTab.States.None;
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

            _writeMetaData(MzTab.MDVersionField, metaData.Version);
            _writeMetaData(MzTab.MDModeField, metaData.Mode);
            _writeMetaData(MzTab.MDTypeField, metaData.Type);
            _writeMetaData(MzTab.MDIDField, metaData.ID); 
            _writeMetaData(MzTab.MDTitleField, metaData.Title);
            _writeMetaData(MzTab.MDDescriptionField, metaData.Description);
            _writeMetaData(metaData.GetListValues(MzTab.MDProteinSearchEngineScoreField));
            _writeMetaData(metaData.GetListValues(MzTab.MDPsmSearchEngineScoreField));
            _writeMetaData(metaData.GetListValues(MzTab.MDFixedModField));
            _writeMetaData(metaData.GetListValues(MzTab.MDVariableModField));
            _writeMetaData(MzTab.MDProteinQuantificationUnit, metaData.ProteinQuantificationUnit);
            _writeMetaData(metaData.GetListValues(MzTab.MDMsRunLocationField));
            _writeMetaData(metaData.GetListValues(MzTab.MDStudyVariableDescriptionField));

        }

        private void _writeMetaData(IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (var datum in data)
            {
                _writer.Write(MzTab.MetaDataLinePrefix);
                _writer.Write(MzTab.FieldSeparator);
                _writer.Write(datum.Key);
                _writer.Write(MzTab.FieldSeparator);
                _writer.WriteLine(datum.Value);
            }
        }

        private void _writeMetaData(string key, object value)
        {
            if (value == null)
                return;
            _writer.Write(MzTab.MetaDataLinePrefix);
            _writer.Write(MzTab.FieldSeparator);
            _writer.Write(key);
            _writer.Write(MzTab.FieldSeparator);
            _writer.WriteLine(value);
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

        #region PSM Section

        private void WritePsmHeader(int numberOfEngines, bool includeRelibaility = false, bool includeURI = false, IEnumerable<string> optionalFields = null)
        {
            List<string> headers = new List<string>();

            headers.Add(MzTab.PSMSequenceField);
            headers.Add(MzTab.PSMIdField);
            headers.Add(MzTab.PSMAcessionField);
            headers.Add(MzTab.PSMUniqueField);
            headers.Add(MzTab.PSMDatabaseField);
            headers.Add(MzTab.PSMDatabaseVersionField);
            headers.Add(MzTab.PSMSearchEngineField);
            
            for (int i = 1; i <= numberOfEngines; i++)
            {
                headers.Add(MzTab.PSMSearchEngineScoreField + "[" + i + "]");
            }

            if (includeRelibaility)
                headers.Add(MzTab.PSMRelibailityField);

            headers.Add(MzTab.PSMModificationsField);
          
            headers.Add(MzTab.PSMRetentionTimeField);
            headers.Add(MzTab.PSMChargeField);
            headers.Add(MzTab.PSMExperimentalMZField);
            headers.Add(MzTab.PSMTheoreticalMZField);

            if (includeURI)
                headers.Add(MzTab.PSMRelibailityField);

            headers.Add(MzTab.PSMSpectraReferenceField);
            headers.Add(MzTab.PSMPreviousAminoAcidField);
            headers.Add(MzTab.PSMFollowingAminoAcidField);
            headers.Add(MzTab.PSMStartResidueField);
            headers.Add(MzTab.PSMEndResidueField);

            if(optionalFields != null)
                headers.AddRange(optionalFields);
         
            WritePsmHeader(headers.ToArray());
        }

        private void WritePsmHeader(params object[] data)
        {
            if ((_currentState & MzTab.States.PsmHeader) == MzTab.States.PsmHeader)
            {
                throw new ArgumentException("Unable to write more than one PSM table to a single mzTab file");
            }

            _currentState |= MzTab.States.PsmHeader;

            WriteLine(MzTab.LinePrefix.PsmTable, data);
        }

        public void WritePsmData(IList<MzTabPSM> psms)
        {
            bool includeRelibaility = psms.Any(psm => psm.Reliability.HasValue);
            bool includeURI = psms.Any(psm => psm.Uri != null);
            int maxSearchEngine = psms.Max(psm => psm.NumberOfSearchEngines);
            string[] optionalParameters = psms.Where(psm => psm.GetOptionalFields() != null).SelectMany(psm => psm.GetOptionalFields()).ToArray();

            WritePsmHeader(maxSearchEngine, includeRelibaility, includeURI, optionalParameters);

            foreach (MzTabPSM psm in psms)
            {
                List<object> data = new List<object>();
                data.Add(psm.Sequence);
                data.Add(psm.ID);
                data.Add(psm.Accession);
                data.Add((psm.Unique) ? 1 : 0);
                data.Add(psm.Database);
                data.Add(psm.DatabaseVersion);
                data.Add(psm.SearchEngines);

                for (int i = 1; i <= maxSearchEngine; i++)
                {
                    data.Add(psm.GetEngineScore(i));
                }

                if (includeRelibaility)
                {
                    int? reliability = psm.Reliability;
                    if (reliability.HasValue)
                    {
                        data.Add(reliability.Value);
                    }
                    else
                    {
                        data.Add("null");
                    }
                }

                data.Add(psm.Modifications);
                data.Add(psm.RetentionTime);
                data.Add(psm.Charge);
                data.Add(psm.ExperimentalMZ);
                data.Add(psm.TheoreticalMZ);

                if (includeURI)
                    data.Add(psm.Uri);

                data.Add(psm.SpectraReference);
                data.Add(psm.PreviousAminoAcid);
                data.Add(psm.FollowingAminoAcid);
                data.Add(psm.StartResiduePosition);
                data.Add(psm.EndResiduePosition);

                if (optionalParameters != null)
                {
                    data.AddRange(optionalParameters.Select(optionalParameter => psm.GetOptionalData(optionalParameter)));
                }

                WriteLine(MzTab.LinePrefix.PsmData, data.ToArray());
            }
        }

        #endregion

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

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
            IsOpen = false;
        }
    }
}
