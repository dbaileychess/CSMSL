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


