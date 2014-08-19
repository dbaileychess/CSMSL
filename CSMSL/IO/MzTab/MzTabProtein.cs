// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabProtein.cs) is part of CSMSL.
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
using System.ComponentModel;
using System.Linq;
using CSMSL.Proteomics;

namespace CSMSL.IO.MzTab
{
    public class MzTabProtein : MzTabEntity
    {
        public static class Fields
        {
            public const string Accession = "accession";
            public const string Description = "description";
            public const string TaxID = "taxid";
            public const string Species = "species";
            public const string Database = "database";
            public const string DatabaseVersion = "database_version";
            public const string SearchEngine = "search_engine";
            public const string BestSearchEngineScore = "best_search_engine_score[]";
            public const string SearchEngineScoreMsRun = "search_engine_score[]_ms_run[]";
            public const string Reliability = "reliability";
            public const string AmbiguityMembers = "ambiguity_members";
            public const string Modifications = "modifications";
            public const string AbundanceStudyVariables = "protein_abundance_study_variable[]";
            public const string AbundanceStdevStudyVariables = "protein_abundance_stdev_study_variable[]";
            public const string AbundanceStandardErrorStudyVariables = "protein_abundance_std_error_study_variable[]";
            public const string NumberOfPsmsPerMsRun = "num_psms_ms_run[]";
            public const string NumberOfDistinctPeptidesPerMsRun = "num_peptides_distinct_ms_run[]";
            public const string NumberOfUniquePeptidesPerMsRun = "num_peptides_unique_ms_run[]";
            public const string Coverage = "protein_coverage";
            public const string AbundancePerAssay = "protein_abundance_assay[]";
            public const string Uri = "uri";
            public const string GoTerms = "go_terms";

            internal static IEnumerable<string> GetHeader(IList<MzTabProtein> proteins)
            {
                List<string> headers = new List<string>();
                headers.Add(Accession);
                headers.Add(Description);
                headers.Add(TaxID);
                headers.Add(Species);
                headers.Add(Database);
                headers.Add(DatabaseVersion);
                headers.Add(SearchEngine);

                headers.AddRange(GetHeaders(proteins, BestSearchEngineScore, (prot => prot.BestSearchEngineScores)));
                headers.AddRange(GetHeaders(proteins, SearchEngineScoreMsRun, (prot => prot.SearchEngineScoreMsRun)));

                // Only report reliability if one psm has a non-null reliability score
                if (proteins.Any(protein => protein.Reliability != MzTab.ReliabilityScore.NotSet))
                    headers.Add(Reliability);

                headers.AddRange(GetHeaders(proteins, NumberOfPsmsPerMsRun, (prot => prot.NumberOfPsmsPerMsRun)));
                headers.AddRange(GetHeaders(proteins, NumberOfDistinctPeptidesPerMsRun, (prot => prot.NumberOfDistinctPeptidesPerMsRun)));
                headers.AddRange(GetHeaders(proteins, NumberOfUniquePeptidesPerMsRun, (prot => prot.NumberOfUniquePeptidesPerMsRun)));

                headers.Add(AmbiguityMembers);
                headers.Add(Modifications);

                if (proteins.Any(protein => protein.Uri != null))
                    headers.Add(Uri);

                if (proteins.Any(protein => protein.GoTerms != null))
                    headers.Add(GoTerms);

                if (proteins.Any(protein => protein.Coverage.HasValue))
                    headers.Add(Coverage);

                headers.AddRange(GetHeaders(proteins, AbundancePerAssay, (prot => prot.AbundanceAssays)));
                headers.AddRange(GetHeaders(proteins, AbundanceStudyVariables, (prot => prot.AbundanceStudyVariables)));
                headers.AddRange(GetHeaders(proteins, AbundanceStdevStudyVariables, (prot => prot.AbundanceStdevStudyVariables)));
                headers.AddRange(GetHeaders(proteins, AbundanceStandardErrorStudyVariables, (prot => prot.AbundanceStandardErrorStudyVariables)));

                return headers;
            }
        }

        public Uri Uri { get; set; }

        public string Accession { get; set; }

        public string Description { get; set; }

        public int TaxID { get; set; }

        public string Species { get; set; }

        public string Database { get; set; }

        public string DatabaseVersion { get; set; }

        public string Modificiations { get; set; }

        public double? Coverage { get; set; }

