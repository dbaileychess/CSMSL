///////////////////////////////////////////////////////////////////////////
//  Fragment.cs - A part of a larger amino acid polymer                   /
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