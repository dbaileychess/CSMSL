///////////////////////////////////////////////////////////////////////////
//  AminoAcidResidue.cs -  An amino acid residue                          /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

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