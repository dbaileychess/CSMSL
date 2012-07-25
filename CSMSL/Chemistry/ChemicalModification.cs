namespace CSMSL.Chemistry
{
    public class ChemicalModification : ChemicalFormula
    {
        private string _name;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    return base.ToString();
                return _name;
            }
        }

        public ChemicalModification(string chemicalFormula)
            : this(chemicalFormula, string.Empty) { }

        public ChemicalModification(string chemicalFormula, string name)
            : base(chemicalFormula)
        {
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}