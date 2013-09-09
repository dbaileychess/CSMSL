using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace CSMSL.IO.OMSSA
{
    public class OmssaModification : Modification
    {
        private static readonly Dictionary<int, string> _modificationKeyDicitonary;
        private static readonly Dictionary<string, OmssaModification> _modifications; 

        static OmssaModification() 
        { 
            _modificationKeyDicitonary = new Dictionary<int, string>();
            _modifications = new Dictionary<string, OmssaModification>();

            // Load in the default omssa mods
            LoadOmssaModifications("Resources/mods.xml", false);         
        }
        
        public int ID { get; set; }

        public OmssaModification(string name, int id, double mono, double average)
            : base(mono, name)
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
            return _modificationKeyDicitonary.TryGetValue(id, out name) && _modifications.TryGetValue(name, out modification);
        }

        public static bool TryGetModification(string name, out OmssaModification modification)
        {
            return _modifications.TryGetValue(name, out modification);
        }

        private static readonly char[] _omssaModDelimiter = { ',', ';' };

        public static void LoadOmssaModifications(string file, bool userMod = true)
        {
            XmlDocument mods_xml = new XmlDocument();
            mods_xml.Load(file);
            XmlNamespaceManager mods_xml_ns = new XmlNamespaceManager(mods_xml.NameTable);
            mods_xml_ns.AddNamespace("omssa", mods_xml.ChildNodes[1].Attributes["xmlns"].Value);
            Dictionary<string, IMass> mods = new Dictionary<string, IMass>();
            foreach (XmlNode mod_node in mods_xml.SelectNodes("/omssa:MSModSpecSet/omssa:MSModSpec", mods_xml_ns))
            {
                string name = mod_node.SelectSingleNode("./omssa:MSModSpec_name", mods_xml_ns).FirstChild.Value;
                int id = int.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_mod/omssa:MSMod", mods_xml_ns).FirstChild.Value);
                double mono = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_monomass", mods_xml_ns).FirstChild.Value);
                double average = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_averagemass", mods_xml_ns).FirstChild.Value);
                if (userMod)
                    name += "*";
                OmssaModification mod = new OmssaModification(name, id, mono, average);
                _modifications.Add(name, mod);
                _modificationKeyDicitonary[id] = name;
            }
        }

        public static IEnumerable<OmssaModification> GetAllOmssaModifications()
        {
            return _modifications.Values;
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
    }
}
