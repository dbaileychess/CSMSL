using CSMSL.Analysis.ExperimentalDesign;
using CSMSL.Analysis.Identification;
using CSMSL.Analysis.Quantitation;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.OMSSA;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSMSL.Examples
{
    public class TMT6plexExample
    {
        public static void Start()
        {
            Console.WriteLine("**Start TMT 6-plex Experiment**");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Quantify();
            watch.Stop(); 
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            Console.WriteLine("**End TMT 6-plex Experiment**");
        }

        public static void PurityCorrection()
        {
            Console.WriteLine("**Start TMT 6-plex Experiment**");
            Stopwatch watch = new Stopwatch();
            watch.Start();

           // IsobaricTagPurityCorrection purityMatrix = IsobaricTagPurityCorrection.Create();
           

            watch.Stop();
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            Console.WriteLine("**End TMT 6-plex Experiment**");
        }

        private static void Quantify()
        {
            // Initial input files
            string fastaFile = "Resources/yeast_uniprot_120226.fasta";
            string psmFile = "Resources/TMT_6plex_yeast_omssa_psms.csv";
            MSDataFile dataFile = new ThermoRawFile("Resources/TMT_6plex_yeast.raw");
            IProtease protease = Protease.Trypsin;

            // Set up experimental conditions
            Sample sample = new Sample("Yeast", "Tryptic digestion");
            QuantitationChannelSet tmt6plex = QuantitationChannelSet.TMT6PlexHeavy;

            ExperimentalCondition cond1_10 = sample.AddCondition("10").AddQuantChannel(tmt6plex["126"]);
            ExperimentalCondition cond1_5 = sample.AddCondition("5").AddQuantChannel(tmt6plex["127h"]);
            ExperimentalCondition cond1_1 = sample.AddCondition("1").AddQuantChannel(tmt6plex["128l"]);
            ExperimentalCondition cond2_1 = sample.AddCondition("1").AddQuantChannel(tmt6plex["129h"]);
            ExperimentalCondition cond2_5 = sample.AddCondition("5").AddQuantChannel(tmt6plex["130l"]);
            ExperimentalCondition cond2_10 = sample.AddCondition("10").AddQuantChannel(tmt6plex["131"]);

            ExperimentalSet experiment = new ExperimentalSet();
            experiment.Add(cond1_1);
            experiment.Add(cond1_5);
            experiment.Add(cond1_10);
            experiment.Add(cond2_10);
            experiment.Add(cond2_5);
            experiment.Add(cond2_1);

            // PSM loading
            List<PeptideSpectralMatch> psms;
            using (PsmReader psmReader = new OmssaCsvPsmReader(psmFile))
            {
                psmReader.LoadProteins(fastaFile);
                psmReader.AddMSDataFile(dataFile);

                // Set modifications
                //psmReader.AddFixedModification(NamedChemicalFormula.Carbamidomethyl, ModificationSites.C);
                //psmReader.AddFixedModification(tmt6plex, ModificationSites.K | ModificationSites.NPep);
                psmReader.AddVariableModification(NamedChemicalFormula.Oxidation, "oxidation of M");
                psmReader.AddVariableModification(tmt6plex, "TMT_Tyrosine");

                psms = psmReader.ReadNextPsm().ToList();
            }

            List<QuantifiedPeptide> quantPeptides = QuantifiedPeptide.GroupPeptideSpectralMatches(psms).ToList();

            QuantifiedPeptide.Quantify(quantPeptides, sample);

            // Example of comparing the ratio of two conditions
            QuantifiedPeptide qpep = quantPeptides[0];

            //double ratio = qpep[cond1_10].Intensity / qpep[cond1_1].Intensity;

            Console.WriteLine(string.Join("\t",experiment.Select(cond => cond.Name)));

            foreach (QuantifiedPeptide quantPeptide in quantPeptides)
            { 
                //Console.Write(quantPeptide.Peptide);
                foreach (ExperimentalCondition cond in experiment)
                {
                    QuantifiedPeakSet quant = quantPeptide[cond];
                    if (quant != null)
                    {
                        Console.Write(quant.DeNormalizedIntensity.ToString("e3"));
                    }
                    else
                    {
                        Console.Write("n/a");
                    }
                    Console.Write("\t");
                }
                Console.WriteLine();
            }         
            
        }
    }
}
