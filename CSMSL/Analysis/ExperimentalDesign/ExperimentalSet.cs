// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ExperimentalSet.cs) is part of CSMSL.
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
using System.Linq;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalSet : IEnumerable<ExperimentalCondition>
    {
        private readonly HashSet<ExperimentalCondition> _conditions;

        public string Name { get; private set; }

        public ExperimentalSet(string name = "")
        {
            Name = name;
            _conditions = new HashSet<ExperimentalCondition>();
        }

        public void Add(ExperimentalCondition condition)
        {
            _conditions.Add(condition);
        }

        public IEnumerable<Modification> GetAllModifications()
        {
            return _conditions.SelectMany(c => c.Modifications);
        }

        public bool Contains(Modification mod)
        {
            return _conditions.Any(c => c.Modifications.Contains(mod));
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerator<ExperimentalCondition> GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }
    }
}