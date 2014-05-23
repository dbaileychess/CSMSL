using CSMSL.IO;
using CSMSL.Proteomics;
using System;

namespace CSMSL.Spectral
{
    public class MsnDataScan : MSDataScan
    {
        public MsnDataScan(int spectrumNumber, int msnOrder, MSDataFile parentFile = null)
            : base(spectrumNumber, msnOrder, parentFile) { }

        public MsnDataScan() { }
    
        private double _precursorMz = double.NaN;
        public double PrecursorMz
        {
            get
            {
                if (double.IsNaN(_precursorMz))
                {
                    if (ParentFile.IsOpen)
                    {
                        _precursorMz = ParentFile.GetPrecusorMz(SpectrumNumber, MsnOrder);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }
                return _precursorMz;             
            }
            internal set
            {
                _precursorMz = value;
            }
        }

        private DoubleRange _isolationRange;
        public DoubleRange IsolationRange
        {
            get
            {
                if (_isolationRange == null)
                {
                    if (ParentFile.IsOpen)
                    {
                        _isolationRange = ParentFile.GetIsolationRange(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }           
                return _isolationRange;
            }
            internal set
            {
                _isolationRange = value;
            }
        }

        private int _precursorCharge;
        public virtual int PrecursorCharge
        {
            get
            {
                if (_precursorCharge == 0)
                {
                    if (ParentFile.IsOpen)
                    {
                        _precursorCharge = ParentFile.GetPrecusorCharge(SpectrumNumber, MsnOrder);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }
                return _precursorCharge;     
            }
            internal set
            {
                _precursorCharge = value;
            }
        }

        private DissociationType _dissociationType = DissociationType.UnKnown;
        public DissociationType DissociationType
        {
            get
            {
                if (_dissociationType == DissociationType.UnKnown)
                {
                    if (ParentFile.IsOpen)
                    {
                        _dissociationType = ParentFile.GetDissociationType(SpectrumNumber);
                    }
                    else
                    {
                        throw new ArgumentException("The parent data file is closed");
                    }
                }             
                return _dissociationType;
            }
            internal set
            {
                _dissociationType = value;
            }
        }      
       
    }
}
