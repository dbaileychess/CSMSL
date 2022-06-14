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

using System;
using System.Collections.Generic;
using System.Globalization;
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
            BiggestIsotopeNumber = -1;
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
                            string a = reader["Mass"];
                            double mass = double.Parse(reader["Mass"], CultureInfo.CurrentCulture);
                            int massNumber = int.Parse(reader["MassNumber"]);
                            float abundance = float.Parse(reader["Abundance"], CultureInfo.CurrentCulture);
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
                    writer.WriteAttributeString("AtomicNumber", element.AtomicNumber.ToString("N", CultureInfo.CurrentCulture));
                    writer.WriteAttributeString("AverageMass", element.AverageMass.ToString("R", CultureInfo.CurrentCulture));
                    writer.WriteAttributeString("ValenceElectrons", element.ValenceElectrons.ToString("N", CultureInfo.CurrentCulture));
                    foreach (var isotope in element.Isotopes.Values)
                    {
                        writer.WriteStartElement("Isotope");
                        writer.WriteAttributeString("Id", isotope.UniqueId.ToString("N", CultureInfo.CurrentCulture));
                        writer.WriteAttributeString("Mass", isotope.AtomicMass.ToString("R", CultureInfo.CurrentCulture));
                        writer.WriteAttributeString("MassNumber", isotope.MassNumber.ToString("N", CultureInfo.CurrentCulture));
                        writer.WriteAttributeString("Abundance", isotope.RelativeAbundance.ToString("R", CultureInfo.CurrentCulture));
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