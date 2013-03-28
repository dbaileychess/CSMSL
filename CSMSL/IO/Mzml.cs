using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSMSL.Spectral;
using CSMSL.Proteomics;
using System.Xml.Serialization;
using System.Xml;
using zlib;

namespace CSMSL.IO
{

    public class Mzml : MSDataFile
    {
        private static string _msnOrderAccession = "MS:1000511";
        private static string _precursorCharge = "MS:1000041";
        private static string _lowestObservedMass = "MS:1000501";
        private static string _highestObservedMass = "MS:1000500";
        private static string _precursorMass = "MS:1000744";
        private static string _isolationWindowLowerOffset = "MS:1000828";
        private static string _isolationWindowUpperOffset = "MS:1000829";
        private static string _retentionTime = "MS:1000016";
        private static string _ionInjectionTime = "MS:1000927";
        private static string _mzArray = "MS:1000514";
        private static string _intensityArray = "MS:1000515";
        private const string _CID = "MS:1001880";
        private const string _HCD = "MS:1000422";
        private const string _ETD = "MS:1000598";
        private const string _MPD = "MS:1000435";
        private const string _ECD = "MS:1000250";
        private const string _PQD = "MS:1000599";
        private const string _quadrupole = "MS:1000081";
        private const string _linearIonTrap = "MS:1000291";
        private const string _IonTrap2DAxialEject = "MS:1000078";
        private const string _IonTrap2DRadialEject = "MS:1000083";
        private const string _IonTrap3D = "MS:1000082";
        private const string _orbitrap = "MS:1000484";
        private const string _TOF = "MS:1000084";
        private const string _FTICR = "MS:1000079";
        private const string _magneticSector = "MS:1000080";

        private static XmlSerializer _serializer = new XmlSerializer(typeof(indexedmzML));

        private indexedmzML _mzMLConnection;
        
        public Mzml(string filePath, bool openImmediately = false)
            : base(filePath, MsDataFileType.Mzml, openImmediately) { }

        public override void Open()
        {
            if (!IsOpen ||_mzMLConnection == null)
            {
                indexedmzML mzMLConnection = _serializer.Deserialize(new FileStream(FilePath, FileMode.Open)) as indexedmzML;
                _mzMLConnection = mzMLConnection;
                base.Open();
            }
        }

        public override void Dispose()
        {
            if (_mzMLConnection != null)
            {
                _mzMLConnection = null;
            }
            base.Dispose();
        }        

