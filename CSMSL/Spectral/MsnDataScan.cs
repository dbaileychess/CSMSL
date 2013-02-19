using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.IO;
using CSMSL.Proteomics;

namespace CSMSL.Spectral
{
    public class MsnDataScan : MSDataScan
    {
        public MsnDataScan(int spectrumNumber, int msnOrder, MSDataFile parentFile = null)
            : base(spectrumNumber, msnOrder, parentFile) { }
    
        private double _precursorMz = double.NaN;
        public double PrecursorMz
        {
            get
            {
                if (double.IsNaN(_precursorMz) && ParentFile != null)
                {                    
                    _precursorMz = ParentFile.GetPrecusorMz(SpectrumNumber, MsnOrder);
                }
                return _precursorMz;
            }
        }

        private Range _isolationRange = null;
        public Range IsolationRange
        {
            get
            {
                if (_isolationRange == null && ParentFile != null)
                {
                    double prec_mz = PrecursorMz;
                    double half_width = ParentFile.GetIsolationWidth(SpectrumNumber, MsnOrder) / 2;
                    _isolationRange = new Range(prec_mz - half_width, prec_mz + half_width);
                }
                return _isolationRange;
            }
        }

        private short _precursorCharge = 0;
        public short PrecursorCharge
        {
            get
            {
                if (_precursorCharge == 0)
                {
                    _precursorCharge = ParentFile.GetPrecusorCharge(SpectrumNumber, MsnOrder);
                }
                return _precursorCharge;
            }
        }

        private DissociationType _dissociationType = DissociationType.UnKnown;
        public DissociationType DissociationType
        {
            get
            {
                if (_dissociationType == DissociationType.UnKnown)
                {
                    _dissociationType = ParentFile.GetDissociationType(SpectrumNumber, MsnOrder);
                }
                return _dissociationType;
            }
        }

    }
}
