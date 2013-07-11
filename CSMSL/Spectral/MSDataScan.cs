using CSMSL.IO;
using System;

namespace CSMSL.Spectral
{
    public class MSDataScan : IEquatable<MSDataScan>, IMassSpectrum
    {
        public MSDataFile ParentFile { get; private set; }

        private MassSpectrum _massSpectrum = null;

        /// <summary>
        /// The mass spectrum associated with the scan
        /// </summary>
        public MassSpectrum MassSpectrum
        {
            get
            {
                if (_massSpectrum == null)
                {
                    if (ParentFile.IsOpen)
                    {
                        _massSpectrum = ParentFile.GetMzSpectrum(_spectrumNumber);
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
                if (double.IsNaN(_resolution))
                {
                    if (ParentFile.IsOpen)
                    {
                        _resolution = ParentFile.GetResolution(_spectrumNumber);
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

        private double _injectionTime = double.NaN;

        public virtual double InjectionTime
        {
            get
            {
                if (double.IsNaN(_injectionTime))
                {
                    if (ParentFile.IsOpen)
                    {
                        _injectionTime = ParentFile.GetInjectionTime(_spectrumNumber);
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
                        _retentionTime = ParentFile.GetRetentionTime(_spectrumNumber);
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
                if (_polarity == Spectral.Polarity.Unknown)
                {
                    if (ParentFile.IsOpen)
                    {
                        _polarity = ParentFile.GetPolarity(_spectrumNumber);
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
                        _mzAnalyzer = ParentFile.GetMzAnalyzer(_spectrumNumber);
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

        private MassRange _mzRange = null;
        public MassRange MzRange
        {
            get
            {
                if (_mzRange == null)
                {
                    if (ParentFile.IsOpen)
                    {
                        _mzRange = ParentFile.GetMzRange(_spectrumNumber);
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
                return string.Format("Scan #{0}");
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