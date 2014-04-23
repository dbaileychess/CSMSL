using System;
using CSMSL.Chemistry;
using CSMSL.Spectral;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.Identification
{
    public class FragmentSpectralMatch : IMass,  IEquatable<FragmentSpectralMatch>
    {
        public Spectrum MassSpectrum { get; private set; }

        public Fragment Fragment { get; private set; }

        public int Charge { get; private set; }

        public Tolerance Tolerance { get; private set; }
        
        public double MonoisotopicMass
        {
            get
            {
                return Fragment.ToMz(Charge);
            }
        }

        public FragmentSpectralMatch(Spectrum spectrum, Fragment fragment, Tolerance tolerance, int charge = 1)
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
