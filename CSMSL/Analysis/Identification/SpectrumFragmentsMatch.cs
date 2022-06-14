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
using System.Collections.Generic;

namespace CSMSL.Analysis.Identification
{
    public class SpectrumFragmentsMatch
    {
        public ISpectrum Spectrum { get; private set; }

        private readonly HashSet<FragmentSpectralMatch> _fragmentSpectralMatches;

        private readonly HashSet<Fragment> _fragments;

        public int Matches
        {
            get { return _fragmentSpectralMatches.Count; }
        }

        public double PercentTIC { get; private set; }

        public bool Contains(Fragment fragment)
        {
            return _fragments.Contains(fragment);
        }

        public void Add(FragmentSpectralMatch fsm)
        {
            _fragmentSpectralMatches.Add(fsm);
            _fragments.Add(fsm.Fragment);
        }

        public SpectrumFragmentsMatch(ISpectrum spectrum)
        {
            Spectrum = spectrum;
            _fragmentSpectralMatches = new HashSet<FragmentSpectralMatch>();
            _fragments = new HashSet<Fragment>();
        }

        public override string ToString()
        {
            return string.Format("{0} Matches", Matches);
        }

        public IEnumerable<Fragment> MatchFragments(IEnumerable<Fragment> fragments, Tolerance tolerance, double percentCutoff, params int[] chargeStates)
        {
            double basePeakInt = Spectrum.GetBasePeakIntensity();
            double lowThreshold = basePeakInt*percentCutoff;
            double summedIntensity = 0;
            double totalIntensity = Spectrum.GetTotalIonCurrent();
            foreach (Fragment fragment in fragments)
            {
                foreach (int chargeState in chargeStates)
                {
                    double mz = fragment.ToMz(chargeState);
                    var peak = Spectrum.GetClosestPeak(tolerance.GetRange(mz));
                    if (peak != null && peak.Intensity >= lowThreshold)
                    {
                        Add(new FragmentSpectralMatch(Spectrum, fragment, tolerance, chargeState));
                        yield return fragment;
                        summedIntensity += peak.Intensity;
                    }
                }
            }
            PercentTIC = 100.0*summedIntensity/totalIntensity;
        }
    }
}