// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (FragmentSpectralMatch.cs) is part of CSMSL.
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