        private List<CVParamater> _searchEngines;

        public List<CVParamater> SearchEngines
        {
            get { return _searchEngines; }
            set { _searchEngines = value; }
        }

        private List<double> _bestSearchEngineScores;

        public List<double> BestSearchEngineScores
        {
            get { return _bestSearchEngineScores; }
            set { _bestSearchEngineScores = value; }
        }

        public List<string> AmbiguityMembers { get; set; }

        public List<string> GoTerms { get; set; }

        private List<double> _abundanceAssays;

        public List<double> AbundanceAssays
        {
            get { return _abundanceAssays; }
            set { _abundanceAssays = value; }
        }

        private List<double> _abundanceStudyVariables;

        public List<double> AbundanceStudyVariables
        {
            get { return _abundanceStudyVariables; }
            set { _abundanceStudyVariables = value; }
        }

        private List<double> _abundanceStdevStudyVariables;

        public List<double> AbundanceStdevStudyVariables
        {
            get { return _abundanceStdevStudyVariables; }
            set { _abundanceStdevStudyVariables = value; }
        }

        private List<double> _abundanceStandardErrorStudyVariables;

        public List<double> AbundanceStandardErrorStudyVariables
        {
            get { return _abundanceStandardErrorStudyVariables; }
            set { _abundanceStandardErrorStudyVariables = value; }
        }

        private MzTabMultipleSet<double?> _searchEngineScoreMsRun;

        public MzTabMultipleSet<double?> SearchEngineScoreMsRun
        {
            get { return _searchEngineScoreMsRun; }
            set { _searchEngineScoreMsRun = value; }
        }

        private List<int> _numberOfPsmsPerMsRun;

        public List<int> NumberOfPsmsPerMsRun
        {
            get { return _numberOfPsmsPerMsRun; }
            set { _numberOfPsmsPerMsRun = value; }
        }

        private List<int> _numberOfDistinctPeptidesPerMsRun;

        public List<int> NumberOfDistinctPeptidesPerMsRun
        {
            get { return _numberOfDistinctPeptidesPerMsRun; }
            set { _numberOfDistinctPeptidesPerMsRun = value; }
        }

        private List<int> _numberOfUniquePeptidesPerMsRun;

        public List<int> NumberOfUniquePeptidesPerMsRun
        {
            get { return _numberOfUniquePeptidesPerMsRun; }
            set { _numberOfUniquePeptidesPerMsRun = value; }
        }

        public MzTab.ReliabilityScore Reliability { get; set; }

        public override string GetValue(string fieldName)
        {
            switch (fieldName)
            {
                case Fields.Accession:
                    return Accession;
                case Fields.Description:
                    return Description;
                case Fields.Database:
                    return Database;
                case Fields.DatabaseVersion:
                    return DatabaseVersion;
                case Fields.SearchEngine:
                    return SearchEngines == null ? MzTab.NullFieldText : string.Join("|", SearchEngines);
                case Fields.TaxID:
                    return TaxID.ToString();
                case Fields.Species:
                    return Species;
                case Fields.Modifications:
                    return Modificiations;
                case Fields.AmbiguityMembers:
                    return AmbiguityMembers == null ? MzTab.NullFieldText : string.Join(",", AmbiguityMembers);
                case Fields.Reliability:
                    if (Reliability == MzTab.ReliabilityScore.NotSet)
                        return MzTab.NullFieldText;
                    return ((int) Reliability).ToString();
                case Fields.Coverage:
                    return Coverage.ToString();
                case Fields.Uri:
                    return Uri != null ? Uri.ToString() : MzTab.NullFieldText;
                case Fields.GoTerms:
                    return GoTerms == null ? MzTab.NullFieldText : string.Join("|", GoTerms);
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                switch (condensedFieldName)
                {
                    case Fields.SearchEngine:
                        return GetListValue(_searchEngines, indices[0]);
                    case Fields.BestSearchEngineScore:
                        return GetListValue(_bestSearchEngineScores, indices[0]);
                    case Fields.AbundancePerAssay:
                        return GetListValue(_abundanceAssays, indices[0]);
                    case Fields.AbundanceStudyVariables:
                        return GetListValue(_abundanceStudyVariables, indices[0]);
                    case Fields.AbundanceStdevStudyVariables:
                        return GetListValue(_abundanceStdevStudyVariables, indices[0]);
                    case Fields.AbundanceStandardErrorStudyVariables:
                        return GetListValue(_abundanceStandardErrorStudyVariables, indices[0]);
                    case Fields.SearchEngineScoreMsRun:
                        return GetListValue(_searchEngineScoreMsRun, indices[0], indices[1]);
                    case Fields.NumberOfPsmsPerMsRun:
                        return GetListValue(_numberOfPsmsPerMsRun, indices[0]);
                    case Fields.NumberOfDistinctPeptidesPerMsRun:
                        return GetListValue(_numberOfDistinctPeptidesPerMsRun, indices[0]);
                    case Fields.NumberOfUniquePeptidesPerMsRun:
                        return GetListValue(_numberOfUniquePeptidesPerMsRun, indices[0]);
                }
            }

            if (fieldName.StartsWith(MzTab.OptionalColumnPrefix))
            {
                return GetOptionalData(fieldName);
            }

            throw new ArgumentException("Unexpected field name: " + fieldName);
        }

