// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Linq;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using MSFileReaderLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CSMSL.IO.Thermo
{
    public class ThermoRawFile : MSDataFile<ThermoSpectrum>
    {
        internal enum RawLabelDataColumn
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

        public enum IntensityCutoffType
        {
            None = 0,
            Absolute = 1,
            Relative = 2
        };

        private IXRawfile5 _rawConnection;

        public ThermoRawFile(string filePath)
            : base(filePath, MSDataFileType.ThermoRawFile)
        {
        }

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

            _rawConnection = (IXRawfile5) new MSFileReader_XRawfile();
            _rawConnection.Open(FilePath);
            _rawConnection.SetCurrentController(0, 1); // first 0 is for mass spectrometer

            base.Open();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_rawConnection != null)
                {
                    _rawConnection.Close();
                    _rawConnection = null;
                }
            }
            base.Dispose(disposing);
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
            object flags = null;
            int size = 0;
            string mzrange = range.Minimum.ToString("F4") + "-" + range.Maximum.ToString("F4");
            _rawConnection.GetChroData(0, 0, 0, scanFilter, mzrange, string.Empty, 0.0, startTime, endTime, (int) smoothing, smoothingPoints, ref chro, ref flags, ref size);
            return (double[,]) chro;
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

        public override ThermoSpectrum GetSpectrum(int spectrumNumber)
        {
            return GetSpectrum(spectrumNumber);
        }

        public ThermoSpectrum GetSpectrum(int spectrumNumber, bool profileIfAvailable = false)
        {
            bool useProfile = false;

            if (profileIfAvailable)
            {
                int isProfile = 0;
                _rawConnection.IsProfileScanForScanNum(spectrumNumber, ref isProfile);
                useProfile = isProfile == 1;
            }
            return new ThermoSpectrum(GetLabeledData(spectrumNumber) ?? GetUnlabeledData(spectrumNumber, !useProfile));
        } 

        public MZSpectrum GetAveragedSpectrum(int firstSpectrumNumber, int lastSpectrumNumber, string scanFilter = "", IntensityCutoffType type = IntensityCutoffType.None, int intensityCutoff = 0)
        {
            object labels = null;
            object flags = null;
            double peakWidth = 0;
            int arraySize = 0;
            int c, d, e, f;
            c = d = e = f = 0;
            _rawConnection.GetAverageMassList(ref firstSpectrumNumber, ref lastSpectrumNumber, ref c, ref d, ref e, ref f, scanFilter, (int) type, intensityCutoff, 0, 0, ref peakWidth, ref labels, ref flags, ref arraySize);
            double[,] spectrum = (double[,]) labels;
            return new MZSpectrum(spectrum, arraySize);
        }

        public ThermoSpectrum GetLabeledSpectrum(int spectrumNumber)
        {
            var labelData = GetLabeledData(spectrumNumber);
            return new ThermoSpectrum(labelData);
        }

        public MZSpectrum GetSNSpectrum(int spectrumNumber, double minSN = 3)
        {
            var labelData = GetLabeledData(spectrumNumber);
            int count = labelData.GetLength(1);
            double[] mz = new double[count];
            double[] sns = new double[count];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                double sn = labelData[1, i]/labelData[4, i];
                if (sn >= minSN)
                {
                    mz[j] = labelData[0, i];
                    sns[j] = sn;
                    j++;
                }
            }
            Array.Resize(ref mz, j);
            Array.Resize(ref sns, j);
            return new MZSpectrum(mz, sns, false);
        }

        private double[,] GetUnlabeledData(int spectrumNumber, bool useCentroid)
        {
            object massList = null;
            object peakFlags = null;
            int arraySize = -1;
            double centroidPeakWidth = 0.001;
            _rawConnection.GetMassListFromScanNum(ref spectrumNumber, null, 0, 0, 0, Convert.ToInt32(useCentroid), ref centroidPeakWidth, ref massList, ref peakFlags, ref arraySize);
            return (double[,]) massList;
        }

        private double[,] GetLabeledData(int spectrumNumber)
        {
            object labels = null;
            object flags = null;
            _rawConnection.GetLabelData(ref labels, ref flags, ref spectrumNumber);
            double[,] data = labels as double[,];
            return data == null || data.Length == 0 ? null : data;
        }

        public override MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            int mzanalyzer = 0;
            _rawConnection.GetMassAnalyzerTypeForScanNum(spectrumNumber, ref mzanalyzer);

            switch ((ThermoMzAnalyzer) mzanalyzer)
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

        public override double GetPrecursorMz(int spectrumNumber, int msnOrder = 2)
        {
            double mz = double.NaN;
            _rawConnection.GetPrecursorMassForScanNum(spectrumNumber, msnOrder, ref mz);
            return mz;
        }

        public double GetPrecusorMz(int spectrumNumber, double searchMZ, int msnOrder = 2)
        {
            int parentScanNumber = GetParentSpectrumNumber(spectrumNumber);
            var ms1Scan = GetSpectrum(parentScanNumber);
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
            return (DissociationType) type;
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

        public override int GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            short charge = Convert.ToInt16(GetExtraValue(spectrumNumber, "Charge State:"));
            return charge*(int) GetPolarity(spectrumNumber);
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
            int arraySize = 0;
            object keys = null;
            object values = null;
            _rawConnection.GetTrailerExtraForScanNum(spectrumNumber, ref keys, ref values, ref arraySize);

            MZAnalyzerType analyzer = GetMzAnalyzer(spectrumNumber);
            double resolution = 0;
            switch (analyzer)
            {
                case MZAnalyzerType.FTICR:
                case MZAnalyzerType.Orbitrap:
                    string name = GetInstrumentName();
                    if (name == "Orbitrap Fusion" || name == "Q Exactive")
                    {
                        object obj = GetExtraValue(spectrumNumber, "Orbitrap Resolution:");
                        resolution = Convert.ToDouble(obj);
                        if (resolution <= 0)
                        {
                            // Find first peak with S/N greater than 3 to use for resolution calculation
                            double[,] data = GetLabeledData(spectrumNumber);
                            int totalPeaks = data.GetLength(1);
                            List<double> avgResolution = new List<double>();

                            for (int i = 0; i < totalPeaks; i++)
                            {
                                double signalToNoise = data[1, i]/data[4, i];
                                if (signalToNoise >= 5)
                                {
                                    double mz = data[0, i];
                                    double peakRes = data[2, i];
                                    double correctedResolution = peakRes*Math.Sqrt(mz/200);
                                    avgResolution.Add(correctedResolution);
                                }
                            }

                            double meanResolution = avgResolution.Median();
                            if (meanResolution <= 25000)
                            {
                                return 15000;
                            }
                            if (meanResolution <= 45000)
                            {
                                return 30000;
                            }
                            if (meanResolution <= 100000)
                            {
                                return 60000;
                            }
                            if (meanResolution <= 200000)
                            {
                                return 120000;
                            }
                            if (meanResolution <= 400000)
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
            }
            return resolution;
        }

        public double ResolutionDefinedAtMZ()
        {
            string name = GetInstrumentName();
            switch (name)
            {
                case "Orbitrap Fusion":
                case "Q Exactive":
                    return 200;
                case "LTQ Orbitrap XL":
                case "LTQ Orbitrap Velos":
                case "Orbitrap Elite":
                    return 400;
                default:
                    return double.NaN;
            }
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
            return double.NaN;
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

            double[,] pvarArray = (double[,]) pvarChroData;

            return new Chromatogram(pvarArray);
        }

        private readonly static Regex _msxRegex = new Regex(@"([\d.]+)@", RegexOptions.Compiled);

        public List<double> GetMSXPrecursors(int spectrumNumber)
        {
            string scanheader = GetScanFilter(spectrumNumber);
            
            int msxnumber = -1;
            _rawConnection.GetMSXMultiplexValueFromScanNum(spectrumNumber, ref msxnumber);

            var matches = _msxRegex.Matches(scanheader);

            return (from Match match in matches select double.Parse(match.Groups[1].Value)).ToList();
        }
    }
}