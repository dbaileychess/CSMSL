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
using System.IO;

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

        private const int DefaultID = 11;
        internal const int RecommendedID = 5;

        static PeriodicTable() { }

        /// <summary>
        /// Creates a default periodictable with the supplied elements in "Resources/Elements.xml"
        /// </summary>
        private PeriodicTable()
        {
            _elements = new Dictionary<string, Element>();
            _uniqueID = DefaultID;
            _isotopes = new Isotope[300];
            // from: http://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            LoadElements(assembly.GetManifestResourceStream("CSMSL.Resources.Elements.xml"));
            //LoadElements("Resources/Elements.xml");
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
   
        private int _uniqueID;

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
        public void LoadElements(Stream elementsListXML)
        {
            using (XmlReader reader = XmlReader.Create(elementsListXML))            
            {
                while (reader.ReadToFollowing("Element"))
                {                  
                    reader.ReadToFollowing("Name");   
                    string name = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("Symbol");
                    string symbol = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("AtomicNumber");
                    int atomicnumber = reader.ReadElementContentAsInt();
                    Element element = new Element(name, symbol, atomicnumber);
                 
                    bool isStartNode = reader.ReadToFollowing("Isotope");
                    while(isStartNode)
                    {                        
                        string unqiueID = reader.GetAttribute("uniqueID");     
                        reader.ReadToFollowing("Mass");
                        double mass = reader.ReadElementContentAsDouble();
                        reader.ReadToFollowing("MassNumber");
                        int massNumber = reader.ReadElementContentAsInt();
                        reader.ReadToFollowing("RelativeAbundance");
                        float abundance = reader.ReadElementContentAsFloat();
                        if (abundance > 0)
                        {
                            Isotope isotope = element.AddIsotope(massNumber, mass, abundance);
                            if (unqiueID != null)
                            {
                                int uniqueId = int.Parse(unqiueID);
                                isotope.UniqueID = uniqueId;
                                _isotopes[uniqueId] = isotope;
                            }
                            else
                            {
                                isotope.UniqueID = _uniqueID;
                                _isotopes[_uniqueID++] = isotope;
                            }
                        }
                        if (!reader.IsStartElement("Isotope"))
                            isStartNode = reader.ReadToNextSibling("Isotope");
                    } 
                    AddElement(element);                 
                }
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