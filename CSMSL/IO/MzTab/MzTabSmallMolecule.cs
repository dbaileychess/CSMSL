// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabSmallMolecule.cs) is part of CSMSL.
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
using CSMSL.Chemistry;

namespace CSMSL.IO.MzTab
{
    public class MzTabSmallMolecule : MzTabEntity
    {
        public static class Fields
        {
            public const string Identifier = "identifier";
            public const string ChemicalFormula = "chemical_formula";
            public const string Smiles = "smiles";
            public const string InChIKey = "inchi_key";
            public const string Description = "description";
            public const string ExperimentMZ = "exp_mass_to_charge";
            public const string TheoreticalMZ = "calc_mass_to_charge";
            public const string Charge = "charge";
            public const string RetentionTime = "retention_time";
            public const string TaxID = "taxid";
            public const string Species = "species";
            public const string Database = "database";
            public const string DatabaseVersion = "database_version";
            public const string Reliability = "reliability";
            public const string Uri = "uri";
            public const string SpectralReference = "spectra_ref";
            public const string SearchEngine = "search_engine";
            public const string BestSearchEngineScores = "best_search_engine_score[]";
            public const string SearchEngineScorePerMsRun = "search_engine_score[]_ms_run[]";
            public const string Modifications = "modifications";
            public const string AbundanceAssay = "smallmolecule_abundance_assary[]";
            public const string AbundanceStudyVariable = "smallmolecule_abudance_study_variable[]";
            public const string AbundanceStDevStudyVariable = "smallmolecule_abundance_stdev_study_variable[]";
            public const string AbudnanceStdErrorStudyVariable = "smallmolecule_abundance_std_error_study_variable[]";

            internal static IEnumerable<string> GetHeader(IList<MzTabSmallMolecule> smallMolecules)
            {
                List<string> headers = new List<string>();
                headers.Add(Identifier);
                headers.Add(ChemicalFormula);
                headers.Add(Smiles);
                headers.Add(InChIKey);
                headers.Add(Description);
                headers.Add(ExperimentMZ);
                headers.Add(TheoreticalMZ);
                headers.Add(Charge);
                headers.Add(RetentionTime);
                headers.Add(TaxID);
                headers.Add(Species);
                headers.Add(Database);
                headers.Add(DatabaseVersion);

                if (smallMolecules.Any(sm => sm.Reliability != MzTab.MetabolomicsReliabilityScore.NotSet))
                    headers.Add(Reliability);

                if (smallMolecules.Any(sm => sm.Uri != null))
                    headers.Add(Uri);

                headers.Add(SpectralReference);
                headers.Add(SearchEngine);

                headers.AddRange(GetHeaders(smallMolecules, BestSearchEngineScores, (sm => sm.BestSearchEngineScores)));
                headers.AddRange(GetHeaders(smallMolecules, SearchEngineScorePerMsRun, (sm => sm.SearchEngineScorePerMsRun)));

                headers.Add(Modifications);

                headers.AddRange(GetHeaders(smallMolecules, AbundanceAssay, (sm => sm.AbundanceAssays)));
                headers.AddRange(GetHeaders(smallMolecules, AbundanceStudyVariable, (sm => sm.AbundanceStudyVariables)));
                headers.AddRange(GetHeaders(smallMolecules, AbundanceStDevStudyVariable, (sm => sm.AbundanceStdevStudyVariables)));
                headers.AddRange(GetHeaders(smallMolecules, AbudnanceStdErrorStudyVariable, (sm => sm.AbundanceStandardErrorStudyVariables)));

                return headers;
            }
        }

        public List<string> Identifiers { get; set; }
        public ChemicalFormula ChemicalFormula { get; set; }
        public string Smiles { get; set; }
        public List<string> InChIKeys { get; set; }
        public string Description { get; set; }
        public double ExperimentalMZ { get; set; }
        public double TheoreticalMZ { get; set; }
        public int Charge { get; set; }
        public List<double> RetentionTimes { get; set; }
        public int TaxID { get; set; }
        public string Species { get; set; }
        public string Database { get; set; }
        public string DatabaseVersion { get; set; }
        public MzTab.MetabolomicsReliabilityScore Reliability { get; set; }
        public Uri Uri { get; set; }
        public string SpectralReference { get; set; }
        public List<CVParamater> SearchEngines { get; set; }

        private List<double> _bestSearchEngineScores;

        public List<double> BestSearchEngineScores
        {
            get { return _bestSearchEngineScores; }
            set { _bestSearchEngineScores = value; }
        }

        private MzTabMultipleSet<double> _searchEngineScorePerMsRun;

        public MzTabMultipleSet<double> SearchEngineScorePerMsRun
        {
            get { return _searchEngineScorePerMsRun; }
            set { _searchEngineScorePerMsRun = value; }
        }

