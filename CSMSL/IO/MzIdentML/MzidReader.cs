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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using MzIdentML;

namespace CSMSL.IO.MzIdentML
{
    public class MzidReader
    {        
        private MzIdentMLType _connection;

        public string Version { 
            get { return _connection.version; } 
            set { _connection.version = value; } 
        }

        public string FilePath {get; private set;}

        public MzidReader(string filePath)
        {
           FilePath = filePath;
        }

        public void GetAnalysis()
        {

           
        }

        public void Open()
        {
            if (File.Exists(FilePath))
            {
                _connection = MzIdentMLType.LoadFromFile(FilePath);            
            }
            else
            {
                // Create a new file
                _connection = new MzIdentMLType();
            }            
        }

        public void Save()
        {            
            SaveAs(FilePath);
        }

        public void SaveAs(string filePath)
        {
            _connection.SaveToFile(filePath);
        }
    }
}
