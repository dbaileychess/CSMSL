using System;
using CSMSL.IO;

namespace CSMSL.Spectral
{
    public enum Polarity { Positive = 1, Negative = -1, Neutral = 0 }

    public enum MzAnalyzerType
    {
        Unknown = 0,
        Quadrupole = 1,
        IonTrap2D = 2,
        IonTrap3D = 3,
        Orbitrap = 4,
        TOF = 5,
        FTICR = 6,
        Sector = 7
    }

    public class MsScan : IEquatable<MsScan>, IDisposable
    {
        private Spectrum _spectrum = null;

        public Spectrum Spectrum
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

        public MsDataFile ParentFile = null;

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

        private MzAnalyzerType _mzAnalyzer = MzAnalyzerType.Unknown;

        public MzAnalyzerType MzAnalyzer
        {
            get
            {
                if (_mzAnalyzer == MzAnalyzerType.Unknown)
                {
                    _mzAnalyzer = ParentFile.GetMzAnalyzer(SpectrumNumber);
                }
                return _mzAnalyzer;
            }
        }

        public MsScan(int spectrumNumber, MsDataFile parentFile = null)
        {
            SpectrumNumber = spectrumNumber;
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

        public bool Equals(MsScan other)
        {
            if (ReferenceEquals(this, other)) return true;
            return SpectrumNumber.Equals(other.SpectrumNumber) && ParentFile.Equals(other.ParentFile);
        }

        public void Dispose()
        {
            if (_spectrum != null)
                _spectrum.Dispose();
            _spectrum = null;
        }
    }
}