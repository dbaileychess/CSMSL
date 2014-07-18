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
        
        private void WriteData<T>(MzTabSection section, IList<T> objects) where T : MzTabEntity
        {
            if (objects == null || objects.Count == 0)
                return;

            MzTab.LinePrefix headerPrefix = MzTab.LinePrefix.Comment;
            MzTab.LinePrefix prefix = MzTab.LinePrefix.Comment;

            switch (section)
            {
                case MzTabSection.SmallMolecule:
                    headerPrefix = MzTab.LinePrefix.SmallMoleculeTable;
                    prefix = MzTab.LinePrefix.SmallMoleculeData;
                    break;
                case MzTabSection.Peptide:
                    headerPrefix = MzTab.LinePrefix.PeptideTable;
                    prefix = MzTab.LinePrefix.PeptideData;
                    break;
                case MzTabSection.PSM:
                    headerPrefix = MzTab.LinePrefix.PsmTable;
                    prefix = MzTab.LinePrefix.PsmData;
                    break;
                case MzTabSection.Protein:
                    headerPrefix = MzTab.LinePrefix.ProteinTable;
                    prefix = MzTab.LinePrefix.ProteinData;
                    break;
            }

            // Write Header
            string[] header = MzTabEntity.GetHeader(objects).ToArray();
            WriteLine(headerPrefix, header);

            // Write table
            foreach (T datum in objects)
            {
                object[] values = datum.GetValues(header).ToArray();
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

        #region Protein Section

        public void WriteProteinData(IList<MzTabProtein> proteins)
        {
            WriteData(MzTabSection.Protein, proteins);
        }

        #endregion

        #region PSM Section

        public void WritePsmData(IList<MzTabPSM> psms)
        {
            WriteData(MzTabSection.PSM, psms);
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
