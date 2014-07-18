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
        }

        public int NumberOfOptionalData { get; private set; }

        /// <summary>
        /// The peptide's sequence corresponding to the PSM
        /// </summary>
        public string Sequence
        {
            get { return GetValue<string>(Fields.Sequence); }
            set { SetValue(Fields.Sequence, value); }
        }

        public int ID
        {
            get { return GetValue<int>(MzTab.ID); }
            set { SetRawValue(MzTab.ID, value); }
        }

        public string Accession
        {
            get { return GetValue<string>(MzTab.PsmAccession); }
            set { SetValue(MzTab.PsmAccession, value); }
        }

        public bool Unique
        {
            get { return GetValue<int>(MzTab.PsmUnique) == 1; }
            set { SetRawValue(MzTab.PsmUnique, value ? 1 : 0); }
        }

        public string Database
        {
            get { return GetValue<string>(MzTab.PsmDatabase); }
            set { SetValue(MzTab.PsmDatabase, value); }
        }

        public string DatabaseVersion
        {
            get { return GetValue<string>(MzTab.PsmDatabaseVersion); }
            set { SetValue(MzTab.PsmDatabaseVersion, value); }
        }

        public List<CVParamater> SearchEngines
        {
            get { return GetValue<List<CVParamater>>(MzTab.PsmSearchEngine); }
            set { SetRawValue(MzTab.PsmSearchEngine, value); }
        }

        public List<double> SearchEngineScores
        {
            get { return GetValue<List<double>>(MzTab.PsmSearchEngineScore); }
            set { SetRawValue(MzTab.PsmSearchEngineScore, value); }
        }

        public MzTab.ReliabilityScore Reliability
        {
            get { return GetValue<MzTab.ReliabilityScore>(MzTab.PsmRelibaility); }
            set { SetRawValue(MzTab.PsmRelibaility, value); }
        }

        public string Modifications
        {
            get { return GetValue<string>(MzTab.PsmModifications); }
            set { SetRawValue(MzTab.PsmModifications, value); }
        }

        public List<double> RetentionTime
        {
            get { return GetValue<List<double>>(MzTab.PsmRetentionTime); }
            set { SetRawValue(MzTab.PsmRetentionTime, value); }
        }

        public int Charge     
        {
            get { return GetValue<int>(MzTab.PsmCharge); }
            set { SetRawValue(MzTab.PsmCharge, value); }
        }

        public double ExperimentalMZ
        {
            get { return GetValue<double>(MzTab.PsmExperimentalMZ); }
            set { SetRawValue(MzTab.PsmExperimentalMZ, value); }
        }

        public double TheoreticalMZ
        {
            get { return GetValue<double>(MzTab.PsmTheoreticalMZ); }
            set { SetRawValue(MzTab.PsmTheoreticalMZ, value); }
        }

        public Uri Uri
        {
            get { return GetValue<Uri>(MzTab.PsmUri); }
            set { SetRawValue(MzTab.PsmUri, value); }
        }

        public string SpectraReference
        {
            get { return GetValue<string>(MzTab.PsmSpectraReference); }
            set { SetRawValue(MzTab.PsmSpectraReference, value); }
        }

        public char PreviousAminoAcid
        {
            get { return GetValue<char>(MzTab.PsmPreviousAminoAcid); }
            set { SetRawValue(MzTab.PsmPreviousAminoAcid, value); }
        }

        public char FollowingAminoAcid
        {
            get { return GetValue<char>(MzTab.PsmFollowingAminoAcid); }
            set { SetRawValue(MzTab.PsmFollowingAminoAcid, value); }
        }
        
        public int EndResiduePosition
        {
            get { return GetValue<int>(MzTab.PsmEndResidue);}
            set { SetRawValue(MzTab.PsmEndResidue, value); }
        }

        public int StartResiduePosition
        {
            get { return GetValue<int>(MzTab.PsmStartResidue); }
            set { SetRawValue(MzTab.PsmStartResidue, value); }
        }

        public MzTabPSM()
            : base(18) { }

        public override string ToString()
        {
            return string.Format("(#{0}) {1}", ID, Sequence);
        }

        public override void SetValue(string fieldName, string value)
        {
            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                if (condensedFieldName == MzTab.PsmSearchEngineScore)
                {
                    SetRawValue(condensedFieldName, indices[0], double.Parse(value));
                    return;
                }
            }
            
            if (fieldName.StartsWith(MzTab.OptionalColumnPrefix))
            {
                NumberOfOptionalData++;
                Data[fieldName] = value;
                return;
            }
          
            switch (fieldName)
            {
                default:
                    Data[fieldName] = value;
                    break;
                case MzTab.PsmUri:
                    Data[fieldName] = new Uri(value);
                    break;
                case MzTab.PsmSearchEngine:
                    Data[fieldName] = value.Split('|').Select(datum => (CVParamater)datum).ToList();
                    break;
                case MzTab.PsmPreviousAminoAcid:
                case MzTab.PsmFollowingAminoAcid:
                    Data[fieldName] = value[0];
                    break;
                case MzTab.PsmUnique:
                case MzTab.PsmStartResidue:
                case MzTab.PsmEndResidue:
                case MzTab.PsmCharge:
                case MzTab.ID:
                case MzTab.PsmRelibaility:
                    Data[fieldName] = int.Parse(value);
                    break;
                case MzTab.PsmRetentionTime:
                    Data[fieldName] = value.Split('|').Select(double.Parse).ToList();
                    break;
                case MzTab.PsmExperimentalMZ:
                case MzTab.PsmTheoreticalMZ:
                    Data[fieldName] = double.Parse(value);
                    break;
            }
        }
        
        internal static IEnumerable<string> GetHeader(IList<MzTabPSM> psms)
        {
            List<string> headers = new List<string>();
            headers.Add(MzTab.Sequence);
            headers.Add(MzTab.ID);
            headers.Add(MzTab.PsmAccession);
            headers.Add(MzTab.PsmUnique);
            headers.Add(MzTab.PsmDatabase);
            headers.Add(MzTab.PsmDatabaseVersion);
            headers.Add(MzTab.PsmSearchEngine);

            headers.AddRange(GetHeaders(psms, MzTab.PsmSearchEngineScore));
            
            // Only report reliability if one psm has a non-null reliability score
            if (psms.Any(psm => psm.Reliability != MzTab.ReliabilityScore.None))
                headers.Add(MzTab.PsmRelibaility);

            headers.Add(MzTab.PsmModifications);
            headers.Add(MzTab.PsmRetentionTime);
            headers.Add(MzTab.PsmCharge);
            headers.Add(MzTab.PsmExperimentalMZ);
            headers.Add(MzTab.PsmTheoreticalMZ);

            if (psms.Any(psm => psm.Uri != null))
                headers.Add(MzTab.PsmUri);

            headers.Add(MzTab.PsmSpectraReference);
            headers.Add(MzTab.PsmPreviousAminoAcid);
            headers.Add(MzTab.PsmFollowingAminoAcid);
            headers.Add(MzTab.PsmStartResidue);
            headers.Add(MzTab.PsmEndResidue);

            // Optional Parameters
            headers.AddRange(psms.Where(psm => psm.GetOptionalFields() != null).SelectMany(psm => psm.GetOptionalFields()));

            return headers;
        }
    }
}
