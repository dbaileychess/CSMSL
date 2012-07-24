using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Fragment : IChemicalFormula, IMass
    {
        private int _number;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        private FragmentType _type;

        public FragmentType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private AminoAcidPolymer _parent;

        public AminoAcidPolymer Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        private ChemicalFormula _chemicalFormula;

        public ChemicalFormula ChemicalFormula
        {
            get { return _chemicalFormula; }
            set { _chemicalFormula = value; }
        }

        public Mass Mass
        {
            get { return _chemicalFormula.Mass; }
        }

        public Fragment(FragmentType type, int number, ChemicalFormula chemicalFormula, AminoAcidPolymer parent)
        {
            _type = type;
            _number = number;
            _chemicalFormula = chemicalFormula;
            _parent = parent;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", System.Enum.GetName(typeof(FragmentType), _type), _number);
        }
    }
}