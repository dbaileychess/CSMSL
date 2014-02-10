using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CSMSL.IO.PepXML;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

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
                writer.WriteProtease(Protease.Trypsin);
                writer.WriteModification(NamedChemicalFormula.Acetyl, ModificationSites.K);
            }

        }


    }
}
