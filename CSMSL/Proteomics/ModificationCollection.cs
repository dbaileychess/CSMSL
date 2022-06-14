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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Proteomics
{
    public class ModificationCollection : ICollection<IMass>, IMass, IEquatable<ModificationCollection>
    {
        private readonly List<IMass> _modifications;

        public double MonoisotopicMass { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IMass mod in _modifications)
            {
                sb.Append(mod);
                sb.Append(" | ");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 3, 3);
            }
            return sb.ToString();
        }

        public ModificationCollection(params IMass[] mods)
        {
            _modifications = mods.ToList();
            MonoisotopicMass = _modifications.Sum(m => m.MonoisotopicMass);
        }

        public void Add(IMass item)
        {
            _modifications.Add(item);
            MonoisotopicMass += item.MonoisotopicMass;
        }

        public void Clear()
        {
            _modifications.Clear();
            MonoisotopicMass = 0;
        }

        public bool Contains(IMass item)
        {
            return _modifications.Contains(item);
        }

        public void CopyTo(IMass[] array, int arrayIndex)
        {
            _modifications.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _modifications.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IMass item)
        {
            if (!_modifications.Remove(item))
                return false;
            MonoisotopicMass -= item.MonoisotopicMass;
            return true;
        }

        public IEnumerator<IMass> GetEnumerator()
        {
            return _modifications.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _modifications.GetEnumerator();
        }

        public override int GetHashCode()
        {
            int hCode = _modifications.GetHashCode();

            return Count + hCode;
        }

        public override bool Equals(object obj)
        {
            ModificationCollection col = obj as ModificationCollection;
            return col != null && Equals(col);
        }

        public bool Equals(ModificationCollection other)
        {
            return Count == other.Count && _modifications.ScrambledEquals(other._modifications);
        }
    }
}