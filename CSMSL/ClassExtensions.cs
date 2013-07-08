using CSMSL.Chemistry;

namespace CSMSL
{
    public static class MassExtension
    {
        public static double ToMz(this IMass mass, int charge)
        {
            return Mass.MzFromMass(mass.MonoisotopicMass, charge);
        }
    }
}
