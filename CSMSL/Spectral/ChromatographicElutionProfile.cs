﻿// Copyright 2022 Derek J. Bailey
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

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CSMSL.Spectral
{
    public class ChromatographicElutionProfile<T> where T : IPeak
    {
        public T StartPeak
        {
            get { return _peaks[0]; }
        }

        public T EndPeak
        {
            get { return _peaks[Count - 1]; }
        }

        private readonly int _maxPeakIndex = 0;
        public T MaxPeak
        {
            get { return _peaks[_maxPeakIndex]; }
        }

        public DoubleRange TimeRange {get; private set;}

        public int Count {get; private set;}

        public double SummedArea { get; private set; }

        private readonly T[] _peaks;

        public ChromatographicElutionProfile(ICollection<T> peaks)
        {
            Count = peaks.Count;
            if (Count == 0)
            {
                return;
            }
            _peaks = peaks.ToArray();
       
            _maxPeakIndex = _peaks.MaxIndex(p => p.Y);
            SummedArea = _peaks.Sum(p => p.Y);
            TimeRange = new DoubleRange(_peaks[0].X, _peaks[Count - 1].X);
        }

        public double TrapezoidalArea()
        {
            double area = 0;
            for (int i = 0; i < Count - 1; i++)
            {
                T peak1 = _peaks[i];
                T peak2 = _peaks[i+1];
                area += (peak2.X - peak1.X) * (peak2.Y + peak1.Y);
            }
            return area / 2.0;
        }





    }
}
