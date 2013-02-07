using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDAL;

namespace CSMSL.IO.Bruker
{
    public class BrukerDDirectory : MsDataFile
    {       
        IMSAnalysis2 analysis;

        public BrukerDDirectory(string directoryPath, bool openImmediately = false) 
            : base(directoryPath, MsDataFileType.BrukerRawFile, openImmediately) {}
    
        public override void Open()
        {
            if(!IsOpen)
            {   
                analysis = (IMSAnalysis2)new MSAnalysis();
                analysis.Open(FilePath);
                base.Open();
            }
        }

        public override void Dispose()
        {
            if(analysis != null)
            {                              
                analysis = null;
            }
            base.Dispose();
        }

        public override Proteomics.DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override int GetMsnOrder(int spectrumNumber)
        {             
            return analysis.MSSpectrumCollection[spectrumNumber].MSMSStage;
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            throw new NotImplementedException();
        }

        public override Range GetMzRange(int spectrumNumber)
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

        public override Spectral.MzAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            throw new NotImplementedException();
        }

        public override Spectral.Spectrum GetMzSpectrum(int spectrumNumber)
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
