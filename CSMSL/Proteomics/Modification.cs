using System.Collections.Generic;
using CSMSL.Chemistry;
using System;

namespace CSMSL.Proteomics
{

    /// <summary>
    /// Represents a modification with a mass and name
    /// </summary>
    public class Modification : IMass
    {
        /// <summary>
        /// The name of the modification
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The monoisotopic mass
        /// </summary>
        public double MonoisotopicMass { get; set; }
        
        /// <summary>
        /// The potentially modified sites of this modification
        /// </summary>
        public ModificationSites Sites { get; set; }

        public Modification(double monoMass = 0.0, ModificationSites sites = ModificationSites.None)
        {
            MonoisotopicMass = monoMass;
            Sites = sites;
        }

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

        public static IEnumerable<ModificationSites> GetSites(ModificationSites sites)
        {
            foreach (ModificationSites site in Enum.GetValues(typeof(ModificationSites)))
            {
                if (site == ModificationSites.None)
                    continue;
                if ((sites & site) == site)
                    yield return site;
            }
        }

        internal IEnumerable<int> GetModifiableSites(AminoAcidPolymer peptide)
        {
            if (Sites == ModificationSites.None || peptide == null)
                yield break;

            if ((Sites & ModificationSites.NPep) == ModificationSites.NPep)
                yield return 0;

            int i = 1;
            foreach (AminoAcid aa in peptide.AminoAcids)
            {
                if ((Sites & aa.Site) == aa.Site)
                    yield return i;
                i++;
            }

            if ((Sites & ModificationSites.PepC) == ModificationSites.PepC)
                yield return i;
        }
    }
}
