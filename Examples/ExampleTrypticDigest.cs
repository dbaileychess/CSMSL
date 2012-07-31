using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSMSL.IO;
using CSMSL.Proteomics;

namespace ExamplesCSMSL
{
    public class ExampleTrypticDigest
    {
        public static void Start()
        {
            Console.WriteLine("**Start Digestion**");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Protease trypsin = ProteaseDictionary.Instance["Trypsin"];
            List<Peptide> peps = new List<Peptide>();
            List<Protein> prots = new List<Protein>();
            List<double> allMzs = new List<double>();
            int maxMissed = 3;
            using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(trypsin, maxMissed, 5, 35))
                    {
                        peps.Add(peptide);
                        allMzs.Add(peptide.Mass.ToMz(1));
                    }
                    prots.Add(protein);
                }
            }
            watch.Stop();
            Console.WriteLine("{0:N0} proteins produced {1:N0} peptides using {2:N0} missed clevages", prots.Count, peps.Count, maxMissed);
            Console.WriteLine("**End Digestion**");
            Console.WriteLine(watch.Elapsed);
        }
    }
}