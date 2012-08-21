///////////////////////////////////////////////////////////////////////////
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

        private VennSet<T> _totalUnique;

        public VennSet<T> TotalUnique
        {
            get { return _totalUnique; }
            private set { _totalUnique = value; }
        }

        private VennSet<T>[] _inputSets;

        public VennDiagram(params VennSet<T>[] sets)
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

        public void CreateDiagram()
        {
            //Regions = new Dictionary<string, VennSet<T>>();
            //for (int i = 0; i < Count; i++)
            //{
            //    SubSets[i] = new VennSet<T>("In " + (i + 1));
            //}

            //_totalUnique = new VennSet<T>("Total Unique");
            //for (int depth = 1; depth <= Count; depth++)
            //{
            //    Combinations<VennSet<T>> combinations = new Combinations<VennSet<T>>(sets, depth);
            //    foreach (IList<VennSet<T>> combo_sets in combinations)
            //    {
            //        HashSet<T> baseSet = new HashSet<T>(combo_sets[0]);
            //        StringBuilder sb = new StringBuilder();
            //        sb.Append(combo_sets[0].Name);
            //        for (int i = 1; i < depth; i++)
            //        {
            //            VennSet<T> currentSet = combo_sets[i];
            //            baseSet.IntersectWith(currentSet);
            //            sb.Append(" ∩ ");
            //            sb.Append(currentSet.Name);
            //        }
            //        yield return new VennSet<T>(baseSet, sb.ToString());

            //        // Skip for the last one
            //        if (depth == number_of_sets)
            //        {
            //            break;
            //        }

            //        baseSet = new HashSet<T>(baseSet);
            //        foreach (VennSet<T> set in sets)
            //        {
            //            if (!combo_sets.Contains(set))
            //            {
            //                baseSet.ExceptWith(set);
            //            }
            //        }
            //        sb.Append(" only");
            //        yield return new VennSet<T>(baseSet, sb.ToString());
            //    }

            //    _totalUnique.Add(_inputSets[depth - 1]);
            //}

            //foreach (VennSet<T> set in VennSet<T>.GenerateDiagram(InputSets))
            //{
            //    int numberofsets = set.Name.Count(c => c == '∩') + 1;
            //    if (numberofsets == Count)
            //    {
            //        SubSets[numberofsets - 1].AddSet(set);
            //    }
            //    else if (set.Name.EndsWith("only"))
            //    {
            //        SubSets[numberofsets - 1].AddSet(set);
            //    }
            //    Regions.Add(set.Name, set);
            //}
        }

        public override string ToString()
        {
            return string.Format("{0:G0} sets with {1:G0} unique items", Count, (TotalUnique != null) ? TotalUnique.Count : 0);
        }

        public IEnumerator<VennSet<T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}