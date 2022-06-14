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

using System.Collections.Generic;

namespace CSMSL.IO.MzTab
{
    public class MzTabSoftware
    {
        public CVParamater Parameter { get; set; }
        public List<string> Settings { get; set; }

        public MzTabSoftware(CVParamater paramater)
        {
            Parameter = paramater;
            Settings = new List<string>();
        }

        public void AddSetting(string setting)
        {
            Settings.Add(setting);
        }

        public override string ToString()
        {
            return Parameter.ToString();
        }
    }
}