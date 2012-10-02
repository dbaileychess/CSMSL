﻿///////////////////////////////////////////////////////////////////////////
//  VennDiagram.cs - A set of overlapping objects divided into regions    /
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
using System.Collections.Generic;
using System.Text;
using Combinatorics;

namespace CSMSL.Util
{
    public class VennDiagram<T> :
        IEnumerable<VennSet<T>>
        where T : IEquatable<T>
    {
        public int Count
        {
            get { return _inputSets.Length; }
        }

        private VennSet<T>[] _subSets;
        private Dictionary<string, VennSet<T>> _regions;

        public VennSet<T>[] SubSets
        {
            get { return _subSets; }
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
            Array.Resize<VennSet<T>>(ref _inputSets, newSize);
            for (int i = 0; i < sets.Length; i++)
            {
                _inputSets[newSize - (i + 1)] = sets[i];
            }
        }

        public static VennDiagram<T> CreateDiagram(params VennSet<T>[] sets)
        {
            VennDiagram<T> diagram = new VennDiagram<T>(sets);
            diagram._regions = new Dictionary<string, VennSet<T>>();

            int count = diagram.Count;
            // Initialize subsets
            diagram._subSets = new VennSet<T>[count];
            for (int i = 0; i < count; i++)
            {
                diagram._subSets[i] = new VennSet<T>(string.Format("In {0:G0} of {1:G0}", i + 1, count));
            }

            //_subSets[Count] = new VennSet<T>("Total Unique");
            for (int depth = 1; depth <= count; depth++)
            {
                Combinations<VennSet<T>> combinations = new Combinations<VennSet<T>>(diagram._inputSets, depth);
                foreach (IList<VennSet<T>> combo_sets in combinations)
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
                        diagram.AddVennSet(new VennSet<T>(baseSet, sb.ToString()), depth);
                    }
                }
            }
            return diagram;
        }

        private void AddVennSet(VennSet<T> set, int depth)
        {
            _subSets[depth - 1].Add(set);
            _regions.Add(set.Name, set);
        }

        public override string ToString()
        {
            return string.Format("{0:G0} sets with {1:G0} unique items", Count, (TotalUnique != null) ? TotalUnique.Count : 0);
        }

        public IEnumerator<VennSet<T>> GetEnumerator()
        {
            return _regions.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _regions.Values.GetEnumerator();
        }
    }
}