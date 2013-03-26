using System;
using System.Collections.Generic;
using System.IO;
using CSMSL.Spectral;
using CSMSL.Proteomics;

namespace CSMSL.IO
{
    public abstract class MSDataFile : IDisposable, IEquatable<MSDataFile>, IEnumerable<MSDataScan>
    {
        internal MSDataScan[] _scans = null;

        private string _filePath;

        private MsDataFileType _fileType;

        private int _firstSpectrumNumber = -1;

        private bool _isOpen;

        private int _lastSpectrumNumber = -1;

        private string _name;

        public MSDataFile(string filePath, MsDataFileType filetype = MsDataFileType.UnKnown, bool openImmediately = false)
        {
            if (!File.Exists(filePath) && !Directory.Exists(filePath))
            {
                throw new IOException(string.Format("The MS data file {0} does not currently exist", filePath));
            }
            FilePath = filePath;
            FileType = filetype;
            if (openImmediately) Open();
        }



        public string FilePath
        {
            get { return _filePath; }
            private set
            {
                _filePath = value;
                _name = Path.GetFileNameWithoutExtension(value);
            }
        }

        public MsDataFileType FileType
        {
            get { return _fileType; }
            private set { _fileType = value; }
        }

        public virtual int FirstSpectrumNumber
        {
            get
            {
                if (_firstSpectrumNumber < 0)
                {
                    _firstSpectrumNumber = GetFirstSpectrumNumber();
                }
                return _firstSpectrumNumber;
            }
        }

        public bool IsOpen { 
            get { return _isOpen; }
            protected set { _isOpen = value; }
        }

        public virtual int LastSpectrumNumber
        {
            get
            {
                if (_lastSpectrumNumber < 0)
                {
                    _lastSpectrumNumber = GetLastSpectrumNumber();
                }
                return _lastSpectrumNumber;
            }
        }

      

        public string Name
        {
            get { return _name; }
        }

        public MSDataScan this[int spectrumNumber]
        {
            get
            {
                return GetMsScan(spectrumNumber);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual void Dispose()
        {
            if (_scans != null)
            {
                foreach (MSDataScan scan in _scans)
                {
                    if (scan != null)
                        scan.Dispose();
                }
                Array.Clear(_scans, 0, _scans.Length);
                _scans = null;
                _isOpen = false;
            }
        }

        public bool Equals(MSDataFile other)
        {
            if (ReferenceEquals(this, other)) return true;
            return this.FilePath.Equals(other.FilePath);
        }

        public IEnumerator<MSDataScan> GetEnumerator()
        {
            return GetMsScans().GetEnumerator();
        }

        public override int GetHashCode()
        {
            return this.FilePath.GetHashCode();
        }

        public abstract DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2);

        public abstract int GetMsnOrder(int spectrumNumber);

        public virtual MSDataScan GetMsScan(int spectrumNumber)
        {
            if (_scans == null)
            {
                _scans = new MSDataScan[LastSpectrumNumber + 1];
            }

            if (_scans[spectrumNumber] == null)
            {
                int msn = GetMsnOrder(spectrumNumber);
                _scans[spectrumNumber] = (msn > 1) ? new MsnDataScan(spectrumNumber, msn, this) : new MSDataScan(spectrumNumber, msn, this);
            }
            return _scans[spectrumNumber];
        }

        public abstract short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2);

        public abstract MassRange GetMzRange(int spectrumNumber);

        public IEnumerable<MSDataScan> GetMsScans()
        {
            return GetMsScans(FirstSpectrumNumber, LastSpectrumNumber);
        }

        public abstract double GetPrecusorMz(int spectrumNumber, int msnOrder = 2);

        public abstract double GetIsolationWidth(int spectrumNumber, int msnOrder = 2);

        public IEnumerable<MSDataScan> GetMsScans(int firstSpectrumNumber, int lastSpectrumNumber)
        {
            for (int spectrumNumber = firstSpectrumNumber; spectrumNumber <= lastSpectrumNumber; spectrumNumber++)
            {
                yield return GetMsScan(spectrumNumber);
            }
            yield break;
        }

        public IEnumerable<MSDataScan> GetMsScans(IRange<int> range)
        {
            return GetMsScans(range.Minimum, range.Maximum);
        }

        public abstract MZAnalyzerType GetMzAnalyzer(int spectrumNumber);

        public abstract MassSpectrum GetMzSpectrum(int spectrumNumber);

        public abstract Polarity GetPolarity(int spectrumNumber);

        public abstract double GetRetentionTime(int spectrumNumber);

        public abstract double GetInjectionTime(int spectrumNumber);

        public abstract double GetResolution(int spectrumNumber);

        /// <summary>
        /// Open up a connection to the underlying MS data stream
        /// </summary>
        public virtual void Open()
        {
            _isOpen = true;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Enum.GetName(typeof(MsDataFileType), FileType));
        }

        protected abstract int GetFirstSpectrumNumber();

        protected abstract int GetLastSpectrumNumber();

        public abstract int GetSpectrumNumber(double retentionTime);     
    }
}