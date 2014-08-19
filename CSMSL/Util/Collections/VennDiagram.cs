// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (VennDiagram.cs) is part of CSMSL.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Util.Collections
{
    public class VennDiagram<T> where T : IEquatable<T>
    {
        public int Count
        {
            get { return _inputSets.Length; }
        }

        public VennSet<T> this[int index]
        {
            get { return _subSets[index - 1]; }
        }

        public VennSet<T> this[string region]
        {
            get { return _regions[region]; }
        }

        private VennSet<T>[] _exclusiveSubSets;
        private VennSet<T>[] _subSets;
        private Dictionary<string, VennSet<T>> _regions;

        public VennSet<T>[] SubSets
        {
            get { return _subSets; }
        }

        public VennSet<T>[] ExclusiveSubSets
        {
            get { return _exclusiveSubSets; }
        }

        public VennSet<T> TotalUnique
        {
            get { return _subSets[0]; }
        }

        private VennSet<T>[] _inputSets;

        private VennDiagram(params VennSet<T>[] sets)
        {
            _inputSets = sets;
        }

        public void Add(params VennSet<T>[] sets)
        {
            int newSize = Count + sets.Length;
            Array.Resize(ref _inputSets, newSize);
            for (int i = 0; i < sets.Length; i++)
            {
                _inputSets[newSize - (i + 1)] = sets[i];
            }
        }

        public static VennDiagram<T> CreateDiagram(params VennSet<T>[] sets)
        {
            VennDiagram<T> diagram = new VennDiagram<T>(sets) {_regions = new Dictionary<string, VennSet<T>>()};

            int count = diagram.Count;
            // Initialize subsets
            diagram._subSets = new VennSet<T>[count];
            diagram._exclusiveSubSets = new VennSet<T>[count];
            for (int i = 0; i < count; i++)
            {
                diagram._subSets[i] = new VennSet<T>(string.Format("In {0:G0} of {1:G0}", i + 1, count));
                diagram._exclusiveSubSets[i] = new VennSet<T>(string.Format("Only in {0:G0} of {1:G0}", i + 1, count));
            }

            //_subSets[Count] = new VennSet<T>("Total Unique");
            for (int depth = 1; depth <= count; depth++)
            {
                // Removed This as this was the only code i needed from the third party: Combinatorics.Collections;
                //Combinations<VennSet<T>> combinations = new Combinations<VennSet<T>>(diagram._inputSets, depth);
                //foreach (IList<VennSet<T>> combo_sets in combinations)
                foreach (VennSet<T>[] combo_sets in Combinatorics.Combinations(diagram._inputSets, depth))
                {
                    HashSet<T> baseSet = new HashSet<T>(combo_sets[0]);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(combo_sets[0].Name);
                    for (int i = 1; i < depth; i++)
                    {
                        VennSet<T> currentSet = combo_sets[i];
                        baseSet.IntersectWith(currentSet);
                        sb.Append(" ∩ ");
                        sb.Append(currentSet.Name);
                    }
                    diagram.AddVennSet(new VennSet<T>(baseSet, sb.ToString()), depth);

                    // Skip the last one
                    if (depth < count)
                    {
                        baseSet = new HashSet<T>(baseSet);
                        foreach (VennSet<T> set in diagram._inputSets)
                        {
                            if (!combo_sets.Contains(set))
                            {
                                baseSet.ExceptWith(set);
                            }
                        }
                        sb.Append(" only");
                        diagram.AddVennSet(new VennSet<T>(baseSet, sb.ToString()), depth, true);
                    }
                }
            }
            diagram._exclusiveSubSets[count - 1].Add(diagram._subSets[count - 1]);
            return diagram;
        }

        private void AddVennSet(VennSet<T> set, int depth, bool exclusive = false)
        {
            _subSets[depth - 1].Add(set);
            _regions.Add(set.Name, set);
            if (exclusive)
                _exclusiveSubSets[depth - 1].Add(set);
        }

        public override string ToString()
        {
            return string.Format("{0:G0} sets with {1:G0} unique items", Count, (TotalUnique != null) ? TotalUnique.Count : 0);
        }

        public IEnumerator<VennSet<T>> GetEnumerator()
        {
            return _regions.Values.GetEnumerator();
        }
    }
}