        public override Proteomics.DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].precursorList.precursor[0].activation.cvParam)
            {
                string accessor = cv.accession;
                switch(accessor)
                {
                    case _CID:
                        return DissociationType.CI; 
                    case _HCD:
                        return DissociationType.HCD;
                    case _ETD:
                        return DissociationType.ETD;
                    case _MPD:
                        return DissociationType.MPD;
                    case _PQD:
                        return DissociationType.PQD;
                    default:
                        return DissociationType.UnKnown;
                }
            }
            throw new ArgumentNullException("Could not find dissociation type");
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].cvParam)
            {             
                if (cv.accession.Equals(_msnOrderAccession))
                {
                    return int.Parse(cv.value);
                }                
            }
            throw new ArgumentNullException("Could not find MSn level for spectrum number " + spectrumNumber);
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam)
            {
                if(cv.accession.Equals(_precursorCharge))
                {
                    return short.Parse(cv.value);
                }
            }
            throw new ArgumentNullException("Couldn't find precursor charge in spectrum number " + spectrumNumber);
        }

        public override MassRange GetMzRange(int spectrumNumber)
        {
            double HighMass = 0.0;
            double LowMass = 0.0;
          

            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].scanList.scan[0].scanWindowList.scanWindow[0].cvParam)
            { 
                if(cv.accession.Equals(_lowestObservedMass))
                {
                    LowMass = double.Parse(cv.value);
                }
                 if(cv.accession.Equals(_highestObservedMass))
                {
                    HighMass = double.Parse(cv.value);
                }
                
            }
            MassRange massRange = new MassRange(LowMass, HighMass);
            return massRange;

            throw new ArgumentNullException("Could not determine mass range for " + spectrumNumber);
        }

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam)
            {
                if (cv.accession.Equals(_precursorMass))
                {
                    return double.Parse(cv.value);
                }

            }
            throw new ArgumentNullException("Could not determine precursor mass for " + spectrumNumber);
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            double isolationWindowLowerOffset = 0.0;
            double isolationWindowUpperOffset = 0.0;
            double isolationWidth = 0.0;
            
            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].precursorList.precursor[0].isolationWindow.cvParam)
            {
                if (cv.accession.Equals(_isolationWindowLowerOffset))
                {
                    isolationWindowLowerOffset = double.Parse(cv.value);
                }
                if (cv.accession.Equals(_isolationWindowUpperOffset))
                {
                    isolationWindowUpperOffset = double.Parse(cv.value);
                }
                return isolationWidth = isolationWindowUpperOffset - isolationWindowLowerOffset;
            }
            throw new ArgumentNullException("Could not determine isolation width for " + spectrumNumber);
        }

        public override Spectral.MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            //currently gets the first analyzer used.
           string analyzer = _mzMLConnection.mzML.instrumentConfigurationList.instrumentConfiguration[0].componentList.analyzer[0].cvParam[0].accession;
  
            switch(analyzer){
               case _quadrupole:
                   return MZAnalyzerType.Quadrupole;
               case _linearIonTrap:
                   return MZAnalyzerType.IonTrap2D;
               case _IonTrap3D:
                   return MZAnalyzerType.IonTrap3D;
               case _orbitrap:
                   return MZAnalyzerType.Orbitrap;
               case _TOF:
                   return MZAnalyzerType.TOF;
               case _FTICR:
                   return MZAnalyzerType.FTICR;
               case _magneticSector:
                   return MZAnalyzerType.Sector;
               default:
                    return MZAnalyzerType.Unknown;
            }
        }

        public override Spectral.MassSpectrum GetMzSpectrum(int spectrumNumber)
        {
           int mzArrayIndex=0;
           int intensityIndex=0;
            for (int i = 0; i < 2; i++)
            {
                foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].binaryDataArrayList.binaryDataArray[i].cvParam)
                {
                    if (cv.accession.Equals(_mzArray))
                    {
                        mzArrayIndex = i;
                    }
                    if (cv.accession.Equals(_intensityArray))
                    {
                        intensityIndex = i;
                    }
                }
            }
            byte[] massBytes = _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].binaryDataArrayList.binaryDataArray[mzArrayIndex].binary;
            byte[] intensityBytes = _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].binaryDataArrayList.binaryDataArray[intensityIndex].binary;
            double[] masses = ConvertBase64ToDoubles(massBytes);
            double[] intensities = ConvertBase64ToDoubles(intensityBytes);
            MassSpectrum massSpectrum = new MassSpectrum(masses, intensities);
            return massSpectrum;
        }

        public override Spectral.Polarity GetPolarity(int spectrumNumber)
        {
            //polarity not carried over by pwiz
            return Polarity.Neutral;
        }

        public override double GetRetentionTime(int spectrumNumber)
        {
            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].scanList.scan[0].cvParam)
            {
                if(cv.accession.Equals(_retentionTime))
                {
                    return double.Parse(cv.value); 
                }
            }
            
            throw new ArgumentNullException("Could not determine retention time for " + spectrumNumber);
        }

        public override double GetInjectionTime(int spectrumNumber)
        {
            foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[spectrumNumber-1].scanList.scan[0].cvParam)
            {
                if (cv.accession.Equals(_ionInjectionTime))
                {
              
                    return double.Parse(cv.value);
                }
            }
            throw new ArgumentNullException("Could not determine injection time for " + spectrumNumber);
        }

        public override double GetResolution(int spectrumNumber)
        {
            return 0.0;
        }

        protected override int GetFirstSpectrumNumber()
        {
            return 1;

        }

        protected override int GetLastSpectrumNumber()
        {
            return _mzMLConnection.mzML.run.spectrumList.spectrum.Length;
           
        }

        public override int GetSpectrumNumber(double retentionTime)
        {
            int totalSpectra = _mzMLConnection.mzML.run.spectrumList.spectrum.Length;
            for (int i = 0; i < totalSpectra; i++)
            {
                foreach (CVParamType cv in _mzMLConnection.mzML.run.spectrumList.spectrum[i].scanList.scan[0].cvParam)
                {
                   if(cv.accession.Equals(_retentionTime))
                    {
                       if(double.Parse(cv.value) == retentionTime)
                       {
                           return i+1;
                       }
                    }
                }
            }
            throw new ArgumentNullException("Could not determine spectrum number");
        }
                
        private static double[] ConvertBase64ToDoubles(byte[] bytes, bool zlibCompressed = false)
        {
            if (zlibCompressed)
            {
                bytes = DecompressData(bytes);
            }

            double[] convertedArray = new double[bytes.Length / 8];

            for (int i = convertedArray.GetLowerBound(0); i <= convertedArray.GetUpperBound(0); i++)
            {
                convertedArray[i] = BitConverter.ToDouble(bytes, i * 8);
            }
            return convertedArray;
        }

        private static byte[] DecompressData(byte[] inData)
        {         
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                return outMemoryStream.ToArray();
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

    }
}
