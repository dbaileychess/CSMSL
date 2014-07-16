namespace CSMSL.IO.MzTab
{
    public class MzTabPSM : MzTabEntity
    {
        public int NumberOfSearchEngines { get; private set; }
        public int NumberOfOptionalData { get; private set; }

        public string Sequence
        {
            get { return GetValue<string>(MzTab.PSMSequenceField); }
            set { SetValue(MzTab.PSMSequenceField, value); }
        }

        public int ID
        {
            get { return GetValue<int>(MzTab.PSMIdField); }
            set { SetRawValue(MzTab.PSMIdField, value); }
        }

        public string Accession
        {
            get { return GetValue<string>(MzTab.PSMAcessionField); }
            set { SetValue(MzTab.PSMAcessionField, value); }
        }

        public bool Unique
        {
            get { return GetValue<bool>(MzTab.PSMUniqueField); }
            set { SetRawValue(MzTab.PSMUniqueField, value); }
        }

        public string Database
        {
            get { return GetValue<string>(MzTab.PSMDatabaseField); }
            set { SetValue(MzTab.PSMDatabaseField, value); }
        }

        public string DatabaseVersion
        {
            get { return GetValue<string>(MzTab.PSMDatabaseVersionField); }
            set { SetValue(MzTab.PSMDatabaseVersionField, value); }
        }

        public string SearchEngines
        {
            get { return GetValue<string>(MzTab.PSMSearchEngineField); }
            set { SetValue(MzTab.PSMSearchEngineField, value); }
        }

        public int? Reliability
        {
            get { return GetValue<int?>(MzTab.PSMRelibailityField); }
            set { SetRawValue(MzTab.PSMRelibailityField, value); }
        }

        public string Modifications
        {
            get { return GetValue<string>(MzTab.PSMModificationsField); }
            set { SetRawValue(MzTab.PSMModificationsField, value); }
        }

        public double RetentionTime
        {
            get { return GetValue<double>(MzTab.PSMRetentionTimeField); }
            set { SetRawValue(MzTab.PSMRetentionTimeField, value); }
        }

        public int Charge     
        {
            get { return GetValue<int>(MzTab.PSMChargeField); }
            set { SetRawValue(MzTab.PSMChargeField, value); }
        }

        public double ExperimentalMZ
        {
            get { return GetValue<double>(MzTab.PSMExperimentalMZField); }
            set { SetRawValue(MzTab.PSMExperimentalMZField, value); }
        }

        public double TheoreticalMZ
        {
            get { return GetValue<double>(MzTab.PSMTheoreticalMZField); }
            set { SetRawValue(MzTab.PSMTheoreticalMZField, value); }
        }

        public string Uri
        {
            get { return GetValue<string>(MzTab.PSMUriField); }
            set { SetRawValue(MzTab.PSMUriField, value); }
        }

        public string SpectraReference
        {
            get { return GetValue<string>(MzTab.PSMSpectraReferenceField); }
            set { SetRawValue(MzTab.PSMSpectraReferenceField, value); }
        }

        public char PreviousAminoAcid
        {
            get { return GetValue<char>(MzTab.PSMPreviousAminoAcidField); }
            set { SetRawValue(MzTab.PSMPreviousAminoAcidField, value); }
        }

        public char FollowingAminoAcid
        {
            get { return GetValue<char>(MzTab.PSMFollowingAminoAcidField); }
            set { SetRawValue(MzTab.PSMFollowingAminoAcidField, value); }
        }
        
        public int EndResiduePosition
        {
            get { return GetValue<int>(MzTab.PSMEndResidueField);}
            set { SetRawValue(MzTab.PSMEndResidueField, value); }
        }

        public int StartResiduePosition
        {
            get { return GetValue<int>(MzTab.PSMStartResidueField); }
            set { SetRawValue(MzTab.PSMStartResidueField, value); }
        }

        public MzTabPSM(int capacity = 18)
            : base(capacity) { }

        public double? GetEngineScore(int index)
        {
            return GetValue<double?>(MzTab.PSMSearchEngineScoreField + "[" + index + "]");
        }

        public string GetOptionalData(string optionalParameter)
        {
            object data;
            if (!Data.TryGetValue(optionalParameter, out data))
                return null;
            return (string)data;
        }
        
        public override void SetValue(string fieldName, string value)
        {
            if (fieldName.StartsWith(MzTab.PSMSearchEngineScoreField))
            {
                Data[fieldName] = double.Parse(value);
                NumberOfSearchEngines++;
                return;
            }

            if (fieldName.StartsWith(MzTab.OptionalColumnPrefix))
            {
                NumberOfOptionalData++;
            }
          
            switch (fieldName)
            {
                default:
                    Data[fieldName] = value;
                    break;
                case MzTab.PSMPreviousAminoAcidField:
                case MzTab.PSMFollowingAminoAcidField:
                    Data[fieldName] = value[0];
                    break;
                case MzTab.PSMUniqueField:
                    Data[fieldName] = value.Equals("1");
                    break;
                case MzTab.PSMStartResidueField:
                case MzTab.PSMEndResidueField:
                case MzTab.PSMChargeField:
                case MzTab.PSMIdField:
                case MzTab.PSMRelibailityField:
                    Data[fieldName] = int.Parse(value);
                    break;
                case MzTab.PSMRetentionTimeField:
                case MzTab.PSMExperimentalMZField:
                case MzTab.PSMTheoreticalMZField:
                    Data[fieldName] = double.Parse(value);
                    break;
            }
        }
        
        public override string ToString()
        {
            return string.Format("(#{0}) {1}", ID, Sequence);
        }

    }
}
