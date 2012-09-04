///////////////////////////////////////////////////////////////////////////
//  ProteaseDictionary.cs - A collection of common proteases              /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Xml;

namespace CSMSL.Proteomics
{
    public sealed class ProteaseDictionary : IEnumerable<Protease>
    {
        private static readonly ProteaseDictionary _instance = new ProteaseDictionary();

        private Dictionary<string, Protease> _proteases;

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
        /// The single instance of the Protease Dictionary created by the program (singleton pattern)
        /// </summary>
        public static ProteaseDictionary Instance
        {
            get
            {
                return _instance;
            }
        }

        public Protease this[string name]
        {
            get
            {
                return _proteases[name];
            }
        }

        /// <summary>
        /// Adds a protease to the singleton dictionary of proteases
        /// </summary>
        /// <param name="protease">The protease to add</param>
        public void AddProtease(Protease protease)
        {
            _proteases.Add(protease.Name, protease);
        }

        /// <summary>
        /// Load a xml file containing proteases into this dictionary
        /// </summary>
        /// <param name="proteaseListXML">The xml file containing the proteases</param>
        public void LoadProteases(string proteaseListXML)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(proteaseListXML);
            XmlNode proteasesNode = doc.SelectSingleNode("Proteases");
            foreach (XmlNode proteaseNode in proteasesNode.SelectNodes("Protease"))
            {
                string name = proteaseNode.SelectSingleNode("Name").InnerText;
                char _char = char.Parse(proteaseNode.SelectSingleNode("Position").InnerText);
                string regex = proteaseNode.SelectSingleNode("Cleavage").InnerText;
                Protease protease = new Protease(name, (_char == 'C') ? Terminus.C : Terminus.N, regex);
                AddProtease(protease);
            }
        }

        public IEnumerator<Protease> GetEnumerator()
        {
            return _proteases.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _proteases.Values.GetEnumerator();
        }
    }
}