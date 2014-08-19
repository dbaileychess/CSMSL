// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ProteinGroup.cs) is part of CSMSL.
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

using CSMSL.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Proteomics
{
    public class ProteinGroup : IEnumerable<Protein>, IEquatable<ProteinGroup>
    {
        public HashSet<Protein> Proteins { get; set; }

        public HashSet<IAminoAcidSequence> Peptides { get; set; }

        public Protein RepresentativeProtein { get; private set; }

        public int Length
        {
            get { return (RepresentativeProtein == null) ? 0 : RepresentativeProtein.Length; }
        }

        public ProteinGroup()
            : this(new AminoAcidLeucineSequenceComparer())
        {
        }

        public ProteinGroup(IEqualityComparer<IAminoAcidSequence> peptideComparer)
        {
            Proteins = new HashSet<Protein>();
            Peptides = new HashSet<IAminoAcidSequence>(peptideComparer);
        }

        public ProteinGroup(IEnumerable<IAminoAcidSequence> peptideSequences, IEqualityComparer<IAminoAcidSequence> peptideComparer)
        {
            Proteins = new HashSet<Protein>();
            Peptides = new HashSet<IAminoAcidSequence>(peptideSequences, peptideComparer);
        }

        public ProteinGroup(Protein protein, IEnumerable<IAminoAcidSequence> peptideSequences, IEqualityComparer<IAminoAcidSequence> peptideComparer)
            : this(peptideSequences, peptideComparer)
        {
            AddProtein(protein);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hCode = 7;
                foreach (Protein prot in Proteins)
                {
                    hCode ^= prot.GetHashCode();
                    hCode = (hCode >> 3);
                }
                return hCode;
            }
        }

        public override bool Equals(object obj)
        {
            var proteinGroup = obj as ProteinGroup;
            if (proteinGroup == null)
                return false;
            return Equals(proteinGroup);
        }

        public bool Equals(ProteinGroup other)
        {
            return Proteins.SetEquals(other.Proteins);
        }

        public int ProteinCount
        {
            get { return Proteins.Count; }
        }

        public int PeptideCount
        {
            get { return Peptides.Count; }
        }

        public void AddProtein(Protein protein)
        {
            Proteins.Add(protein);
            if (RepresentativeProtein == null)
            {
                RepresentativeProtein = protein;
            }
            else
            {
                if (protein.Length > RepresentativeProtein.Length)
                {
                    RepresentativeProtein = protein;
                }
            }
        }

        public void AddPeptide(IAminoAcidSequence peptide)
        {
            Peptides.Add(peptide);
        }

        public IEnumerator<Protein> GetEnumerator()
        {
            return Proteins.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Proteins.GetEnumerator();
        }

        public int[] GetSequenceCoverage(IEnumerable<IAminoAcidSequence> peptides)
        {
            int[] bits = new int[Length];
            string leucineSequence = RepresentativeProtein.GetLeucineSequence();
            foreach (IAminoAcidSequence pep in peptides)
            {
                int start_index = 0;
                while (true)
                {
                    int index = leucineSequence.IndexOf(pep.GetLeucineSequence(), start_index);
                    start_index = index + 1;
                    if (index < 0)
                    {
                        break;
                    }

                    for (int aa = index; aa < index + pep.Length; aa++)
                    {
                        bits[aa]++;
                    }
                }
            }
            return bits;
        }

        public double CalculateSequenceCoverage(IEnumerable<IAminoAcidSequence> peptides)
        {
            int[] bits = GetSequenceCoverage(peptides);

            int observedAminoAcids = bits.Count(bit => bit > 0);

            return (double) observedAminoAcids/Length*100.0;
        }

        #region Statics

        public static IEnumerable<ProteinGroup> GroupProteins(string fastaFile, IProtease protease, IEnumerable<IAminoAcidSequence> observeredSequences, IEqualityComparer<IAminoAcidSequence> peptideComparer, int MaxMissedCleavages = 3)
        {
            using (FastaReader fasta = new FastaReader(fastaFile))
            {
                return GroupProteins(fasta.ReadNextProtein(), new[] {protease}, observeredSequences, peptideComparer, MaxMissedCleavages);
            }
        }

        public static IEnumerable<ProteinGroup> GroupProteins(IEnumerable<Protein> proteins, IList<IProtease> proteases, IEnumerable<IAminoAcidSequence> observeredSequences, IEqualityComparer<IAminoAcidSequence> peptideComparer, int MaxMissedCleavages = 3, int minPepPerProtein = 1)
        {
            var proteinGroups = new List<ProteinGroup>();

            // Get all the unique peptides defined by the comparere passed in
            HashSet<IAminoAcidSequence> peptideSequences = new HashSet<IAminoAcidSequence>(observeredSequences, peptideComparer);

            // Peptides that were mapped to proteins, for error checking
            var mappedSequences = new Dictionary<IAminoAcidSequence, int>(peptideComparer);

            // Find smallest and largest peptide
            int smallestPeptide = int.MaxValue;
            int largestPeptide = 0;
            foreach (var peptideSequence in peptideSequences)
            {
                int length = peptideSequence.Length;
                if (length > largestPeptide)
                    largestPeptide = length;
                if (length < smallestPeptide)
                    smallestPeptide = length;
            }

            foreach (Protein protein in proteins)
            {
                HashSet<IAminoAcidSequence> proteinSequences = new HashSet<IAminoAcidSequence>();

                foreach (Protease protease in proteases)
                {
                    foreach (Peptide peptide in protein.Digest(protease, MaxMissedCleavages, smallestPeptide, largestPeptide))
                    {
                        if (!peptideSequences.Contains(peptide))
                            continue;

                        if (proteinSequences.Add(peptide))
                        {
                            int proteinCount;
                            if (mappedSequences.TryGetValue(peptide, out proteinCount))
                            {
                                mappedSequences[peptide] = proteinCount + 1;
                            }
                            else
                            {
                                mappedSequences.Add(peptide, 1);
                            }
                        }
                    }
                }

                if (proteinSequences.Count > 0)
                {
                    var proteinGroup = new ProteinGroup(protein, proteinSequences, peptideComparer);
                    proteinGroups.Add(proteinGroup);
                }
            }

            if (mappedSequences.Count != peptideSequences.Count)
            {
                throw new Exception("All peptides were not mapped to a protein!");
            }

            return CombinedProteins(proteinGroups, mappedSequences, minPepPerProtein);
        }

        public static IEnumerable<ProteinGroup> GroupProteins(IEnumerable<Protein> proteins, IProtease protease, IEnumerable<IAminoAcidSequence> observeredSequences, IEqualityComparer<IAminoAcidSequence> peptideComparer, int MaxMissedCleavages = 3, int minPepPerProtein = 1)
        {
            return GroupProteins(proteins, new[] {protease}, observeredSequences, peptideComparer, MaxMissedCleavages);
        }

        private static IEnumerable<ProteinGroup> CombinedProteins(IList<ProteinGroup> proteins, Dictionary<IAminoAcidSequence, int> sharedPeptides, int minPeptidesPerProtein = 1)
        {
            // A list of protein groups that, at the end of this method, will have distinct protein groups.
            List<ProteinGroup> proteinGroups = new List<ProteinGroup>();

            // 1) Find Indistinguishable Proteins and group them together into Protein Groups
            // If they are not indistinguishable, then they are still converted to Protein Groups
            // but only contain one protein.
            // A 1 2 3 4
            // B 1 2 3 4
            // C 1   3 4
            // Proteins A and B are indistinguisable (have same set of peptides 1,2,3,4), and thus would become a Protein Group (PG1 [a,b])
            // C is distinguishable and would become a Protein Group (PG2 [c]).

            #region Indistinguishable

            // Loop over each protein
            int p1 = 0;
            while (p1 < proteins.Count)
            {
                // Grab the next protein and its associated peptides from the list of all proteins
                ProteinGroup proteinGroup = proteins[p1];
                HashSet<IAminoAcidSequence> peptides = proteinGroup.Peptides;

                // Start looking at the next protein in the list
                int p2 = p1 + 1;

                // Loop over each other protein skipping the one you just made into the PG
                while (p2 < proteins.Count)
                {
                    // Does the next protein contain the same set of peptides as the protein group?
                    if (proteins[p2].Peptides.SetEquals(peptides))
                    {
                        // Yes they are indistinguishable (i.e. proteins A and B from above), so add this protein to the protein group
                        foreach (var protein in proteins[p2].Proteins)
                            proteinGroup.AddProtein(protein);

                        // Then remove this protein from the list of all proteins as not to make it into its own PG later
                        proteins.RemoveAt(p2);
                    }
                    else
                    {
                        // Go to next protein in question
                        p2++;
                    }
                }

                // We have gone through every protein possible and thus have completed the grouping of this PG
                proteinGroups.Add(proteinGroup);
                p1++;
            }
            //if (printMessages)
            //    Log("{0:N0} protein groups are left after combining indistinguishable proteins (having the exact same set of peptides)", proteinGroups.Count);

            #endregion Indistinguishable

            // 2) Find Subsumable Proteins
            // Sort proteins from worst to best to remove the worst scoring groups first (note well, lower p-values mean better scores)
            // Case Example: P-Value, Protein Group, Peptides
            // 0.1  A 1 2
            // 0.05 B 1   3
            // 0.01 C   2 3
            // These are subsumable and we remove the worst scoring protein group (in this case, Protein Group A at p-value of 0.1) first. This would leave:
            // 0.05 B 1   3
            // 0.01 C   2 3
            // Which would mean Protein Group B and C are distinct groups, but share a common peptide (3), peptides 1 and 2 would remain unshared.
            // Protein Group A is removed, as it its peptides can be explained by groups B and C.

            #region Subsumable

            // First, make sure all the peptides know which protein groups they belong too, so we can determined shared peptides
            // and thus get correct p-value for the PGs.
            //MappedPeptidesToProteinGroups(proteinGroups);

            //// First update each protein's p-value
            //foreach (ProteinGroup proteinGroup in proteinGroups)
            //{
            //    proteinGroup.UpdatePValue(PScoreCalculationMethod, UseConservativePScore);
            //}

            //// Then sort the groups on decreasing p-values
            //proteinGroups.Sort(ProteinGroup.CompareDecreasing);

            p1 = 0;
            while (p1 < proteinGroups.Count)
            {
                // Get the peptides in the protein group
                ProteinGroup proteinGroup = proteinGroups[p1];
                HashSet<IAminoAcidSequence> referencePeptides = proteinGroup.Peptides;

                // Check if all the peptides are shared, if they are then the protein group is subsumable and should be removed
                if (referencePeptides.All(p => sharedPeptides[p] > 1))
                {
                    // Since this protein group is being eliminated, remove its reference from all the peptides
                    foreach (Peptide pep in referencePeptides)
                    {
                        int value = sharedPeptides[pep];
                        sharedPeptides[pep] = value - 1;
                    }

                    // Remove the protein group from the master list
                    proteinGroups.RemoveAt(p1);
                }
                else
                {
                    p1++;
                }
            }

            #endregion Subsumable

            // 3) Remove protein groups that do not have enough peptides within them

            #region MinimumGroupSize

            // No need to filter if this is one or less
            if (minPeptidesPerProtein > 1)
            {
                p1 = 0;
                while (p1 < proteinGroups.Count)
                {
                    ProteinGroup proteinGroup = proteinGroups[p1];

                    // Check to see if this protein has enough peptides to be considered indentified
                    if (proteinGroup.Peptides.Count < minPeptidesPerProtein)
                    {
                        //// Since this protein group is being eliminated, remove its reference from all the peptides
                        //foreach (Peptide pep in proteinGroup.Peptides)
                        //{
                        //    pep.ProteinGroups.Remove(proteinGroup);
                        //}

                        // This protein didn't have enough peptides, so remove it from future consideration
                        proteinGroups.RemoveAt(p1);
                    }
                    else
                    {
                        p1++;
                    }
                }
            }

            #endregion MinimumGroupSize

            //// 4) Apply false discovery filtering at the protein level

            #region FDR filtering

            //proteinGroups.Sort();
            //// Mark each protein group that passes fdr filtering
            //int count = 0;
            //foreach (ProteinGroup proteinGroup in FalseDiscoveryRate<ProteinGroup, double>.Filter(proteinGroups, MaxFdr / 100, true))
            //{
            //    proteinGroup.PassesFDR = true;
            //    count++;
            //}

            #endregion FDR filtering

            return proteinGroups;
        }

        #endregion Statics
    }
}