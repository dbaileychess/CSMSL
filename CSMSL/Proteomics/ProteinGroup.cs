using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.IO;

namespace CSMSL.Proteomics
{
    public class ProteinGroup : IEnumerable<Protein>, IEquatable<ProteinGroup>
    {
        public HashSet<Protein> Proteins { get; set; }
        public HashSet<string> Peptides { get; set; }

        public ProteinGroup()
        {
            Proteins = new HashSet<Protein>();
            Peptides = new HashSet<string>();        
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
            if (obj is ProteinGroup)
                return this.Equals((ProteinGroup)obj);
            return false;
        }

        public bool Equals(ProteinGroup other)
        {
            return Proteins.SetEquals(other.Proteins);
        }

        public ProteinGroup(Protein protein)
            : this()
        {
            Add(protein);
        }

        public int Count { get { return Proteins.Count; } }

        public void Add(Protein protein)
        {
            if(protein == null)
                return;
            Proteins.Add(protein);
        }

        public void Add(string peptide)
        {
            if (peptide == null)
                return;
            Peptides.Add(peptide);
        }

        public void Combine(ProteinGroup pg)
        {
            Proteins.UnionWith(pg.Proteins);
        }

        public IEnumerator<Protein> GetEnumerator()
        {
            return Proteins.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Proteins.GetEnumerator();
        }

        #region Statics

        private static Dictionary<Peptide, ProteinGroup> _pepToProts;

        public static IEnumerable<ProteinGroup> GroupProteins(string fastaFile, Protease protease, IEnumerable<IAminoAcidSequence> observeredSequences, int MaxMissedCleavages = 3)
        {
            return GroupProteins(fastaFile, new Protease[] { protease }, observeredSequences, MaxMissedCleavages);
        }

        public static IEnumerable<ProteinGroup> GroupProteins(string fastaFile, IEnumerable<Protease> proteases, IEnumerable<IAminoAcidSequence> observeredSequences, int MaxMissedCleavages = 3, int minPepPerProtein = 1)
        {
            HashSet<ProteinGroup> mappedProteins = new HashSet<ProteinGroup>();

            HashSet<string> peptideSequences = new HashSet<string>(observeredSequences.Select(p => p.GetLeucineSequence()));
            int smallestPeptide = peptideSequences.Min(seq => seq.Length) - 1;
            int largestPeptide = peptideSequences.Max(seq => seq.Length) + 1;

            using (FastaReader reader = new FastaReader(fastaFile))
            {
                // Read in each protein one-by-one
                foreach (Protein protein in reader.ReadNextProtein())
                {                   
                    foreach (string peptide in AminoAcidPolymer.Digest(protein.GetLeucineSequence() ,proteases, MaxMissedCleavages, smallestPeptide, largestPeptide))
                    {
                        if (peptideSequences.Contains(peptide))
                        {
                            ProteinGroup pg = new ProteinGroup(protein);

                            mappedProteins.Add(pg);

                            pg.Add(peptide);
                        }
                    }
                }
            }

            return CombinedProteins(new List<ProteinGroup>(mappedProteins), minPepPerProtein);
        }

        private static IEnumerable<ProteinGroup> CombinedProteins(List<ProteinGroup> proteins, int minPeptidesPerProtein = 1)
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
                HashSet<string> peptides = proteinGroup.Peptides;
     
                // Start looking at the next protein in the list
                int p2 = p1 + 1;

                // Loop over each other protein skipping the one you just made into the PG
                while (p2 < proteins.Count)
                {
                    // Does the next protein contain the same set of peptides as the protein group?
                    if (proteins[p2].Peptides.SetEquals(peptides))
                    {
                        // Yes they are indistinguishable (i.e. proteins A and B from above), so add this protein to the protein group   
                        proteinGroup.Combine(proteins[p2]);

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

            #endregion Indistinguishable

            // 2) Find Subsumable Proteins
            // Sort proteins from worst to best to remove the worst scoring groups first (note well, lower p-values mean better scores)
            // Case Example: P-Value, Protein, Peptides
            // 0.1  A 1 2
            // 0.05 B 1   3
            // 0.01 C   2 3
            // These are subsumable and we remove the worst scoring protein (in this case, Protein A at p-value of 0.1) first. This would leave:
            // 0.05 B 1   3
            // 0.01 C   2 3
            // Which would mean Protein Group B and C are seperate groups, but share a common peptide (3), peptides 1 and 2 would remain unshared.
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

            // Loop over each protein group
            p1 = 0;
            while (p1 < proteinGroups.Count)
            {
                // Get the peptides in the protein group
                ProteinGroup proteinGroup = proteinGroups[p1];
                HashSet<string> reference_peptides = new HashSet<string>(proteinGroup.Peptides);

                bool subsumable_protein_group = false;

                // Loop over each protein group again
                for (int p2 = 0; p2 < proteinGroups.Count; p2++)
                {
                    // Don't compare the same protein group to each other, move to the next protein group then
                    if (p1 == p2)
                    {
                        continue;
                    }

                    // Remove all the pepetides that are in the second protein group (p2) from the peptides in the first protein group (p1, reference_peptides);
                    reference_peptides.ExceptWith(proteinGroups[p2].Peptides);

                    // If the first protein group (p1) has no peptides left, it is subsumable (e.g. Protein A in above example, Peptides 1 and 2 are found in other groups)
                    if (reference_peptides.Count == 0)
                    {
                        subsumable_protein_group = true;
                        break;
                    }
                }

                // Remove the group since it was subsumable and has a worst p-value then the other groups.
                if (subsumable_protein_group)
                {    
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
                        // This protein didn't have enough peptides, so remove it from future consideration
                        proteinGroups.RemoveAt(p1);
                    }
                    else
                    {
                        p1++;
                    }
                }              
            }

            #endregion

            return proteinGroups;
        }

        #endregion
        
    }

    class AminoAcidSequenceComparer : IEqualityComparer<IAminoAcidSequence>
    {
        public bool Equals(IAminoAcidSequence x, IAminoAcidSequence y)
        {
            return x.Sequence.Equals(y.Sequence);
        }

        public int GetHashCode(IAminoAcidSequence obj)
        {
            return obj.Sequence.GetHashCode();
        }
    }

    class AminoAcidLeucineSequenceComparer : IEqualityComparer<IAminoAcidSequence>
    {    

        public int GetHashCode(IAminoAcidSequence obj)
        {
            return obj.GetLeucineSequence().GetHashCode();
        }

        public bool Equals(IAminoAcidSequence x, IAminoAcidSequence y)
        {
            return x.GetLeucineSequence().Equals(y.GetLeucineSequence());
        }
    }
}
