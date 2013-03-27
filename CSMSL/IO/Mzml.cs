using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//using pwiz.CLI.msdata;
//using pwiz.CLI.analysis;
//using pwiz.CLI.util;
using CSMSL.Spectral;
using CSMSL.Proteomics;
using System.Xml.Serialization;
using System.Xml;

namespace CSMSL.IO
{
    public class Mzml : MSDataFile
    {
        private static XmlSerializer _serializer = new XmlSerializer(typeof(indexedmzML));

        private indexedmzML _mzMLConnection;
        
        public Mzml(string filePath, bool openImmediately = false)
            : base(filePath, MsDataFileType.Mzml, openImmediately) { }

        public override void Open()
        {
            if (!IsOpen ||_mzMLConnection == null)
            {
                indexedmzML mzMLConnection = _serializer.Deserialize(new FileStream(FilePath, FileMode.Open)) as indexedmzML;
                _mzMLConnection = mzMLConnection;
                base.Open();
            }
        }

        public override void Dispose()
        {
            if (_mzMLConnection != null)
            {
                //_mzMLConnection.Close();
                _mzMLConnection = null;
            }
            base.Dispose();
        }        
        

        public override Proteomics.DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            
            throw new NotImplementedException();
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override MassRange GetMzRange(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override Spectral.MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            
           
            
            throw new NotImplementedException();
        }

        public override Spectral.MassSpectrum GetMzSpectrum(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override Spectral.Polarity GetPolarity(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override double GetRetentionTime(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override double GetInjectionTime(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override double GetResolution(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        protected override int GetFirstSpectrumNumber()
        {
            throw new NotImplementedException();
        }

        protected override int GetLastSpectrumNumber()
        {
            throw new NotImplementedException();
        }

        public override int GetSpectrumNumber(double retentionTime)
        {
            throw new NotImplementedException();
        }

    }
}
