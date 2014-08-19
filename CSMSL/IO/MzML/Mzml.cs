// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Mzml.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using CSMSL.Proteomics;
using CSMSL.Spectral;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CSMSL.IO.MzML
{
    public class Mzml : MSDataFile<MZSpectrum>
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
        private const string _64bit = "MS:1000523";
        private const string _32bit = "MS:1000521";

        private static XmlSerializer _indexedSerializer = new XmlSerializer(typeof (indexedmzML));
        private static XmlSerializer _mzMLSerializer = new XmlSerializer(typeof (mzMLType));

        private indexedmzML _indexedmzMLConnection;
        private mzMLType _mzMLConnection;

        public Mzml(string filePath)
            : base(filePath, MSDataFileType.Mzml)
        {
        }

        public override void Open()
        {
            if (!IsOpen || _mzMLConnection == null)
            {
                Stream stream = new FileStream(FilePath, FileMode.Open);
                try
                {
                    _indexedmzMLConnection = _indexedSerializer.Deserialize(stream) as indexedmzML;
                    _mzMLConnection = _indexedmzMLConnection.mzML;
                }
                catch (Exception e1)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mzMLConnection = null;
                _indexedmzMLConnection = null;
            }
            base.Dispose(disposing);
        }

        public override DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2)
        {
            spectrumNumber--;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].precursorList.precursor[0].activation.cvParam)
            {
                switch (cv.accession)
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

        public override int GetPrecusorCharge(int spectrumNumber, int msnOrder = 2)
        {
            spectrumNumber--;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam)
            {
                if (cv.accession.Equals(_precursorCharge))
                {
                    return short.Parse(cv.value);
                }
            }
            throw new ArgumentNullException("Couldn't find precursor charge in spectrum number " + spectrumNumber + 1);
        }

        public override MzRange GetMzRange(int spectrumNumber)
        {
            spectrumNumber--;
            double high = double.NaN;
            double low = double.NaN;

            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].scanList.scan[0].scanWindowList.scanWindow[0].cvParam)
            {
                if (cv.accession.Equals(_lowestObservedMass))
                {
                    low = double.Parse(cv.value);
                }
                if (cv.accession.Equals(_highestObservedMass))
                {
                    high = double.Parse(cv.value);
                }
            }
            if (double.IsNaN(low) || double.IsNaN(high))
            {
                throw new ArgumentNullException("Could not determine isolation width for " + spectrumNumber + 1);
            }
            return new MzRange(low, high);
        }

        public override double GetPrecursorMz(int spectrumNumber, int msnOrder = 2)
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

        public override MZAnalyzerType GetMzAnalyzer(int spectrumNumber)
        {
            // TODO need to do this on a spectrum-by-spectrum basis.
            //currently gets the first analyzer used.          
            string analyzer = _mzMLConnection.instrumentConfigurationList.instrumentConfiguration[0].componentList.analyzer[0].cvParam[0].accession;

            switch (analyzer)
            {
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

        public override MZSpectrum GetSpectrum(int spectrumNumber)
        {
            spectrumNumber--; // 0-based indexing

            double[] masses = null;
            double[] intensities = null;

            foreach (BinaryDataArrayType binaryData in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].binaryDataArrayList.binaryDataArray)
            {
                bool compressed = false;
                bool mzArray = false;
                bool intensityArray = false;
                bool bit32 = true;
                foreach (CVParamType cv in binaryData.cvParam)
                {
                    if (cv.accession.Equals(_zlibCompression))
                    {
                        compressed = true;
                    }
                    if (cv.accession.Equals(_64bit))
                    {
                        bit32 = false;
                    }
                    if (cv.accession.Equals(_32bit))
                    {
                        bit32 = true;
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

                double[] data = ConvertBase64ToDoubles(binaryData.binary, compressed, bit32);
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

            return new MZSpectrum(masses, intensities);
        }

        public override Polarity GetPolarity(int spectrumNumber)
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
            double rt = -1;
            foreach (CVParamType cv in _mzMLConnection.run.spectrumList.spectrum[spectrumNumber].scanList.scan[0].cvParam)
            {
                if (cv.accession.Equals(_retentionTime))
                {
                    rt = double.Parse(cv.value);
                }

                if (cv.unitName == "second")
                    rt /= 60;
            }

            if (rt >= 0)
                return rt;

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
            return _mzMLConnection.run.spectrumList.spectrum.Count;
        }

        public override int GetSpectrumNumber(double retentionTime)
        {
            // TODO need to convert this to a binary search of some sort. Or if the data is indexedMZML see if the indices work better.
            int totalSpectra = _mzMLConnection.run.spectrumList.spectrum.Count;
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
        private static double[] ConvertBase64ToDoubles(byte[] bytes, bool zlibCompressed = false, bool bit32 = true)
        {
            int size = bit32 ? sizeof (float) : sizeof (double);

            int length = bytes.Length/size;
            double[] convertedArray = new double[length];

            for (int i = 0; i < length; i++)
            {
                if (bit32)
                {
                    convertedArray[i] = BitConverter.ToSingle(bytes, i*size);
                }
                else
                {
                    convertedArray[i] = BitConverter.ToDouble(bytes, i*size);
                }
            }
            return convertedArray;
        }
    }
}