using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.IO;

namespace CSMSL.Spectral
{
    public class MsScan
    {
        public Spectrum Spectrum;

        public MsDataFile ParentFile = null;

        public int SpectrumNumber;

        private int _msnOrder = -1;
        public int MsnOrder
        {
            get
            {
                if (_msnOrder < 0)
                {
                    if(ParentFile.IsOpen)
                        _msnOrder = ParentFile.GetMsnOrder(SpectrumNumber);
                }
                return _msnOrder;
            }
        }

        public MsScan(int spectrumNumber, MsDataFile parentFile = null)
        {
            SpectrumNumber = spectrumNumber;
            ParentFile = parentFile;
        }

    }
}
