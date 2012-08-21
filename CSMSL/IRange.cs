///////////////////////////////////////////////////////////////////////////
//  IRange.cs - Represents a span of objects, with a minimum and maximum  /
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

namespace CSMSL
{
    public interface IRange<T> : IEquatable<IRange<T>> where T : IComparable<T>
    {
        T Minimum { get; set; }

        T Maximum { get; set; }

        bool Contains(T item);

        int CompareTo(T item);

        bool IsSubRange(IRange<T> other);

        bool IsSuperRange(IRange<T> other);

        bool IsOverlaping(IRange<T> other);
    }
}