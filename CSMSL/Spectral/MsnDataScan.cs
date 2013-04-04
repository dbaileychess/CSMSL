using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.IO;
using CSMSL.Proteomics;

namespace CSMSL.Spectral
{
    public class MsnDataScan : MSDataScan, IMassSpectrum
    {
        public MsnDataScan(int spectrumNumber, int msnOrder, MSDataFile parentFile = null)
            : base(spectrumNumber, msnOrder, parentFile) { }

        public MsnDataScan() { }
    
        private double _precursorMz = double.NaN;
        public double PrecursorMz
        {
            get
            {              
                return _precursorMz;
            }
            internal set
            {
                _precursorMz = value;
            }
        }

        private MassRange _isolationRange = null;
        public MassRange IsolationRange
        {
            get
            {               
                return _isolationRange;
            }
            internal set
            {
                _isolationRange = value;
            }
        }

        private short _precursorCharge = 0;
        public virtual short PrecursorCharge
        {
            get
            {              
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
                return _dissociationType;
            }
            internal set
            {
                _dissociationType = value;
            }
        }      
       
    }
}
