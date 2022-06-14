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

using System.Collections;
using System.Collections.Generic;


namespace CSMSL.Analysis.ExperimentalDesign
{
    public class Sample : IEnumerable<ExperimentalCondition>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        private readonly List<ExperimentalCondition> _conditions;

        public ExperimentalCondition AddCondition(string name, string description = "")
        {
            ExperimentalCondition condition = new ExperimentalCondition(name, description);
            _conditions.Add(condition);
            return condition;
        }

        public Sample(string name, string description = "")
        {
            Name = name;
            Description = description;
            _conditions = new List<ExperimentalCondition>();
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