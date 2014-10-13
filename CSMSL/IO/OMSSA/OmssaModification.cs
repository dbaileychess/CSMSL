// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (OmssaModification.cs) is part of CSMSL.
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
using System.Reflection;
using System.Text;
using System.Xml;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace CSMSL.IO.OMSSA
{
    public class OmssaModification : Modification
    {
        private static readonly Dictionary<int, string> _modificationKeyDicitonary;

        private static readonly Dictionary<string, OmssaModification> Modifications;

        static OmssaModification()
        {
            _modificationKeyDicitonary = new Dictionary<int, string>();
            Modifications = new Dictionary<string, OmssaModification>();

            // Load in the default omssa mods
            var assembly = Assembly.GetExecutingAssembly();

            LoadOmssaModifications(assembly.GetManifestResourceStream("CSMSL.IO.OMSSA.Resources.mods.xml"), false);
        }

        public int ID { get; set; }

        public bool UserMod { get; set; }

        public OmssaModification(string name, int id, double mono, double average, bool userMod, ModificationSites sites = ModificationSites.None)
            : base(mono, name, sites)
        {
            ID = id;
            UserMod = userMod;
        }

        public static bool TryGetModification(int id, out OmssaModification modification)
        {
            string name;
            modification = null;
            return _modificationKeyDicitonary.TryGetValue(id, out name) && TryGetModification(name, out modification);
        }

        public static bool TryGetModification(string name, out OmssaModification modification)
        {
            return Modifications.TryGetValue(name, out modification);
        }

        private static readonly char[] _omssaModDelimiter = {',', ';'};
        private static readonly char[] _omssaModPartDelimiter = {':'};

        public static void LoadOmssaModifications(Stream stream, bool userMod = true)
        {
            XmlDocument mods_xml = new XmlDocument();
            mods_xml.Load(stream);
            XmlNamespaceManager mods_xml_ns = new XmlNamespaceManager(mods_xml.NameTable);
            mods_xml_ns.AddNamespace("omssa", mods_xml.ChildNodes[1].Attributes["xmlns"].Value);
            foreach (XmlNode mod_node in mods_xml.SelectNodes("/omssa:MSModSpecSet/omssa:MSModSpec", mods_xml_ns))
            {
                string name = mod_node.SelectSingleNode("./omssa:MSModSpec_name", mods_xml_ns).FirstChild.Value;
                int id = int.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_mod/omssa:MSMod", mods_xml_ns).FirstChild.Value);
                double mono = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_monomass", mods_xml_ns).FirstChild.Value);
                double average = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_averagemass", mods_xml_ns).FirstChild.Value);
                ModificationSites sites = ModificationSites.None;
                foreach (
                    XmlNode node in
                        mod_node.SelectNodes("./omssa:MSModSpec_residues/omssa:MSModSpec_residues_E", mods_xml_ns))
                {
                    string aa = node.FirstChild.Value;
                    sites = sites.Set(aa[0]);
                }
                OmssaModification mod = new OmssaModification(name, id, mono, average, userMod, sites);
                Modifications.Add(name, mod);
                _modificationKeyDicitonary[id] = name;
            }
        }

        public static void LoadOmssaModifications(string file, bool userMod = true)
        {
            XmlDocument mods_xml = new XmlDocument();
            mods_xml.Load(file);
            XmlNamespaceManager mods_xml_ns = new XmlNamespaceManager(mods_xml.NameTable);
            mods_xml_ns.AddNamespace("omssa", mods_xml.ChildNodes[1].Attributes["xmlns"].Value);
            foreach (XmlNode mod_node in mods_xml.SelectNodes("/omssa:MSModSpecSet/omssa:MSModSpec", mods_xml_ns))
            {
                string name = mod_node.SelectSingleNode("./omssa:MSModSpec_name", mods_xml_ns).FirstChild.Value;
                int id = int.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_mod/omssa:MSMod", mods_xml_ns).FirstChild.Value);
                double mono = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_monomass", mods_xml_ns).FirstChild.Value);
                double average = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_averagemass", mods_xml_ns).FirstChild.Value);
                int modType = int.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_type/omssa:MSModType", mods_xml_ns).FirstChild.Value);
                ModificationSites sites = ModificationSites.None;
                switch (modType)
                {
                    case 0:
                        foreach (
                            XmlNode node in
                                mod_node.SelectNodes("./omssa:MSModSpec_residues/omssa:MSModSpec_residues_E", mods_xml_ns))
                        {
                            string aa = node.FirstChild.Value;
                            sites = sites.Set(aa[0]);
                        }
                        break;
                    case 1:
                        sites |= ModificationSites.NProt;
                        break;
                    case 3:
                        sites |= ModificationSites.ProtC;
                        break;
                    case 5:
                        sites |= ModificationSites.NPep;
                        break;
                    case 6:
                        sites |= ModificationSites.PepC;
                        break;
                }


                OmssaModification mod = new OmssaModification(name, id, mono, average, userMod, sites);
                Modifications[name] = mod;
                _modificationKeyDicitonary[id] = name;
            }
        }

        public static IEnumerable<Tuple<string, int>> SplitModificationLine(string line)
        {
            foreach (string mod in line.Split(_omssaModDelimiter, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] modParts = mod.Split(_omssaModPartDelimiter, StringSplitOptions.RemoveEmptyEntries);
                yield return new Tuple<string, int>(modParts[0].Trim(), int.Parse(modParts[1]));
            }
        }

        public static Dictionary<string, Modification> GroupedModifications = new Dictionary<string, Modification>();

        public static IEnumerable<Tuple<Modification, int>> ParseModificationLine(string line)
        {
            foreach (Tuple<string, int> mod in SplitModificationLine(line))
            {
                string modName = mod.Item1;
                Modification groupedMod;
                if (GroupedModifications.TryGetValue(modName, out groupedMod))
                {
                    yield return new Tuple<Modification, int>(groupedMod, mod.Item2);
                }
                else
                {
                    OmssaModification modification;
                    if (TryGetModification(modName, out modification))
                    {
                        yield return new Tuple<Modification, int>(modification, mod.Item2);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Modification: " + mod.Item1 + " is not found in the modification dictionary");
                    }
                }
            }
        }

        public static string WriteModificationString(AminoAcidPolymer polymer)
        {
            IMass[] mods = polymer.GetModifications();
            int count = mods.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                if (mods[i] == null)
                    continue;
                sb.Append(mods[i].ToString());
                sb.Append(":");
                sb.Append(i);
                sb.Append(',');
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public static IEnumerable<OmssaModification> GetAllModifications()
        {
            return Modifications.Values;
        }
    }

    public static class OmssaModExtension
    {
        /// <summary>
        /// Parse and apply an omssa modification line to this current amino acid polymer
        /// </summary>
        /// <param name="aap"></param>
        /// <param name="omssaModificationLine"></param>
        /// <returns></returns>
        public static AminoAcidPolymer SetModifications(this AminoAcidPolymer aap, string omssaModificationLine)
        {
            if (string.IsNullOrEmpty(omssaModificationLine))
                return aap;

            foreach (Tuple<Modification, int> modPosition in OmssaModification.ParseModificationLine(omssaModificationLine))
            {
                int pos = modPosition.Item2;
                if (pos == 0)
                {
                    aap.NTerminusModification = modPosition.Item1;
                }
                else if (pos == aap.Length + 1)
                {
                    aap.CTerminusModification = modPosition.Item1;
                }
                else
                {
                    aap.SetModification(modPosition.Item1, pos);
                }
            }

            return aap;
        }
    }
}