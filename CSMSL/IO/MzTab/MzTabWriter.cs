using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CSMSL.IO.MzTab
{
    public class MzTabWriter : IDisposable
    {
        private StreamWriter _writer;

        public string FilePath { get; private set; }
        public bool IsOpen { get; private set; }

        public MzTab.MzTabMode Mode { get; private set; }
        public MzTab.MzTabType Type { get; private set; }
        public string Version { get; private set; }
        public string Description { get; private set; }

        private MzTab.States _currentState;

        public MzTabWriter(string filePath, string description = null, MzTab.MzTabMode mode = MzTab.MzTabMode.Summary, MzTab.MzTabType type = MzTab.MzTabType.Identification, string version = MzTab.Version)
        {
            FilePath = filePath;
            Description = description;
            Mode = mode;
            Type = type;
            Version = version;
        }

        public void Open(bool writeMandatoryMetaDataOnOpen = true)
        {
            _writer = new StreamWriter(FilePath, false, MzTab.DefaultEncoding);
            _currentState = MzTab.States.None;
            IsOpen = true;
            if (writeMandatoryMetaDataOnOpen)
                WriteMandatoryMetaData();
        }

        public void WriteMandatoryMetaData()
        {
            WriteLine(MzTab.LinePrefix.MetaData, MzTab.MDVersionField, Version);
            WriteLine(MzTab.LinePrefix.MetaData, MzTab.MDModeField, Mode);
            WriteLine(MzTab.LinePrefix.MetaData, MzTab.MDTypeField, Type);
            WriteLine(MzTab.LinePrefix.MetaData, MzTab.MDDescriptionField, Description);
        }

        public void WriteMetaData(string key, string value)
        {
            _currentState |= MzTab.States.MetaData;
            WriteLine(MzTab.LinePrefix.MetaData, key, value);
        }

        private void WritePsmHeader(bool includeRelibaility = false, bool includeURI = false, IEnumerable<string> optionalFields = null)
        {
            // Write the mandatory psm header
            WritePsmHeader(MzTab.PSMSequenceField, MzTab.PSMIdField, MzTab.PSMAcessionField, MzTab.PSMUniqueField,
                MzTab.PSMDatabaseField, MzTab.PSMDatabaseVersionField, MzTab.PSMSearchEngineField, MzTab.PSMSearchEngineScoreField, 
                MzTab.PSMModificationsField, MzTab.PSMSpectraReferenceField, MzTab.PSMRetentionTimeField, MzTab.PSMChargeField,
                MzTab.PSMExperimentalMZField, MzTab.PSMTheoreticalMZField, MzTab.PSMPreviousAminoAcidField, MzTab.PSMFollowingAminoAcidField,
                MzTab.PSMStartResidueField, MzTab.PSMEndResidueField);
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

        public void WritePsmData(IEnumerable<MzTabPSM> psms)
        {
            bool includeRelibaility = psms.Any(psm => psm.Reliability.HasValue);
            bool includeURI = psms.Any(psm => psm.URI != null);
            string[] optionalParameters = psms.Where(psm => psm.OptionalData != null).SelectMany(psm => psm.OptionalData.Keys).ToArray();

            WritePsmHeader(includeRelibaility, includeURI, optionalParameters);

        }

        /// <summary>
        /// Writes a blank line to the file, parsers should skip these lines
        /// </summary>
        public void WriteLine()
        {
            _writer.WriteLine();
        }

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
