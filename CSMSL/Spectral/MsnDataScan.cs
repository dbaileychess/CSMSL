// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CSMSL.IO;
using CSMSL.Proteomics;
using System;

namespace CSMSL.Spectral
{
    public class MsnDataScan<TSpectrum> : MSDataScan<TSpectrum>, IMsnDataScan<TSpectrum>
        where TSpectrum : ISpectrum
    {
        public MsnDataScan(int spectrumNumber, int msnOrder, MSDataFile<TSpectrum> parentFile = null)
            : base(spectrumNumber, msnOrder, parentFile)
        {
        }

        public MsnDataScan()
        {
        }

        private double _precursorMz = double.NaN;

        public double GetPrecursorMz()
        {
            if (double.IsNaN(_precursorMz))
            {
                if (ParentFile.IsOpen)
                {
                    _precursorMz = ParentFile.GetPrecursorMz(SpectrumNumber, MsnOrder);
                }
                else
                {
                    throw new ArgumentException("The parent data file is closed");
                }
            }
            return _precursorMz;
        }

        private DoubleRange _isolationRange;

        public DoubleRange GetIsolationRange()
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

        private int _precursorCharge;

        public virtual int GetPrecursorCharge()
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

        private double _injectionTime = -1;

        public virtual double GetInjectionTime()
        {
            if (_injectionTime >= 0)
                return _injectionTime;
            if (ParentFile.IsOpen)
            {
                _injectionTime = ParentFile.GetInjectionTime(SpectrumNumber);
            }
            else
            {
                throw new ArgumentException("The parent data file is closed");
            }
            return _injectionTime;
        }

        private DissociationType _dissociationType = DissociationType.UnKnown;

        public DissociationType GetDissociationType()
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

        public int _parentScanNumber = -1;

        public virtual int GetParentSpectrumNumber()
        {
            if (_parentScanNumber >= 0)
                return _parentScanNumber;
            if (ParentFile.IsOpen)
            {
                _parentScanNumber = ParentFile.GetParentSpectrumNumber(SpectrumNumber);
            }
            else
            {
                throw new ArgumentException("The parent data file is closed");
            }
            return _parentScanNumber;
        }

        public new ISpectrum MassSpectrum
        {
            get { return base.MassSpectrum; }
        }
    }
}