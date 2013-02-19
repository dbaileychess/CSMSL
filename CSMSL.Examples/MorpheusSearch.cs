using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CSMSL.IO;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;

namespace CSMSL.Examples
{
    public class MorpheusSearch
    {
        public static void Start(IProtease protease, int maxMissed = 3, int minLength = 5, int maxLength = 35)
        {
            Console.WriteLine("**Start Morpheus Search**");
            Stopwatch watch = new Stopwatch();
            watch.Start();          
            List<int> hashCodes = new List<int>();
            // Generate peptide candidates

            HashSet<Peptide> peptides = new HashSet<Peptide>();            
            using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(protease, maxMissed, minLength, maxLength))
                    {
                        //hashCodes.Add(peptide.GetHashCode());
                        peptides.Add(peptide);                       
                    }          
                }
            }

            Morpheus engine = new Morpheus();

            watch.Stop();           
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            Console.WriteLine("**End Morpheus Search**");
        }
    }

}
