///////////////////////////////////////////////////////////////////////////
//  Range.cs - Represents a span of objects, with a minimum and maximum   /
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

namespace CSMSL
{
    /// <summary>
    /// A continuous, inclusive range of values
    /// </summary>
    public class Range<T> : IRange<T>
        where T : IComparable<T>, IEquatable<T>
    {
        protected T _max;
        protected T _min;

        public Range()
            : this(default(T), default(T)) { }

        public Range(IRange<T> range)
            : this(range.Minimum, range.Maximum) { }

        public Range(T minimum, T maximum)
        {
            _min = minimum;
            _max = maximum;
        }

        /// <summary>
        /// The maximum value of the range
        /// </summary>
        public T Maximum
        {
            get { return _max; }
            set { T _max = value; }
        }

        /// <summary>
        /// The minimum value of the range
        /// </summary>
        public T Minimum
        {
            get { return _min; }
            set { T _min = value; }
        }

        /// <summary>
        /// Determines whether an item is below, above, or contained within a range of values
        /// </summary>
        /// <param name="item">The item to compare against the range</param>
        /// <returns>-1 if item is below the range, 1 if item is above the range, 0 otherwise</returns>
        public int CompareTo(T item)
        {
            if (_min.CompareTo(item) > 0) return -1;
            if (_max.CompareTo(item) < 0) return 1;
            return 0;
        }

        /// <summary>
        /// Checks to see if this range is a proper super range of another range (inclusive)
        /// </summary>
        /// <param name="other">Thje other range to compare to</param>
        /// <returns>True if this range is fully encloses the other range, false otherwise</returns>
        public bool IsSuperRange(IRange<T> other)
        {
            return (_max.CompareTo(other.Maximum) >= 0 && _min.CompareTo(other.Minimum) <= 0);
        }

        /// <summary>
        /// Checks to see if this range is a proper sub range of another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if this range is fully enclosed by the other range, false otherwise</returns>
        public bool IsSubRange(IRange<T> other)
        {
            return (_max.CompareTo(other.Maximum) <= 0 && _min.CompareTo(other.Minimum) >= 0);
        }

        /// <summary>
        /// Checks to see if this range overlaps another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if the other range in any way overlaps this range, false otherwise</returns>
        public bool IsOverlapping(IRange<T> other)
        {
            return Contains(other.Minimum) || Contains(other.Maximum);
        }

        /// <summary>
        /// Determines if the item is contained within a range of values
        /// </summary>
        /// <param name="item">The item to compare against the range</param>
        /// <returns>True if the item is within the range (inclusive), false otherwise</returns>
        public bool Contains(T item)
        {
            return CompareTo(item).Equals(0);
        }

        /// <summary>
        /// Returns a formated string of the range [min - max]
        /// </summary>
        /// <returns>Format: [min - max]</returns>
        public override string ToString()
        {
            return string.Format("[{0:F4} - {1:F4}]", _min, _max);
        }

        /// <summary>
        /// Compares if two ranges are identical
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if both the minimum and maximum values are equivalent, false otherwise</returns>
        public bool Equals(IRange<T> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if ((this == null) != (other == null)) return false;
            return _max.Equals(other.Maximum) && _min.Equals(other.Minimum);
        }     
    }
}