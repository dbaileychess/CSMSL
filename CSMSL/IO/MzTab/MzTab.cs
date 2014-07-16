using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CSMSL.IO.MzTab
{

    /// <summary>
    /// mzTab: exchange format for proteomics and metabolomics results
    /// </summary>
    public static class MzTab
    {
        /// <summary>
        /// The column separator is a tab character
        /// </summary>
        public const char FieldSeparator = '\t'; // Unicode 0009

        /// <summary>
        /// The default encoding is UTF8
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/2437666/write-text-files-without-byte-order-mark-bom
        /// </remarks>
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false);

        /// <summary>
        /// A regex for extracting the index part of a multiple field
        /// <para>
        /// i.e. fixed_mod[1] fixed_mod[5] etc..
        /// </para>
        /// </summary>
        public static Regex MultipleEntryRegex = new Regex(@"\[([1-9][0-9]*)\]", RegexOptions.Compiled);

        /// <summary>
        /// All column labels and field names are case-sensitive
        /// </summary>
        public const bool CaseSensitive = true;

        /// <summary>
        /// Arrays are 1-based
        /// </summary>
        public const int IndexBased = 1;

        public const string OptionalColumnPrefix = "opt_";

        /// <summary>
        /// Version of the mzTab specification 
        /// <para>
        /// 20 June 2014
        /// </para>
        /// </summary>
        public const string Version = "1.0.0 rc 5";

        #region Line Prefixes

        public const string MetaDataLinePrefix = "MTD";
        public const string CommentLinePrefix = "COM";
        public const string ProteinTableLinePrefix = "PRH";
        public const string ProteinDataLinePrefix = "PRT";
        public const string PeptideTableLinePrefix = "PEH";
        public const string PeptideDataLinePrefix = "PEP";
        public const string PsmTableLinePrefix = "PSH";
        public const string PsmDataLinePrefix = "PSM";
        public const string SmallMoleculeTableLinePrefix = "SMH";
        public const string SmallMoleculeDataLinePrefix = "SML";

        public enum LinePrefix
        {          
            MetaData,
            Comment,
            ProteinTable,
            ProteinData,
            PeptideTable,
            PeptideData,
            PsmTable,
            PsmData,
            SmallMoleculeTable,
            SmallMoleculeData
        }

        public enum ReliabilityScore
        {
            High = 1,
            Medium = 2,
            Poor = 3
        }

        public enum MetabolomicsReliabilityScore
        {
            IdentifiedMetabolite = 1,
            PutativelyAnnotatedCompound = 2,
            PutativelyCharacterizedCompoundClass = 3,
            UnkownCompound = 4
        }

        #endregion

        #region Table Sections

        public const string PeptideSection = "Peptides";
        public const string ProteinSection = "Proteins";
        public const string PsmSection = "PSMs";
        public const string SmallMoleculeSection = "SmallMolecules";

        #endregion

        #region MetaData Fields

        public const string MDVersionField = "mzTab-version";
        public const string MDModeField = "mzTab-mode";
        public const string MDTypeField = "mzTab-type";
        public const string MDDescriptionField = "description";
        public const string MDProteinQuantificationUnit = "protein-quantification_unit";
        public const string MDIDField = "mzTab-ID";
        public const string MDTitleField = "title";
        public const string MDMsRunLocationField = "ms_run[]-location";
        public const string MDFixedModField = "fixed_mod[]";
        public const string MDVariableModField = "variable_mod[]";
        public const string MDStudyVariableDescriptionField = "study_variable[]-description";
        public const string MDPsmSearchEngineScoreField = "psm_search_engine_score[]";
        public const string MDProteinSearchEngineScoreField = "protein_search_engine_score[]";

        #endregion

        #region PSM Section Mandatory Fields

        public const string PSMSequenceField = "sequence";
        public const string PSMIdField = "PSM_ID";
        public const string PSMAcessionField = "accession";
        public const string PSMUniqueField = "unique";
        public const string PSMDatabaseField = "database";
        public const string PSMDatabaseVersionField = "database_version";
        public const string PSMSearchEngineField = "search_engine";
        public const string PSMSearchEngineScoreField = "search_engine_score";
        public const string PSMRelibailityField = "reliability";
        public const string PSMModificationsField = "modifications";
        public const string PSMRetentionTimeField = "retention_time";
        public const string PSMChargeField = "charge";
        public const string PSMExperimentalMZField = "exp_mass_to_charge";
        public const string PSMTheoreticalMZField = "calc_mass_to_charge";
        public const string PSMUriField = "uri";
        public const string PSMSpectraReferenceField = "spectra_ref";
        public const string PSMPreviousAminoAcidField = "pre";
        public const string PSMFollowingAminoAcidField = "post";
        public const string PSMStartResidueField = "start";
        public const string PSMEndResidueField = "end";
        
        #endregion

        #region Date/time Format

        /// <summary>
        /// The mzTab format for date (ISO 8601)
        /// </summary>
        public const string DateFormat = "YYYY-MM-DD";

        /// <summary>
        /// The mzTab format for datetime (ISO 8601)
        /// </summary>
        public const string DateTimeFormat = "YYYY-MM-DDTHH:MMZ";

        #endregion

        public enum MzTabMode { Summary, Complete };
        public enum MzTabType { Identification, Quantification };

        [Flags]
        internal enum States
        {
            None = 0,
            MetaData = 1 << 0,
            ProteinHeader = 1 << 1,
            ProteinData = 1 << 2,
            PeptideHeader = 1 << 3,
            PeptideData = 1 << 4,
            PsmHeader = 1 << 5,
            PsmData = 1 << 6,
            SmallMoleculeHeader = 1 << 7,
            SmallMoleculeData = 1 << 8
        }

        public static string ParamsToString(string cvLabel = "", string accession = "", string name = "", string value = "")
        {
            // According to specs, the name field needs to be quoted if it contains an internal comma
            if (name.Contains(","))
            {
                name = "\"" + name + "\"";
            }
            return string.Format("[{0},{1},{2},{3}]", cvLabel, accession, name, value);
        }

        public static int GetIndex(string value)
        {
            Match m = MultipleEntryRegex.Match(value);
            if (!m.Success)
            {
                throw new Exception("The value " + value + " is in a incorrect format for extracting an index");
            }
            int index = int.Parse(m.Groups[1].Value);
            return index;
        }

        public static string GetLinePrefixText(LinePrefix prefix)
        {
            switch (prefix)
            {
                case LinePrefix.Comment:
                    return CommentLinePrefix;
                case LinePrefix.MetaData:
                    return MetaDataLinePrefix;
                case LinePrefix.PeptideData:
                    return PeptideDataLinePrefix;
                case LinePrefix.PeptideTable:
                    return PeptideTableLinePrefix;
                case LinePrefix.ProteinData:
                    return ProteinDataLinePrefix;
                case LinePrefix.ProteinTable:
                    return ProteinTableLinePrefix;
                case LinePrefix.PsmData:
                    return PsmDataLinePrefix;
                case LinePrefix.PsmTable:
                    return PsmTableLinePrefix;
                case LinePrefix.SmallMoleculeData:
                    return SmallMoleculeDataLinePrefix;
                case LinePrefix.SmallMoleculeTable:
                    return SmallMoleculeTableLinePrefix;
            }
            throw new Exception("Shouldn't be able to get here");
        }
    }
}