        public string Modifications { get; set; }

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
                case Fields.Identifier:
                    return Identifiers == null ? MzTab.NullFieldText : string.Join("|", Identifiers);
                case Fields.ChemicalFormula:
                    return ChemicalFormula.ToString();
                case Fields.Smiles:
                    return Smiles ?? MzTab.NullFieldText;
                case Fields.InChIKey:
                    return InChIKeys == null ? MzTab.NullFieldText : string.Join("|", InChIKeys);
                case Fields.Description:
                    return Description ?? MzTab.NullFieldText;
                case Fields.ExperimentMZ:
                    return ExperimentalMZ.ToString();
                case Fields.TheoreticalMZ:
                    return TheoreticalMZ.ToString();
                case Fields.Charge:
                    return Charge.ToString();
                case Fields.RetentionTime:
                    return RetentionTimes == null ? MzTab.NullFieldText : string.Join("|", RetentionTimes);
                case Fields.TaxID:
                    return TaxID.ToString();
                case Fields.Species:
                    return Species ?? MzTab.NullFieldText;
                case Fields.Database:
                    return Database ?? MzTab.NullFieldText;
                case Fields.DatabaseVersion:
                    return DatabaseVersion ?? MzTab.NullFieldText;
                case Fields.Reliability:
                    return (Reliability == MzTab.MetabolomicsReliabilityScore.NotSet) ? MzTab.NullFieldText : ((int) Reliability).ToString();
                case Fields.Uri:
                    return Uri == null ? MzTab.NullFieldText : Uri.ToString();
                case Fields.SpectralReference:
                    return SpectralReference ?? MzTab.NullFieldText;
                case Fields.SearchEngine:
                    return SearchEngines == null ? MzTab.NullFieldText : string.Join("|", SearchEngines);
                case Fields.Modifications:
                    return Modifications ?? MzTab.NullFieldText;
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                switch (condensedFieldName)
                {
                    case Fields.BestSearchEngineScores:
                        return GetListValue(_bestSearchEngineScores, indices[0]);
                    case Fields.SearchEngineScorePerMsRun:
                        return GetListValue(_searchEngineScorePerMsRun, indices[0], indices[1]);
                    case Fields.AbundanceAssay:
                        return GetListValue(_abundanceAssays, indices[0]);
                    case Fields.AbundanceStudyVariable:
                        return GetListValue(_abundanceStudyVariables, indices[0]);
                    case Fields.AbundanceStDevStudyVariable:
                        return GetListValue(_abundanceStdevStudyVariables, indices[0]);
                    case Fields.AbudnanceStdErrorStudyVariable:
                        return GetListValue(_abundanceStandardErrorStudyVariables, indices[0]);
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
                case Fields.Identifier:
                    Identifiers = value.Split('|').ToList();
                    return;
                case Fields.ChemicalFormula:
                    ChemicalFormula = new ChemicalFormula(value);
                    return;
                case Fields.Smiles:
                    Smiles = value;
                    return;
                case Fields.InChIKey:
                    InChIKeys = value.Split('|').ToList();
                    return;
                case Fields.Description:
                    Description = value;
                    return;
                case Fields.ExperimentMZ:
                    ExperimentalMZ = double.Parse(value);
                    return;
                case Fields.TheoreticalMZ:
                    TheoreticalMZ = double.Parse(value);
                    return;
                case Fields.Charge:
                    Charge = int.Parse(value);
                    return;
                case Fields.RetentionTime:
                    RetentionTimes = value.Split('|').Select(double.Parse).ToList();
                    return;
                case Fields.TaxID:
                    TaxID = int.Parse(value);
                    return;
                case Fields.Species:
                    Species = value;
                    return;
                case Fields.Database:
                    Database = value;
                    return;
                case Fields.DatabaseVersion:
                    DatabaseVersion = value;
                    return;
                case Fields.Reliability:
                    Reliability = (MzTab.MetabolomicsReliabilityScore) int.Parse(value);
                    return;
                case Fields.Uri:
                    Uri = new Uri(value);
                    return;
                case Fields.SpectralReference:
                    SpectralReference = value;
                    return;
                case Fields.SearchEngine:
                    SearchEngines = value.Split('|').Select(v => new CVParamater(v)).ToList();
                    return;
                case Fields.Modifications:
                    Modifications = value;
                    return;
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                switch (condensedFieldName)
                {
                    case Fields.BestSearchEngineScores:
                        SetListValue(ref _bestSearchEngineScores, indices[0], double.Parse(value));
                        return;
                    case Fields.SearchEngineScorePerMsRun:
                        SetListValue(ref _searchEngineScorePerMsRun, indices[0], indices[1], double.Parse(value));
                        return;
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