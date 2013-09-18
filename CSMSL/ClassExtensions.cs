using System;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace CSMSL
{
    public static class MassExtension
    {
        public static double ToMz(this IMass mass, int charge)
        {
            return Mass.MzFromMass(mass.MonoisotopicMass, charge);
        }

        public static bool MassEquals(this IMass mass1, IMass mass2)
        {
            return Math.Abs(mass1.MonoisotopicMass - mass2.MonoisotopicMass) < 0.00000000001;
        }
    }

    public static class ModificationSiteExtensions
    {
        public static ModificationSites Set(this ModificationSites sites, char aminoacid)
        {
            AminoAcid aa;
            if(AminoAcid.TryGetResidue(aminoacid, out aa))
            {
                sites |= aa.Site;
            }
            return sites;
        }
    }
}


