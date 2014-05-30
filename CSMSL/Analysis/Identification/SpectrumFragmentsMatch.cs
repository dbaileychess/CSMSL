using System.Collections.Generic;
using CSMSL.Spectral;
using CSMSL.Proteomics;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Identification
{
    public class SpectrumFragmentsMatch
    {
        public ISpectrum Spectrum { get; private set; }

        private readonly HashSet<FragmentSpectralMatch> _fragmentSpectralMatches;

        private readonly HashSet<Fragment> _fragments; 

        public int Matches
        {
            get
            {
                return _fragmentSpectralMatches.Count;
            }
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
            PercentTIC = 100.0 * summedIntensity / totalIntensity;
        }
    }
}
