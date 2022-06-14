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