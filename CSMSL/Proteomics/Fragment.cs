// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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