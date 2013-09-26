using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      
            LoadOmssaModifications(assembly.GetManifestResourceStream("CSMSL.IO.OMSSA.Resources.mods.xml"),false);
        }
        
        public int ID { get; set; }

        public OmssaModification(string name, int id, double mono, double average, ModificationSites sites = ModificationSites.None)
            : base(mono, name, sites)
        {
            ID = id;
        }
         
        public override string ToString()
        {
            return Name;
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

        private static readonly char[] _omssaModDelimiter = { ',', ';' };

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
                if (userMod)
                    name += "*";
                OmssaModification mod = new OmssaModification(name, id, mono, average, sites);
                Modifications.Add(name, mod);
                _modificationKeyDicitonary[id] = name;
            }
        }

        public static void LoadOmssaModifications(string file,  bool userMod = true)
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
                ModificationSites sites = ModificationSites.None;
                foreach (
                    XmlNode node in
                        mod_node.SelectNodes("./omssa:MSModSpec_residues/omssa:MSModSpec_residues_E", mods_xml_ns))
                {
                    string aa = node.FirstChild.Value;
                    sites = sites.Set(aa[0]);
                }
                OmssaModification mod = new OmssaModification(name, id, mono, average, sites);
                Modifications[name] = mod;
                _modificationKeyDicitonary[id] = name;
            }
        }

        public static IEnumerable<OmssaModification> ParseModificationLine(string line)
        {
            foreach (string modname in line.Split(_omssaModDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(mod => mod.Split(':')[0]))
            {
                OmssaModification modification;
                if (TryGetModification(modname, out modification))
                {
                    yield return modification;
                }
                else
                {
                    throw new KeyNotFoundException("Modification: " + modname + " is not found in the modification dictionary");
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
            if(sb.Length > 0) {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }


        public static IEnumerable<OmssaModification> GetAllModifications()
        {
            return Modifications.Values;
        }
    }
}
