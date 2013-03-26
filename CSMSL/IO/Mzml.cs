using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.IO
{
    public class Mzml : MSDataFile
    {

         public Mzml(string filePath, bool openImmediately = false)
            : base(filePath, MsDataFileType.Mzml, openImmediately) { }

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
