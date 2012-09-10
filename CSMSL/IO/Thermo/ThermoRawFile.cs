using System;
using CSMSL.Spectral;
using MSFileReaderLib;

namespace CSMSL.IO.Thermo
{   
    public class ThermoRawFile : MsDataFile
    {
        private enum RawLabelDataColumn
        {
            MZ = 0,
            Intensity = 1,
            Resolution = 2,
            NoiseBaseline = 3,
            NoiseLevel = 4,
            Charge = 5
        }

        private enum ThermoMzAnalyzer
        {
            None = -1,
            ITMS = 0,
            TQMS = 1,
            SQMS = 2,
            TOFMS = 3,
            FTMS = 4,
            Sector = 5
        }

        private IXRawfile5 _rawConnection;

        public ThermoRawFile(string filePath, bool openImmediately = false)
            : base(filePath, MsDataFileType.ThermoRawFile, openImmediately) { }

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

        protected override int GetFirstSpectrumNumber()
        {
            int spectrumNumber = 0;
            if (_rawConnection != null)
                _rawConnection.GetFirstSpectrumNumber(ref spectrumNumber);
            return spectrumNumber;
        }

        protected override int GetLastSpectrumNumber()
        {
            int spectrumNumber = 0;
            if (_rawConnection != null)
                _rawConnection.GetLastSpectrumNumber(ref spectrumNumber);
            return spectrumNumber;
        }

        public override double GetRetentionTime(int spectrumNumber)
        {
            double retentionTime = 0;
            if (_rawConnection != null)
                _rawConnection.RTFromScanNum(spectrumNumber, ref retentionTime);
            return retentionTime;
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            int msnOrder = 0;
            if (_rawConnection != null)
                _rawConnection.GetMSOrderForScanNum(spectrumNumber, ref msnOrder);
            return msnOrder;
        }

        private object GetExtravalue(int spectrumNumber, string filter)
        {
            object value = null;
            if (_rawConnection != null)
                _rawConnection.GetTrailerExtraValueForScanNum(spectrumNumber, filter, ref value);
            return value;
        }

        public override Polarity GetPolarity(int spectrumNumber)
        {
            short charge = (short)GetExtravalue(spectrumNumber, "Charge State:");
            return (Polarity)(Math.Sign(charge));
        }

        public void DoSomething()
        {
        }

        public override Spectral.Spectrum GetMzSpectrum(int spectrumNumber)
        {
            double[,] peakData = GetLabeledData(spectrumNumber);
            return new Spectral.Spectrum(peakData);
        }

        private double[,] GetLabeledData(int spectrumNumber)
        {
            object labels = null;
            object flags = null;
            _rawConnection.GetLabelData(ref labels, ref flags, ref spectrumNumber);
            double[,] peakData = (double[,])labels;
            double[,] transformedPeakData = new double[peakData.GetLength(1), 2];
            for (int i = 0; i < peakData.GetLength(1); i++)
            {
                transformedPeakData[i, 0] = peakData[0, i];
                transformedPeakData[i, 1] = peakData[1, i];
            }
            return transformedPeakData;
        }        

        public override MzAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            int mzanalyzer = 0;
            if(_rawConnection != null)
                _rawConnection.GetMassAnalyzerTypeForScanNum(spectrumNumber, ref mzanalyzer);
            switch ((ThermoMzAnalyzer)mzanalyzer)
            {
                case ThermoMzAnalyzer.FTMS:
                    return MzAnalyzerType.Orbitrap;
                case ThermoMzAnalyzer.ITMS:
                    return MzAnalyzerType.IonTrap2D;
                case ThermoMzAnalyzer.Sector:
                    return MzAnalyzerType.Sector;
                case ThermoMzAnalyzer.TOFMS:
                    return MzAnalyzerType.TOF;
                case ThermoMzAnalyzer.TQMS:
                case ThermoMzAnalyzer.SQMS:
                case ThermoMzAnalyzer.None:
                default:
                    return MzAnalyzerType.Unknown;
            }              
        }
    }
}