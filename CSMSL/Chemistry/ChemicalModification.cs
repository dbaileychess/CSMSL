namespace CSMSL.Chemistry
{
    public class ChemicalModification : ChemicalFormula
    {
        private string _name = string.Empty;
        
        public ChemicalModification(string chemicalFormula)
            : base(chemicalFormula)
        {
            
        }

        public ChemicalModification(string chemicalFormula, string name)
            : base(chemicalFormula)
        {
            _name = name;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_name)) 
                return base.ToString();
            return _name;
        }
    }
}