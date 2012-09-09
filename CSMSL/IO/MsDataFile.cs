using System;
using System.Collections.Generic;
using System.IO;

namespace CSMSL.IO
{
    public abstract class MsDataFile : IDisposable, IEquatable<MsDataFile>, IEnumerable<MsScan>
    {
        private bool _isOpen;

        public bool IsOpen { get { return _isOpen; } }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            private set
            {
                _filePath = value;
                _name = Path.GetFileNameWithoutExtension(value);
            }
        }

        private MsDataFileType _fileType;

        public MsDataFileType FileType
        {
            get { return _fileType; }
            private set { _fileType = value; }
        }

        private int _firstSpectrumNumber = -1;
        private int _lastSpectrumNumber = -1;

        public virtual int FirstSpectrumNumber
        {
            get { return _firstSpectrumNumber; }
            protected set { _firstSpectrumNumber = value; }
        }

        public virtual int LastSpectrumNumber
        {
            get { return _lastSpectrumNumber; }
            protected set { _lastSpectrumNumber = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
        }

        protected MsScan[] _scans;

        public MsScan this[int spectrumNumber]
        {
            get
            {
                return GetMsScan(spectrumNumber);
            }
        }

        public MsDataFile(string filePath, MsDataFileType filetype = MsDataFileType.UnKnown)
        {
            if (!File.Exists(filePath))
            {
                throw new IOException(string.Format("The file {0} does not currently exist", filePath));
            }
            FilePath = filePath;
            FileType = filetype;
            _scans = new MsScan[1000];
        }

        /// <summary>
        /// Open up a connection to the underlying MS data stream
        /// </summary>
        public virtual void Open()
        {
            _isOpen = true;
        }

        public virtual int GetMsnOrder(int spectrumNumber)
        {
            return 1;
        }   
  
        public virtual double GetRetentionTime(int spectrumNumber)
        {
            return double.NaN;
        }

        public virtual Polarity GetPolarity(int spectrumNumber)
        {
            return Polarity.Neutral;
        }

        public virtual MsScan GetMsScan(int spectrumNumber)
        {
            if (spectrumNumber >= _scans.Length)
            {
                Array.Resize(ref _scans, spectrumNumber + 1000);
                return _scans[spectrumNumber] = new MsScan(spectrumNumber, this);
            }

            if (_scans[spectrumNumber] == null)
            {
                _scans[spectrumNumber] = new MsScan(spectrumNumber, this);
            }
            return _scans[spectrumNumber];
        }

        public virtual void Dispose()
        {
            _scans = null;
            _isOpen = false;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Enum.GetName(typeof(MsDataFileType), FileType));
        }

        public override int GetHashCode()
        {
            return this.FilePath.GetHashCode();
        }

        public bool Equals(MsDataFile other)
        {
            if (ReferenceEquals(this, other)) return true;
            return this.FilePath.Equals(other.FilePath);
        }

        public IEnumerator<MsScan> GetEnumerator()
        {
            int lastscan = LastSpectrumNumber; // Grab once
            for (int spectrumNumber = FirstSpectrumNumber; spectrumNumber < lastscan; spectrumNumber++)
            {
                yield return GetMsScan(spectrumNumber);
            }
            yield break;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}