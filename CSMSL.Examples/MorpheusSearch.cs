using CSMSL.Analysis.Identification;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using CSMSL.Util.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                        peptides.Add(peptide);                       
                    }          
                }
            }

            MSSearchEngine engine = new MorpheusSearchEngine();
            engine.PrecursorMassTolerance = MassTolerance.FromPPM(100);
            engine.ProductMassTolerance = MassTolerance.FromPPM(10);
         
            engine.LoadPeptides(peptides);
            using (MSDataFile msDataFile = new ThermoRawFile("Resources/ThermoRawFileMS1MS2.raw", true))
            {
                SortedMaxSizedContainer<PeptideSpectralMatch> psms = engine.Search(msDataFile.Where(scan => scan.MsnOrder > 1));
                
                //foreach (MSDataScan scan in msDataFile.Where(scan => scan.MsnOrder > 1))
                //{
                //    List<PeptideSpectralMatch> psms = engine.Search(scan);
                //    Console.WriteLine("{0} {1}", scan.SpectrumNumber, psms.Count);
                //}
            }
            watch.Stop();           
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            Console.WriteLine("**End Morpheus Search**");
        }
    }

}
