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
using System;

namespace CSMSL.Spectral
{
    public class MSDataScan<TSpectrum> : IMSDataScan<TSpectrum>, IEquatable<MSDataScan<TSpectrum>> 
        where TSpectrum : ISpectrum
    {
        public MSDataFile<TSpectrum> ParentFile { get; private set; }

        private TSpectrum _massMzSpectrum;

        /// <summary>
        /// The mass spectrum associated with the scan
        /// </summary>
        public TSpectrum MassSpectrum
        {
            get
            {
                if (_massMzSpectrum != null || ParentFile == null) 
                    return _massMzSpectrum;

                if (ParentFile.IsOpen)
                {
                    _massMzSpectrum = ParentFile.GetSpectrum(SpectrumNumber);
                }
                return _massMzSpectrum;
            }
            internal set { _massMzSpectrum = value; }
        }

        ISpectrum IMassSpectrum.MassSpectrum
        {
            get { return MassSpectrum; }
        }

        public int SpectrumNumber { get; protected set; }

        private double _resolution = double.NaN;

        public double Resolution
        {
            get
            {
                if (!double.IsNaN(_resolution) || ParentFile == null) 
                    return _resolution;

                if (ParentFile.IsOpen)
                {
                    _resolution = ParentFile.GetResolution(SpectrumNumber);
                }
                return _resolution;
            }
            internal set { _resolution = value; }
        }

        public int MsnOrder { get; protected set; }

        private double _injectionTime = double.NaN;

        public virtual double InjectionTime
        {
            get
            {
                if (double.IsNaN(_injectionTime))
                {
                    if (ParentFile.IsOpen)
                    {
                        _injectionTime = ParentFile.GetInjectionTime(SpectrumNumber);
                    }
                }
                return _injectionTime;
            }
            internal set { _injectionTime = value; }
        }

        private double _retentionTime = double.NaN;

        public double RetentionTime
        {
            get
            {
                if (double.IsNaN(_retentionTime))
                {
                    if (ParentFile.IsOpen)
                    {
                        _retentionTime = ParentFile.GetRetentionTime(SpectrumNumber);
                    }
                }
                return _retentionTime;
            }
            internal set { _retentionTime = value; }
        }

        private Polarity _polarity = Polarity.Neutral;

        public Polarity Polarity
        {
            get
            {
                if (_polarity == Polarity.Neutral)
                {
                    if (ParentFile.IsOpen)
                    {
                        _polarity = ParentFile.GetPolarity(SpectrumNumber);
                    }
                }
                return _polarity;
            }
            internal set { _polarity = value; }
        }

        private MZAnalyzerType _mzAnalyzer = MZAnalyzerType.Unknown;

        public MZAnalyzerType MzAnalyzer
        {
            get
            {
                if (_mzAnalyzer == MZAnalyzerType.Unknown)
                {
                    if (ParentFile.IsOpen)
                    {
                        _mzAnalyzer = ParentFile.GetMzAnalyzer(SpectrumNumber);
                    }
                }
                return _mzAnalyzer;
            }
            internal set { _mzAnalyzer = value; }
        }

        private DoubleRange _mzRange;

        public DoubleRange MzRange
        {
            get
            {
                if (_mzRange == null)
                {
                    if (ParentFile.IsOpen)
                    {
                        _mzRange = ParentFile.GetMzRange(SpectrumNumber);
                    }
                }
                return _mzRange;
            }
            internal set { _mzRange = value; }
        }

        private int _parentScanNumber = -1;

        public int ParentScanNumber
        {
            get
            {
                if (_parentScanNumber < 0)
                {
                    _parentScanNumber = ParentFile.GetParentSpectrumNumber(SpectrumNumber);
                }
                return _parentScanNumber;
            }
            internal set { _parentScanNumber = value; }
        }

        public MSDataScan()
        {
            MsnOrder = -1;
        }

        public MSDataScan(int spectrumNumber, int msnOrder = 1, MSDataFile<TSpectrum> parentFile = null)
        {
            SpectrumNumber = spectrumNumber;
            MsnOrder = msnOrder;
            ParentFile = parentFile;
        }

        public override string ToString()
        {
            if (ParentFile == null)
            {
                return string.Format("Scan #{0}", SpectrumNumber);
            }
            return string.Format("Scan #{0} from {1}", SpectrumNumber, ParentFile);
        }

        public override int GetHashCode()
        {
            return ParentFile.GetHashCode() ^ SpectrumNumber;
        }

        public bool Equals(MSDataScan<TSpectrum> other)
        {
            if (ReferenceEquals(this, other)) return true;
            return SpectrumNumber.Equals(other.SpectrumNumber) && ParentFile.Equals(other.ParentFile);
        }
    }
}