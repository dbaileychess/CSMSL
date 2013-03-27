using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CSMSL;
using CSMSL.IO;
using CSMSL.Spectral;
using CSMSL.IO.Thermo;


namespace CSMSL.Examples
{
    public class MsDataFileExamples
    {

        public static void Start()
        {
            Console.WriteLine("**MS I/O Examples**");
            Console.WriteLine("{0,-4} {1,3} {2,-6:F4} {3,-5} {4,7} {5,-10} {6}", "SN", "Msn", "RT", "Polarity", "# Peaks", "Analyzer", "M/Z Range");

            List<MSDataFile> exampleRawFiles = new List<MSDataFile>();
            exampleRawFiles.Add(new ThermoRawFile("Resources/ThermoRawFileMS1MS2.raw"));
            exampleRawFiles.Add(new Mzml("Resources/ThermoRawFileMS1MS2.mzML"));
            exampleRawFiles.Add(new Mzml("Resources/tiny.pwiz.1.1.mzML"));
            //exampleRawFiles.Add(new AgilentDDirectory("Resources/AgilentDDirectoryMS1MS2.d"));   
            //exampleRawFiles.Add(new BrukerDDirectory(@"E:\Software\Third Parties\BRUKER sample data & executable\BRUKER sample data & executable\10 fmol BSA_01_637.d"));
            //ex
            foreach (MSDataFile dataFile in exampleRawFiles)
            {
                dataFile.Open();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (MSDataScan scan in dataFile)
                {
                    List<MZPeak> peaks = null;
                    scan.MassSpectrum.TryGetPeaks(500, 510, out peaks);
                    //Console.WriteLine(peaks.Count);
                    Console.WriteLine("{0,-4} {1,3} {2,-6:F4} {3,-5} {4,7} {5,-10} {6}",
                        scan.SpectrumNumber,
                        scan.MsnOrder,
                        scan.RetentionTime,
                        scan.Polarity,
                        scan.MassSpectrum.Count,
                        scan.MzAnalyzer,
                        scan.MzRange);
                }
                watch.Stop();
                Console.WriteLine("File: {0}", dataFile.Name);
                Console.WriteLine("Time: {0}", watch.Elapsed);
                Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            }    
        }
    }
}
