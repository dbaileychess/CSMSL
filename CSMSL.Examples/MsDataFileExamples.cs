// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Linq;
using CSMSL.IO;
using CSMSL.IO.Agilent;
using CSMSL.IO.MzML;
using CSMSL.IO.Thermo;
using CSMSL.Spectral;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace CSMSL.Examples
{
    public static class MsDataFileExamples
    {
        /// <summary>
        /// Code to show how one could write one set of code to interact with multiple different types of MS data files.
        /// 
        /// </summary>
        public static void VendorNeutralDataAccess()
        {
            
            Console.WriteLine("**MS I/O Examples**");
            Console.WriteLine("{0,-4} {1,3} {2,-6} {3,-5} {4,7} {5,-10} {6}", "SN", "Msn", "RT", "Polarity", "# Peaks", "Analyzer", "M/Z Range");

            // Generic MS data file reader
            List<IMSDataFile> exampleRawFiles = new List<IMSDataFile>
            {
                new ThermoRawFile("Resources/ThermoRawFileMS1MS2.raw"),
                new AgilentDDirectory(@"Resources\AgilentDDirectoryMS1MS2.d"),
                new Mzml("Resources/ThermoRawFileMS1MS2_Profile.mzML")
            };

            foreach (IMSDataFile dataFile in exampleRawFiles)
            {
                dataFile.Open();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (IMSDataScan scan in dataFile.Where(s => s.MassSpectrum.Count > 1).Take(4))
                {
                    Console.WriteLine("{0,-4} {1,3} {2,-6:F4} {3,-5} {4,7} {5,-10} {6}", scan.SpectrumNumber, scan.MsnOrder, scan.RetentionTime,
                        scan.Polarity, scan.MassSpectrum.Count, scan.MzAnalyzer, scan.MzRange);
                }
                watch.Stop();
                Console.WriteLine("File: {0}", dataFile);
                Console.WriteLine("Time: {0}", watch.Elapsed);
                Console.WriteLine("Memory used: {0:N0} MB", Environment.WorkingSet/(1024*1024));
            }
        }

      
    }
}