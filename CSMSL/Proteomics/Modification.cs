using System.Collections.Generic;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Modification : IMass
    {
        public string Name { get; set; }

        public double MonoisotopicMass { get; set; }

        public ModificationSites Sites { get; set; }

        public Modification(double monoMass, string name, ModificationSites sites = ModificationSites.None)
        {
            MonoisotopicMass = monoMass;
            Name = name;
            Sites = sites;
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<int> GetSites(AminoAcidPolymer peptide)
        {
            if ((Sites & ModificationSites.NPep) == ModificationSites.NPep)
                yield return 0;

            int i = 1;
            foreach (AminoAcid aa in peptide.AminoAcids)
            {
                if ((aa.Site & Sites) == aa.Site)
                    yield return i;
                i++;
            }

            if ((Sites & ModificationSites.PepC) == ModificationSites.PepC)
                yield return peptide.Length;
        }
    }
}
