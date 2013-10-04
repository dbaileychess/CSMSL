using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clearcore2.Data.AnalystDataProvider;
using Clearcore2.Data.DataAccess.SampleData;
using CSMSL.IO;
using CSMSL.Proteomics;
using CSMSL.Spectral;

namespace CSMSL.IO.ABSciex
{
    public class WiffFile : MSDataFile
    {
        public static string[] LicenseKeys
        {
            get
            {
                return Clearcore2.Licensing.LicenseKeys.Keys;
            }
        }

        public static void AddLicense(string key)
        {
            string[] keys = LicenseKeys;
            string[] newKeys = new string[1];
            int length = 1;
            if (keys != null)
            {
                length = keys.Length + 1;
                newKeys = new string[length];
                Array.Copy(keys, newKeys, length - 1);
            }
            newKeys[length - 1] = key;
            Clearcore2.Licensing.LicenseKeys.Keys = newKeys;
        }

        private AnalystWiffDataProvider _dataProvider;
        private Batch _wiffFile;
        private Sample _activeSample;
        private MSExperiment _activeMSExperiment;

        public WiffFile(string filePath)
            : base(filePath, MSDataFileType.WiffRawFile)
        {
            
        }

        public override void Open()
        {
            if (IsOpen)
                return;
            _dataProvider = new AnalystWiffDataProvider();
            _wiffFile = AnalystDataProviderFactory.CreateBatch(FilePath, _dataProvider);
            base.Open();
        }

        public string[] GetSampleNames()
        {
            return _wiffFile.GetSampleNames();
        }

        public void SetActiveSample(string sampleName)
        {
            string[] allSamples = _wiffFile.GetSampleNames();
            for (int i = 0; i < allSamples.Length; i++)
            {
                if (allSamples[i].Equals(sampleName))
                {
                    SetActiveSample(i);
                    return;
                }
            }
            throw new ArgumentException("Sample "+sampleName+" could not be found in this file");
        }

        public void SetActiveSample(int sampleIndex)
        {
            _activeSample = _wiffFile.GetSample(sampleIndex);
            _activeMSExperiment = _activeSample.MassSpectrometerSample.GetMSExperiment(0);
        }
        
        public override DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            var spec = _activeMSExperiment.GetMassSpectrumInfo(spectrumNumber);
            return spec.MSLevel;
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            var spec = _activeMSExperiment.GetMassSpectrumInfo(spectrumNumber);
            return (short)spec.ParentChargeState;
        }

        public override MassRange GetMzRange(int spectrumNumber)
        {
            var details = _activeMSExperiment.Details;
            return new MassRange(details.StartMass, details.EndMass);
        }

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override MassSpectrum GetMzSpectrum(int spectrumNumber)
        {
            var spectrum = _activeMSExperiment.GetMassSpectrum(spectrumNumber);
            double[] x = spectrum.GetActualXValues();
            double[] y = spectrum.GetActualYValues();
            double conversion = spectrum.GetCountsConversionFactor();
            var peaks = _activeMSExperiment.GetPeakArray(spectrumNumber);
        
            return null;
        }

        public override Polarity GetPolarity(int spectrumNumber)
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

        public override int GetSpectrumNumber(double retentionTime)
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
    }
}
