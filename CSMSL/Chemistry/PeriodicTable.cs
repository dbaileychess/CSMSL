///////////////////////////////////////////////////////////////////////////
//  PeriodicTable.cs - The periodic table of the elements                 /
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

using System;
using System.Collections.Generic;
using System.Xml;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// The Periodic Table of Elements.
    /// </summary>
    public sealed class PeriodicTable
    {
        /// <summary>
        /// The singleton instance of the periodic table
        /// </summary>
        private static readonly PeriodicTable _instance = new PeriodicTable();

        /// <summary>
        /// The internal dictionary housing all the elements, keyed by their unique atomic symbol
        /// </summary>
        private Dictionary<string, Element> _elements;

        static PeriodicTable() { }

        /// <summary>
        /// Creates a default periodictable with the supplied elements in "Resources/Elements.xml"
        /// </summary>
        private PeriodicTable()
        {
            _elements = new Dictionary<string, Element>();
            _isotopes = new Isotope[500];
            LoadElements("Resources/Elements.xml");
            Array.Resize(ref _isotopes, _uniqueID);
        }

        /// <summary>
        /// The single instance of the Periodic Table created by the program (singleton pattern)
        /// </summary>
        public static PeriodicTable Instance
        {
            get
            {
                return _instance;
            }
        }

        private int _uniqueID = 11;
        private Isotope[] _isotopes;

        public Element this[string element]
        {
            get
            {
                return _elements[element];
            }
        }

        internal Isotope this[int uniqueID]
        {
            get
            {
                return _isotopes[uniqueID];
            }
        }

        public bool TryGetElement(string elementSymbol, out Element element)
        {
            return _elements.TryGetValue(elementSymbol, out element);
        }

        /// <summary>
        /// Load a xml file containing elemental and isotopic data into the periodic table
        /// </summary>
        /// <param name="elementListXML">The xml file containing the data</param>
        public void LoadElements(string elementListXML)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(elementListXML);
            XmlNode periodictableNode = doc.SelectSingleNode("/PeriodicTable");
            foreach (XmlNode elementNode in periodictableNode.SelectNodes("Element"))
            {
                string name = elementNode.SelectSingleNode("Name").InnerText;
                string symbol = elementNode.SelectSingleNode("Symbol").InnerText;
                int atomicnumber = int.Parse(elementNode.SelectSingleNode("AtomicNumber").InnerText);
                Element element = new Element(name, symbol, atomicnumber);
                foreach (XmlNode isotopeNode in elementNode.SelectNodes("Isotope"))
                {
                    float abundance = float.Parse(isotopeNode.SelectSingleNode("RelativeAbundance").InnerText);

                    if (abundance > 0)
                    {
                        double mass = double.Parse(isotopeNode.SelectSingleNode("Mass").InnerText);
                        int massnumber = int.Parse(isotopeNode.SelectSingleNode("MassNumber").InnerText);
                        Isotope isotope = element.AddIsotope(massnumber, mass, abundance);
                        XmlNode idNode = isotopeNode.Attributes["uniqueID"];
                        if (idNode != null)
                        {
                            int uniqueId = int.Parse(idNode.Value);
                            isotope._uniqueID = uniqueId;
                            _isotopes[uniqueId] = isotope;
                        }
                        else
                        {
                            isotope._uniqueID = _uniqueID;
                            _isotopes[_uniqueID++] = isotope;
                        }
                    }
                }
                AddElement(element);
            }
        }

        /// <summary>
        /// Adds an element to this periodic table if the element atomic symbol is not already present.
        /// Overrides an element if the symbol is already present.
        /// </summary>
        /// <param name="element">The element to add to the periodic table</param>
        /// <returns>True if the element was not present before, false if the element existed and was overwritten</returns>
        public bool AddElement(Element element)
        {
            if (_elements.ContainsKey(element.AtomicSymbol))
            {
                _elements[element.AtomicSymbol] = element;
                return false;
            }
            else
            {
                _elements.Add(element.AtomicSymbol, element);
                return true;
            }
        }
    }
}