using System;
using CSMSL.IO;

namespace CSMSL.Spectral
{
    public class MSDataScan : IEquatable<MSDataScan>, IDisposable, IMassSpectrum
    {
        private MassSpectrum _spectrum = null;

        public MassSpectrum MassSpectrum
        {
            get
            {
                if (_spectrum == null)
                {
                    if (ParentFile.IsOpen)
                        _spectrum = ParentFile.GetMzSpectrum(SpectrumNumber);
                }
                return _spectrum;
            }
        }

        public MSDataFile ParentFile = null;

        private int _spectrumNumber;

        public int SpectrumNumber
        {
            get
            {
                return _spectrumNumber;
            }
            private set
            {
                _spectrumNumber = value;
            }
        }

        private int _msnOrder = -1;

        public int MsnOrder
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
            _spectrum = null;           
        }
    }
}