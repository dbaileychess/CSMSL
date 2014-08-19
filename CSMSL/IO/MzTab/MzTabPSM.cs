// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabPSM.cs) is part of CSMSL.
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
    public class MzTabPSM : MzTabEntity
    {
        public static class Fields
        {
            public const string Sequence = "sequence";
            public const string ID = "PSM_ID";
            public const string Accession = "accession";
            public const string Unique = "unique";
            public const string Database = "database";
            public const string DatabaseVersion = "database_version";
            public const string SearchEngine = "search_engine";
            public const string SearchEngineScore = "search_engine_score[]";
            public const string Reliability = "reliability";
            public const string Modifications = "modifications";
            public const string RetentionTime = "retention_time";
            public const string Charge = "charge";
            public const string ExperimentalMZ = "exp_mass_to_charge";
            public const string TheoreticalMZ = "calc_mass_to_charge";
            public const string Uri = "uri";
            public const string SpectraReference = "spectra_ref";
            public const string PreviousAminoAcid = "pre";
            public const string FollowingAminoAcid = "post";
            public const string StartResidue = "start";
            public const string EndResidue = "end";

            internal static IEnumerable<string> GetHeader(IList<MzTabPSM> psms)
            {
                List<string> headers = new List<string>();
                headers.Add(Sequence);
                headers.Add(ID);
                headers.Add(Accession);
                headers.Add(Unique);
                headers.Add(Database);
                headers.Add(DatabaseVersion);
                headers.Add(SearchEngine);

                headers.AddRange(GetHeaders(psms, SearchEngineScore, (psm => psm.SearchEngineScores)));

                // Only report reliability if one psm has a non-null reliability score
                if (psms.Any(psm => psm.Reliability != MzTab.ReliabilityScore.NotSet))
                    headers.Add(Reliability);

                headers.Add(Modifications);
                headers.Add(RetentionTime);
                headers.Add(Charge);
                headers.Add(ExperimentalMZ);
                headers.Add(TheoreticalMZ);

                if (psms.Any(psm => psm.Uri != null))
                    headers.Add(Uri);

                headers.Add(SpectraReference);
                headers.Add(PreviousAminoAcid);
                headers.Add(FollowingAminoAcid);
                headers.Add(StartResidue);
                headers.Add(EndResidue);

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

        private List<double> _searchEngineScores;

        public List<double> SearchEngineScores
        {
            get { return _searchEngineScores; }
            set { _searchEngineScores = value; }
        }

        public MzTab.ReliabilityScore Reliability { get; set; }

        public string Modifications { get; set; }

        public List<double> RetentionTime { get; set; }

        public int Charge { get; set; }

        public double ExperimentalMZ { get; set; }

        public double TheoreticalMZ { get; set; }

        public Uri Uri { get; set; }

        public string SpectraReference { get; set; }

        public char PreviousAminoAcid { get; set; }

        public char FollowingAminoAcid { get; set; }

        public int EndResiduePosition { get; set; }

        public int StartResiduePosition { get; set; }

        public override string ToString()
        {
            return string.Format("(#{0}) {1}", ID, Sequence);
        }

        public override string GetValue(string fieldName)
        {
            switch (fieldName)
            {
                case Fields.Sequence:
                    return string.IsNullOrEmpty(Sequence) ? MzTab.NullFieldText : Sequence;
                case Fields.ID:
                    return ID.ToString();
                case Fields.Accession:
                    return Accession;
                case Fields.Unique:
                    return Unique ? "1" : "0";
                case Fields.Database:
                    return string.IsNullOrEmpty(Database) ? MzTab.NullFieldText : Database;
                case Fields.DatabaseVersion:
                    return string.IsNullOrEmpty(DatabaseVersion) ? MzTab.NullFieldText : DatabaseVersion;
                case Fields.SearchEngine:
                    return SearchEngines == null ? MzTab.NullFieldText : string.Join("|", SearchEngines);
                case Fields.Reliability:
                    if (Reliability == MzTab.ReliabilityScore.NotSet)
                        return MzTab.NullFieldText;
                    return ((int) Reliability).ToString();
                case Fields.Modifications:
                    return string.IsNullOrEmpty(Modifications) ? MzTab.NullFieldText : Modifications;
                case Fields.RetentionTime:
                    return RetentionTime == null ? MzTab.NullFieldText : string.Join("|", RetentionTime);
                case Fields.Charge:
                    return Charge.ToString();
                case Fields.ExperimentalMZ:
                    return ExperimentalMZ.ToString();
                case Fields.TheoreticalMZ:
                    return TheoreticalMZ.ToString();
                case Fields.Uri:
                    return Uri == null ? MzTab.NullFieldText : Uri.ToString();
                case Fields.SpectraReference:
                    return string.IsNullOrEmpty(SpectraReference) ? MzTab.NullFieldText : SpectraReference;
                case Fields.PreviousAminoAcid:
                    return default(char).Equals(PreviousAminoAcid) ? "-" : PreviousAminoAcid.ToString();
                case Fields.FollowingAminoAcid:
                    return default(char).Equals(FollowingAminoAcid) ? "-" : FollowingAminoAcid.ToString();
                case Fields.StartResidue:
                    return StartResiduePosition.ToString();
                case Fields.EndResidue:
                    return EndResiduePosition.ToString();
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                if (condensedFieldName == Fields.SearchEngineScore)
                {
                    return GetListValue(_searchEngineScores, indices[0]);
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
            switch (fieldName)
            {
                case Fields.Sequence:
                    Sequence = value;
                    return;
                case Fields.ID:
                    ID = int.Parse(value);
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
                case Fields.Charge:
                    Charge = int.Parse(value);
                    return;
                case Fields.ExperimentalMZ:
                    ExperimentalMZ = double.Parse(value);
                    return;
                case Fields.TheoreticalMZ:
                    TheoreticalMZ = double.Parse(value);
                    return;
                case Fields.Uri:
                    Uri = new Uri(value);
                    return;
                case Fields.SpectraReference:
                    SpectraReference = value;
                    return;
                case Fields.PreviousAminoAcid:
                    PreviousAminoAcid = value[0];
                    return;
                case Fields.FollowingAminoAcid:
                    FollowingAminoAcid = value[0];
                    return;
                case Fields.StartResidue:
                    StartResiduePosition = int.Parse(value);
                    return;
                case Fields.EndResidue:
                    EndResiduePosition = int.Parse(value);
                    return;
            }

            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                if (condensedFieldName == Fields.SearchEngineScore)
                {
                    SetListValue(ref _searchEngineScores, indices[0], double.Parse(value));
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
    }
}