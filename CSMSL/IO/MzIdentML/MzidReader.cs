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
