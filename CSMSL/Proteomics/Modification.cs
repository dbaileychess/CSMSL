using System.Collections.Generic;
using CSMSL.Chemistry;
using System;

namespace CSMSL.Proteomics
{

    /// <summary>
    /// Represents a modification with a mass and name and default amino acid sites of modification
    /// </summary>
    public class Modification : IMass, IEquatable<Modification>
    {
        /// <summary>
        /// The default empty modification
        /// </summary>
        public static readonly Modification Empty = new Modification();

        /// <summary>
        /// The name of the modification
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The monoisotopic mass of the modification, commoningly known as the delta mass
        /// </summary>
        public double MonoisotopicMass { get; protected set; }
        
        /// <summary>
        /// The potentially modified sites of this modification
        /// </summary>
        public ModificationSites Sites { get; set; }

        /// <summary>
        /// Displays the name of the mod and the sites it modified in a formated string
        /// </summary>
        public string NameAndSites
        {
            get { return string.Format("{0} ({1})", Name, Sites); }
        }

        public Modification(Modification modification)
            : this(modification.MonoisotopicMass, modification.Name, modification.Sites) { }
      

        public Modification(double monoMass = 0.0, string name = "", ModificationSites sites = ModificationSites.Any)
        {
            MonoisotopicMass = monoMass;
            Name = name;
            Sites = sites;
        }
        
        public override string ToString()
        {
            return Name;
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

        public override int GetHashCode()
        {
            return MonoisotopicMass.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Modification modObj = obj as Modification;
            return modObj != null && Equals(modObj);   
        }

        public bool Equals(Modification other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!this.MassEquals(other))
                return false;

            if (!Name.Equals(other.Name))
                return false;

            if (!Sites.Equals(other.Sites))
                return false;

            return true;
        }
    }
}
