using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Fragment : IChemicalFormula, IMass
    {
        private ChemicalFormula _chemicalFormula;
        private int _number;

        private AminoAcidPolymer _parent;

        private FragmentType _type;

        public Fragment(FragmentType type, int number, ChemicalFormula chemicalFormula, AminoAcidPolymer parent)
        {
            _type = type;
            _number = number;
            _chemicalFormula = chemicalFormula;
            _parent = parent;
        }

        public ChemicalFormula ChemicalFormula
        {
            get { return _chemicalFormula; }
            set { _chemicalFormula = value; }
        }

        public Mass Mass
        {
            get { return _chemicalFormula.Mass; }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public AminoAcidPolymer Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public FragmentType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", System.Enum.GetName(typeof(FragmentType), _type), _number);
        }
    }
}