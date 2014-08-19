// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabPeptide.cs) is part of CSMSL.
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
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public class MzTabPeptide : MzTabEntity
    {
        public static class Fields
        {
            public const string Sequence = "sequence";
            public const string Accession = "accession";
            public const string Unique = "unique";
            public const string Database = "database";
            public const string DatabaseVersion = "database_version";
            public const string SearchEngine = "search_engine";
            public const string BestSearchEngineScore = "best_search_engine_score[]";
            public const string SearchEngineScorePerMsRun = "search_engine_score[]_ms_run[]";
            public const string Reliability = "reliability";
            public const string Modifications = "modifications";
            public const string RetentionTime = "retention_time";
            public const string RetentionTimeWindow = "retention_time_window";
            public const string Charge = "charge";
            public const string MZ = "mass_to_charge";
            public const string Uri = "uri";
            public const string SpectraReference = "spectra_ref";
            public const string AbundanceAssay = "peptide_abundance_assary[]";
            public const string AbundanceStudyVariable = "peptide_abudance_study_variable[]";
            public const string AbundanceStDevStudyVariable = "peptide_abundance_stdev_study_variable[]";
            public const string AbudnanceStdErrorStudyVariable = "peptide_abundance_std_error_study_variable[]";

            internal static IEnumerable<string> GetHeader(IList<MzTabPeptide> peptides)
            {
                List<string> headers = new List<string>();
                headers.Add(Sequence);
                headers.Add(Accession);
                headers.Add(Unique);
                headers.Add(Database);
                headers.Add(DatabaseVersion);
                headers.Add(SearchEngine);

                headers.AddRange(GetHeaders(peptides, BestSearchEngineScore, (peptide => peptide.BestSearchEngineScores)));
                headers.AddRange(GetHeaders(peptides, SearchEngineScorePerMsRun, (peptide => peptide.BestSearchEngineScores)));

                // Only report reliability if one psm has a non-null reliability score
                if (peptides.Any(peptide => peptide.Reliability != MzTab.ReliabilityScore.NotSet))
                    headers.Add(Reliability);

                headers.Add(Modifications);
                headers.Add(RetentionTime);
                headers.Add(RetentionTimeWindow);
                headers.Add(Charge);
                headers.Add(MZ);

                if (peptides.Any(psm => psm.Uri != null))
                    headers.Add(Uri);

                headers.Add(SpectraReference);
                headers.AddRange(GetHeaders(peptides, AbundanceAssay, (peptide => peptide.AbundanceAssays)));
                headers.AddRange(GetHeaders(peptides, AbundanceStudyVariable, (peptide => peptide.AbundanceStudyVariables)));
                headers.AddRange(GetHeaders(peptides, AbundanceStDevStudyVariable, (peptide => peptide.AbundanceStdevStudyVariables)));
                headers.AddRange(GetHeaders(peptides, AbudnanceStdErrorStudyVariable, (peptide => peptide.AbundanceStandardErrorStudyVariables)));

                return headers;
            }
        }

        /// <summary>
        /// The peptide's sequence corresponding to the PSM
        /// </summary>
        public string Sequence { get; set; }

        public int ID { get; set; }

        public string Accession { get; set; }

        public bool Unique { get; set; }

        public string Database { get; set; }

        public string DatabaseVersion { get; set; }

        public List<CVParamater> SearchEngines { get; set; }

        private List<double> _bestSearchEngineScores;

        public List<double> BestSearchEngineScores
        {
            get { return _bestSearchEngineScores; }
            set { _bestSearchEngineScores = value; }
        }

        public MzTab.ReliabilityScore Reliability { get; set; }

        public string Modifications { get; set; }

        public List<double> RetentionTime { get; set; }
        public List<double> RetentionTimeWindows { get; set; }

        public int Charge { get; set; }

        public double MZ { get; set; }

        public double TheoreticalMZ { get; set; }

        public Uri Uri { get; set; }

        public string SpectraReference { get; set; }

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

        public override string GetValue(string fieldName)
        {
            switch (fieldName)
            {
                case Fields.Sequence:
                    return Sequence;
                case Fields.Accession:
                    return Accession;
                case Fields.Unique:
                    return Unique ? "1" : "0";
                case Fields.Database:
                    return Database;
                case Fields.DatabaseVersion:
                    return DatabaseVersion;
                case Fields.SearchEngine:
                    return string.Join("|", SearchEngines);
                case Fields.Reliability:
                    if (Reliability == MzTab.ReliabilityScore.NotSet)
                        return MzTab.NullFieldText;
                    return ((int) Reliability).ToString();
                case Fields.Modifications:
                    return Modifications;
                case Fields.RetentionTime:
                    return string.Join("|", RetentionTime);
                case Fields.Charge:
                    return Charge.ToString();
                case Fields.MZ:
                    return MZ.ToString();
                case Fields.Uri:
                    return Uri.ToString();
                case Fields.SpectraReference:
                    return SpectraReference;
                case Fields.RetentionTimeWindow:
                    return string.Join("|", RetentionTimeWindows);
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                switch (condensedFieldName)
                {
                    case Fields.AbundanceAssay:
                        return GetListValue(_abundanceAssays, indices[0]);
                    case Fields.AbundanceStudyVariable:
                        return GetListValue(_abundanceStudyVariables, indices[0]);
                    case Fields.AbundanceStDevStudyVariable:
                        return GetListValue(_abundanceStdevStudyVariables, indices[0]);
                    case Fields.AbudnanceStdErrorStudyVariable:
                        return GetListValue(_abundanceStandardErrorStudyVariables, indices[0]);
                    case Fields.BestSearchEngineScore:
                        return GetListValue(_bestSearchEngineScores, indices[0]);
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
                case Fields.Sequence:
                    Sequence = value;
                    return;
                case Fields.Accession:
                    Accession = value;
                    return;
                case Fields.Unique:
                    Unique = value.Equals("1");
                    return;
                case Fields.Database:
                    Database = value;
                    return;
                case Fields.DatabaseVersion:
                    DatabaseVersion = value;
                    return;
                case Fields.SearchEngine:
                    SearchEngines = value.Split('|').Select(datum => (CVParamater) datum).ToList();
                    return;
                case Fields.Reliability:
                    Reliability = (MzTab.ReliabilityScore) int.Parse(value);
                    return;
                case Fields.Modifications:
                    Modifications = value;
                    return;
                case Fields.RetentionTime:
                    RetentionTime = value.Split('|').Select(double.Parse).ToList();
                    return;
                case Fields.RetentionTimeWindow:
                    RetentionTimeWindows = value.Split('|').Select(double.Parse).ToList();
                    return;
                case Fields.Charge:
                    Charge = int.Parse(value);
                    return;
                case Fields.MZ:
                    MZ = double.Parse(value);
                    return;
                case Fields.Uri:
                    Uri = new Uri(value);
                    return;
                case Fields.SpectraReference:
                    SpectraReference = value;
                    return;
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                switch (condensedFieldName)
                {
                    case Fields.AbundanceAssay:
                        SetListValue(ref _abundanceAssays, indices[0], double.Parse(value));
                        return;
                    case Fields.AbundanceStudyVariable:
                        SetListValue(ref _abundanceStudyVariables, indices[0], double.Parse(value));
                        return;
                    case Fields.AbundanceStDevStudyVariable:
                        SetListValue(ref _abundanceStdevStudyVariables, indices[0], double.Parse(value));
                        return;
                    case Fields.AbudnanceStdErrorStudyVariable:
                        SetListValue(ref _abundanceStandardErrorStudyVariables, indices[0], double.Parse(value));
                        return;
                    case Fields.BestSearchEngineScore:
                        SetListValue(ref _bestSearchEngineScores, indices[0], double.Parse(value));
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
            return Sequence;
        }
    }
}