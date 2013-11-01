using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSMSL.IO
{
    public class UniProtXml : IDisposable
    {
        public string FilePath { get; set; }

        private Stream _baseStream;
        private XmlSerializer _serializer;
        private uniprot _baseUniprot { get; set; }

        public UniProtXml(string filePath)
        {
            _baseStream  = new FileStream(filePath, FileMode.Open);
            _serializer = new XmlSerializer(typeof(uniprot));
            _baseUniprot = _serializer.Deserialize(_baseStream) as uniprot;
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
