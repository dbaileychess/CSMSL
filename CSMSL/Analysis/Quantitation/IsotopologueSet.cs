using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class IsotopologueSet: IChemicalFormula, IMass, IEnumerable<Isotopologue>
    {
        public string Name { get; set; }

        private SortedList<double,Isotopologue> _isotopologues;

        public ChemicalFormula ChemicalFormula {
            get
            {
                if (_isotopologues.Count < 1)
                    return null;
                return _isotopologues.Values[0];
            }        
        }
        
        private ChemicalFormula _totalFormula;
       
        public Mass Mass
        {
            get { return _totalFormula.Mass / Count; }
        }

        public Isotopologue this[int index]
        {
            get
            {
                return _isotopologues.Values[index];
            }
        }

        public IsotopologueSet(string name)
        {
            Name = name;
            _isotopologues = new SortedList<double, Isotopologue>();          
            _totalFormula = new ChemicalFormula();
        }

        public void AddIsotopologue(Isotopologue isotopologue)
        {
            _isotopologues.Add(isotopologue.Mass.Monoisotopic, isotopologue);            
            _totalFormula.Add(isotopologue);
        }

        public Isotopologue LightestIsotopologue
        {
            get
            {
                if (_isotopologues.Count < 1)
                    return null;
                return _isotopologues.Values[0];
            }
        }

        public Isotopologue HeaviestIsotopologue
        {
            get
            {
                if (_isotopologues.Count < 1)
                    return null;
                return _isotopologues.Values[_isotopologues.Count - 1];
            }
        }

        public MassRange MassRange
        {
            get
            {
                return new MassRange(LightestIsotopologue.Mass.Monoisotopic, HeaviestIsotopologue.Mass.Monoisotopic);
            }
        }

        public int Count
        {
            get
            {
                return _isotopologues.Count;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerator<Isotopologue> GetEnumerator()
        {
            return _isotopologues.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _isotopologues.Values.GetEnumerator();
        }
    }
}
