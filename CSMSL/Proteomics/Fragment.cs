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
using System.Collections.Generic;

namespace CSMSL.Proteomics
{
    public class Fragment : IChemicalFormula, IMass
    {

        private static readonly Dictionary<FragmentTypes, ChemicalFormula> _fragmentIonCaps = new Dictionary<FragmentTypes, ChemicalFormula>()
        {
          {FragmentTypes.a, new ChemicalFormula("C-1H-1O-1")},
          {FragmentTypes.adot, new ChemicalFormula("C-1O-1")},
          {FragmentTypes.b, new ChemicalFormula("H-1")},
          {FragmentTypes.bdot, new ChemicalFormula()},
          {FragmentTypes.c, new ChemicalFormula("NH2")},
          {FragmentTypes.cdot, new ChemicalFormula("NH3")},
          {FragmentTypes.x, new ChemicalFormula("COH-1")},
          {FragmentTypes.xdot, new ChemicalFormula("CO")},
          {FragmentTypes.y, new ChemicalFormula("H")},
          {FragmentTypes.ydot, new ChemicalFormula("H2")},
          {FragmentTypes.z, new ChemicalFormula("N-1H-2")},
          {FragmentTypes.zdot, new ChemicalFormula("N-1H-1")},
        };
        

        private ChemicalFormula _chemicalFormula;
        private int _number;

        private AminoAcidPolymer _parent;

        private FragmentTypes _type;

        public Fragment(FragmentTypes type, int number, ChemicalFormula chemicalFormula, AminoAcidPolymer parent)
        {
            _type = type;
            _number = number;
            _chemicalFormula = chemicalFormula;
            _chemicalFormula.Add(_fragmentIonCaps[type]);
            _parent = parent;
        }

        public ChemicalFormula ChemicalFormula
        {
            get { return _chemicalFormula; }
            private set { _chemicalFormula = value; }
        }

        public Mass Mass
        {
            get { return _chemicalFormula.Mass; }
        }

        public int Number
        {
            get { return _number; }
            private set { _number = value; }
        }

        public AminoAcidPolymer Parent
        {
            get { return _parent; }
            private set { _parent = value; }
        }

        public FragmentTypes Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", System.Enum.GetName(typeof(FragmentTypes), _type), _number);
        }
                
    }
}