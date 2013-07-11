using CSMSL.Proteomics;

namespace CSMSL.Examples
{
    public static class ProteinGroupingExample
    {

        public static void Start(IProtease protease, int maxMissed = 3, int minLength = 5, int maxLength = 35)
        {
            //Console.WriteLine("**Start Protein Grouping**");
            //Stopwatch watch = new Stopwatch();
            //watch.Start();          
            //List<Peptide> peps = new List<Peptide>();
            //List<Protein> prots = new List<Protein>();           
            //using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            //{
            //    foreach (Protein protein in reader.ReadNextProtein())
            //    {
            //        foreach (Peptide peptide in protein.Digest(protease, maxMissed, minLength, maxLength))
            //        {
            //            peps.Add(peptide);                      
            //        }
            //        prots.Add(protein);
            //    }
            //}

            //List<ProteinGroup> groups = new List<ProteinGroup>();
            //ProteinGroup.SetProteins(prots, protease, maxMissed, minLength, maxLength);
            //foreach (Peptide pep in peps)
            //{
            //    ProteinGroup pg = ProteinGroup.Group(pep);
            //    groups.Add(pg);
            //}

            //watch.Stop();
            //Console.WriteLine("{0:N0} proteins produced {1:N0} protein groups using {2:N0} missed clevages", prots.Count, groups.Count, maxMissed);
            //Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            //Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            //Console.WriteLine("**End Digestion**");

        }
    }
}
