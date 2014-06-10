// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (UniProtXML.cs) is part of CSMSL.
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
using System.IO;
using System.Xml.Serialization;

namespace CSMSL.IO
{
    public class UniProtXml : IDisposable
    {
        public string FilePath { get; set; }

        private readonly Stream _baseStream;

        private uniprot _baseUniprot { get; set; }

        public UniProtXml(string filePath)
        {
            _baseStream = new FileStream(filePath, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(uniprot));
            _baseUniprot = serializer.Deserialize(_baseStream) as uniprot;
        }

        public entry[] Entries
        {
            get { return _baseUniprot.entry; }
        }

        public void Dispose()
        {
            _baseStream.Dispose();
            _baseUniprot = null;
        }
    }
}