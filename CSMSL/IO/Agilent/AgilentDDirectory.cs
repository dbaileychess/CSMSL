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
using Agilent.MassSpectrometry.DataAnalysis;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CSMSL.IO.Agilent
{
    public class AgilentDDirectory : MSDataFile<MZSpectrum>
    {
        private IMsdrDataReader _msdr;

        public AgilentDDirectory(string directoryPath)
            : base(directoryPath, MSDataFileType.AgilentRawFile)
        {
        }

        public override void Open()
        {
            if (IsOpen)
                return;
            _msdr = new MassSpecDataReader();
            _msdr.OpenDataFile(FilePath);
            base.Open();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_msdr != null)
                { 
                    _msdr.CloseDataFile();
                    _msdr = null;
                }
            }
            base.Dispose(disposing);
        }

        protected override int GetFirstSpectrumNumber()
        {
            return 1;
        }

        protected override int GetLastSpectrumNumber()
        {
            return (int) (_msdr.MSScanFileInformation.TotalScansPresent);
            ;
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
            foreach (IBDAActualData actual in actuals)
            {
                if (actual.DisplayName == filter)
                {
                    return actual.DisplayValue;
                }
            }
            return null;
        }

        public override Polarity GetPolarity(int spectrumNumber)
        {
            IMSScanRecord scan_record = _msdr.GetScanRecord(spectrumNumber - 1);
            switch (scan_record.IonPolarity)
            {
                case IonPolarity.Positive:
                    return Polarity.Positive;
                case IonPolarity.Negative:
                    return Polarity.Negative;
                default:
                    return Polarity.Neutral;
            }
        }

        public override MZSpectrum GetSpectrum(int spectrumNumber)
        {
            IBDASpecData spectrum = _msdr.GetSpectrum(spectrumNumber - 1);

            double[] doubleArray = new double[spectrum.YArray.Length];
            for (int i = 0; i < doubleArray.Length; i++)
            {
                doubleArray[i] = spectrum.YArray[i];
            }

            return new MZSpectrum(spectrum.XArray, doubleArray);
        }

        public override MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            IBDASpecData spectrum = _msdr.GetSpectrum(spectrumNumber - 1);
            switch (spectrum.DeviceType)
            {
                case DeviceType.IonTrap:
                    return MZAnalyzerType.IonTrap3D;
                case DeviceType.Quadrupole:
                case DeviceType.TandemQuadrupole:
                    return MZAnalyzerType.Quadrupole;
                case DeviceType.QuadrupoleTimeOfFlight:
                case DeviceType.TimeOfFlight:
                    return MZAnalyzerType.TOF;
                default:
                    return MZAnalyzerType.Unknown;
            }
        }

        public override double GetPrecursorMz(int spectrumNumber, int msnOrder = 2)
        {
            IMSScanRecord scan_record = _msdr.GetScanRecord(spectrumNumber - 1);
            return scan_record.MZOfInterest;
        }

        private static Regex ISOLATION_WIDTH_REGEX;

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            string acquisition_method;
            using (StreamReader acquisition_method_sr = new StreamReader(Path.Combine(_msdr.FileInformation.DataFileName, @"/AcqData/AcqMethod.xml")))
            {
                acquisition_method = acquisition_method_sr.ReadToEnd();
            }
            if (ISOLATION_WIDTH_REGEX == null)
            {
                ISOLATION_WIDTH_REGEX = new Regex(@"\s*(?:&lt;|<)ID(?:&gt;|>)TargetIsolationWidth(?:&lt;|<)/ID(?:&gt;|>)\s*(?:&lt;|<)Value(?:&gt;|>).*\(~([0-9.])+ amu\)(?:&lt;|<)/Value(?:&gt;|>)");
            }
            Match match = ISOLATION_WIDTH_REGEX.Match(acquisition_method);
            return double.Parse(match.Groups[1].Value);
        }

        public override DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            return DissociationType.CID;
        }

        public override MzRange GetMzRange(int spectrumNumber)
        {
            IBDASpecData spectrum = _msdr.GetSpectrum(spectrumNumber - 1);
            return new MzRange(spectrum.MeasuredMassRange.Start, spectrum.MeasuredMassRange.End);
        }

        public override int GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            IBDASpecData spectrum = _msdr.GetSpectrum(spectrumNumber - 1);
            int precursor_charge;
            spectrum.GetPrecursorCharge(out precursor_charge);
            return (short) precursor_charge;
        }

        public override int GetSpectrumNumber(double retentionTime)
        {
            IBDAChromData tic = _msdr.GetTIC();
            int index = -1;
            for (int i = 0; i < tic.TotalDataPoints; i++)
            {
                if (index < 0 || Math.Abs(tic.XArray[i] - retentionTime) < Math.Abs(tic.XArray[index] - retentionTime))
                {
                    index = i;
                }
            }
            return index + 1;
        }

        public override double GetInjectionTime(int spectrumNumber)
        {
            IMSScanRecord scan_record = _msdr.GetScanRecord(spectrumNumber - 1);
            int num_transients = 0;
            double length_transient = double.NaN;
            IBDAActualData[] actuals = _msdr.ActualsInformation.GetActualCollection(scan_record.RetentionTime);
            foreach (IBDAActualData actual in actuals)
            {
                if (actual.DisplayName == "Number of Transients")
                {
                    num_transients = int.Parse(actual.DisplayValue);
                }
                else if (actual.DisplayName == "Length of Transients")
                {
                    length_transient = double.Parse(actual.DisplayValue);
                }
            }
            return num_transients*length_transient; // may be off by a factor of two for extended dynamic range mode
        }

        public override double GetResolution(int spectrumNumber)
        {
            return double.NaN;
        }
    }
}