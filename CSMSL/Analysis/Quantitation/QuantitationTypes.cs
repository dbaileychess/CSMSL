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

namespace CSMSL.Analysis.Quantitation
{
    [Flags]
    public enum QuantitationTypes
    {
        None = 0,
        ReporterTag = 1,
        SILAC = 2,
        Chemical = 4,
        Metabolic = 8,
        MS1Based = 16,
        MS2Based = 32
    }
}