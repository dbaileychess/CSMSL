using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.OMSSA;
using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            using (PsmReader reader = new OmssaCsvPsmReader("Resources/Omssa_yeast.csv"))
            {               
                reader.LoadProteins("Resources/yeast_uniprot_120226.fasta");

                reader.AddFixedModification(new Modification(NamedChemicalFormula.Carbamidomethyl.MonoisotopicMass,
                    NamedChemicalFormula.Carbamidomethyl.Name, ModificationSites.C));
                reader.AddFixedModification(new Modification(NamedChemicalFormula.TMT6plex.MonoisotopicMass,NamedChemicalFormula.TMT6plex.Name, ModificationSites.NPep | ModificationSites.K));                
                reader.AddVariableModification(NamedChemicalFormula.Oxidation, "oxdiation of M");
                psms = reader.ReadNextPsm().OrderBy(psm => psm.Score).ToList();
            }

            watch.Stop();
            Console.WriteLine("{0:N0} psms were read in", psms.Count);          
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", (System.Environment.WorkingSet - startMem) / (1024 * 1024));
            Console.WriteLine("**End OMSSA Reader**");
        }

    }
}
