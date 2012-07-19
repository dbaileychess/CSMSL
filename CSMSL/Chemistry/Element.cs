using System.Collections.Generic;

namespace CSMSL.Chemistry
{
    public class Element : IEnumerable<Isotope>
    {
        private int _atomicNumber;
        private string _atomicSymbol;

        private double _avgmass = 0;

        private Dictionary<int, Isotope> _isotopes;

        private double _mass = 0;

        private Isotope _mostAbundant = null;

        private string _name;

        internal Element(string name, string symbol, int atomicNumber)
        {
            _name = name;
            _atomicSymbol = symbol;
            _atomicNumber = atomicNumber;
            _isotopes = new Dictionary<int, Isotope>();
        }

        public int AtomicNumber
        {
            get { return _atomicNumber; }
            set { _atomicNumber = value; }
        }

        public string AtomicSymbol
        {
            get { return _atomicSymbol; }
            set { _atomicSymbol = value; }
        }
        public double AverageMass
        {
            get
            {
                return _avgmass;
            }
        }

        public double Mass
        {
            get
            {
                return _mass;
            }
        }

        public Isotope MostAbundant
        {
            get
            {
                return _mostAbundant;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public IEnumerator<Isotope> GetEnumerator()
        {
            return _isotopes.Values.GetEnumerator();
        }

        internal void AddIsotope(int atomicNumber, double mass, float abundance)
        {
            Isotope isotope = new Isotope(this, atomicNumber, mass, abundance);
            if (!_isotopes.ContainsKey(atomicNumber))
            {
                _isotopes.Add(atomicNumber, isotope);

                if (_mostAbundant == null || abundance > _mostAbundant.RelativeAbundance)
                {
                    _mostAbundant = isotope;
                    _mass = mass;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _isotopes.Values.GetEnumerator();
        }
    }
}