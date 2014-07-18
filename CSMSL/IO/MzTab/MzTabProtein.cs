using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public class MzTabProtein : MzTabEntity
    {
        public MzTabProtein()
            : base(15) { }

        public string Accession
        {
            get { return GetValue<string>(MzTab.ProteinAccessionField); }
            set { SetRawValue(MzTab.ProteinAccessionField, value); }
        }

        public string Description
        {
            get { return GetValue<string>(MzTab.ProteinDescriptionField); }
            set { SetRawValue(MzTab.ProteinDescriptionField, value); }
        }
        
        public int TaxID
        {
            get { return GetValue<int>(MzTab.ProteinTaxIDField); }
            set { SetRawValue(MzTab.ProteinTaxIDField, value); }
        }

        public string Species
        {
            get { return GetValue<string>(MzTab.ProteinSpeciesField); }
            set { SetRawValue(MzTab.ProteinSpeciesField, value); }
        }

        public string Database
        {
            get { return GetValue<string>(MzTab.ProteinDatabaseField); }
            set { SetRawValue(MzTab.ProteinDatabaseField, value); }
        }

        public string DatabaseVersion
        {
            get { return GetValue<string>(MzTab.ProteinDatabaseVersionField); }
            set { SetRawValue(MzTab.ProteinDatabaseVersionField, value); }
        }

        public List<CVParamater> SearchEngine
        {
            get { return GetValue<List<CVParamater>>(MzTab.ProteinSearchEngineField); }
            set { SetRawValue(MzTab.ProteinSearchEngineField, value); }
        }

        public List<double> BestSearchEngineScore
        {
            get { return GetValue<List<double>>(MzTab.ProteinBestSearchEngineScoreField); }
            set { SetRawValue(MzTab.ProteinBestSearchEngineScoreField, value); }
        }

        public MzTab.ReliabilityScore Reliability
        {
            get { return (MzTab.ReliabilityScore)GetValue<int>(MzTab.ProteinReliability); }
            set { SetRawValue(MzTab.ProteinReliability, (int)value); }
        }

        public override void SetValue(string fieldName, string value)
        {
            // Handle array fields
            if (fieldName.Contains("["))
            {
                string condensedFieldName;
                List<int> indices = MzTab.GetFieldIndicies(fieldName, out condensedFieldName);

                switch (condensedFieldName)
                {
                    case MzTab.ProteinBestSearchEngineScoreField:
                        SetRawValue(condensedFieldName, indices[0], double.Parse(value));
                        break;
                    case MzTab.ProteinSearchEngineScoreMsRunField:
                        SetRawValue(condensedFieldName, indices[0],indices[1], double.Parse(value));
                        break;
                }
                return;
            }
            
            switch (fieldName)
            {
                default:
                    Data[fieldName] = value;
                    break;

                case MzTab.ProteinReliability:
                    Data[fieldName] = int.Parse(value);
                    break;
                case MzTab.PsmPreviousAminoAcid:
                case MzTab.PsmFollowingAminoAcid:
                    Data[fieldName] = value[0];
                    break;
                case MzTab.PsmUnique:
                    Data[fieldName] = value.Equals("1");
                    break;
                case MzTab.PsmStartResidue:
                case MzTab.PsmEndResidue:
                case MzTab.PsmCharge:
                case MzTab.ID:
                    Data[fieldName] = int.Parse(value);
                    break;
                case MzTab.PsmRetentionTime:
                case MzTab.PsmExperimentalMZ:
                case MzTab.PsmTheoreticalMZ:
                    Data[fieldName] = double.Parse(value);
                    break;
            }
        }

        internal static IEnumerable<string> GetHeader(IList<MzTabProtein> proteins)
        {
            List<string> headers = new List<string>();
            headers.Add(MzTab.ProteinAccessionField);
            headers.Add(MzTab.ProteinDescriptionField);
            headers.Add(MzTab.ProteinTaxIDField);
            headers.Add(MzTab.ProteinSpeciesField);
            headers.Add(MzTab.ProteinDatabaseField);
            headers.Add(MzTab.ProteinDatabaseVersionField);
            headers.Add(MzTab.ProteinSearchEngineField);

            headers.AddRange(GetHeaders(proteins, MzTab.ProteinBestSearchEngineScoreField));
            
            //TODO search_engine_score[]_ms_run[]

            //if(proteins.Any(p => p.r))
            //headers.Add(MzTab.ProteinReliability);

            return headers;
        }

    }
}
