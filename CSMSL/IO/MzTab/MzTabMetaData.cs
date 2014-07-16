using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSMSL.IO.MzTab
{
    public class MzTabMetaData : MzTabEntity
    {
        public MzTab.MzTabMode Mode
        {
            get { return GetValue<MzTab.MzTabMode>(MzTab.MDModeField); }
            set { SetRawValue(MzTab.MDModeField, value); }
        }

        public MzTab.MzTabType Type
        {
            get { return GetValue<MzTab.MzTabType>(MzTab.MDTypeField); }
            set { SetRawValue(MzTab.MDTypeField, value); }
        }

        public string Version
        {
            get { return GetValue<string>(MzTab.MDVersionField); }
            set { SetRawValue(MzTab.MDVersionField, value); }
        }

        public string Description
        {
            get { return GetValue<string>(MzTab.MDDescriptionField); }
            set { SetRawValue(MzTab.MDDescriptionField, value); }
        }

        public string ID
        {
            get { return GetValue<string>(MzTab.MDIDField); }
            set { SetRawValue(MzTab.MDIDField, value); }
        }

        public string Title
        {
            get { return GetValue<string>(MzTab.MDTitleField); }
            set { SetRawValue(MzTab.MDTitleField, value); }
        }

        public CVParamater ProteinQuantificationUnit
        {
            get { return GetValue<CVParamater>(MzTab.MDProteinQuantificationUnit); }
            set { SetRawValue(MzTab.MDProteinQuantificationUnit, value); }
        }

        public string[] SampleProcessing
        {
            get { return GetValue<string[]>("sample_processing");}
            set { SetRawValue("sample_processing", value); }
        }

        public List<string> MsRunLocation
        {
            get { return GetValue<List<string>>(MzTab.MDMsRunLocationField); }
            set { SetRawValue(MzTab.MDMsRunLocationField, value); }
        }
        
        public List<string> StudyVariableDescription
        {
            get { return GetValue<List<string>>(MzTab.MDStudyVariableDescriptionField); }
            set { SetRawValue(MzTab.MDStudyVariableDescriptionField, value); }
        }

        public List<CVParamater> FixedModifications
        {
            get { return GetValue<List<CVParamater>>(MzTab.MDFixedModField); }
            set { SetRawValue(MzTab.MDFixedModField, value); }
        }

        public List<CVParamater> VariableModifications
        {
            get { return GetValue<List<CVParamater>>(MzTab.MDVariableModField); }
            set { SetRawValue(MzTab.MDVariableModField, value); }
        }

        public List<CVParamater> PsmSearchEngineScore
        {
            get { return GetValue<List<CVParamater>>(MzTab.MDPsmSearchEngineScoreField); }
            set { SetRawValue(MzTab.MDPsmSearchEngineScoreField, value); }
        }

        public List<CVParamater> ProteinSearchEngineScore
        {
            get { return GetValue<List<CVParamater>>(MzTab.MDProteinSearchEngineScoreField); }
            set { SetRawValue(MzTab.MDProteinSearchEngineScoreField, value); }
        }

        public override void SetValue(string fieldName, string value)
        {
            // Contains a index variable
            if (fieldName.Contains("["))
            {
                string condensedFieldName = fieldName;
                List<int> indices = new List<int>();
                Match match;
                int index = 0;
                while ((match = MzTab.MultipleEntryRegex.Match(condensedFieldName, index)).Success)
                {
                    indices.Add(int.Parse(match.Groups[1].Value));
                    condensedFieldName = condensedFieldName.Remove(match.Index + 1, match.Length - 2); // ± 1 for ignoring the [ ] 
                    index = match.Index + 1;
                }

                switch (condensedFieldName)
                {
                    // String Versions
                    default:
                        SetRawValue(condensedFieldName, indices[0], value);
                        break;

                    // CV Parameters
                    case MzTab.MDFixedModField:
                    case MzTab.MDVariableModField:
                    case MzTab.MDPsmSearchEngineScoreField:
                    case MzTab.MDProteinSearchEngineScoreField:
                        SetRawValue(condensedFieldName, indices[0], new CVParamater(value));
                        break;
                }

            }
            else
            {
                switch (fieldName)
                {
                    // CV Parameters
                    default:
                        Data[fieldName] = new CVParamater(value);
                        break;

                    // String Parameters
                    case MzTab.MDIDField:
                    case MzTab.MDDescriptionField:
                    case MzTab.MDTitleField:
                    case MzTab.MDVersionField:
                        Data[fieldName] = value;
                        break;

                    case MzTab.MDModeField:
                        Data[fieldName] = (MzTab.MzTabMode) Enum.Parse(typeof (MzTab.MzTabMode), value, true);
                        break;
                    case MzTab.MDTypeField:
                        Data[fieldName] = (MzTab.MzTabType) Enum.Parse(typeof (MzTab.MzTabType), value, true);
                        break;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, object>> GetListValues(string fieldName)
        {
            object o;
            if (!Data.TryGetValue(fieldName, out o))
            {
                yield break;
            }

            var list = o as System.Collections.IEnumerable;
            if (list == null)
                yield break;

            int i = 1;
            foreach(var item in list)
            {
                string expandedName = fieldName.Replace("[]", "[" + i + "]");
                yield return new KeyValuePair<string, object>(expandedName, item);
                i++;
            }
        }

        public MzTabMetaData(MzTab.MzTabMode mode = MzTab.MzTabMode.Summary, MzTab.MzTabType type = MzTab.MzTabType.Identification, string description = null, string version = MzTab.Version)
            : base(4)
        {
            Mode = mode;
            Type = type;
            Description = description;
            Version = version;
        }

    }
}
