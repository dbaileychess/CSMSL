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

namespace CSMSL
{
    /// <summary>
    /// The units of tolerance
    /// </summary>
    public enum ToleranceUnit
    {
        /// <summary>
        /// Parts per million
        /// </summary>
        PPM,

        /// <summary>
        /// Daltons
        /// </summary>
        DA,

        /// <summary>
        /// Milli Mass Units
        /// <para>
        /// Equivalent to mDa
        /// </para>
        /// </summary>
        MMU
    }
}