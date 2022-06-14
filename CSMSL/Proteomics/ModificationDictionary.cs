// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CSMSL.Chemistry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace CSMSL.Proteomics
{
    public static class ModificationDictionary
    {
        private static readonly Dictionary<string, Modification> Modifications;

        private static readonly string UserModificationPath;

        static ModificationDictionary()
        {
            UserModificationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"CSMSL\Modifications.xml");

            Modifications = new Dictionary<string, Modification>();

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
            if (!File.Exists(UserModificationPath))
            {
                RestoreDefaults();
            }

            Load(UserModificationPath);
        }

        /// <summary>
        /// Load a modification file
        /// </summary>
        /// <param name="filePath">The path to the modification file</param>
        public static void Load(string filePath)
        {
            try
            {
                var modsXml = new XmlDocument();
                modsXml.Load(filePath);

                foreach (XmlNode modNode in modsXml.SelectNodes("//Modifications/Modification"))
                {
                    string modName = modNode.Attributes["name"].Value;
                    string chemicalFormula = "";
                    bool isChemicalFormula = false;
                    double mass = 0;
                    var chemModNode = modNode.SelectSingleNode("ChemicalFormula");
                    if (chemModNode != null)
                    {
                        chemicalFormula = modNode.SelectSingleNode("ChemicalFormula").InnerText;
                        isChemicalFormula = true;
                    }
                    else
                    {
                        mass = double.Parse(modNode.SelectSingleNode("DeltaMass").InnerText, CultureInfo.CurrentCulture);
                    }

                    ModificationSites sites = ModificationSites.None;
                    foreach (XmlNode siteNode in modNode.SelectNodes("ModificationSite"))
                    {
                        string modSite = siteNode.InnerText;
                        var site = (ModificationSites) Enum.Parse(typeof (ModificationSites), modSite);
                        sites |= site;
                    }

                    var modification = isChemicalFormula ? new ChemicalFormulaModification(chemicalFormula, modName, sites) : new Modification(mass, modName, sites);

                    Modifications.Add(modName, modification);

                    foreach (XmlNode modAltNameNode in modNode.SelectNodes("AlternativeName"))
                    {
                        string altName = modAltNameNode.InnerText;
                        Modifications.Add(altName, modification);
                    }
                }
                OnModificationsChanged(false);
            }
            catch (XmlException)
            {
                //RestoreDefaults();
            }
        }

        /// <summary>
        /// Saves the current modifications and isotopologues to the default modification file
        /// </summary>
        public static void Save()
        {
            SaveTo(UserModificationPath);
        }

        /// <summary>
        /// Saves the current modifications and isotopologues
        /// </summary>
        public static void SaveTo(string filePath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings {Indent = true}))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Modifications");

                foreach (var mod in Modifications.Values.Distinct())
                {
                    writer.WriteStartElement("Modification");
                    writer.WriteAttributeString("name", mod.Name);

                    IChemicalFormula chemFormula = mod as IChemicalFormula;
                    if (chemFormula != null)
                    {
                        writer.WriteElementString("ChemicalFormula", chemFormula.ToString());
                    }
                    else
                    {
                        writer.WriteElementString("DeltaMass", mod.MonoisotopicMass.ToString("F10"));
                    }

                    foreach (ModificationSites site in mod.Sites.GetActiveSites())
                    {
                        writer.WriteElementString("ModificationSite", site.ToString());
                    }

                    foreach (KeyValuePair<string, Modification> mods in Modifications)
                    {
                        if (mods.Value.Equals(mod))
                        {
                            writer.WriteElementString("AlternativeName", mods.Key);
                        }
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // end Modifications

                writer.WriteEndDocument();
            }
        }

        public static void RestoreDefaults()
        {
            Modifications.Clear();

            var assembly = Assembly.GetExecutingAssembly();
            Stream defaultModsStream = assembly.GetManifestResourceStream("CSMSL.Resources.Modifications.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(UserModificationPath));
            using (var fileStream = File.Create(UserModificationPath))
            {
                if (defaultModsStream != null) defaultModsStream.CopyTo(fileStream);
            }

            Load();
        }

        private static void OnModificationsChanged(bool saveToDisk = true)
        {
            // Flush to disk
            if (saveToDisk)
                Save();

            var handler = ModificationsChanged;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }

        public static event EventHandler ModificationsChanged;

        public static IEnumerable<Modification> GetAllModifications()
        {
            return Modifications.Values.Distinct();
        }

        public static Modification GetModification(string name)
        {
            return GetModification<Modification>(name);
        }

        public static T GetModification<T>(string name) where T : Modification
        {
            Modification mod;
            Modifications.TryGetValue(name, out mod);
            return mod as T;
        }

        public static bool TryGetModification(string name, out Modification modification)
        {
            return TryGetModification<Modification>(name, out modification);
        }

        public static bool TryGetModification<T>(string name, out T modification) where T : Modification
        {
            Modification mod;
            if (!Modifications.TryGetValue(name, out mod))
            {
                modification = null;
                return false;
            }
            modification = mod as T;
            return modification != null;
        }

        public static void AddModification(Modification modification, string name = "", bool saveToDisk = false)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Modifications[name] = modification;
            }
            else
            {
                Modifications[modification.Name] = modification;
            }

            OnModificationsChanged(saveToDisk);
        }

        public static bool RemoveModification(string name)
        {
            if (!Modifications.Remove(name))
                return false;

            OnModificationsChanged();
            return true;
        }
    }
}