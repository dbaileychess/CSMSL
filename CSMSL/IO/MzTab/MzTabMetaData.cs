using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public class MzTabMetaData : MzTabEntity
    {
        public static class Fields
        {
            public const string Version = "mzTab-version";
            public const string Mode = "mzTab-mode";
            public const string Type = "mzTab-type";
            public const string Description = "description";
            public const string ProteinQuantificationUnit = "protein-quantification_unit";
            public const string ID = "mzTab-ID";
            public const string Title = "title";
            public const string MsRunLocation = "ms_run[]-location";
            public const string FixedMod = "fixed_mod[]";
            public const string VariableMod = "variable_mod[]";
            public const string StudyVariableDescription = "study_variable[]-description";
            public const string PsmSearchEngineScore = "psm_search_engine_score[]";
            public const string ProteinSearchEngineScore = "protein_search_engine_score[]";

            public static readonly List<string> FieldOrder = new List<string>
            {
                Version, Mode, Type, ID,
                Title, Description, ProteinSearchEngineScore, PsmSearchEngineScore,
                FixedMod, VariableMod, ProteinQuantificationUnit, MsRunLocation,
                StudyVariableDescription
            };
        }

        public MzTabMetaData(MzTab.MzTabMode mode = MzTab.MzTabMode.Summary, MzTab.MzTabType type = MzTab.MzTabType.Identification, string description = null, string version = MzTab.Version)
            : base(4)
        {
            Mode = mode;
            Type = type;
            Description = description;
            Version = version;
        }

        public MzTab.MzTabMode Mode { get; set; }

        public MzTab.MzTabType Type { get; set; }
    
        public string Version { get; set; }
      
        public string Description { get; set; }
     
        public string ID { get; set; }

        public string Title { get; set; }
      
        public CVParamater ProteinQuantificationUnit { get; set; }

        private List<string> _msRunLocations;
        public List<string> MsRunLocations
        {
            get { return _msRunLocations; }
            set { _msRunLocations = value; }
        }

        private List<string> _studyVariableDescriptions;
        public List<string> StudyVariableDescriptions
        {
            get { return _studyVariableDescriptions; }
            set { _studyVariableDescriptions = value; }
        }

        private List<CVParamater> _fixedModifications;
        public List<CVParamater> FixedModifications
        {
            get { return _fixedModifications; }
            set { _fixedModifications = value; }
        }

        private List<CVParamater> _variableModifications;
        public List<CVParamater> VariableModifications
        {
            get { return _variableModifications; }
            set { _variableModifications = value; }
        }

        private List<CVParamater> _psmSearchEngineScores;
        public List<CVParamater> PsmSearchEngineScores
        {
            get { return _psmSearchEngineScores; }
            set { _psmSearchEngineScores = value; }
        }

        private List<CVParamater> _proteinSearchEngineScores;
        public List<CVParamater> ProteinSearchEngineScores
        {
            get { return _proteinSearchEngineScores; }
            set { _proteinSearchEngineScores = value; }
        }
  
        public override string GetValue(string fieldName)
        {
            switch (fieldName)
            {
                case Fields.Mode:
                    return Enum.GetName(typeof(MzTab.MzTabMode), Mode);
                case Fields.Type:
                    return Enum.GetName(typeof(MzTab.MzTabType), Type);
                case Fields.Description:
                   return Description;
                case Fields.Title:
                   return Title;
                case Fields.Version:
                   return Version;
                case Fields.ID:
                   return ID;
                case Fields.ProteinQuantificationUnit:
                   return ProteinQuantificationUnit.ToString();
                default:
                    if (fieldName.Contains("["))
                    {
                        string condensedFieldName;
                        List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);
                        switch (condensedFieldName)
                        {
                            case Fields.FixedMod:
                                return GetListValue(_fixedModifications, indices[0]);
                            case Fields.VariableMod:
                                return GetListValue(_variableModifications, indices[0]);
                            case Fields.PsmSearchEngineScore:
                                return GetListValue(_psmSearchEngineScores, indices[0]);
                            case Fields.ProteinSearchEngineScore:
                                return GetListValue(_proteinSearchEngineScores, indices[0]);
                            case Fields.StudyVariableDescription:
                                return GetListValue(_studyVariableDescriptions, indices[0]);
                            case Fields.MsRunLocation:
                                return GetListValue(_msRunLocations, indices[0]);
                            default:
                                break;
                        }
                    }
                    throw new ArgumentException("Unexpected field name: " + fieldName);
            }
        }

        public override void SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case Fields.Mode:
                    Mode = (MzTab.MzTabMode)Enum.Parse(typeof(MzTab.MzTabMode), value, true); return;
                case Fields.Type:
                    Type = (MzTab.MzTabType)Enum.Parse(typeof(MzTab.MzTabType), value, true); return;
                case Fields.Description:
                    Description = value; return;
                case Fields.Title:
                    Title = value; return;
                case Fields.Version:
                    Version = value; return;
                case Fields.ID:
                    ID = value; return;
                case Fields.ProteinQuantificationUnit:
                    ProteinQuantificationUnit = new CVParamater(value); return;
                default:
                    if (fieldName.Contains("["))
                    {
                        string condensedFieldName;
                        List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);
                        switch (condensedFieldName)
                        {
                            case Fields.FixedMod:
                                SetRawValue(ref _fixedModifications, indices[0], new CVParamater(value)); return;
                            case Fields.VariableMod:
                                SetRawValue(ref _variableModifications, indices[0], new CVParamater(value)); return;
                            case Fields.PsmSearchEngineScore:
                                SetRawValue(ref _psmSearchEngineScores, indices[0], new CVParamater(value)); return;
                            case Fields.ProteinSearchEngineScore:
                                SetRawValue(ref _proteinSearchEngineScores, indices[0], new CVParamater(value)); return;
                            case Fields.StudyVariableDescription:
                                SetRawValue(ref _studyVariableDescriptions, indices[0], value); return;
                            case Fields.MsRunLocation:
                                SetRawValue(ref _msRunLocations, indices[0], value); return;
                            default:
                                break;
                        }
                    }
                    throw new ArgumentException("Unexpected field name: " + fieldName);
            }
        }
        
        private IEnumerable<KeyValuePair<string, string>> _getKeyValuePairs(string fieldName)
        {
            IList list;
            switch (fieldName)
            {
                case Fields.PsmSearchEngineScore:
                    list = _psmSearchEngineScores; break;
                case Fields.ProteinSearchEngineScore:
                    list = _proteinSearchEngineScores; break;
                case Fields.StudyVariableDescription:
                    list = _studyVariableDescriptions; break;
                case Fields.MsRunLocation:
                    list = _msRunLocations; break;
                case Fields.FixedMod:
                    list = _fixedModifications; break;
                case Fields.VariableMod:
                    list = _variableModifications; break;
                default:
                    string value = GetValue(fieldName);
                    if (string.IsNullOrEmpty(value))
                    {
                        yield break;
                    }
                    yield return new KeyValuePair<string, string>(fieldName, value);
                    yield break;
            }

            int count = list == null ? 0 : list.Count;
            if (count <= 0)
                yield break;

            for (int i = 1; i <= count; i++)
            {
                string value = list[i - MzTab.IndexBased].ToString();
                string expandedFieldName = fieldName.Replace("[]", "[" + i + "]");
                //string value = GetValue(expandedFieldName);
                if (!string.IsNullOrEmpty(value))
                {
                    yield return new KeyValuePair<string, string>(expandedFieldName, value);
                }
            }
        }
        
        public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs()
        {
            return Fields.FieldOrder.SelectMany(_getKeyValuePairs);
        }
    }
}
