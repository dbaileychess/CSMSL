﻿// Copyright 2022 Derek J. Bailey
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

using System;

namespace CSMSL
{
    /// <summary>
    /// A continuous, inclusive range of values, with a well defined minimum and maximum value
    /// </summary>
    public class Range<T> : IRange<T> where T : IComparable<T>, IEquatable<T>
    {
        public Range()
            : this(default(T), default(T))
        {
        }

        public Range(IRange<T> range)
            : this(range.Minimum, range.Maximum)
        {
        }

        public Range(T minimum, T maximum)
        {
            if (maximum.CompareTo(minimum) < 0)
                throw new ArgumentException(minimum + " > " + maximum + ", unable to create negative ranges.");

            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        /// The maximum value of the range
        /// </summary>
        public T Maximum { get; protected set; }

        /// <summary>
        /// The minimum value of the range
        /// </summary>
        public T Minimum { get; protected set; }

        /// <summary>
        /// Determines whether an item is below, above, or contained within a range of values
        /// </summary>
        /// <param name="item">The item to compare against the range</param>
        /// <returns>-1 if item is below the range, 1 if item is above the range, 0 otherwise</returns>
        public int CompareTo(T item)
        {
            if (Minimum.CompareTo(item) > 0)
                return -1;
            if (Maximum.CompareTo(item) < 0)
                return 1;
            return 0;
        }

        /// <summary>
        /// Checks to see if this range is a proper super range of another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if this range is fully encloses the other range, false otherwise</returns>
        public bool IsSuperRange(IRange<T> other)
        {
            if (other == null)
                return false;

            return Maximum.CompareTo(other.Maximum) >= 0 && Minimum.CompareTo(other.Minimum) <= 0;
        }

        /// <summary>
        /// Checks to see if this range is a proper sub range of another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if this range is fully enclosed by the other range, false otherwise</returns>
        public bool IsSubRange(IRange<T> other)
        {
            if (other == null)
                return false;

            return Maximum.CompareTo(other.Maximum) <= 0 && Minimum.CompareTo(other.Minimum) >= 0;
        }

        /// <summary>
        /// Checks to see if this range overlaps another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if the other range in any way overlaps this range, false otherwise</returns>
        public bool IsOverlapping(IRange<T> other)
        {
            if (other == null)
                return false;

            return Maximum.CompareTo(other.Minimum) >= 0 && Minimum.CompareTo(other.Maximum) <= 0;
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
        /// Returns a formatted string of the range [min - max]
        /// </summary>
        /// <returns>Format: [min - max]</returns>
        public override string ToString()
        {
            return string.Format("[{0} - {1}]", Minimum, Maximum);
        }

        /// <summary>
        /// Compares if two ranges are identical
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if both the minimum and maximum values are equivalent, false otherwise</returns>
        public bool Equals(IRange<T> other)
        {
            if (other == null)
                return false;

            return Maximum.Equals(other.Maximum) && Minimum.Equals(other.Minimum);
        }

        public override int GetHashCode()
        {
            return Minimum.GetHashCode() + (Maximum.GetHashCode() << 3);
        }

        public override bool Equals(object obj)
        {
            IRange<T> other = obj as IRange<T>;

            return other != null && Equals(other);
        }
    }
}