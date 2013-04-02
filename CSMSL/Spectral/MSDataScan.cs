using System;
using CSMSL.IO;

namespace CSMSL.Spectral
{
    public class MSDataScan : IEquatable<MSDataScan>, IDisposable, IMassSpectrum
    {
        public MSDataFile ParentFile = null;

        private MassSpectrum _massSpectrum = null;

        /// <summary>
        /// The mass spectrum associated with the scan
        /// </summary>
        public MassSpectrum MassSpectrum
        {
            get
            {
                return _massSpectrum;
            }
            internal set
            {
                _massSpectrum = value;
            }
        }       

        private int _spectrumNumber;

        public virtual int SpectrumNumber
        {
            get
            {
                return _spectrumNumber;
            }
            internal set
            {
                _spectrumNumber = value;
            }
        }

        private double _resolution = double.NaN;
        public double Resolution
        {
            get
            {           
                return _resolution;
            }
            internal set
            {
                _resolution = value;
            }
        }

        private int _msnOrder = -1;
        public virtual int MsnOrder
        {
            get
            {              
                return _msnOrder;
            }
            internal set
            {
                _msnOrder = value;
            }
        }

        public virtual double InjectionTime
        {
            get;
            set;
        }

        private double _retentionTime = double.NaN;

        public double RetentionTime
        {
            get
            {              
                return _retentionTime;
            }
            internal set
            {
                _retentionTime = value;
            }
        }

        private Polarity _polarity = Polarity.Neutral;
        public Polarity Polarity
        {
            get
            {               
                return _polarity;
            }
            internal set
            {
                _polarity = value;
            }
        }

        private MZAnalyzerType _mzAnalyzer = MZAnalyzerType.Unknown;

        public MZAnalyzerType MzAnalyzer
        {
            get
            {              
                return _mzAnalyzer;
            }
            internal set
            {
                _mzAnalyzer = value;
            }
        }

        private MassRange _mzRange = null;
        public MassRange MzRange
        {
            get
            {               
                return _mzRange;
            }
            internal set
            {
                _mzRange = value;
            }
        }

        public MSDataScan()
        {

        }

        public MSDataScan(int spectrumNumber, int msnOrder = 1, MSDataFile parentFile = null)
        {
            SpectrumNumber = spectrumNumber;
            MsnOrder = msnOrder;
            ParentFile = parentFile;
        }

        public override string ToString()
        {
            return string.Format("Scan #{0} from {1}", SpectrumNumber, ParentFile);
        }

        public override int GetHashCode()
        {
            return ParentFile.GetHashCode() ^ SpectrumNumber;
        }

        public bool Equals(MSDataScan other)
        {
            if (ReferenceEquals(this, other)) return true;
            return SpectrumNumber.Equals(other.SpectrumNumber) && ParentFile.Equals(other.ParentFile);
        }

        public void Dispose()
        {
            ParentFile._scans[SpectrumNumber] = null; // clear the cache in the parent file      
            _massSpectrum = null;           
        }
    }
}