using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CSMSL.IO.OMSSA;
using CSMSL.Analysis.Identification;

namespace CSMSL.Examples
{
    public class OmssaReader
    {
        public static void Start()
        {
            Console.WriteLine("**Start OMSSA Reader**");
            long startMem = System.Environment.WorkingSet;
            Stopwatch watch = new Stopwatch();          
            watch.Start();
            List<PeptideSpectralMatch> psms;
            using (OmssaCsvReader reader = new OmssaCsvReader())
            {
                reader.LoadProteins("Resources/yeast_uniprot_120226.fasta");
              
                psms = reader.ReadPsmsFrom("Resources/Omssa_yeast.csv").ToList();
            }
         
            
            watch.Stop();
            Console.WriteLine("{0:N0} psms were read in", psms.Count);
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", (System.Environment.WorkingSet - startMem) / (1024 * 1024));
            Console.WriteLine("**End Digestion**");
        }


    }
}
