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
            _baseStream  = new FileStream(filePath, FileMode.Open);
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
