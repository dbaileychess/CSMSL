using CSMSL.IO;
using System;

namespace CSMSL.Spectral
{
    public class MSDataScan : IEquatable<MSDataScan>, IMassSpectrum
    {
        public MSDataFile ParentFile { get; private set; }

        private Spectrum _massSpectrum;

        /// <summary>
        /// The mass spectrum associated with the scan
        /// </summary>
        public Spectrum MassSpectrum
        {
            get
            {
                if (_massSpectrum == null)
                {
                    if (ParentFile.IsOpen)
                    {
                        _massSpectrum = ParentFile.GetMzSpectrum(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }
                return _massSpectrum;
            }
            internal set
            {
                _massSpectrum = value;
            }
        }

        public int SpectrumNumber { get; protected set; }

        private double _resolution = double.NaN;
        public double Resolution
        {
            get
            {
                if (double.IsNaN(_resolution))
                {
                    if (ParentFile.IsOpen)
                    {
                        _resolution = ParentFile.GetResolution(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }             
                return _resolution;
            }
            internal set
            {
                _resolution = value;
            }
        }

        private int _msnOrder = -1;
        public int MsnOrder
        {
            get
            {              
                return _msnOrder;
            }
            protected set
            {
                _msnOrder = value;
            }
        }

        private double _injectionTime = double.NaN;
        public virtual double InjectionTime
        {
            get
            {
                if (double.IsNaN(_injectionTime))
                {
                    if (ParentFile.IsOpen)
                    {
                        _injectionTime = ParentFile.GetInjectionTime(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }
                return _injectionTime;
            }
            internal set
            {
                _injectionTime = value;
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
                    {
                        _retentionTime = ParentFile.GetRetentionTime(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }              
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
                if (_polarity == Polarity.Neutral)
                {
                    if (ParentFile.IsOpen)
                    {
                        _polarity = ParentFile.GetPolarity(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }
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
                if (_mzAnalyzer == MZAnalyzerType.Unknown)
                {
                    if (ParentFile.IsOpen)
                    {
                        _mzAnalyzer = ParentFile.GetMzAnalyzer(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }
                return _mzAnalyzer;
            }
            internal set
            {
                _mzAnalyzer = value;
            }
        }

        private DoubleRange _mzRange;
        public DoubleRange MzRange
        {
            get
            {
                if (_mzRange == null)
                {
                    if (ParentFile.IsOpen)
                    {
                        _mzRange = ParentFile.GetMzRange(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }
                return _mzRange;
            }
            internal set
            {
                _mzRange = value;
            }
        }

        private int _parentScanNumber = -1;
        public int ParentScanNumber
        {
            get
            {
                if(_parentScanNumber < 0)
                {
                    _parentScanNumber = ParentFile.GetParentSpectrumNumber(SpectrumNumber);
                }
                else
                {
                    throw new ArgumentException("The parent data file is closed");
                }
                return _parentScanNumber;
            }
            internal set
            {
                _parentScanNumber = value;
            }
        }

        private Spectrum _readOnlySpectrum;
        public Spectrum GetReadOnlySpectrum()
        {
            if (_readOnlySpectrum == null)
            {
                if (ParentFile.IsOpen)
                {
                    _readOnlySpectrum = ParentFile.GetReadOnlyMZSpectrum(SpectrumNumber, true);
                }
                else
                {
                    throw new ArgumentException("The parent data file is closed");
                }
            }
            return _readOnlySpectrum;
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
            if (ParentFile == null)
            {
                return string.Format("Scan #{0}", SpectrumNumber);
            }
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

       
    }
}