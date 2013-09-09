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

        public void MatchFragments(IEnumerable<Fragment> fragments, MassTolerance tolerance, params int[] chargeStates)
        {
            List<IPeak> peaks = null;
            foreach (Fragment fragment in fragments)
            {
                foreach (int chargeState in chargeStates)
                {
                    double mz = fragment.ToMz(chargeState);
                    if (MassSpectrum.ContainsPeak(new MassRange(mz, tolerance)))
                    {
                        Add(new FragmentSpectralMatch(MassSpectrum, fragment, tolerance, chargeState));
                    }
                }
            }
        }
    }
}
