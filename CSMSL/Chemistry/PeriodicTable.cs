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
using System.Linq;
using System.Reflection;
using System.Xml;

namespace CSMSL.Chemistry
{
    /// <summary>
    /// The Periodic Table of Elements.
    /// </summary>
    public static class PeriodicTable
    {
        public static readonly string UserPerodicTablePath;

        /// <summary>
        /// The internal dictionary housing all the elements, keyed by their unique atomic symbol
        /// </summary>
        private static readonly Dictionary<string, Element> _elements;

        /// <summary>
        /// The default size for chemical formula arrays. This is recommend based on the 5 most common elements for proteomics (C H O N P)
        /// </summary>
        internal const int RecommendedId = 5;

        static PeriodicTable()
        {
            UserPerodicTablePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"CSMSL\Elements.xml");

            _elements = new Dictionary<string, Element>();

            Load();
        }

        public static void Load()
        {
            // Create file if it doesn't exist
            if (!File.Exists(UserPerodicTablePath))
            {
                RestoreDefaults();
            }

            Load(UserPerodicTablePath);
        }

        public static void RestoreDefaults()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream defaultModsStream = assembly.GetManifestResourceStream("CSMSL.Resources.Elements.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(UserPerodicTablePath));
            using (var fileStream = File.Create(UserPerodicTablePath))
            {
                if (defaultModsStream != null) defaultModsStream.CopyTo(fileStream);
            }

            Load();
        }

        /// <summary>
        /// Load a xml file containing elemental and isotopic data into the periodic table
        /// </summary>
        public static void Load(string filePath)
        {
            _elements.Clear();
            Element element = null;
            _isotopes = new Isotope[500];
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement())
                        continue;

                    switch (reader.Name)
                    {
                        case "Element":
                            string name = reader["Name"];
                            string symbol = reader["Symbol"];
                            int atomicnumber = int.Parse(reader["AtomicNumber"]);
                            int valenceElectrons = int.Parse(reader["ValenceElectrons"]);
                            element = new Element(name, symbol, atomicnumber, valenceElectrons);
                            AddElement(element);
                            break;
                        case "Isotope":
                            string unqiueId = reader["Id"];
                            double mass = double.Parse(reader["Mass"]);
                            int massNumber = int.Parse(reader["MassNumber"]);
                            float abundance = float.Parse(reader["Abundance"]);
                            if (abundance > 0 && element != null)
                            {
                                Isotope isotope = element.AddIsotope(massNumber, mass, abundance);

                                if (unqiueId != null)
                                {
                                    int uniqueId = int.Parse(unqiueId);
                                    if (uniqueId > BiggestIsotopeNumber)
                                        BiggestIsotopeNumber = uniqueId;
                                    isotope.UniqueId = uniqueId;
                                    _isotopes[uniqueId] = isotope;
                                }
                                else
                                {
                                    isotope.UniqueId = BiggestIsotopeNumber;
                                    _isotopes[BiggestIsotopeNumber++] = isotope;
                                }
                            }
                            break;
                    }
                }
            }

            if (_isotopes.Length > BiggestIsotopeNumber)
                Array.Resize(ref _isotopes, BiggestIsotopeNumber + 1);
        }

        public static void Save()
        {
            SaveTo(UserPerodicTablePath);
        }

        public static void SaveTo(string filePath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings {Indent = true}))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("PeriodicTable");
                foreach (var element in _elements.Values.Where(element => element.Isotopes.Count > 0).GroupBy(element => element.Name).Select(g => g.First()))
                {
                    writer.WriteStartElement("Element");
                    writer.WriteAttributeString("Name", element.Name);
                    writer.WriteAttributeString("Symbol", element.AtomicSymbol);
                    writer.WriteAttributeString("AtomicNumber", element.AtomicNumber.ToString("N"));
                    writer.WriteAttributeString("AverageMass", element.AverageMass.ToString("R"));
                    writer.WriteAttributeString("ValenceElectrons", element.ValenceElectrons.ToString("N"));
                    foreach (var isotope in element.Isotopes.Values)
                    {
                        writer.WriteStartElement("Isotope");
                        writer.WriteAttributeString("Id", isotope.UniqueId.ToString("N"));
                        writer.WriteAttributeString("Mass", isotope.AtomicMass.ToString("R"));
                        writer.WriteAttributeString("MassNumber", isotope.MassNumber.ToString("N"));
                        writer.WriteAttributeString("Abundance", isotope.RelativeAbundance.ToString("R"));
                        writer.WriteEndElement(); // Isotope
                    }
                    writer.WriteEndElement(); // Element
                }
                writer.WriteEndElement(); // PeriodicTable
            }
        }

        public static int BiggestIsotopeNumber { get; private set; }

        /// <summary>
        /// The main data store for all the isotopes in this periodic table. The isotope unique ID serves as the index in the array, these IDs are unique for each isotope.
        /// </summary>
        private static Isotope[] _isotopes;

        public static Element GetElement(string element)
        {
            return _elements[element];
        }

        public static Isotope GetIsotope(string element, int atomicNumber)
        {
            return _elements[element][atomicNumber];
        }

        /// <summary>
        /// Get an isotope based on its unique isotope id
        /// </summary>
        /// <param name="uniqueId">The unique isotope id of isotope to get</param>
        /// <returns>An isotope</returns>
        internal static Isotope GetIsotope(int uniqueId)
        {
            return _isotopes[uniqueId];
        }

        public static bool TryGetElement(string elementSymbol, out Element element)
        {
            return _elements.TryGetValue(elementSymbol, out element);
        }


        /// <summary>
        /// Adds an element to this periodic table if the element atomic symbol is not already present.
        /// Overrides an element if the symbol is already present.
        /// </summary>
        /// <param name="element">The element to add to the periodic table</param>
        /// <returns>True if the element was not present before, false if the element existed and was overwritten</returns>
        public static bool AddElement(Element element)
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