// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTab.cs) is part of CSMSL.
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

        /// <summary>
        /// Missing Field Null Text
        /// </summary>
        public const string NullFieldText = "null";

        /// <summary>
        /// Optional Field Prefix
        /// </summary>
        public const string OptionalColumnPrefix = "opt_";

        /// <summary>
        /// Version of the mzTab specification 
        /// <para>
        /// 20 June 2014
        /// </para>
        /// </summary>
        public const string Version = "1.0.0";

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
            NotSet = 0,
            High = 1,
            Medium = 2,
            Poor = 3
        }

        public enum MetabolomicsReliabilityScore
        {
            NotSet = 0,
            IdentifiedMetabolite = 1,
            PutativelyAnnotatedCompound = 2,
            PutativelyCharacterizedCompoundClass = 3,
            UnkownCompound = 4
        }

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

        public enum MzTabMode
        {
            Summary,
            Complete
        };

        public enum MzTabType
        {
            Identification,
            Quantification
        };

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

        internal static string GetArrayName(string fieldName, int index)
        {
            return fieldName.Replace("[]", "[" + index + "]");
        }

        internal static string GetArrayName(string fieldName, int index, int index2)
        {
            int i1 = fieldName.IndexOf('[', 0);
            int i2 = fieldName.IndexOf('[', i1 + 1);
            string iStr = index.ToString();
            string temp = fieldName.Insert(i1 + 1, index.ToString());
            return temp.Insert(i2 + 1 + iStr.Length, index2.ToString());
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