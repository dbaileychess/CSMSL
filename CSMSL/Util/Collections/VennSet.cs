///////////////////////////////////////////////////////////////////////////
//  VennSet.cs - A set of objects in collection                           /
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

namespace CSMSL.Util.Collections
{
    public class VennSet<T> : HashSet<T> where T : IEquatable<T>
    {
        private string _name;

        public VennSet(IEnumerable<T> items, string name = "")
            : base(items)
        {
            Name = name;
        }

        public VennSet(string name = "")
            : base()
        {
            Name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void Add(IEnumerable<T> items)
        {
            this.UnionWith(items);
        }

        public override string ToString()
        {
            return string.Format("{0} Count = {1:G0}", Name, Count);
        }
    }
}