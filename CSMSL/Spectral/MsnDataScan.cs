// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MsnDataScan.cs) is part of CSMSL.
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