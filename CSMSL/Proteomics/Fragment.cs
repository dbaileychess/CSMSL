// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Fragment.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using CSMSL.Chemistry;
using System;
using System.Collections.Generic;

namespace CSMSL.Proteomics
{
    public class Fragment : IMass, IEquatable<Fragment>
    {
        public Fragment(FragmentTypes type, int number, double monoisotopicMass, AminoAcidPolymer parent)
        {
            Type = type;
            Number = number;
            Parent = parent;
            MonoisotopicMass = monoisotopicMass;
        }

        public double MonoisotopicMass { get; private set; }

        public int Number { get; private set; }

        public AminoAcidPolymer Parent { get; private set; }

        public FragmentTypes Type { get; private set; }

        // TODO figure if this is the best way to do chemical formula fragments
        //public bool TryGetFormula(out ChemicalFormula formula)
        //{
        // Might add this
        //}

        public IEnumerable<IMass> GetModifications()
        {
            if (Parent == null)
                yield break;

            var mods = Parent.Modifications;
            if (Type.GetTerminus() == Terminus.N)
            {
                for (int i = 0; i <= Number; i++)
                {
                    if (mods[i] != null)
                        yield return mods[i];
                }
            }
            else
            {
                int length = Parent.Length + 1;
                for (int i = length - Number; i <= length; i++)
                {
                    if (mods[i] != null)
                        yield return mods[i];
                }
            }
        }

        public string GetSequence()
        {
            if (Parent == null)
                return "";

            string parentSeq = Parent.Sequence;
            if (Type.GetTerminus() == Terminus.N)
            {
                return parentSeq.Substring(0, Number);
            }

            return parentSeq.Substring(parentSeq.Length - Number, Number);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", Enum.GetName(typeof (FragmentTypes), Type), Number);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hCode = 23;
                hCode = hCode*31 + Number;
                hCode = hCode*31 + (int) Type;
                hCode = hCode*31 + Math.Round(MonoisotopicMass).GetHashCode();
                return hCode;
            }
        }

        public bool Equals(Fragment other)
        {
            return Type.Equals(other.Type) && Number.Equals(other.Number) && MonoisotopicMass.MassEquals(other.MonoisotopicMass);
        }
    }
}