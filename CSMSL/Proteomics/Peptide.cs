// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Peptide.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using CSMSL.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Proteomics
{
    public class Peptide : AminoAcidPolymer
    {
        /// <summary>
        /// The amino acid number this peptide is located in its parent
        /// </summary>
        public int StartResidue { get; set; }

        /// <summary>
        /// The amino acid number this peptide is located in its parent
        /// </summary>
        public int EndResidue { get; set; }

        /// <summary>
        /// The amino acid polymer this peptide came from
        /// </summary>
        public AminoAcidPolymer Parent { get; set; }

        /// <summary>
        /// The preceding amino acid in its parent
        /// </summary>
        public AminoAcid PreviousAminoAcid { get; set; }

        /// <summary>
        /// The next amino acid in its parent
        /// </summary>
        public AminoAcid NextAminoAcid { get; set; }

        public Peptide()
        {
        }

        /// <summary>
        /// Create a new peptide based on another amino acid polymer
        /// </summary>
        /// <param name="aminoAcidPolymer">The other amino acid polymer to copy</param>
        /// <param name="includeModifications">Whether to copy the modifications to the new peptide</param>
        public Peptide(AminoAcidPolymer aminoAcidPolymer, bool includeModifications = true)
            : base(aminoAcidPolymer, includeModifications)
        {
            Parent = aminoAcidPolymer;
            StartResidue = 0;
            EndResidue = Length - 1;
        }

        public Peptide(AminoAcidPolymer aminoAcidPolymer, int firstResidue, int length, bool includeModifications = true)
            : base(aminoAcidPolymer, firstResidue, length, includeModifications)
        {
            Parent = aminoAcidPolymer;
            StartResidue = firstResidue;
            EndResidue = firstResidue + length - 1;
            PreviousAminoAcid = aminoAcidPolymer.GetResidue(StartResidue - 1);
            NextAminoAcid = aminoAcidPolymer.GetResidue(EndResidue + 1);
        }

        public Peptide(string sequence)
            : this(sequence, null, 0)
        {
        }

        public Peptide(string sequence, AminoAcidPolymer parent)
            : this(sequence, parent, 0)
        {
        }

        public Peptide(string sequence, AminoAcidPolymer parent, int startResidue)
            : base(sequence)
        {
            Parent = parent;
            StartResidue = startResidue;
            EndResidue = startResidue + Length - 1;

            if (parent != null)
            {
                if (StartResidue > 0)
                    PreviousAminoAcid = parent.AminoAcids[StartResidue - 1];

                if (EndResidue < parent.Length - 1)
                    NextAminoAcid = parent.AminoAcids[EndResidue + 1];
            }
        }

        public IEnumerable<Peptide> GenerateIsotopologues()
        {
            // Get all the modifications that are isotopologues
            var isotopologues = GetUniqueModifications<IIsotopologue>().ToArray();

            // Base condition, no more isotopologues to make, so just return
            if (isotopologues.Length < 1)
            {
                yield break;
            }

            // Grab the the first isotopologue
            IIsotopologue isotopologue = isotopologues[0];

            // Loop over each modification in the isotopologue
            foreach (Modification mod in isotopologue)
            {
                // Create a clone of the peptide, cloning modifications as well.
                Peptide peptide = new Peptide(this);

                // Replace the base isotopologue mod with the specific version
                peptide.ReplaceModification(isotopologue, mod);

                // There were more than one isotopologue, so we must go deeper
                if (isotopologues.Length > 1)
                {
                    // Call the same rotuine on the newly generate peptide that has one less isotopologue
                    foreach (var subpeptide in peptide.GenerateIsotopologues())
                    {
                        yield return subpeptide;
                    }
                }
                else
                {
                    // Return this peptide
                    yield return peptide;
                }
            }
        }

        public IEnumerable<Peptide> GenerateIsoforms()
        {
            return GenerateIsoforms(this, false, GetModifications().Where(mod => mod != null).Cast<Modification>().ToArray());
        }

        public IEnumerable<Peptide> GenerateIsoforms(bool keepBaseModifications, params Modification[] modifications)
        {
            return GenerateIsoforms(this, keepBaseModifications, modifications); // Just call the static method
        }

        public Peptide GetSubPeptide(int firstResidue, int length)
        {
            return new Peptide(this, firstResidue, length);
        }

        public new bool Equals(AminoAcidPolymer other)
        {
            return base.Equals(other);
        }

        public static IEnumerable<Peptide> GenerateIsoforms(Peptide peptide, bool keepBaseModifications, params Modification[] modifications)
        {
            // Get the number of modifications to this peptide
            int numberOfModifications = modifications.Length;

            if (numberOfModifications < 1)
            {
                // No modifications, return the base peptide
                yield return peptide;
            }
            else if (numberOfModifications == 1)
            {
                // Only one modification, use the faster algorithm
                foreach (Peptide pep in GenerateIsoforms(peptide, modifications[0], 1))
                {
                    yield return pep;
                }
            }
            else
            {
                // More than one ptm case

                // Get all the unique modified sites for all the mods
                Dictionary<Modification, List<int>> allowedSites = new Dictionary<Modification, List<int>>();
                foreach (Modification mod in modifications)
                {
                    List<int> sites;
                    if (!allowedSites.TryGetValue(mod, out sites))
                    {
                        allowedSites.Add(mod, mod.GetModifiableSites(peptide).ToList());
                    }
                }

                // Only one type of mod, use the faster algorithm
                if (allowedSites.Count == 1)
                {
                    foreach (Peptide pep in GenerateIsoforms(peptide, modifications[0], numberOfModifications))
                    {
                        yield return pep;
                    }
                    yield break;
                }

                HashSet<Modification[]> results = new HashSet<Modification[]>(new ModificationArrayComparer());

                // Call out to the recursive helper method, starting with mod 0 and site 0
                GenIsoHelper(results, new Modification[peptide.Length + 2], modifications, allowedSites, 0, 0);

                // Create correct peptide mappings
                foreach (Modification[] modArray in results)
                {
                    Peptide pep = new Peptide(peptide, keepBaseModifications);
                    for (int i = 0; i < modArray.Length; i++)
                    {
                        var mod = modArray[i];
                        if (mod == null)
                            continue;

                        if (i == 0)
                        {
                            pep.AddModification(mod, Terminus.N);
                        }
                        else if (i > peptide.Length)
                        {
                            pep.AddModification(mod, Terminus.C);
                        }
                        else
                        {
                            pep.AddModification(mod, i);
                        }
                    }
                    yield return pep;
                }
            }
        }

        private static Modification[] GenIsoHelper(ISet<Modification[]> results, Modification[] modArray, Modification[] mods, Dictionary<Modification, List<int>> allowedSites, int modIndex, int siteIndex)
        {
            if (modIndex >= mods.Count())
            {
                return modArray; // Out of mods
            }

            // Get the current mod under consideration
            Modification currentMod = mods[modIndex];

            // Retrieve the list of sites that it can modify
            List<int> sites = allowedSites[currentMod];

            while (siteIndex < sites.Count)
            {
                // Get the index to the peptide where the mod would occur
                int index = sites[siteIndex];

                // Check to see if this site is already modded
                if (modArray[index] == null)
                {
                    // Set the current mod to this site
                    modArray[index] = currentMod;

                    // Check to see if there are any more mods
                    if (modIndex < mods.Count() - 1)
                    {
                        // Still have more mods to add so start so the new mod at the beginning of it's sites
                        Modification[] templist = GenIsoHelper(results, modArray, mods, allowedSites, ++modIndex, 0);

                        // All done for this master level, go up a level
                        modIndex--;

                        // Create a deep-copy clone
                        Array.Copy(templist, modArray, templist.Length);

                        // Remove the last mod added
                        modArray[index] = null;
                    }
                    else
                    {
                        // Completed all the mods, add the configuration to the saved list, if possible
                        results.Add((Modification[]) modArray.Clone());

                        // Remove the last mod added
                        modArray[index] = null;
                    }
                }

                // Go to the next site for this mod
                siteIndex++;
            }

            // All Done with this level
            return modArray;
        }

        public static IEnumerable<Peptide> GenerateIsoforms(Peptide peptide, Modification modification, long ptms)
        {
            // Get all the possible modified-residues' indices (zero-based)
            List<int> sites = modification.GetModifiableSites(peptide).ToList();

            // Total number of PTM sites
            int ptmsites = sites.Count;

            // Exact number of possible isoforms
            long isoforms = Combinatorics.BinomCoefficient(ptmsites, ptms);

            // For each possible isoform
            for (long isoform = 0; isoform < isoforms; isoform++)
            {
                // Create a new peptide based on the one passed in
                Peptide pep = new Peptide(peptide);

                long x = isoforms - isoform - 1;
                long a = ptmsites;
                long b = ptms;

                // For each ptm
                for (int i = 0; i < ptms; i++)
                {
                    long ans = Combinatorics.LargestV(a, b, x);
                    int index = sites[(int) (ptmsites - ans - 1)];
                    if (index == 0)
                    {
                        pep.AddModification(modification, Terminus.N);
                    }
                    else if (index > pep.Length)
                    {
                        pep.AddModification(modification, Terminus.C);
                    }
                    else
                    {
                        pep.AddModification(modification, index);
                    }
                    x -= Combinatorics.BinomCoefficient(ans, b);
                    a = ans;
                    b--;
                }

                yield return pep;
            }
        }
    }

    internal class ModificationArrayComparer : IEqualityComparer<Modification[]>
    {
        public bool Equals(Modification[] x, Modification[] y)
        {
            int length = x.Length;
            if (length != y.Length)
                return false;
            for (int i = 0; i < length; i++)
            {
                Modification a = x[i];
                Modification b = y[i];
                if (a == null)
                {
                    if (b != null)
                        return false;
                }
                else
                {
                    if (!a.Equals(b))
                        return false;
                }
            }
            return true;
        }

        public int GetHashCode(Modification[] obj)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = obj.Where(t => t != null).Aggregate((int) 2166136261, (current, t) => (current ^ t.GetHashCode())*p);
                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }
}