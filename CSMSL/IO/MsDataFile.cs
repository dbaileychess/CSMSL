using System;
using System.IO;
using CSMSL.Spectral;

namespace CSMSL.IO
{
    public abstract class MsDataFile : IDisposable , IEquatable<MsDataFile>
    {
        private bool _isOpen;

        public bool IsOpen { get { return _isOpen; } }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            private set { _filePath = value; }
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

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(FilePath); }
        }

        public MsDataFile(string filePath, MsDataFileType filetype = MsDataFileType.UnKnown)
        {
            if (!File.Exists(filePath))
            {
                throw new IOException(string.Format("The file {0} does not currently exist", filePath));
            }
            FilePath = filePath;
            FileType = filetype;
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

        public virtual MsScan GetMsScan(int spectrumNumber)
        {
            return new MsScan(spectrumNumber, this);
        }

        public virtual void Dispose()
        {
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
            return this.FilePath.Equals(other.FilePath);
        }
    }
}