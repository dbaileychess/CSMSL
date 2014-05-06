using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CSMSL.IO.PepXML;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;

namespace CSMSL.Examples
{
    public static class PepXmlExamples
    {
        public static void WritePepXml()
        {
            string filePath = Path.Combine(Examples.BASE_DIRECTORY, "example.pepXML");

            Console.WriteLine("Writting to " + filePath);
            using (PepXmlWriter writer = new PepXmlWriter(filePath))
            {                
                writer.WriteSampleProtease(Protease.Trypsin);

                writer.StartSearchSummary("OMSSA", true, true);

                writer.WriteProteinDatabase("Resources/yeast_uniprot_120226.fasta");

                writer.WriteSearchProtease(Protease.Trypsin, 3);

                writer.WriteModification(ModificationDictionary.GetModification("Acetyl"), ModificationSites.K | ModificationSites.NPep);
                writer.WriteModification(ModificationDictionary.GetModification("CAM"), ModificationSites.C);

                writer.WriteModification(ModificationDictionary.GetModification("Phospho"), ModificationSites.S | ModificationSites.T | ModificationSites.Y, false);

                writer.SetCurrentStage(PepXmlWriter.Stage.Spectra, true);

                writer.StartSpectrum(15, 1.234, 523.4324, 3);

                PeptideSpectralMatch psm = new PeptideSpectralMatch(PeptideSpectralMatchScoreType.OmssaEvalue);
                psm.Score = 1.5e-5;
                Protein protein = new Protein("", "Test Protein");
                psm.Peptide = new Peptide("DEREK",protein);
                psm.Charge = 3;
                writer.WritePSM(psm);

                writer.EndSpectrum();
            }

        }

        public static void ReadPepXml()
        {
            WritePepXml();
            //string filePath = Path.Combine(Examples.BASE_DIRECTORY, "example.pepXML");
            string filePath = @"E:\Desktop\test\27Nov2013_CEM_WellsProtein_CAD_filter.pep.xml";
            Console.WriteLine("Reading from " + filePath);
            using (PepXmlReader reader = new PepXmlReader(filePath))
            {
                Protease protease = reader.GetSampleProtease();
                Console.WriteLine("Protease: " + protease);
                Console.WriteLine();
                List<Modification> fixedMods = reader.GetFixedModifications();
                Console.WriteLine("==Fixed Modifications==");
                foreach (Modification mod in fixedMods)
                {
                    Console.WriteLine("\t" + mod);
                }
                Console.WriteLine();
                List<Modification> varMods = reader.GetVariableModifications();
                Console.WriteLine("==Variable Modifications==");
                foreach (Modification mod in varMods)
                {
                    Console.WriteLine("\t" + mod);
                }
            }

        }
    }
}
