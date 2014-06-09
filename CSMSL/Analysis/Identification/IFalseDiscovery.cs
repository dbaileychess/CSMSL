// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (IFalseDiscovery.cs) is part of CSMSL.
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

namespace CSMSL.Analysis.Identification
{
    /// <summary>
    /// An interface to filter lists based on the detection of decoy hits that are known to be wrong
    /// </summary>
    /// <typeparam name="T">The type of scoring metric of the object (must implement IComparable)</typeparam>
    public interface IFalseDiscovery<out T> where T : IComparable<T>
    {
        /// <summary>
        /// States whether the object is a Decoy hit (Known False Positive) or a Foward Hit (Unknown True Positive)
        /// </summary>
        bool IsDecoy { get; }

        /// <summary>
        /// The scoring metric for the object
        /// </summary>
        T FdrScoreMetric { get; }
    }
}