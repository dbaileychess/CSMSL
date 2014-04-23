using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.IO.MzIdentML;

namespace CSMSL.Examples
{
    public class MzIdentMLExamples
    {

        public static void ReadMzIdentML()
        {
            //string filePath = "Resources/omssa_example.mzid";
            string filePath = System.IO.Path.Combine(Examples.BASE_DIRECTORY, "example2.mzid");
            MzidReader reader = new MzidReader(filePath);
            reader.Open();

            string version = reader.Version;

            Console.WriteLine("Version: " + version);

            //reader.Version = "1.1.1";

            string outputPath = System.IO.Path.Combine(Examples.BASE_DIRECTORY, "example.mzid");

            reader.SaveAs(outputPath);

        }

    }
}
