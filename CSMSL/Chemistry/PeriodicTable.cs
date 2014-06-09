// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (PeriodicTable.cs) is part of CSMSL.
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
        private readonly Dictionary<string, Element> _elements;

        /// <summary>
        /// The default size for chemical formula arrays. This is recommend based on the 5 most common elements for proteomics (C H O N P)
        /// </summary>
        internal const int RecommendedId = 5;

        static PeriodicTable()
        {
        }

        /// <summary>
        /// Creates a default periodictable with the supplied elements in "Resources/Elements.xml"
        /// </summary>
        private PeriodicTable()
        {
            _elements = new Dictionary<string, Element>();

            // from: http://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
            var assembly = Assembly.GetExecutingAssembly();
            LoadElements(assembly.GetManifestResourceStream("CSMSL.Resources.Elements.xml"));

            // For reading from an content file
            //LoadElements("Resources/Elements.xml");
        }

        /// <summary>
        /// The single instance of the Periodic Table created by the program (singleton pattern)
        /// </summary>
        public static PeriodicTable Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// The number of unique isotopes in this periodic table
        /// </summary>
        private int _uniqueId;

        public int BiggestIsotopeNumber
        {
            get { return _uniqueId; }
        }

        /// <summary>
        /// The main data store for all the isotopes in this periodic table. The isotope unique ID serves as the index in the array, these IDs are unique for each isotope.
        /// </summary>
        private Isotope[] _isotopes;

        public Element this[string element]
        {
            get { return _elements[element]; }
        }

        internal Isotope this[int uniqueId]
        {
            get { return _isotopes[uniqueId]; }
        }

        public bool TryGetElement(string elementSymbol, out Element element)
        {
            return _elements.TryGetValue(elementSymbol, out element);
        }

        /// <summary>
        /// Load a xml file containing elemental and isotopic data into the periodic table
        /// </summary>
        public void LoadElements(Stream elementsListXml)
        {
            using (XmlReader reader = XmlReader.Create(elementsListXml))
            {
                reader.ReadToFollowing("PeriodicTable");
                var idstr = reader.GetAttribute("defaultID");
                if (idstr == null)
                    throw new Exception();
                _uniqueId = int.Parse(idstr);
                var isoStr = reader.GetAttribute("isotopesCount");
                if (isoStr == null)
                    throw new Exception();
                int isotopes = int.Parse(isoStr);
                _isotopes = new Isotope[isotopes];
                while (reader.ReadToFollowing("Element"))
                {
                    reader.ReadToFollowing("Name");
                    string name = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("Symbol");
                    string symbol = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("AtomicNumber");
                    int atomicnumber = reader.ReadElementContentAsInt();
                    reader.ReadToFollowing("ValenceElectrons");
                    int valenceElectrons = reader.ReadElementContentAsInt();
                    Element element = new Element(name, symbol, atomicnumber, valenceElectrons);

                    bool isStartNode = reader.ReadToNextSibling("Isotope");
                    while (isStartNode)
                    {
                        string unqiueId = reader.GetAttribute("uniqueID");
                        reader.ReadToFollowing("Mass");
                        double mass = reader.ReadElementContentAsDouble();
                        reader.ReadToFollowing("MassNumber");
                        int massNumber = reader.ReadElementContentAsInt();
                        reader.ReadToFollowing("RelativeAbundance");
                        float abundance = reader.ReadElementContentAsFloat();
                        if (abundance > 0)
                        {
                            Isotope isotope = element.AddIsotope(massNumber, mass, abundance);

                            if (unqiueId != null)
                            {
                                int uniqueId = int.Parse(unqiueId);
                                isotope.UniqueId = uniqueId;
                                _isotopes[uniqueId] = isotope;
                            }
                            else
                            {
                                isotope.UniqueId = _uniqueId;
                                _isotopes[_uniqueId++] = isotope;
                            }
                        }
                        if (!reader.IsStartElement("Isotope"))
                            isStartNode = reader.ReadToNextSibling("Isotope");
                    }


                    AddElement(element);
                }
            }

            if (_isotopes.Length != _uniqueId)
                Array.Resize(ref _isotopes, _uniqueId);
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

            _elements.Add(element.AtomicSymbol, element);

            // Special case for Deuterium
            if (element.AtomicSymbol == "H")
            {
                _elements.Add("D", element);
            }
            return true;
        }
    }
}