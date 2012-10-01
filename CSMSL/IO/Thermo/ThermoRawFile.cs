using System;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        private object GetExtraValue(int spectrumNumber, string filter)
        {
            object value = null;
            if (_rawConnection != null)              
                _rawConnection.GetTrailerExtraValueForScanNum(spectrumNumber, filter, ref value);
            return value;
        }

        private string GetScanFilter(int spectrumNumber)
        {
            string filter = null;
            if(_rawConnection != null)
                _rawConnection.GetFilterForScanNum(spectrumNumber, ref filter);
            return filter;
        }

        private static Regex _polarityRegex = new Regex(@" \+ ", RegexOptions.Compiled);

        public override Polarity GetPolarity(int spectrumNumber)
        {
            string filter = GetScanFilter(spectrumNumber);
            if (_polarityRegex.IsMatch(filter))
            {
                return Polarity.Positive;
            }
            else
            {
                return Polarity.Negative;
            }
        }

        public override Spectrum GetMzSpectrum(int spectrumNumber)
        {
            double[,] peakData = GetLabeledData(spectrumNumber);
            int count = peakData.GetLength(1);
            List<Peak> peaks = new List<Peak>();
            for (int i = 0; i < count; i++)
            {
                peaks.Add(new Peak(peakData[0, i], (float)peakData[1, i]));
            }
            return new Spectrum(peaks);
        }

        private double[,] GetLabeledData(int spectrumNumber)
        {
            object labels = null;
            object flags = null;            
            if(_rawConnection !=null)       
                _rawConnection.GetLabelData(ref labels, ref flags, ref spectrumNumber);
            return (double[,])labels; 
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

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            double mz = double.NaN;
            if (_rawConnection != null)
                _rawConnection.GetPrecursorMassForScanNum(spectrumNumber, msnOrder, ref mz);
            return mz;
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            object width = GetExtraValue(spectrumNumber, string.Format("MS{0} Isolation Width:", msnOrder));
            if (width is double)
            {
                return (double)width;
            }
            return (float)width;            
        }

        public override DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            int type = 0;
            if (_rawConnection != null)
                _rawConnection.GetActivationTypeForScanNum(spectrumNumber, msnOrder, ref type);
            return (DissociationType)type;
        }

        public override Range GetMzRange(int spectrumNumber)
        {
            int number_of_packets = -1;
            double start_time = double.NaN;
            double low_mass = double.NaN;
            double high_mass = double.NaN;
            double total_ion_current = double.NaN;
            double base_peak_mass = double.NaN;
            double base_peak_intensity = double.NaN;
            int number_of_channels = -1;
            int uniform_time = -1;
            double frequency = double.NaN;
            if (_rawConnection != null)
                _rawConnection.GetScanHeaderInfoForScanNum(spectrumNumber, ref number_of_packets, ref start_time, ref low_mass, ref high_mass,
                    ref total_ion_current, ref base_peak_mass, ref base_peak_intensity, ref number_of_channels, ref uniform_time, ref frequency);

            return new Range(low_mass, high_mass);
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            return (short)GetExtraValue(spectrumNumber, "Charge State:");
        }
    }
}