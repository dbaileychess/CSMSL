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
    /// <summary>
    /// A range of values with a well defined minimum and maximum value
    /// </summary>
    /// <typeparam name="T">Any type that uses IComparable</typeparam>
    public interface IRange<T> : IEquatable<IRange<T>> where T : IComparable<T>
    {
        /// <summary>
        /// The minimum value of this range
        /// </summary>
        T Minimum { get; set; }

        /// <summary>
        /// The maximum value of this range
        /// </summary>
        T Maximum { get; set; }

        /// <summary>
        /// Checks if an item is within the range
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item is >= minimum and <= maximum</returns>
        bool Contains(T item);

        int CompareTo(T item);

        /// <summary>
        /// Checks to see if this range is a proper sub range of another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if this range is fully enclosed by the other range, false otherwise</returns>
        bool IsSubRange(IRange<T> other);

        bool IsSuperRange(IRange<T> other);

        bool IsOverlapping(IRange<T> other);
    }
}