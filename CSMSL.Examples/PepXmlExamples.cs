// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (PepXmlExamples.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using CSMSL.IO.PepXML;
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
                writer.WriteSampleProtease(Protease.GetProtease("Trypsin"));

                writer.StartSearchSummary("OMSSA", true, true);

                writer.WriteProteinDatabase("Resources/yeast_uniprot_120226.fasta");

                writer.WriteSearchProtease(Protease.GetProtease("Trypsin"), 3);

                writer.WriteModification(ModificationDictionary.GetModification("Acetyl"), ModificationSites.K | ModificationSites.NPep);
                writer.WriteModification(ModificationDictionary.GetModification("CAM"), ModificationSites.C);

                writer.WriteModification(ModificationDictionary.GetModification("Phospho"), ModificationSites.S | ModificationSites.T | ModificationSites.Y, false);

                writer.SetCurrentStage(PepXmlWriter.Stage.Spectra, true);

                writer.StartSpectrum(15, 1.234, 523.4324, 3);

                PeptideSpectralMatch psm = new PeptideSpectralMatch(PeptideSpectralMatchScoreType.OmssaEvalue);
                psm.Score = 1.5e-5;
                Protein protein = new Protein("", "Test Protein");
                psm.Peptide = new Peptide("DEREK", protein);
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