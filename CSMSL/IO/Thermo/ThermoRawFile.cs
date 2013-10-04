using CSMSL.Proteomics;
using CSMSL.Spectral;
using MSFileReaderLib;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSMSL.IO.Thermo
{   
    public class ThermoRawFile : MSDataFile
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

        public ThermoRawFile(string filePath)
            : base(filePath, MSDataFileType.ThermoRawFile) { }

        /// <summary>
        /// Opens the connection to the underlying data
        /// </summary>
        public override void Open()
        {
            if (IsOpen && _rawConnection != null)
                return;

            _rawConnection = (IXRawfile5)new MSFileReader_XRawfile();
            _rawConnection.Open(FilePath);
            _rawConnection.SetCurrentController(0, 1); // first 0 is for mass spectrometer
            base.Open();
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
            _rawConnection.GetFirstSpectrumNumber(ref spectrumNumber);
            return spectrumNumber;
        }

        protected override int GetLastSpectrumNumber()
        {
            int spectrumNumber = 0;
            _rawConnection.GetLastSpectrumNumber(ref spectrumNumber);
            return spectrumNumber;
        }

        public override double GetRetentionTime(int spectrumNumber)
        {
            double retentionTime = 0;
            _rawConnection.RTFromScanNum(spectrumNumber, ref retentionTime);
            return retentionTime;
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            int msnOrder = 0;
            _rawConnection.GetMSOrderForScanNum(spectrumNumber, ref msnOrder);
            return msnOrder;
        }

        public override int GetParentSpectrumNumber(int spectrumNumber)
        {
            object parentScanNumber = GetExtraValue(spectrumNumber, "Master Scan Number:");
            return (int) parentScanNumber;
        }

        private object GetExtraValue(int spectrumNumber, string filter)
        {
            object value = null;
            _rawConnection.GetTrailerExtraValueForScanNum(spectrumNumber, filter, ref value);
            return value;
        }

        private string GetScanFilter(int spectrumNumber)
        {
            string filter = null;
            _rawConnection.GetFilterForScanNum(spectrumNumber, ref filter);
            return filter;
        }

        private static readonly Regex PolarityRegex = new Regex(@" \+ ", RegexOptions.Compiled);

        public override Polarity GetPolarity(int spectrumNumber)
        {
            string filter = GetScanFilter(spectrumNumber);
            return PolarityRegex.IsMatch(filter) ? Polarity.Positive : Polarity.Negative;
        }

        public override MassSpectrum GetMzSpectrum(int spectrumNumber)
        {
            return GetMzSpectrum(spectrumNumber, false);
        }
 
        public MassSpectrum GetMzSpectrum(int spectrumNumber, bool profileIfAvailable = false)
        {
            MZAnalyzerType mzAnalyzer = GetMzAnalyzer(spectrumNumber);
            double[,] peakData;
            int count;
            bool useProfile = false;

            if (profileIfAvailable)
            {
                int isProfile = 0;
                _rawConnection.IsProfileScanForScanNum(spectrumNumber, ref isProfile);
                useProfile = isProfile == 1;
            }


            if (useProfile || mzAnalyzer == MZAnalyzerType.IonTrap2D)
            {
                peakData = GetUnlabeledData(spectrumNumber, !useProfile);
                count = peakData.GetLength(1);
                List<MZPeak> lowResPeaks = new List<MZPeak>();
                for (int i = 0; i < count; i++)
                {
                    lowResPeaks.Add(new MZPeak(peakData[0, i], (float)peakData[1, i]));
                }
                return new MassSpectrum(lowResPeaks);     
            }
            
            peakData = GetLabeledData(spectrumNumber);
            count = peakData.GetLength(1);
            List<ThermoLabeledPeak> peaks = new List<ThermoLabeledPeak>();
            for (int i = 0; i < count; i++)
            {
                peaks.Add(new ThermoLabeledPeak(peakData[0, i], (float)peakData[1, i], (short)peakData[5, i], (float)peakData[4, i]));
            }
            return new MassSpectrum(peaks);    
        }

        private double[,] GetUnlabeledData(int spectrumNumber, bool useCentroid)
        {
            object massList = null;
            object peakFlags = null;
            int arraySize = -1;
            double centroidPeakWidth = double.NaN;
            _rawConnection.GetMassListFromScanNum(ref spectrumNumber, null, 0, 0, 0, Convert.ToInt32(useCentroid), ref centroidPeakWidth, ref massList, ref peakFlags, ref arraySize);
            return (double[,])massList;                
        }

        private double[,] GetLabeledData(int spectrumNumber)
        {
            object labels = null;
            object flags = null; 
            _rawConnection.GetLabelData(ref labels, ref flags, ref spectrumNumber);
            return (double[,])labels; 
        }        

        public override MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            int mzanalyzer = 0;
            _rawConnection.GetMassAnalyzerTypeForScanNum(spectrumNumber, ref mzanalyzer);
            
            switch ((ThermoMzAnalyzer)mzanalyzer)
            {
                case ThermoMzAnalyzer.FTMS:
                    return MZAnalyzerType.Orbitrap;
                case ThermoMzAnalyzer.ITMS:
                    return MZAnalyzerType.IonTrap2D;
                case ThermoMzAnalyzer.Sector:
                    return MZAnalyzerType.Sector;
                case ThermoMzAnalyzer.TOFMS:
                    return MZAnalyzerType.TOF;
                default:
                    return MZAnalyzerType.Unknown;
            }              
        }

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            double mz = double.NaN;
            _rawConnection.GetPrecursorMassForScanNum(spectrumNumber, msnOrder, ref mz);
            return mz;
        }

        public double GetPrecusorMz(int spectrumNumber, double searchMZ, int msnOrder = 2)
        {
            int parentScanNumber = GetParentSpectrumNumber(spectrumNumber);
            MassSpectrum ms1Scan = GetMzSpectrum(parentScanNumber);
            MZPeak peak = ms1Scan.GetClosestPeak(searchMZ, MassTolerance.FromDA(50));
            if (peak != null)
                return peak.MZ;
            return double.NaN;
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            object width = GetExtraValue(spectrumNumber, string.Format("MS{0} Isolation Width:", msnOrder));
            return Convert.ToDouble(width);
        }

        public override DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            int type = 0;
            _rawConnection.GetActivationTypeForScanNum(spectrumNumber, msnOrder, ref type);
            return (DissociationType)type;
        }

        public override MassRange GetMzRange(int spectrumNumber)
        {
            int numberOfPackets = -1;
            double startTime = double.NaN;
            double lowMass = double.NaN;
            double highMass = double.NaN;
            double totalIonCurrent = double.NaN;
            double basePeakMass = double.NaN;
            double basePeakIntensity = double.NaN;
            int numberOfChannels = -1;
            int uniformTime = -1;
            double frequency = double.NaN;
            _rawConnection.GetScanHeaderInfoForScanNum(spectrumNumber, ref numberOfPackets, ref startTime, ref lowMass,
                                                       ref highMass,
                                                       ref totalIonCurrent, ref basePeakMass, ref basePeakIntensity,
                                                       ref numberOfChannels, ref uniformTime, ref frequency);

            return new MassRange(lowMass, highMass);
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            return (short)GetExtraValue(spectrumNumber, "Charge State:");
        }

        public override int GetSpectrumNumber(double retentionTime)
        {
            int spectrumNumber = 0;
            _rawConnection.ScanNumFromRT(retentionTime, ref spectrumNumber);
            return spectrumNumber;
        }

        public override double GetInjectionTime(int spectrumNumber)
        {
            object time = GetExtraValue(spectrumNumber, "Ion Injection Time (ms):");
            if (time == null)
                return double.NaN;
            return (float) time;
        }

        public override double GetResolution(int spectrumNumber)
        {
            double resolution = Convert.ToDouble(GetExtraValue(spectrumNumber, "FT Resolution:"));
            return resolution;
        }

        public double GetElapsedScanTime(int spectrumNumber)
        {
            double elapsedScanTime = Convert.ToDouble(GetExtraValue(spectrumNumber, "Elapsed Scan Time (sec):")) * 1000;
            return elapsedScanTime;
        }

        private Regex _etdReactTimeRegex = new Regex(@"@etd(\d+).(\d+)(\d+)", RegexOptions.Compiled);

        public double GetETDReactionTime(int spectrumNumber)
        {
            string scanheader = GetScanFilter(spectrumNumber);
            Match m = _etdReactTimeRegex.Match(scanheader);
            if (m.Success)
            {
                string etdTime = m.ToString();
                string Time = etdTime.Remove(0, 4);
                double reactTime = double.Parse(Time);
                return reactTime;
            }
            else
            {
                return double.NaN;
            }
        }

    }
}