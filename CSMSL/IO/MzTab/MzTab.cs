using System;
using System.Collections.Generic;
using System.Linq;
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

       // public static Regex OptionalColumnName= new Regex(@"[A-Za-z0-9_-:\]\[]+", RegexOptions.Compiled);

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
            None = 0,
            High = 1,
            Medium = 2,
            Poor = 3
        }

        public enum MetabolomicsReliabilityScore
        {
            None = 0,
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

        #region Protein Fields

        public const string ProteinAccessionField = "accession";
        public const string ProteinDescriptionField = "description";
        public const string ProteinTaxIDField = "taxid";
        public const string ProteinSpeciesField = "species";
        public const string ProteinDatabaseField = "database";
        public const string ProteinDatabaseVersionField = "database_version";
        public const string ProteinSearchEngineField = "search_engine";
        public const string ProteinBestSearchEngineScoreField = "best_search_engine_score[]";
        public const string ProteinSearchEngineScoreMsRunField = "search_engine_score[]_ms_run[]";
        public const string ProteinReliability = "reliability";

        #endregion

        #region PSM Fields
        
        public const string Sequence = "sequence";
        public const string ID = "PSM_ID";
        public const string PsmAccession = "accession";
        public const string PsmUnique = "unique";
        public const string PsmDatabase = "database";
        public const string PsmDatabaseVersion = "database_version";
        public const string PsmSearchEngine = "search_engine";
        public const string PsmSearchEngineScore = "search_engine_score[]";
        public const string PsmRelibaility = "reliability";
        public const string PsmModifications = "modifications";
        public const string PsmRetentionTime = "retention_time";
        public const string PsmCharge = "charge";
        public const string PsmExperimentalMZ = "exp_mass_to_charge";
        public const string PsmTheoreticalMZ = "calc_mass_to_charge";
        public const string PsmUri = "uri";
        public const string PsmSpectraReference = "spectra_ref";
        public const string PsmPreviousAminoAcid = "pre";
        public const string PsmFollowingAminoAcid = "post";
        public const string PsmStartResidue = "start";
        public const string PsmEndResidue = "end";
        
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
        
        public static List<int> GetFieldIndicies(string fieldName, out string condensedFieldName)
        {
            condensedFieldName = fieldName;
            List<int> indices = new List<int>();
            Match match;
            int index = 0;
            while ((match = MultipleEntryRegex.Match(condensedFieldName, index)).Success)
            {
                indices.Add(int.Parse(match.Groups[1].Value));
                condensedFieldName = condensedFieldName.Remove(match.Index + 1, match.Length - 2); // ± 1 for ignoring the [ ] 
                index = match.Index + 1;
            }
            return indices;
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
