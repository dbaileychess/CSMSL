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
        private static readonly PeriodicTable instance = new PeriodicTable();

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
            LoadElements("Resources/Elements.xml");
        }

        /// <summary>
        /// The single instance of the Periodic Table created by the program (singleton pattern) 
        /// </summary>
        public static PeriodicTable Instance
        {
            get
            {
                return instance;
            }
        }

        public Element this[string element]
        {
            get
            {
                return _elements[element];
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
                    double mass = double.Parse(isotopeNode.SelectSingleNode("Mass").InnerText);
                    int massnumber = int.Parse(isotopeNode.SelectSingleNode("MassNumber").InnerText);
                    float abundance = float.Parse(isotopeNode.SelectSingleNode("RelativeAbundance").InnerText);
                    element.AddIsotope(massnumber, mass, abundance);
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
            if(_elements.ContainsKey(element.AtomicSymbol)) {
                _elements[element.AtomicSymbol] = element;
                return false;
            } else {
                _elements.Add(element.AtomicSymbol, element);
                return true;
            }
        }
    }
}