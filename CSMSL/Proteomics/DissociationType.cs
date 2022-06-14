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
namespace CSMSL.Proteomics
{
    public enum DissociationType
    {
        UnKnown = -1,
        None = 6,
        CID = 0,
        HCD = 5,
        ETD = 4,
        MPD = 1,
        ECD = 2,
        PQD = 3,
        SA = 7,
        PTR = 8,
        NETD = 9,
        NPTR = 10,
        CI = 11
    }
}