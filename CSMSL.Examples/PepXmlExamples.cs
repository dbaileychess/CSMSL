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

                writer.WriteModification(NamedChemicalFormula.Acetyl, ModificationSites.K);

                writer.SetCurrentStage(PepXmlWriter.Stage.Spectra, true);


                writer.StartSpectrum(15, 1.234, 523.4324, 3);

                PeptideSpectralMatch psm = new PeptideSpectralMatch(PeptideSpectralMatchScoreType.OmssaEvalue);
                psm.Score = 1.5e-5;
                Protein protein = new Protein("", "Test Protein");
                psm.Peptide = new Peptide("DEREK",protein);
                psm.Charge = 3;
                writer.WritePSM(psm, null);

                writer.EndSpectrum();
            }

        }


    }
}
