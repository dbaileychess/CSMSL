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
            set { _mass = value; }
        }

        public int MassNumber
        {
            get { return _massNumber; }
            set { _massNumber = value; }
        }

        public bool IsPrincipalIsotope
        {
            get
            {
                return _element.Principal == this;
            }
        }

        public float RelativeAbundance
        {
            get { return _abundance; }
            set { _abundance = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}{1:G0}", AtomicSymbol, _massNumber);
        }
    }
}