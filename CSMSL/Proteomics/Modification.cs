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
        public virtual string Name { get; protected set; }

        /// <summary>
        /// The monoisotopic mass of the modification, commoningly known as the delta mass
        /// </summary>
        public virtual double MonoisotopicMass { get; protected set; }
        
        /// <summary>
        /// The potentially modified sites of this modification
        /// </summary>
        public ModificationSites Sites { get; set; }

        public Modification(Modification modification)
            : this(modification.MonoisotopicMass, modification.Name, modification.Sites)
        {

        }

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

        public Modification(string chemicalFormula, string name, ModificationSites sites = ModificationSites.None)
        {
            MonoisotopicMass = new ChemicalFormula(chemicalFormula).MonoisotopicMass;
            Name = name;
            Sites = sites;
        }

        //NWK Added 140428
        public Modification(string chemicalFormula, string name, List<ModificationSites> sites)
        {
            MonoisotopicMass = new ChemicalFormula(chemicalFormula).MonoisotopicMass;
            Name = name;
            foreach (ModificationSites site in sites)
            {
                Sites.Set(char.Parse(site.ToString()));
            }
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

        public static Modification EmptyModification = new Modification(0,ModificationSites.All);
    }
}
