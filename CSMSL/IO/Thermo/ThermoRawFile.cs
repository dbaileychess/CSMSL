using CSMSL.Proteomics;
using CSMSL.Spectral;
using MSFileReaderLib;
using System;
using System.Collections.Generic;
using System.IO;
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

        public enum Smoothing
        {
            None = 0,
            Boxcar = 1,
            Gauusian = 2
        }
                
        private IXRawfile5 _rawConnection;

        public ThermoRawFile(string filePath)
            : base(filePath, MSDataFileType.ThermoRawFile) { }

        public static bool AlwaysGetUnlabeledData = false;

        /// <summary>
        /// Opens the connection to the underlying data
        /// </summary>
        public override void Open()
        {
            if (IsOpen && _rawConnection != null)
                return;

            if (!File.Exists(FilePath) && !Directory.Exists(FilePath))
            {
                throw new IOException(string.Format("The MS data file {0} does not currently exist", FilePath));
            }

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
            if (GetMsnOrder(spectrumNumber) == 1)
                return 0;

            object parentScanNumber = GetExtraValue(spectrumNumber, "Master Scan Number:");
            int scanNumber = Convert.ToInt32(parentScanNumber);

            if (scanNumber == 0)
            {
                int masterIndex = Convert.ToInt32(GetExtraValue(spectrumNumber, "Master Index:"));
                if (masterIndex == 0)
                    throw new ArgumentException("Scan Number " + spectrumNumber + " has no parent");
                int scanIndex = Convert.ToInt32(GetExtraValue(spectrumNumber, "Scan Event:"));
                scanNumber = spectrumNumber - scanIndex + masterIndex;
            }
           
            return scanNumber;           
        }

        public double[,] GetChro(string scanFilter, MzRange range, double startTime, double endTime, Smoothing smoothing = Smoothing.None, int smoothingPoints = 3)
        {
            object chro = null;
            object flags =null;
            int size = 0;
            string mzrange = range.Minimum.ToString("F4") + "-" + range.Maximum.ToString("F4");
            _rawConnection.GetChroData(0, 0, 0, scanFilter, mzrange, string.Empty, 0.0, startTime, endTime, (int)smoothing, smoothingPoints, ref chro, ref flags, ref size);
            return (double[,])chro;
        }

        private object GetExtraValue(int spectrumNumber, string filter)
        {
            object value = null;
            _rawConnection.GetTrailerExtraValueForScanNum(spectrumNumber, filter, ref value);
            return value;
        }

        public string GetScanFilter(int spectrumNumber)
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

        public override Spectrum GetMzSpectrum(int spectrumNumber)
        {
            return GetMzSpectrum(spectrumNumber, false);
        }

        [Obsolete]
        public Spectrum GetMzSpectrum(int spectrumNumber, bool profileIfAvailable = false)
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


            if (useProfile || mzAnalyzer == MZAnalyzerType.IonTrap2D || AlwaysGetUnlabeledData)
            {
                peakData = GetUnlabeledData(spectrumNumber, !useProfile);
                //count = peakData.GetLength(1);
                //List<MZPeak> lowResPeaks = new List<MZPeak>();
                //List<double>
                //for (int i = 0; i < count; i++)
                //{
                //    lowResPeaks.Add(new MZPeak(peakData[0, i], (float)peakData[1, i]));
                //}
                return new Spectrum(peakData);     
            }
            
            peakData = GetLabeledData(spectrumNumber);
            //count = peakData.GetLength(1);
            //List<ThermoLabeledPeak> peaks = new List<ThermoLabeledPeak>();
            //for (int i = 0; i < count; i++)
            //{
            //    peaks.Add(new ThermoLabeledPeak(peakData[0, i], (float)peakData[1, i], (short)peakData[5, i], (float)peakData[4, i]));
            //}
            return new ThermoSpectrum(peakData);    
        }

        public Spectrum GetSNSpectrum(int spectrumNumber, double minSN = 3)
        {
            var labelData = GetLabeledData(spectrumNumber);           
            int count = labelData.GetLength(1);
            double[] mz = new double[count];
            double[] sns = new double[count];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                double sn = labelData[1, i] / labelData[4, i];
                if (sn >= minSN)
                {
                    mz[j] = labelData[0, i];
                    sns[j] = sn;
                    j++;
                }
            }
            Array.Resize(ref mz, j);
            Array.Resize(ref sns, j);
            return new Spectrum(mz, sns, false);
        }

        public Spectrum GetReadOnlyMZSpectrum(int spectrumNumber, bool centorid = true, bool labeled = false)
        {           
            if (labeled)
            {
                var labelData = GetLabeledData(spectrumNumber);
                return new Spectrum(labelData);     
            }

            var peakData = GetUnlabeledData(spectrumNumber, centorid);
            return new Spectrum(peakData);     
        }

        public override string GetBase64Spectrum(int spectrumNumber, bool zlibCompressed = false)
        {     
            var peakData = GetUnlabeledData(spectrumNumber, false);
            int count = peakData.GetLength(1);
            int length = count * sizeof(double);
            byte[] bytes = new byte[length * 2];
            Buffer.BlockCopy(peakData, 0, bytes, 0, length);
            Buffer.BlockCopy(peakData, length, bytes, length, length);
            if (zlibCompressed)
            {
                bytes = bytes.Decompress();              
            }         
            return Convert.ToBase64String(bytes);  
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
            var ms1Scan = GetMzSpectrum(parentScanNumber);
            MZPeak peak = ms1Scan.GetClosestPeak(MassRange.FromDa(searchMZ, 50));
            if (peak != null)
                return peak.MZ;
            return double.NaN;
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            object width = GetExtraValue(spectrumNumber, string.Format("MS{0} Isolation Width:", msnOrder));
            return Convert.ToDouble(width);
        }

        //public double GetElapsedScanTime(int spectrumNumber)
        //{
        //    double elapsedScanTime = Convert.ToDouble(GetExtraValue(spectrumNumber, "Elapsed Scan Time (sec):")) * 1000;
        //    return elapsedScanTime;
        //}

        public double GetElapsedScanTime(int spectrumNumber)
        {
            object elapsedScanTime = GetExtraValue(spectrumNumber, "Elapsed Scan Time (sec):");
            return Convert.ToDouble(elapsedScanTime);
        }

        public double GetTIC(int spectrumNumber)
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

            return totalIonCurrent;

        }

        public override DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            int type = 0;
            _rawConnection.GetActivationTypeForScanNum(spectrumNumber, msnOrder, ref type);
            return (DissociationType)type;
        }

        public override MzRange GetMzRange(int spectrumNumber)
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

            return new MzRange(lowMass, highMass);
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            return Convert.ToInt16(GetExtraValue(spectrumNumber, "Charge State:"));
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
            return Convert.ToDouble(time);
        }

        public override double GetResolution(int spectrumNumber)
        {
            MZAnalyzerType analyzer = GetMzAnalyzer(spectrumNumber);
            double resolution = 0;
            switch (analyzer)
            {
                case MZAnalyzerType.FTICR:
                case MZAnalyzerType.Orbitrap:
                    string name = GetInstrumentName();
                    if (name == "Orbitrap Fusion" | name == "Q Exactive")
                    {
                        object obj = GetExtraValue(spectrumNumber, "Orbitrap Resolution:");
                        resolution = Convert.ToDouble(obj);
                        if (resolution <= 0)
                        {
                            // Find first peak with S/N greater than 3 to use for resolution calculation
                            double[,] data = GetLabeledData(spectrumNumber);
                            int totalPeaks = data.GetLength(0);
                            for (int i = 0; i < totalPeaks; i++)
                            {
                                double signalToNoise = data[1, i] / data[4, i];
                                if (signalToNoise >= 5)
                                {
                                    double mz = data[0, i];
                                    double peakRes = data[2, i];
                                    double correctedResolution = peakRes * Math.Sqrt(mz / 200);

                                    if (correctedResolution <= 15000)
                                    {
                                        return 15000;
                                    }
                                    if (correctedResolution <= 30000)
                                    {
                                        return 30000;
                                    }
                                    if (correctedResolution <= 60000)
                                    {
                                        return 60000;
                                    }
                                    if (correctedResolution <= 120000)
                                    {
                                        return 120000;
                                    }
                                    if (correctedResolution <= 240000)
                                    {
                                        return 240000;
                                    }
                                    return 450000;
                                }
                            }

                            double firstMZ = data[0, 0];
                            double firstPeakRes = data[2, 0];
                            double firstCorrectedResolution = firstPeakRes * Math.Sqrt(firstMZ / 200);

                            if (firstCorrectedResolution <= 15000)
                            {
                                return 15000;
                            }
                            if (firstCorrectedResolution <= 30000)
                            {
                                return 30000;
                            }
                            if (firstCorrectedResolution <= 60000)
                            {
                                return 60000;
                            }
                            if (firstCorrectedResolution <= 120000)
                            {
                                return 120000;
                            }
                            if (firstCorrectedResolution <= 240000)
                            {
                                return 240000;
                            }
                            return 450000;                       
                        }
                        return resolution;
                    }
                    else
                    {
                        object obj = GetExtraValue(spectrumNumber, "FT Resolution:");
                        resolution = Convert.ToDouble(obj);
                        if (resolution > 300000) return 480000;
                        return resolution;                      
                    }
                    break;
                default:
                    break;
            }
            return resolution;
        }

        public double ResolutionDefinedAtMZ()
        {
            MZAnalyzerType analyzer = GetMzAnalyzer(1); // Get analyzer name from first spectrum
            switch (analyzer)
            {
                case MZAnalyzerType.FTICR:
                case MZAnalyzerType.Orbitrap:
                    string name = GetInstrumentName();
                    if (name == "Orbitrap Fusion" | name == "Q Exactive")
                    {
                        return 200;
                    }
                    else
                    {
                        return 400;
                    }
                default:
                    break;
            }
            return double.NaN;
        }

        public string GetInstrumentName()
        {
            string name = null;
            _rawConnection.GetInstName(ref name);
            return name;
        }

        public string GetInstrumentModel()
        {
            string model = null;
            _rawConnection.GetInstModel(ref model);
            return model;
        }
        
        private static Regex _etdReactTimeRegex = new Regex(@"@etd(\d+).(\d+)(\d+)", RegexOptions.Compiled);

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


        public Chromatogram GetTICChroma()
        {
          
            int nChroType1 = 1; //1=TIC 0=MassRange
            int nChroOperator = 0;
            int nChroType2 = 0;
            string bstrFilter = null;
            string bstrMassRanges1 = null;
            string bstrMassRanges2 = null;
            double dDelay = 0.0;
            double dStartTime = 0.0;
            double dEndTime = 0.0;
            int nSmoothingType = 1; //0=None 1=Boxcar 2=Gaussian
            int nSmoothingValue = 7;
            object pvarChroData = null;
            object pvarPeakFlags = null;
            int pnArraySize = 0;

            //(int nChroType1, int nChroOperator, int nChroType2, string bstrFilter, string bstrMassRanges1, string bstrMassRanges2, double dDelay, ref double pdStartTime, 
            //ref double pdEndTime, int nSmoothingType, int nSmoothingValue, ref object pvarChroData, ref object pvarPeakFlags, ref int pnArraySize);
            _rawConnection.GetChroData(nChroType1, nChroOperator, nChroType2, bstrFilter, bstrMassRanges1, bstrMassRanges2, dDelay, dStartTime, dEndTime, nSmoothingType, nSmoothingValue, ref pvarChroData, ref pvarPeakFlags, ref pnArraySize);

            double[,] pvarArray = (double[,])pvarChroData;     

            return new Chromatogram(pvarArray);
        }


        public IEnumerable<double> GetMsxValues(int spectrumNumber)
        {
            int numberMSX = -1;
            _rawConnection.GetMSXMultiplexValueFromScanNum(spectrumNumber, ref numberMSX);
            object data =null ;
            _rawConnection.GetFullMSOrderPrecursorDataFromScanNum(spectrumNumber, 2, ref data);
            for (int i = 0; i < numberMSX; i++)
            {
                //TODO
            } 
            yield break;
        }
    }
}