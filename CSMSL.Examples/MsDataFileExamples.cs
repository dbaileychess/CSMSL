using CSMSL.IO;

using CSMSL.IO.MzML;
using CSMSL.IO.Thermo;
using CSMSL.IO.Agilent;
using CSMSL.Spectral;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChromatogramType = CSMSL.Spectral.ChromatogramType;

namespace CSMSL.Examples
{
    public static class MsDataFileExamples
    {

        public static void Start()
        {
            Console.WriteLine("**MS I/O Examples**");
            Console.WriteLine("{0,-4} {1,3} {2,-6} {3,-5} {4,7} {5,-10} {6}", "SN", "Msn", "RT", "Polarity", "# Peaks", "Analyzer", "M/Z Range");
     
            List<MSDataFile> exampleRawFiles = new List<MSDataFile>
                {
                    //new ThermoRawFile("Resources/ThermoRawFileMS1MS2.raw"),
                    //new AgilentDDirectory(@"Resources\AgilentDDirectoryMS1MS2.d"),
                    //new WiffFile(@"Resources/Enolase_repeats_AQv1.4.2.wiff")
                    //new Mzml("Resources/ThermoRawFileMS1MS2_Profile.mzML"),
                    //new Mzml("Resources/ThermoRawFileMS1MS2_Centroided.mzML")
                };

            foreach (MSDataFile dataFile in exampleRawFiles)
            {
                dataFile.Open();               
                Stopwatch watch = new Stopwatch();
                watch.Start();
                
                foreach (MSDataScan scan in dataFile)
                {
                    List<MZPeak> peaks;
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
                Console.WriteLine("Memory used: {0:N0} MB", Environment.WorkingSet / (1024 * 1024));
            }    
        }

        //public static void WiffExample()
        //{
        //    WiffFile.AddLicense("<?xml version=\"1.0\" encoding=\"utf-8\"?><license_key>    <company_name>Promega Corporation|Re-Distributable Beta Agreement 2013-06-04</company_name>    <product_name>ProcessingFramework</product_name>    <features>WiffReader SDK</features>    <key_data>        coGue7N5kug7nWfiCQLXDWlJHvBhQJQ33hDLSkvG4JGd0w2wkjaehw==    </key_data></license_key>");
        //    WiffFile wiff = new WiffFile(@"Resources/Enolase_repeats_AQv1.4.2.wiff");
        //    wiff.Open();

        //    string[] experiments = wiff.GetSampleNames();
        //    wiff.SetActiveSample(2);
        //    var spectrum = wiff.GetMzSpectrum(100);

        //}
    }
}
