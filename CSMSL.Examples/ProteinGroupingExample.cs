using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSMSL.IO;
using CSMSL.Proteomics;

namespace CSMSL.Examples
{
    public static class ProteinGroupingExample
    {

        public static void Start(IProtease protease, double percentIdentified = 0.05, int maxMissed = 3, int minLength = 5, int maxLength = 35)
        {
            Console.WriteLine("**Start Protein Grouping**");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Peptide> peps = new List<Peptide>();
            List<Protein> proteins = new List<Protein>();
            
            using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(protease, maxMissed, minLength, maxLength))
                    {
                        peps.Add(peptide);
                    }
                    proteins.Add(protein);
                }
            }
            Console.WriteLine("Loaded {0:N0} peptides from {1:N0} proteins in {2} ms", peps.Count, proteins.Count, watch.ElapsedMilliseconds);

            // Fixed seed to make it reproducible
            Random random = new Random(480912341);

            // Take the first x % to act as our identified peptides
            List<Peptide> identifiedPeptides = peps.OrderBy(x => random.Next()).Take((int) (peps.Count*percentIdentified)).ToList();
            
            List<ProteinGroup> proteinGroups = ProteinGroup.GroupProteins(proteins, protease, identifiedPeptides, new AminoAcidLeucineSequenceComparer(), maxMissed).ToList();


            watch.Stop();
            Console.WriteLine("{0:N0} proteins produced {1:N0} protein groups from {2:N0} identified sequences", proteins.Count, proteinGroups.Count, identifiedPeptides.Count);
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            Console.WriteLine("**End Digestion**");

        }

        public static void StartRamp(IProtease protease, double percentIdentifiedSteps = 0.05, int maxMissed = 3, int minLength = 5, int maxLength = 35)
        {
            List<Peptide> peps = new List<Peptide>();
            List<Protein> proteins = new List<Protein>();

            using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(protease, maxMissed, minLength, maxLength))
                    {
                        peps.Add(peptide);
                    }
                    proteins.Add(protein);
                }
            }

            // Fixed seed to make it reproducible
            Random random = new Random(480912341);
            peps = peps.OrderBy(x => random.Next()).ToList();
            
            for (double percentIdentified = 0; percentIdentified <= 1; percentIdentified += percentIdentifiedSteps)
            {
                // Take the first x % to act as our identified peptides
                List<Peptide> identifiedPeptides = peps.Take((int) (peps.Count*percentIdentified)).ToList();

                List<ProteinGroup> proteinGroups = ProteinGroup.GroupProteins(proteins, protease, identifiedPeptides, new AminoAcidLeucineSequenceComparer(), maxMissed).ToList();
                Console.WriteLine("{0} peptides {1} protein groups", identifiedPeptides.Count, proteinGroups.Count);
            }
            
        }
    }
}
