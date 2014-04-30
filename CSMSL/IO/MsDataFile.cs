using CSMSL.Proteomics;
using CSMSL.Spectral;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSMSL.IO
{
    public abstract class MSDataFile : IDisposable, IEquatable<MSDataFile>, IEnumerable<MSDataScan>
    {
        /// <summary>
        /// Defines if MS scans should be cached for quicker retrieval. Cached scans are held in an internal
        /// array and don't get cleared until the file is disposed or the ClearCacheScans() method is called.
        /// Of course, if you store the scans somewhere else, they will persist. The default value is True.
        /// </summary>
        public static bool CacheScans;

        internal MSDataScan[] Scans = null;

        private string _filePath;

        private int _firstSpectrumNumber = -1;

        private bool _isOpen;

        private int _lastSpectrumNumber = -1;

        private string _name;

        static MSDataFile()
        {
            CacheScans = true;
        }

        protected MSDataFile(string filePath, MSDataFileType filetype = MSDataFileType.UnKnown)
        {
            FilePath = filePath;
            FileType = filetype;
            _isOpen = false;
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

        public MSDataFileType FileType { get; private set; }

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
            set
            {
                _firstSpectrumNumber = value;
            }
        }

        public bool IsOpen
        {
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
            return GetEnumerator();
        }

        public virtual void Dispose()
        {
            if (Scans != null)
            {               
                ClearCachedScans();
                Scans = null;                
            }
            _isOpen = false;
        }

        public bool Equals(MSDataFile other)
        {
            if (ReferenceEquals(this, other)) return true;
            return FilePath.Equals(other.FilePath);
        }

        public IEnumerator<MSDataScan> GetEnumerator()
        {
            return GetMsScans().GetEnumerator();
        }

        public override int GetHashCode()
        {
            return FilePath.GetHashCode();
        }

        public abstract DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2);

        public abstract int GetMsnOrder(int spectrumNumber);

        /// <summary>
        /// Get the spectrum number of the parent scan that caused this scan to be executed.
        /// Typically MS1s will return 0 and MS2s will return the preceding MS1 scan (if in DDA mode)
        /// </summary>
        /// <param name="spectrumNumber">The spectrum number to get the parent scan number of</param>
        /// <returns>The spectrum number of the parent scan. 0 if no parent</returns>
        public virtual int GetParentSpectrumNumber(int spectrumNumber)
        {
            return 0;
        }

        /// <summary>
        /// Get the MS Scan at the specific spectrum number.
        /// </summary>
        /// <param name="spectrumNumber">The spectrum number to get the MS Scan at</param>      
        /// <returns></returns>
        public virtual MSDataScan GetMsScan(int spectrumNumber)
        {
            if (!CacheScans)
                return GetMSDataScan(spectrumNumber);

            if (Scans == null)
            {
                Scans = new MSDataScan[LastSpectrumNumber + 1];
            }

            if (Scans[spectrumNumber] == null)
            {
                return Scans[spectrumNumber] = GetMSDataScan(spectrumNumber);
            }

            return Scans[spectrumNumber];
        }

        public virtual void LoadAllScansInMemory()
        {
            if (!CacheScans)
            {
                throw new ArgumentException("Cache scans needs to be enabled for this to work properly", "CacheScans");
            }

            if (Scans == null)
            {
                Scans = new MSDataScan[LastSpectrumNumber + 1];
            }

            for (int spectrumNumber = FirstSpectrumNumber; spectrumNumber < LastSpectrumNumber; spectrumNumber++)
            {
                if (Scans[spectrumNumber] == null)
                {
                    Scans[spectrumNumber] = GetMSDataScan(spectrumNumber);
                }
            }
        }

        public virtual void ClearCachedScans()
        {
            if (Scans == null)
                return;
            Array.Clear(Scans, 0, Scans.Length);
        }

        protected virtual MSDataScan GetMSDataScan(int spectrumNumber)
        {           
            MSDataScan scan;
            int msn = GetMsnOrder(spectrumNumber);
            
            scan = msn > 1 ? new MsnDataScan(spectrumNumber, msn, this) : new MSDataScan(spectrumNumber, msn, this);

            return scan;            
        }

        public abstract short GetPrecusorCharge(int spectrumNumber, int msnOrder = 2);

        public abstract MzRange GetMzRange(int spectrumNumber);

        public abstract double GetPrecusorMz(int spectrumNumber, int msnOrder = 2);
        
        public abstract double GetIsolationWidth(int spectrumNumber, int msnOrder = 2);

        public virtual MzRange GetIsolationRange(int spectrumNumber, int msnOrder = 2)
        {
            double precursormz = GetPrecusorMz(spectrumNumber, msnOrder);
            double halfWidth = GetIsolationWidth(spectrumNumber, msnOrder) / 2;
            return new MzRange(precursormz - halfWidth, precursormz + halfWidth);
        }

        public virtual IEnumerable<MSDataScan> GetMsScans()
        {
            return GetMsScans(FirstSpectrumNumber, LastSpectrumNumber);
        }

        public virtual IEnumerable<MSDataScan> GetMsScans(int firstSpectrumNumber, int lastSpectrumNumber)
        {
            for (int spectrumNumber = firstSpectrumNumber; spectrumNumber <= lastSpectrumNumber; spectrumNumber++)
            {
                yield return GetMsScan(spectrumNumber);
            }
        }

        public virtual IEnumerable<MSDataScan> GetMsScans(double firstRT, double lastRT)
        {
            int spectrumNumber = GetSpectrumNumber(firstRT - 0.0000001);         
            while (spectrumNumber <= LastSpectrumNumber)
            {
                MSDataScan scan = GetMsScan(spectrumNumber++);
                double rt = scan.RetentionTime;
                if (rt < firstRT)
                    continue;
                else if (rt > lastRT)
                    yield break;
                yield return scan;
            }
        }

        public virtual IEnumerable<MSDataScan> GetMsScans(IRange<int> range)
        {
            return GetMsScans(range.Minimum, range.Maximum);
        }

        public abstract MZAnalyzerType GetMzAnalyzer(int spectrumNumber);

        public abstract MZSpectrum GetMzSpectrum(int spectrumNumber);

        public virtual Spectrum GetReadOnlyMZSpectrum(int spectrumNumber, bool centroid)
        {
            return GetMzSpectrum(spectrumNumber).ToReadOnlySpectrum();
        }

        public abstract Polarity GetPolarity(int spectrumNumber);

        public abstract double GetRetentionTime(int spectrumNumber);

        public abstract double GetInjectionTime(int spectrumNumber);

        public abstract double GetResolution(int spectrumNumber);

        public virtual string GetBase64Spectrum(int spectrumNumber, bool zlibCompressed = false)
        {
            return GetMzSpectrum(spectrumNumber).ToReadOnlySpectrum().ToBase64String(zlibCompressed);
        }

        public abstract int GetSpectrumNumber(double retentionTime);     

        /// <summary>
        /// Open up a connection to the underlying MS data stream
        /// </summary>
        public virtual void Open()
        {
            _isOpen = true;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Enum.GetName(typeof(MSDataFileType), FileType));
        }

        protected abstract int GetFirstSpectrumNumber();

        protected abstract int GetLastSpectrumNumber();

        
       
    }
}