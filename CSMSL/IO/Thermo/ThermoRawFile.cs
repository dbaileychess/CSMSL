using CSMSL.Spectral;
using MSFileReaderLib;

namespace CSMSL.IO.Thermo
{
    public class ThermoRawFile : MsDataFile
    {
        private IXRawfile5 _rawConnection;

        public ThermoRawFile(string filePath, bool openImmediately = false)
            : base(filePath, MsDataFileType.ThermoRawFile) 
        {
            if (openImmediately) Open();
        }

        public override int FirstSpectrumNumber
        {
            get
            {
                if (base.FirstSpectrumNumber < 0)
                {
                    base.FirstSpectrumNumber = GetFirstSpectrumNumber();
                }
                return base.FirstSpectrumNumber;
            }
        }

        public override int LastSpectrumNumber
        {
            get
            {
                if (base.LastSpectrumNumber < 0)
                {
                    base.LastSpectrumNumber = GetLastSpectrumNumber();
                }
                return base.LastSpectrumNumber;
            }
        }

        public override void Open()
        {
            if (!IsOpen)
            {
                _rawConnection = (IXRawfile5)new MSFileReader_XRawfile();
                _rawConnection.Open(FilePath);
                _rawConnection.SetCurrentController(0, 1); // first 0 is for mass spectrometer
                base.Open();
            }           
        }

        public override void Dispose()
        {
            if (_rawConnection != null)
            {
                _rawConnection.Close();
                _rawConnection = null;
            }           
            base.Dispose();
        }

        private int GetFirstSpectrumNumber()
        {
            int spectrumNumber = 0;
            if (_rawConnection != null)
                _rawConnection.GetFirstSpectrumNumber(ref spectrumNumber);
            return spectrumNumber;
        }

        private int GetLastSpectrumNumber()
        {
            int spectrumNumber = 0;
            if (_rawConnection != null)
                _rawConnection.GetLastSpectrumNumber(ref spectrumNumber);
            return spectrumNumber;
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            int msnOrder = 0;
            if (_rawConnection != null)
                _rawConnection.GetMSOrderForScanNum(spectrumNumber, ref msnOrder);
            return msnOrder;
        }

        public override MsScan GetMsScan(int spectrumNumber)
        {
            MsScan scan = new MsScan(spectrumNumber, this);         
            return new MsScan(spectrumNumber, this);
        }

        public void DoSomething()
        {
            
        }
    }
}
