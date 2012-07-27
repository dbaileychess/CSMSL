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

        private Isotope _principal = null;

        private string _name;

        public Isotope this[int atomicNumber]
        {
            get
            {
                return _isotopes[atomicNumber];
            }
        }

        public Element(string name, string symbol, int atomicNumber)
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

        public Isotope Principal
        {
            get
            {
                return _principal;
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

        public override string ToString()
        {
            return _atomicSymbol;
        }

        private double _totalAbundance = 0;
        private double _totalMass = 0;

        public void AddIsotope(int atomicNumber, double mass, float abundance)
        {
            Isotope isotope = new Isotope(this, atomicNumber, mass, abundance);
            if (!_isotopes.ContainsKey(atomicNumber))
            {
                _isotopes.Add(atomicNumber, isotope);
                _totalAbundance += abundance;
                _totalMass += abundance * mass;
                _avgmass = _totalMass / _totalAbundance;
                if (_principal == null || abundance > _principal.RelativeAbundance)
                {
                    _principal = isotope;
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