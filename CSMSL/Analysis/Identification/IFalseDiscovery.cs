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