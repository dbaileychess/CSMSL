using System;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class AminoAcidResidue : IEquatable<AminoAcidResidue>, IChemicalFormula, IMass
    {
        private ChemicalFormula _chemicalFormula;
        private char _letter;
        private string _name;

        private string _symbol;

        public AminoAcidResidue(string name, char oneLetterAbbreviation, string threeLetterAbbreviation, ChemicalFormula chemicalFormula)
        {
            _name = name;
            _letter = oneLetterAbbreviation;
            _symbol = threeLetterAbbreviation;
            _chemicalFormula = chemicalFormula;
        }

        public AminoAcidResidue(AminoAcidResidue aar)
            : this(aar._name, aar._letter, aar._symbol, new ChemicalFormula(aar._chemicalFormula)) { }

        public ChemicalFormula ChemicalFormula
        {
            get { return _chemicalFormula; }
            private set { _chemicalFormula = value; }
        }

        public char Letter
        {
            get { return _letter; }
            set { _letter = value; }
        }

        public Mass Mass
        {
            get { return _chemicalFormula.Mass; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public AminoAcidResidue Clone()
        {
            return new AminoAcidResidue(this);
        }

        public bool Equals(AminoAcidResidue other)
        {
            return _chemicalFormula.Equals(other._chemicalFormula);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}