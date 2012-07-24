using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CSMSL.Proteomics
{
    public sealed class ProteaseDictionary
    {
        private static readonly ProteaseDictionary _instance = new ProteaseDictionary();

        /// <summary>
        /// The single instance of the Protease Dictionary created by the program (singleton pattern) 
        /// </summary>
        public static ProteaseDictionary Instance
        {
            get
            {
                return _instance;
            }
        }

        private Dictionary<string, Protease> _proteases;

        public Protease this[string name]
        {
            get
            {
                return _proteases[name];
            }
        }

       static ProteaseDictionary() { }

        /// <summary>
        /// Creates a default periodictable with the supplied elements in "Resources/Elements.xml"
        /// </summary>
       private ProteaseDictionary()
        {
            _proteases = new Dictionary<string, Protease>(20);    
            LoadProteases("Resources/Proteases.xml");
        }

       /// <summary>
       /// Load a xml file containing amino acid residues into this dictionary
       /// </summary>
       /// <param name="residueListXML">The xml file containing the data</param>
       public void LoadProteases(string proteaseListXML)
       {
           XmlDocument doc = new XmlDocument();
           doc.Load(proteaseListXML);
           XmlNode periodictableNode = doc.SelectSingleNode("/Proteases");
           foreach (XmlNode elementNode in periodictableNode.SelectNodes("Protease"))
           {
               string name = elementNode.SelectSingleNode("Name").InnerText;
               char _char = char.Parse(elementNode.SelectSingleNode("Position").InnerText);
               Protease protease = new Protease(name, _char == 'C');
               foreach (XmlNode cleavageRuleNode in elementNode.SelectNodes("CleavageRule"))
               {
                   List<KeyValuePair<char, int>> exceptions = new List<KeyValuePair<char, int>>();
                   foreach (XmlNode residue in cleavageRuleNode.SelectNodes("Residue"))
                   {
                       _char = char.Parse(residue.InnerText);
                       int _position = int.Parse(residue.Attributes.GetNamedItem("position").Value);
                       bool prevent = bool.Parse(residue.Attributes.GetNamedItem("prevent").Value);
                       if (prevent)
                       {
                           exceptions.Add(new KeyValuePair<char, int>(_char, _position));
                       }
                     //  protease.CleaveAt(
                       //protease.CleaveAt.Add(_char);
                   }


               }         
               AddProtease(protease);
           }
       }

       public void AddProtease(Protease protease)
       {
           _proteases.Add(protease.Name, protease);     
       }
    }
}