        public override void SetValue(string fieldName, string value)
        {
            if (MzTab.NullFieldText.Equals(value))
                return;

            switch (fieldName)
            {
                case Fields.Accession:
                    Accession = value;
                    return;
                case Fields.Description:
                    Description = value;
                    return;
                case Fields.Database:
                    Database = value;
                    return;
                case Fields.DatabaseVersion:
                    DatabaseVersion = value;
                    return;
                case Fields.TaxID:
                    TaxID = int.Parse(value);
                    return;
                case Fields.Species:
                    Species = value;
                    return;
                case Fields.Modifications:
                    Modificiations = value;
                    return;
                case Fields.SearchEngine:
                    SearchEngines = value.Split('|').Select(datum => (CVParamater) datum).ToList();
                    return;
                case Fields.Reliability:
                    Reliability = (MzTab.ReliabilityScore) int.Parse(value);
                    return;
                case Fields.AmbiguityMembers:
                    AmbiguityMembers = value.Split(',').ToList();
                    return;
                case Fields.Coverage:
                    Coverage = double.Parse(value);
                    return;
                case Fields.Uri:
                    Uri = new Uri(value);
                    return;
                case Fields.GoTerms:
                    GoTerms = value.Split('|').Select(datum => datum).ToList();
                    return;
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                double tempDbl;
                switch (condensedFieldName)
                {
                    case Fields.SearchEngine:
                        SetListValue(ref _searchEngines, indices[0], new CVParamater(value));
                        return;
                    case Fields.BestSearchEngineScore:
                        SetListValue(ref _bestSearchEngineScores, indices[0], double.Parse(value));
                        return;
                    case Fields.AbundancePerAssay:
                        SetListValue(ref _abundanceAssays, indices[0], double.Parse(value));
                        return;
                    case Fields.AbundanceStudyVariables:
                        SetListValue(ref _abundanceStudyVariables, indices[0], double.Parse(value));
                        return;
                    case Fields.AbundanceStdevStudyVariables:
                        SetListValue(ref _abundanceStdevStudyVariables, indices[0], double.Parse(value));
                        return;
                    case Fields.AbundanceStandardErrorStudyVariables:
                        SetListValue(ref _abundanceStandardErrorStudyVariables, indices[0], double.Parse(value));
                        return;
                    case Fields.SearchEngineScoreMsRun:
                        SetListValue(ref _searchEngineScoreMsRun, indices[0], indices[1], double.TryParse(value, out tempDbl) ? tempDbl : default(double?));
                        return;
                    case Fields.NumberOfPsmsPerMsRun:
                        SetListValue(ref _numberOfPsmsPerMsRun, indices[0], int.Parse(value));
                        return;
                    case Fields.NumberOfDistinctPeptidesPerMsRun:
                        SetListValue(ref _numberOfDistinctPeptidesPerMsRun, indices[0], int.Parse(value));
                        return;
                    case Fields.NumberOfUniquePeptidesPerMsRun:
                        SetListValue(ref _numberOfUniquePeptidesPerMsRun, indices[0], int.Parse(value));
                        return;
                }
            }

            if (fieldName.StartsWith(MzTab.OptionalColumnPrefix))
            {
                SetOptionalData(fieldName, value);
                return;
            }

            throw new ArgumentException("Unexpected field name: " + fieldName);
        }

        public override string ToString()
        {
            return Description;
        }
    }
}