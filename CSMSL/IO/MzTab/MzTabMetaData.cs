// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
            public const string MsRun = "ms_run[]";
            public const string MsRunLocation = "ms_run[]-location";
            public const string FixedMod = "fixed_mod[]";
            public const string FixedModSite = "fixed_mod[]-site";
            public const string FixedModPosition = "fixed_mod[]-position";
            public const string VariableMod = "variable_mod[]";
            public const string VariableModSite = "variable_mod[]-site";
            public const string VariableModPosition = "variable_mod[]-position";
            public const string StudyVariableDescription = "study_variable[]-description";
            public const string PsmSearchEngineScore = "psm_search_engine_score[]";
            public const string ProteinSearchEngineScore = "protein_search_engine_score[]";
            public const string Software = "software[]";
            public const string SoftwareSettings = "software[]-setting[]";
        }

        public MzTabMetaData(MzTab.MzTabMode mode = MzTab.MzTabMode.Summary, MzTab.MzTabType type = MzTab.MzTabType.Identification, string description = null, string version = MzTab.Version)
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

        private List<MzTabSoftware> _software;

        public List<MzTabSoftware> Software
        {
            get { return _software; }
            set { _software = value; }
        }

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

        private List<string> _fixedModificationSites;

        public List<string> FixedModificationSites
        {
            get { return _fixedModificationSites; }
            set { _fixedModificationSites = value; }
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
                    return Enum.GetName(typeof (MzTab.MzTabMode), Mode);
                case Fields.Type:
                    return Enum.GetName(typeof (MzTab.MzTabType), Type);
                case Fields.Description:
                    return Description;
                case Fields.Title:
                    return Title;
                case Fields.Version:
                    return Version;
                case Fields.ID:
                    return ID;
                case Fields.ProteinQuantificationUnit:
                    return ProteinQuantificationUnit == null ? MzTab.NullFieldText : ProteinQuantificationUnit.ToString();
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);
                switch (condensedFieldName)
                {
                    case Fields.FixedMod:
                        return GetListValue(_fixedModifications, indices[0]);
                    case Fields.FixedModSite:
                        return GetListValue(_fixedModificationSites, indices[0]);
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
                    case Fields.Software:
                        return GetListValue(_software, indices[0]);
                    case Fields.SoftwareSettings:
                        return GetFieldValue(_software, indices[0], sw => sw.Settings, indices[1]);
                }
            }

            throw new ArgumentException("Unexpected field name: " + fieldName);
        }

        public override void SetValue(string fieldName, string value)
        {
            switch (fieldName)
            {
                case Fields.Mode:
                    Mode = (MzTab.MzTabMode) Enum.Parse(typeof (MzTab.MzTabMode), value, true);
                    return;
                case Fields.Type:
                    Type = (MzTab.MzTabType) Enum.Parse(typeof (MzTab.MzTabType), value, true);
                    return;
                case Fields.Description:
                    Description = value;
                    return;
                case Fields.Title:
                    Title = value;
                    return;
                case Fields.Version:
                    Version = value;
                    return;
                case Fields.ID:
                    ID = value;
                    return;
                case Fields.ProteinQuantificationUnit:
                    ProteinQuantificationUnit = new CVParamater(value);
                    return;
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);
                switch (condensedFieldName)
                {
                    case Fields.FixedMod:
                        SetListValue(ref _fixedModifications, indices[0], new CVParamater(value));
                        return;
                    case Fields.FixedModSite:
                        SetListValue(ref _fixedModificationSites, indices[0], value);
                        return;
                    case Fields.VariableMod:
                        SetListValue(ref _variableModifications, indices[0], new CVParamater(value));
                        return;
                    case Fields.PsmSearchEngineScore:
                        SetListValue(ref _psmSearchEngineScores, indices[0], new CVParamater(value));
                        return;
                    case Fields.ProteinSearchEngineScore:
                        SetListValue(ref _proteinSearchEngineScores, indices[0], new CVParamater(value));
                        return;
                    case Fields.StudyVariableDescription:
                        SetListValue(ref _studyVariableDescriptions, indices[0], value);
                        return;
                    case Fields.MsRunLocation:
                        SetListValue(ref _msRunLocations, indices[0], value);
                        return;
                    case Fields.Software:
                        SetListValue(ref _software, indices[0], new MzTabSoftware(new CVParamater(value)));
                        return;
                    case Fields.SoftwareSettings:
                        SetFieldValue(ref _software, indices[0], sw => sw.Settings, value);
                        return;
                }
            }
            //throw new ArgumentException("Unexpected field name: " + fieldName);
        }

        public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs()
        {
            int i, j;
            string name;

            yield return new KeyValuePair<string, string>(Fields.Version, Version);
            yield return new KeyValuePair<string, string>(Fields.Mode, Mode.ToString());
            yield return new KeyValuePair<string, string>(Fields.Type, Type.ToString());
            yield return new KeyValuePair<string, string>(Fields.Description, Description);

            i = MzTab.IndexBased;
            foreach (string msrunlocation in MsRunLocations)
            {
                name = MzTab.GetArrayName(Fields.MsRunLocation, i);
                yield return new KeyValuePair<string, string>(name, msrunlocation);
                i++;
            }

            i = MzTab.IndexBased;
            foreach (MzTabSoftware software in Software)
            {
                name = MzTab.GetArrayName(Fields.Software, i);
                yield return new KeyValuePair<string, string>(name, software.ToString());
                j = MzTab.IndexBased;
                foreach (string setting in software.Settings)
                {
                    name = MzTab.GetArrayName(Fields.SoftwareSettings, i, j);
                    yield return new KeyValuePair<string, string>(name, setting);
                    j++;
                }
                i++;
            }
        }

        #region Helper Methods

        public string AddMsRun(string filePath)
        {
            if (MsRunLocations == null)
            {
                MsRunLocations = new List<string>();
            }

            MsRunLocations.Add(filePath);

            return MzTab.GetArrayName(Fields.MsRun, MsRunLocations.Count - 1 + MzTab.IndexBased);
        }

        public int AddSoftware(MzTabSoftware software)
        {
            if (Software == null)
            {
                Software = new List<MzTabSoftware>();
            }

            Software.Add(software);

            return Software.Count - 1 + MzTab.IndexBased;
        }

        #endregion
    }
}