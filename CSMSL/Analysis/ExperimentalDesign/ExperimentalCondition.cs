// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ExperimentalCondition.cs) is part of CSMSL.
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

using CSMSL.Proteomics;
using System.Collections;
using System.Collections.Generic;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalCondition : IEnumerable<Modification>
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public int Count
        {
            get { return Modifications.Count; }
        }

        public HashSet<Modification> Modifications { get; set; }

        public ExperimentalCondition(string name, string description = "")
        {
            Name = name;
            Description = description;
            Modifications = new HashSet<Modification>();
        }

        public void AddModification(Modification mod)
        {
            Modifications.Add(mod);
        }

        public void AddModifications(params Modification[] mods)
        {
            foreach (Modification mod in mods)
            {
                Modifications.Add(mod);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerator<Modification> GetEnumerator()
        {
            return Modifications.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Modifications.GetEnumerator();
        }
    }
}