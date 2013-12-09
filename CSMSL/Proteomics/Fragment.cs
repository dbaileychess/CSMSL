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

using System;
using CSMSL.Chemistry;
using System.Collections.Generic;
using CSMSL.Spectral;

namespace CSMSL.Proteomics
{
    public class Fragment : IMass, IEquatable<Fragment>
    {
        private static readonly Dictionary<FragmentTypes, IMass> FragmentIonCaps = new Dictionary<FragmentTypes, IMass>
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

        public Fragment(FragmentTypes type, int number, double monoisotopicMass, AminoAcidPolymer parent, IEnumerable<IMass> mods = null)
        {
            Type = type;
            Number = number;
            Parent = parent;
            MonoisotopicMass = monoisotopicMass + FragmentIonCaps[type].MonoisotopicMass;
            if(mods != null) 
                Modifications = new List<IMass>(mods);
        }

        public List<IMass> Modifications;
     
        public double MonoisotopicMass { get; private set; }

        public int Number { get; private set; }

        public AminoAcidPolymer Parent { get; private set; }

        public FragmentTypes Type { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}{1}", Enum.GetName(typeof(FragmentTypes), Type), Number);
        }

        public override int GetHashCode()
        {
            return Number + Type.GetHashCode() + Math.Round(MonoisotopicMass).GetHashCode();
        }

        public bool Equals(Fragment other)
        {
            return Type.Equals(other.Type) && Number == other.Number &&
                   Math.Abs(MonoisotopicMass - other.MonoisotopicMass) < 0.00001;
        }
    }
}