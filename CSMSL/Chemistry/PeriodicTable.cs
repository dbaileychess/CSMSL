using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Chemistry
{
    public sealed class PeriodicTable
    {
        private static readonly PeriodicTable instance = new PeriodicTable();

        public static PeriodicTable Instance
        {
            get
            {
                return instance;
            }
        }

        private Dictionary<string, Element> _elements;

        public Element this[string element]
        {
            get
            {
                return _elements[element];
            }
        }

        static PeriodicTable() { }

        PeriodicTable()
        {                     
            CreateElements();   
        }

        private void CreateElements()
        {
            _elements = new Dictionary<string, Element>();
                            
            Element carbon = new Element("Carbon", "C", 6);
            carbon.AddIsotope(12, 12.0, 98.93F);
            carbon.AddIsotope(13, 13.0033548378, 1.07F);
            carbon.AddIsotope(14, 14.003241988, 0F);
            AddElement(carbon);               

        }

        private void AddElement(Element element)
        {
            _elements.Add(element.AtomicSymbol, element);
        }

    }
}
