using System;

namespace CSMSL.Chemistry
{
    public class Isotope : IEquatable<Isotope>
    {
        internal bool _isPrincipalIsotope;
        internal int _uniqueID;
        private float _abundance;
        private Element _element;

        private int _hashCode;
        private double _mass;
        private int _massNumber;

        internal Isotope(Element parentElement, int massNumber, double mass, float abundance)
        {
            _element = parentElement;
            _massNumber = massNumber;
            _mass = mass;
            _abundance = abundance;
            _hashCode = 13 + (_massNumber << 5) + (_element.AtomicNumber >> 2 + 12);
        }

        public string AtomicSymbol
        {
            get { return _element.AtomicSymbol; }
        }

        public Element Element
        {
            get { return _element; }
        }

        public bool IsPrincipalIsotope
        {
            get
            {
                return _isPrincipalIsotope;
            }
            set
            {
                _isPrincipalIsotope = value;
            }
        }

        public double Mass
        {
            get { return _mass; }
            private set { _mass = value; }
        }

        public int MassNumber
        {
            get { return _massNumber; }
            private set { _massNumber = value; }
        }

        public float RelativeAbundance
        {
            get { return _abundance; }
            private set { _abundance = value; }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(Isotope other)
        {
            return _massNumber.Equals(other._massNumber);
        }

        public override string ToString()
        {
            return string.Format("{0}{1:G0}", AtomicSymbol, _massNumber);
        }
    }
}