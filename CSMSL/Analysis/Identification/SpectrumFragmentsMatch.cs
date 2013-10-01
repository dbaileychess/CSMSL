using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.Spectral;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.Identification
{
    public class SpectrumFragmentsMatch : IMassSpectrum
    {
        public MassSpectrum MassSpectrum { get; private set; }

        private readonly HashSet<FragmentSpectralMatch> _fragmentSpectralMatches;

        public int Matches
        {
            get
            {
                return _fragmentSpectralMatches.Count;
            }
        }

        public bool Contains(Fragment fragment)
        {
            return _fragmentSpectralMatches.Select(f => f.Fragment).Contains(fragment);
        }

        public void Add(FragmentSpectralMatch fsm)
        {
            _fragmentSpectralMatches.Add(fsm);
        }

        public SpectrumFragmentsMatch(MassSpectrum spectrum)
        {
            MassSpectrum = spectrum;
            _fragmentSpectralMatches = new HashSet<FragmentSpectralMatch>();
        }

        public override string ToString()
        {
            return string.Format("{0} Matches", Matches);
        }

        public void MatchFragments(IEnumerable<Fragment> fragments, MassTolerance tolerance, double percentCutoff, params int[] chargeStates)
        {
            List<IPeak> peaks = null;
            double basePeakInt = MassSpectrum.BasePeak.Intensity;
            double lowThreshold = basePeakInt*percentCutoff;
            foreach (Fragment fragment in fragments)
            {
                foreach (int chargeState in chargeStates)
                {
                    double mz = fragment.ToMz(chargeState);
                    var peak = MassSpectrum.GetClosestPeak(new MassRange(mz, tolerance));
                    if (peak != null && peak.Intensity >= lowThreshold)
                    {
                        Add(new FragmentSpectralMatch(MassSpectrum, fragment, tolerance, chargeState));
                    }
                }
            }
        }
    }
}
