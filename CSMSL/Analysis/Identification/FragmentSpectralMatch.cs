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

using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using System;

namespace CSMSL.Analysis.Identification
{
    public class FragmentSpectralMatch : IMass, IEquatable<FragmentSpectralMatch>
    {
        public ISpectrum Spectrum { get; private set; }

        public Fragment Fragment { get; private set; }

        public int Charge { get; private set; }

        public Tolerance Tolerance { get; private set; }

        public double MonoisotopicMass
        {
            get { return Fragment.ToMz(Charge); }
        }

        public FragmentSpectralMatch(ISpectrum spectrum, Fragment fragment, Tolerance tolerance, int charge = 1)
        {
            Spectrum = spectrum;
            Fragment = fragment;
            Tolerance = tolerance;
            Charge = charge;
        }

        public override int GetHashCode()
        {
            return Fragment.GetHashCode() + Charge;
        }

        public bool Equals(FragmentSpectralMatch other)
        {
            return Fragment.Equals(other.Fragment) && Charge.Equals(other.Charge);
        }
    }
}