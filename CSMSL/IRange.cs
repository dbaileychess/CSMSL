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
        T Minimum { get; }

        /// <summary>
        /// The maximum value of this range
        /// </summary>
        T Maximum { get; }

        /// <summary>
        /// Checks if an item is within the range
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item is within minimum and maximum (inclusive)</returns>
        bool Contains(T item);

        /// <summary>
        /// Checks if an item is below, within, or above this range
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>-1 if the item is below, 0 if within (inclusive), or 1 if above</returns>
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