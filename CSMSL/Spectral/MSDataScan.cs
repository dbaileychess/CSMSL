using System;
using CSMSL.IO;

namespace CSMSL.Spectral
{
    public class MSDataScan : IEquatable<MSDataScan>, IDisposable, IMassSpectrum
    {
        private MassSpectrum _massSpectrum = null;

        /// <summary>
        /// The mass spectrum associated with the scan
        /// </summary>
        public MassSpectrum MassSpectrum
        {
            get
            {
                //if (_massSpectrum == null)
                //{
                //    if (ParentFile.IsOpen)
                //        _massSpectrum = ParentFile.GetMzSpectrum(SpectrumNumber);
                //}
                return _massSpectrum;
            }
            set
            {
                _massSpectrum = value;
            }
        }

        public MSDataFile ParentFile = null;

        private int _spectrumNumber;

        public virtual int SpectrumNumber
        {
            get
            {
                return _spectrumNumber;
            }
            protected set
            {
                _spectrumNumber = value;
            }
        }

        private double _resolution = double.NaN;
        public double Resolution
        {
            get
            {
                if (double.IsNaN(_resolution))
                {
                    if (ParentFile.IsOpen)
                        _resolution = ParentFile.GetResolution(SpectrumNumber);
                }
                return _resolution;
            }
            protected set
            {
                _resolution = value;
            }
        }

        private int _msnOrder = -1;
        public virtual int MsnOrder
        {
            get
            {
                if (_msnOrder < 0)
                {
                    if (ParentFile.IsOpen)
                        _msnOrder = ParentFile.GetMsnOrder(SpectrumNumber);
                }
                return _msnOrder;
            }
            protected set
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
                if (double.IsNaN(_retentionTime))
                {
                    if (ParentFile.IsOpen)
                        _retentionTime = ParentFile.GetRetentionTime(SpectrumNumber);
                }
                return _retentionTime;
            }
            protected set
            {
                _retentionTime = value;
            }
        }

        private Polarity _polarity = Polarity.Neutral;
        public Polarity Polarity
        {
            get
            {
                if (_polarity == Polarity.Neutral)
                {
                    if (ParentFile.IsOpen)
                        _polarity = ParentFile.GetPolarity(SpectrumNumber);
                }
                return _polarity;
            }
            protected set
            {
                _polarity = value;
            }
        }

        private MZAnalyzerType _mzAnalyzer = MZAnalyzerType.Unknown;

        public MZAnalyzerType MzAnalyzer
        {
            get
            {
                if (_mzAnalyzer == MZAnalyzerType.Unknown)
                {
                    _mzAnalyzer = ParentFile.GetMzAnalyzer(SpectrumNumber);
                }
                return _mzAnalyzer;
            }
            set
            {
                _mzAnalyzer = value;
            }
        }

        private MassRange _mzRange = null;
        public MassRange MzRange
        {
            get
            {
                if (_mzRange == null)
                {
                    _mzRange = ParentFile.GetMzRange(SpectrumNumber);
                }
                return _mzRange;
            }
        }

        public MSDataScan()
        {

        }

        public MSDataScan(int spectrumNumber,int msnOrder = 1, MSDataFile parentFile = null)
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