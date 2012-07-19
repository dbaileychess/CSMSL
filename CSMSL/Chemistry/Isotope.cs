namespace CSMSL.Chemistry
{
    public class Isotope
    {
        private float _abundance;
        private Element _element;

        private int _massNumber;

        public int MassNumber
        {
            get { return _massNumber; }
            set { _massNumber = value; }
        }

        private double _mass;

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

        public float RelativeAbundance
        {
            get { return _abundance; }
            set { _abundance = value; }
        }
    }
}