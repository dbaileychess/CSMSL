// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Protease.cs) is part of CSMSL.
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace CSMSL.Proteomics
{
    public class Protease : IProtease
    {
        #region Static

        public static readonly string UserProteasePath;

        private static readonly Dictionary<string, Protease> Proteases;

        static Protease()
        {
            UserProteasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"CSMSL\Proteases.xml");

            Proteases = new Dictionary<string, Protease>();

            // Load the default modification file
            Load();
        }

        /// <summary>
        /// Load the default modification file
        /// If the default modification is missing or corrupted, it will auto generate it
        /// </summary>
        public static void Load()
        {
            // Create file if it doesn't exist
            if (!File.Exists(UserProteasePath))
            {
                RestoreDefaults();
            }

            Load(UserProteasePath);
        }

        public static void RestoreDefaults()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream defaultModsStream = assembly.GetManifestResourceStream("CSMSL.Resources.Proteases.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(UserProteasePath));
            using (var fileStream = File.Create(UserProteasePath))
            {
                if (defaultModsStream != null) defaultModsStream.CopyTo(fileStream);
            }

            Load();
        }

        /// <summary>
        /// Load a protease file
        /// </summary>
        /// <param name="filePath">The path to the protase file</param>
        public static void Load(string filePath)
        {
            Proteases.Clear();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement() || !reader.Name.Equals("Protease"))
                        continue;

                    string name = reader["name"];
                    Terminus terminus = reader["terminus"] == "N" ? Terminus.N : Terminus.C;
                    string cut = reader["cut"] ?? "";
                    string nocut = reader["nocut"] ?? "";
                    Proteases.Add(name, new Protease(name, terminus, cut, nocut));
                }
            }
        }

        public static void Save()
        {
            SaveTo(UserProteasePath);
        }

        public static void SaveTo(string filePath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings {Indent = true}))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Proteases");

                foreach (var protease in Proteases.Values.Distinct())
                {
                    writer.WriteStartElement("Protease");
                    writer.WriteAttributeString("name", protease.Name);
                    writer.WriteAttributeString("terminus", protease.Terminal.ToString());
                    if (!string.IsNullOrEmpty(protease.Cut))
                    {
                        writer.WriteAttributeString("cut", protease.Cut);
                    }
                    if (!string.IsNullOrEmpty(protease.NoCut))
                    {
                        writer.WriteAttributeString("nocut", protease.NoCut);
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement(); // end Proteases

                writer.WriteEndDocument();
            }
        }

        public static IEnumerable<Protease> GetAllProteases()
        {
            return Proteases.Values;
        }

        public static Protease GetProtease(string name)
        {
            return Proteases[name];
        }

        public static bool TryGetProtease(string name, out Protease protease)
        {
            return Proteases.TryGetValue(name, out protease);
        }

        public static Protease AddProtease(string name, Terminus terminus, string cut, string noCut = "")
        {
            Protease protease = new Protease(name, terminus, cut, noCut);
            Proteases[name] = protease;

            OnProteasesChanged();

            return protease;
        }

        public static bool RemoveProtease(Protease protease)
        {
            return RemoveProtease(protease.Name);
        }

        public static bool RemoveProtease(string name)
        {
            if (Proteases.Remove(name))
            {
                OnProteasesChanged();
                return true;
            }
            return false;
        }

        private static void OnProteasesChanged()
        {
            var handler = ProteasesChanged;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }

        public static event EventHandler ProteasesChanged;

        #endregion

        public Protease this[string name]
        {
            get { return GetProtease(name); }
        }

        private readonly Regex _cleavageRegex;

        public string Cut { get; private set; }

        public string NoCut { get; private set; }

        public string Name { get; set; }

        public Terminus Terminal { get; private set; }

        public string CleavagePattern
        {
            get { return _cleavageRegex.ToString(); }
        }

        public int MissedCleavages(string sequence)
        {
            sequence = sequence.ToUpper();
            MatchCollection matches = _cleavageRegex.Matches(sequence);

            int count = matches.Count;

            if (count == 0)
                return 0;

            if (Terminal == Terminus.N)
            {
                if (matches[0].Index == 0)
                    return count - 1;
                return count;
            }

            if (matches[count - 1].Index == sequence.Length - 1)
                return count - 1;

            return count;
        }

        public int MissedCleavages(IAminoAcidSequence aminoAcidSequence)
        {
            return MissedCleavages(aminoAcidSequence.Sequence);
        }

        public Protease(string name, Terminus terminus, string cut, string nocut = "", string cleavePattern = "")
        {
            Name = name;
            Terminal = terminus;
            Cut = cut;
            NoCut = nocut;

            if (string.IsNullOrEmpty(cleavePattern))
            {
                StringBuilder constructedRegex = new StringBuilder();

                // Add grouping [ ] if more than one residue
                string cutregex = (Cut.Length > 1) ? "[" + Cut + "]" : Cut;
                string nocutregex = (NoCut.Length > 1) ? "[" + NoCut + "]" : NoCut;

                if (terminus == Terminus.N)
                {
                    // Apply negative look-a-head for N-terminal no cuts
                    if (!string.IsNullOrEmpty(nocut))
                    {
                        constructedRegex.Append("(?<!" + nocutregex + ")");
                    }
                    constructedRegex.Append("(?'cut')");
                    constructedRegex.Append(cutregex);
                }
                else
                {
                    constructedRegex.Append(cutregex);
                    constructedRegex.Append("(?'cut')");
                    if (!string.IsNullOrEmpty(nocut))
                    {
                        constructedRegex.Append("(?!" + nocutregex + ")");
                    }
                }

                _cleavageRegex = new Regex(constructedRegex.ToString(), RegexOptions.Compiled);
            }
            else
            {
                _cleavageRegex = new Regex(cleavePattern, RegexOptions.Compiled);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<int> GetDigestionSites(IAminoAcidSequence aminoacidpolymer)
        {
            return GetDigestionSites(aminoacidpolymer.Sequence);
        }

        public IEnumerable<int> GetDigestionSites(string sequence)
        {
            return (from Match match in _cleavageRegex.Matches(sequence) select match.Groups["cut"].Index - 1);
        }
    }
}