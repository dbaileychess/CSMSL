// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MSDataFile.cs) is part of CSMSL.
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
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CSMSL.IO
{
    /// <summary>
    /// A data file for storing data collected from a Mass Spectrometer
    /// </summary>
    public abstract class MSDataFile<TSpectrum> : IMSDataFile<TSpectrum>
        where TSpectrum : ISpectrum
    {
        /// <summary>
        /// Defines if MS scans should be cached for quicker retrieval. Cached scans are held in an internal
        /// array and don't get cleared until the file is disposed or the ClearCacheScans() method is called.
        /// Of course, if you store the scans somewhere else, they will persist. The default value is True.
        /// </summary>
        public static bool CacheScans;

        internal MSDataScan<TSpectrum>[] Scans = null;

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
            set { _firstSpectrumNumber = value; }
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

        IMSDataScan IMSDataFile.this[int spectrumNumber]
        {
            get { return GetMsScan(spectrumNumber); }
        }

        public IMSDataScan<TSpectrum> this[int spectrumNumber]
        {
            get { return GetMsScan(spectrumNumber); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected bool _isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                if (Scans != null)
                {
                    ClearCachedScans();
                    Scans = null;
                }
            }

            _isOpen = false;
            _isDisposed = true;
        }

        public bool Equals(MSDataFile<TSpectrum> other)
        {
            if (ReferenceEquals(this, other)) return true;
            return FilePath.Equals(other.FilePath);
        }

        public IEnumerator<IMSDataScan<TSpectrum>> GetEnumerator()
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
        public virtual MSDataScan<TSpectrum> GetMsScan(int spectrumNumber)
        {
            if (!CacheScans)
                return GetMSDataScan(spectrumNumber);

            if (Scans == null)
            {
                Scans = new MSDataScan<TSpectrum>[LastSpectrumNumber + 1];
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
                throw new ArgumentException("Cache scans needs to be enabled for this to work properly");
            }

            if (Scans == null)
            {
                Scans = new MSDataScan<TSpectrum>[LastSpectrumNumber + 1];
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

        protected virtual MSDataScan<TSpectrum> GetMSDataScan(int spectrumNumber)
        {
            int msn = GetMsnOrder(spectrumNumber);

            MSDataScan<TSpectrum> scan = msn > 1 ? new MsnDataScan<TSpectrum>(spectrumNumber, msn, this) : new MSDataScan<TSpectrum>(spectrumNumber, msn, this);

            return scan;
        }

        public abstract int GetPrecusorCharge(int spectrumNumber, int msnOrder = 2);

        public abstract MzRange GetMzRange(int spectrumNumber);

        public abstract double GetPrecursorMz(int spectrumNumber, int msnOrder = 2);

        public abstract double GetIsolationWidth(int spectrumNumber, int msnOrder = 2);

        public virtual MzRange GetIsolationRange(int spectrumNumber, int msnOrder = 2)
        {
            double precursormz = GetPrecursorMz(spectrumNumber, msnOrder);
            double halfWidth = GetIsolationWidth(spectrumNumber, msnOrder)/2;
            return new MzRange(precursormz - halfWidth, precursormz + halfWidth);
        }

        public virtual IEnumerable<MSDataScan<TSpectrum>> GetMsScans()
        {
            return GetMsScans(FirstSpectrumNumber, LastSpectrumNumber);
        }

        public virtual IEnumerable<MSDataScan<TSpectrum>> GetMsScans(int firstSpectrumNumber, int lastSpectrumNumber)
        {
            for (int spectrumNumber = firstSpectrumNumber; spectrumNumber <= lastSpectrumNumber; spectrumNumber++)
            {
                yield return GetMsScan(spectrumNumber);
            }
        }

        public virtual IEnumerable<MSDataScan<TSpectrum>> GetMsScans(double firstRT, double lastRT)
        {
            int spectrumNumber = GetSpectrumNumber(firstRT - 0.0000001);
            while (spectrumNumber <= LastSpectrumNumber)
            {
                MSDataScan<TSpectrum> scan = GetMsScan(spectrumNumber++);
                double rt = scan.RetentionTime;
                if (rt < firstRT)
                    continue;
                if (rt > lastRT)
                    yield break;
                yield return scan;
            }
        }

        public virtual IEnumerable<MSDataScan<TSpectrum>> GetMsScans(IRange<int> range)
        {
            return GetMsScans(range.Minimum, range.Maximum);
        }

        public abstract MZAnalyzerType GetMzAnalyzer(int spectrumNumber);

        public abstract TSpectrum GetSpectrum(int spectrumNumber);

        public abstract Polarity GetPolarity(int spectrumNumber);

        public abstract double GetRetentionTime(int spectrumNumber);

        public abstract double GetInjectionTime(int spectrumNumber);

        public abstract double GetResolution(int spectrumNumber);

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
            return string.Format("{0} ({1})", Name, Enum.GetName(typeof (MSDataFileType), FileType));
        }

        protected abstract int GetFirstSpectrumNumber();

        protected abstract int GetLastSpectrumNumber();

        IEnumerator<IMSDataScan> IEnumerable<IMSDataScan>.GetEnumerator()
        {
            return GetEnumerator();
        }

        ISpectrum IMSDataFile.GetSpectrum(int spectrumNumber)
        {
            return GetSpectrum(spectrumNumber);
        }

        public bool Equals(IMSDataFile other)
        {
            return Name.Equals(other.Name);
        }
    }
}