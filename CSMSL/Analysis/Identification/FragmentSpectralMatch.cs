using System;
using CSMSL.Chemistry;
using CSMSL.Spectral;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.Identification
{
    public class FragmentSpectralMatch : IMassSpectrum, IMass,  IEquatable<FragmentSpectralMatch>
    {
        public MassSpectrum MassSpectrum { get; private set; }

        public Fragment Fragment { get; private set; }

        public int Charge { get; private set; }

        public MassTolerance Tolerance { get; private set; }
        
        public double MonoisotopicMass
        {
            get
            {
                return Fragment.ToMz(Charge);
            }
        }

        public FragmentSpectralMatch(MassSpectrum spectrum, Fragment fragment, MassTolerance tolerance, int charge = 1)
        {
            MassSpectrum = spectrum;
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
