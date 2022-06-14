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

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDAL;

namespace CSMSL.IO.Bruker
{
    public class BrukerDDirectory : MSDataFile
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
            object mode = analysis.MSSpectrumCollection[spectrumNumber].MSSpectrumParameterCollection["Fragmentation Mode"].ParameterValue;
            switch ((string)mode)
            {
                case "CID":
                    return Proteomics.DissociationType.CID;
                default:
                    return Proteomics.DissociationType.UnKnown;
            }         
        }

        public override int GetMsnOrder(int spectrumNumber)
        {             
            return analysis.MSSpectrumCollection[spectrumNumber].MSMSStage;
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            IMSSpectrumParameter parameter = analysis.MSSpectrumCollection[spectrumNumber].MSSpectrumParameterCollection["Precursor Charge State"];
            return (short)parameter.ParameterValue;            
        }

        public override MassRange GetMzRange(int spectrumNumber)
        {
            IMSSpectrum spectrum = analysis.MSSpectrumCollection[spectrumNumber];         
            double firstMass = (double)spectrum.MSSpectrumParameterCollection["Set Target Mass Start"].ParameterValue;
            double lastMass = (double)spectrum.MSSpectrumParameterCollection["Set Target Mass End"].ParameterValue;
            return new MassRange(firstMass, lastMass);
        }

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            return (double)analysis.MSSpectrumCollection[spectrumNumber].MSSpectrumParameterCollection["Isolation mass"].ParameterValue;    
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            object pMasses;
            Array pIsolationModi = null;
            analysis.MSSpectrumCollection[spectrumNumber].GetIsolationData(out pMasses, out pIsolationModi);
            return 0;
        }

        public override Spectral.MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            return Spectral.MZAnalyzerType.TOF;
        }

        public override Spectral.MassSpectrum GetMzSpectrum(int spectrumNumber)
        {
            IMSSpectrum2 spectra = (IMSSpectrum2)analysis.MSSpectrumCollection[spectrumNumber];
            if (spectra != null)
            {
                if (spectra.HasSpecType(SpectrumTypes.SpectrumType_Line))
                {
                    object masses, intensities;
                    spectra.GetMassIntensityValues(SpectrumTypes.SpectrumType_Line, out masses, out intensities);
                    double[] massesLine = (double[])masses;
                    double[] intensitiesLine = (double[])intensities;
                    Spectral.MassSpectrum spectrum = new Spectral.MassSpectrum(massesLine, intensitiesLine);
                    return spectrum;
                }
            }           
            throw new NotImplementedException();            
        }

        public override Spectral.Polarity GetPolarity(int spectrumNumber)
        {
            switch (analysis.MSSpectrumCollection[spectrumNumber].Polarity)
            {
                case SpectrumPolarity.IonPolarity_Negative:
                    return Spectral.Polarity.Negative;          
                case SpectrumPolarity.IonPolarity_Positive:
                    return Spectral.Polarity.Positive;
                default:
                case SpectrumPolarity.IonPolarity_Unknown:
                    return Spectral.Polarity.Neutral;                   
            }            
        }

        public override double GetRetentionTime(int spectrumNumber)
        {
            return analysis.MSSpectrumCollection[spectrumNumber].RetentionTime;
        }

        public override double GetInjectionTime(int spectrumNumber)
        {
            return double.NaN;
            //return analysis.MSSpectrumCollection[spectrumNumber].MSSpectrumParameterCollection;
            throw new NotImplementedException();
        }

        protected override int GetFirstSpectrumNumber()
        {
            return 1;          
        }

        protected override int GetLastSpectrumNumber()
        {
            return analysis.MSSpectrumCollection.Count;      
        }

        public override int GetSpectrumNumber(double retentionTime)
        {
            // Probably have to do a binary search
             throw new NotImplementedException();        
        }
    }
}
