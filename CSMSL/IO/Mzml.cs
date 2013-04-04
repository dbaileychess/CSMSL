using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CSMSL.Spectral;
using CSMSL.Proteomics;
using System.Xml.Serialization;
using System.Xml;
using Ionic.Zlib;

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
        private const string _nozlibCompress = "MS:1000576";
        private const string _zlibCompression = "MS:1000574";

        private static XmlSerializer _indexedSerializer = new XmlSerializer(typeof(indexedmzML));
        private static XmlSerializer _mzMLSerializer = new XmlSerializer(typeof(mzMLType));

        private indexedmzML _indexedmzMLConnection;
        private mzMLType _mzMLConnection;
        
        public Mzml(string filePath, bool openImmediately = false)
            : base(filePath, MSDataFileType.Mzml, openImmediately) { }

        public override void Open()
        {
            if (!IsOpen ||_mzMLConnection == null)
            {
                Stream stream = new FileStream(FilePath, FileMode.Open);
                // Need these nested try catch to test if the file is a indexed mzML or just a plain mzML
                try
                {
                    _indexedmzMLConnection = _indexedSerializer.Deserialize(stream) as indexedmzML;
                    _mzMLConnection = _indexedmzMLConnection.mzML;
                }
                catch (Exception e)
                {
                    try
                    {
                        _mzMLConnection = _mzMLSerializer.Deserialize(stream) as mzMLType;
                    }
                    catch (Exception e2)
                    {
                        throw new InvalidDataException("Unable to parse " + FilePath + " as a mzML file!");
                    }
                }
                base.Open();
            }
        }

        public bool IsIndexedMzML
        {
            get { return _indexedmzMLConnection != null; }
        }

        public override void Dispose()
        {
            if (_mzMLConnection != null)
            {
                _mzMLConnection = null;
            }
            if (_indexedmzMLConnection != null)
            {
                _indexedmzMLConnection = null;
            }
            base.Dispose();
        }        

        public override DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            spectrumNumber--;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].precursorList.precursor[0].activation.cvParam)
            {               
                switch(cv.accession)
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
            throw new ArgumentNullException("Could not find dissociation type for spectrum number " + spectrumNumber + 1);
        }

        public override int GetMsnOrder(int spectrumNumber)
        {
            spectrumNumber--;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].cvParam)
            {             
                if (cv.accession.Equals(_msnOrderAccession))
                {
                    return int.Parse(cv.value);
                }                
            }
            throw new ArgumentNullException("Could not find MSn level for spectrum number " + spectrumNumber + 1);
        }

        public override short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            spectrumNumber--;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam)
            {
                if(cv.accession.Equals(_precursorCharge))
                {
                    return short.Parse(cv.value);
                }
            }
            throw new ArgumentNullException("Couldn't find precursor charge in spectrum number " + spectrumNumber + 1);
        }

        public override MassRange GetMzRange(int spectrumNumber)
        {
            spectrumNumber--;
            double high = double.NaN;
            double low = double.NaN;          

            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].scanList.scan[0].scanWindowList.scanWindow[0].cvParam)
            { 
                if(cv.accession.Equals(_lowestObservedMass))
                {
                    low = double.Parse(cv.value);
                }
                if(cv.accession.Equals(_highestObservedMass))
                {
                    high = double.Parse(cv.value);
                }                
            }
            if (double.IsNaN(low) || double.IsNaN(high))
            {
                throw new ArgumentNullException("Could not determine isolation width for " + spectrumNumber + 1);
            }            
            return new MassRange(low, high);        
        }

        public override double GetPrecusorMz(int spectrumNumber, int msnOrder = 2)
        {
            spectrumNumber--;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam)
            {
                if (cv.accession.Equals(_precursorMass))
                {
                    return double.Parse(cv.value);
                }
            }
            throw new ArgumentNullException("Could not determine precursor mass for " + spectrumNumber + 1);
        }

        public override double GetIsolationWidth(int spectrumNumber, int msnOrder = 2)
        {
            spectrumNumber--;
            double low = double.NaN;
            double high = double.NaN;
                      
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].precursorList.precursor[0].isolationWindow.cvParam)
            {
                if (cv.accession.Equals(_isolationWindowLowerOffset))
                {
                    low = double.Parse(cv.value);
                }
                if (cv.accession.Equals(_isolationWindowUpperOffset))
                {
                    high = double.Parse(cv.value);
                }               
            }
            if (double.IsNaN(low) || double.IsNaN(high))
            {
                throw new ArgumentNullException("Could not determine isolation width for " + spectrumNumber + 1);
            }
            return high - low;            
        }

        public override Spectral.MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            // TODO need to do this on a spectrum-by-spectrum basis.
            //currently gets the first analyzer used.          
           string analyzer = _mzMLConnection.instrumentConfigurationList.instrumentConfiguration[0].componentList.analyzer[0].cvParam[0].accession;
  
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
            spectrumNumber--; // 0-based indexing
        
            double[] masses = null;
            double[] intensities = null;

            foreach (BinaryDataArrayType binaryData in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].binaryDataArrayList.binaryDataArray)
            {
                bool compressed = false;
                bool mzArray = false;
                bool intensityArray = false;
                foreach (CVParamType cv in binaryData.cvParam)
                {
                    if (cv.accession.Equals(_zlibCompression))
                    {
                        compressed = true;
                    }
                    if (cv.accession.Equals(_mzArray))
                    {
                        mzArray = true;
                    }
                    if (cv.accession.Equals(_intensityArray))
                    {
                        intensityArray = true;
                    }
                }

                double[] data = ConvertBase64ToDoubles(binaryData.binary, compressed);
                if (mzArray)
                {
                    masses = data;
                }

                if (intensityArray)
                {
                    intensities = data;
                }
            }

            if (masses == null || intensities == null)
            {
                throw new InvalidDataException("Unable to find spectral data for spectrum number " + spectrumNumber + 1);
            }           
           
            return new MassSpectrum(masses, intensities);
        }

        public override Spectral.Polarity GetPolarity(int spectrumNumber)
        {
            //TODO add polarity checking
            return Polarity.Neutral;
        }

        public override double GetRetentionTime(int spectrumNumber)
        {
            spectrumNumber--;
            if (_mzMLConnection.run.spectrumList.spectrum[spectrumNumber].scanList.scan[0].cvParam == null)
            {
                return double.NaN;
            }
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].scanList.scan[0].cvParam)
            {
                if(cv.accession.Equals(_retentionTime))
                {
                    return double.Parse(cv.value); 
                }
            }            
            throw new ArgumentNullException("Could not determine retention time for " + spectrumNumber + 1);
        }

        public override double GetInjectionTime(int spectrumNumber)
        {
            spectrumNumber--;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].scanList.scan[0].cvParam)
            {
                if (cv.accession.Equals(_ionInjectionTime))
                {              
                    return double.Parse(cv.value);
                }
            }
            throw new ArgumentNullException("Could not determine injection time for " + spectrumNumber + 1);
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
            return _mzMLConnection.run.spectrumList.spectrum.Length;           
        }

        public override int GetSpectrumNumber(double retentionTime)
        {
            // TODO need to convert this to a binary search of some sort. Or if the data is indexedMZML see if the indices work better.
            int totalSpectra = _mzMLConnection.run.spectrumList.spectrum.Length;
            for (int i = 0; i < totalSpectra; i++)
            {
                foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[i].scanList.scan[0].cvParam)
                {
                    if (cv.accession.Equals(_retentionTime))
                    {
                        if (double.Parse(cv.value) == retentionTime)
                        {
                            return i + 1;
                        }
                    }
                }
            }
            throw new ArgumentNullException("Could not determine spectrum number");
        }
                
        /// <summary>
        /// Converts a 64-based encoded byte array into an double[]
        /// </summary>
        /// <param name="bytes">the 64-bit encoded byte array</param>
        /// <param name="zlibCompressed">Specifies if the byte array is zlib compressed</param>
        /// <returns>a decompressed, de-encoded double[]</returns>
        private static double[] ConvertBase64ToDoubles(byte[] bytes, bool zlibCompressed = false)
        {
            if (zlibCompressed)
            {
                bytes = ZlibStream.UncompressBuffer(bytes); 
            }      
         
            int length = bytes.Length / 8;
            double[] convertedArray = new double[length];

            for (int i = 0; i < length; i++)
            {
                convertedArray[i] = BitConverter.ToDouble(bytes, i * 8);
            }
            return convertedArray;
        }

    }
}
