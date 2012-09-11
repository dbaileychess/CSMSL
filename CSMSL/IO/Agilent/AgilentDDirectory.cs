using Agilent.MassSpectrometry.DataAnalysis;
using CSMSL.IO;
using CSMSL.Spectral;

namespace CSMSL.IO.Agilent
{
    public class AgilentDDirectory : MsDataFile
    {
        private IMsdrDataReader _msdr;

        public AgilentDDirectory(string directoryPath, bool openImmediately = false)
            : base(directoryPath, MsDataFileType.AgilentRawFile, openImmediately) { }       
          
        public override void Open()
        {
            if(!IsOpen)
            {
                _msdr = (IMsdrDataReader)new MassSpecDataReader();
                _msdr.OpenDataFile(FilePath);
                base.Open();
            }
        }

        public override void Dispose()
        {
            if(_msdr != null)
            {
                _msdr.CloseDataFile();
                _msdr = null;
            }
            base.Dispose();
        }

        protected override int GetFirstSpectrumNumber()
        {
            return 1;
        }

        protected override int GetLastSpectrumNumber()
        {
            return (int)(_msdr.MSScanFileInformation.TotalScansPresent + 1);
        }

        public override double GetRetentionTime(int spectrumNumber)
        {
            IMSScanRecord scan_record = _msdr.GetScanRecord(spectrumNumber - 1);
            return scan_record.RetentionTime;
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            IMSScanRecord scan_record = _msdr.GetScanRecord(spectrumNumber - 1);
            return scan_record.MSLevel == MSLevel.MSMS ? 2 : 1;
        }

        private object GetExtraValue(int spectrumNumber, string filter)
        {
            IBDAActualData[] actuals = _msdr.ActualsInformation.GetActualCollection(GetRetentionTime(spectrumNumber));
            foreach(IBDAActualData actual in actuals)
            {
                if(actual.DisplayName == filter)
                {
                    return actual.DisplayValue;
                }
            }
            return null;
        }

        public override Polarity GetPolarity(int spectrumNumber)
        {
            IMSScanRecord scan_record = _msdr.GetScanRecord(spectrumNumber - 1);
            switch(scan_record.IonPolarity)
            {
                case IonPolarity.Positive:
                    return Polarity.Positive;
                case IonPolarity.Negative:
                    return Polarity.Negative;
                default:
                    return Polarity.Neutral;
            }
        }

        public override Spectral.Spectrum GetMzSpectrum(int spectrumNumber)
        {
            throw new System.NotImplementedException();
        }

        public override MzAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            throw new System.NotImplementedException();
        }

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            throw new System.NotImplementedException();
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            throw new System.NotImplementedException();
        }

        public override Proteomics.DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            throw new System.NotImplementedException();
        }

        public override Range GetMzRange(int spectrumNumber)
        {
            throw new System.NotImplementedException();
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            throw new System.NotImplementedException();
        }
    }
}