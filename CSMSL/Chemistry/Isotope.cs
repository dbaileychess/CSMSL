namespace CSMSL.Chemistry
{
    public class Isotope
    {
        private float _abundance;
        private Element _element;

        private double _mass;
        private int _massNumber;

        public Isotope(Element parentElement, int massNumber, double mass, float abundance)
        {
            _element = parentElement;
            _massNumber = massNumber;
            _mass = mass;
            _abundance = abundance;
            _hashCode = base.GetHashCode();
        }

        public string AtomicSymbol
        {
            get { return _element.AtomicSymbol; }
        }

        public Element Element
        {
            get { return _element; }
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

        public bool IsPrincipalIsotope
        {
            get
            {
                return _element.Principal == this;
            }
        }

        private int _hashCode;
        public override int GetHashCode()
        {
            return _hashCode;           
        }

        public float RelativeAbundance
        {
            get { return _abundance; }
            private set { _abundance = value; }
        }
        
        public override string ToString()
        {
            return string.Format("{0}{1:G0}", AtomicSymbol, _massNumber);
        }
    }
}