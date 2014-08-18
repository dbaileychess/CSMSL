// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (FileOutputExamples.cs) is part of CSMSL.
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
using System.Diagnostics;
using System.IO;
using System.Text;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using CSMSL.Spectral;

namespace CSMSL.Examples
{
    public static class FileOutputExamples
    {
        public static void Start()
        {
            BasicCsvWriting(Path.GetTempPath() + "BasicCsvWriting.csv");
            RawFileToCsv(new ThermoRawFile("Resources/ThermoRawFileMS1MS2.raw"), Path.GetTempPath() + "RawFileToCsv.csv");

            // Opens up a file browser to the temporary location the above files are written to
            Process.Start(Path.GetTempPath());
        }

        public static void BasicCsvWriting(string outputFilePath)
        {
            Console.WriteLine("Writing file to: " + outputFilePath);

            // Create a stream writer to output data to a stream. In this case, the stream points to a file path on the
            // computer and is saved as a file. The using statement is the same as doing the following:
            // Stream writer = new StreamWriter(outputFilePath);
            // writer.Open();
            // writer.WriteLine("Hello World");
            // writer.Close();
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                // Now that the stream is open, we can a line of text to it to serve as the header row.
                // CSV formats are just text files with fields seperated by commas.
                writer.WriteLine("Protein Name,# of Amino Acids,Mass (da)");

                // Open a connection to a fasta file (very similar syntax as the StreamWriter above)
                using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
                {
                    // Loop over each protein in the fasta file
                    foreach (Protein protein in reader.ReadNextProtein())
                    {
                        // StringBuilder objects are an effective tool for constructing strings to write to files.
                        StringBuilder sb = new StringBuilder();

                        // To add items to the string builder, just call the append method with whatever you want to add
                        sb.Append(protein.Description);
                        sb.Append(','); // we need to add the delimiter after each field we add as well.

                        // Add the next item
                        sb.Append(protein.Length);
                        sb.Append(',');

                        // The append method can take any object, it will simply call the .ToString() method on the object supplied.
                        sb.Append(protein.MonoisotopicMass);

                        // No delimiter is needed after the last field is added.

                        // Now to write this string to the file itself.
                        // We convert the StringBuilder object (named: sb) to a string, and then write it on its own line in the 
                        // file writer (named: writer)
                        writer.WriteLine(sb.ToString());
                    }
                }
            } // The file is automatically written and closed after exiting the using {} block. 
        }

        public static void RawFileToCsv<T>(MSDataFile<T> msDataFile, string outputFilePath) where T : ISpectrum
        {
            // See the BasicCsvWriting example for more details

            Console.WriteLine("Writing file to: " + outputFilePath);

            // The main field delimiter, for CSV it is a comma
            const char delimiter = ',';

            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                // Now that the stream is open, we can a line of text to it to serve as the header row.
                // CSV formats are just text files with fields separated by commas.
                writer.WriteLine("Spectrum Number,MS Level,Precursor m/z,# of Peaks in Spectrum,m/z Range");

                // The using statement on the msDataFile will ensure that things are properly disposed of after
                // you are done using the file, or if an exception has occurred.
                using (msDataFile)
                {
                    // MsDataFiles require opening their data connection manually.
                    msDataFile.Open();

                    // Simply loop over every data scan in the file
                    foreach (MSDataScan<T> scan in msDataFile)
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Append(scan.SpectrumNumber);
                        sb.Append(delimiter);

                        sb.Append(scan.MsnOrder);
                        sb.Append(delimiter);

                        // We need to check if this is an MS1 or MS2 scan. Both scans are of type MSDataScan,
                        // but for MS2 scans, there is additional information (precursor m/z for example). So we need
                        // to cast it to MsnDataScan to access those properties.
                        // Try casting using the 'as' keyword. It will either return the cast object or null, so we need to
                        // check for null before trying to access a property of it.
                        MsnDataScan<T> msnScan = scan as MsnDataScan<T>;
                        if (msnScan != null)
                        {
                            // It is a MS2 scan

                            sb.Append(msnScan.GetPrecursorMz());
                        }
                        else
                        {
                            // It is a MS1 scan, so no precursor information. So lets put Not an Number in this column.
                            // you could skip this if you want to have blank cells.
                            sb.Append(double.NaN);
                        }
                        sb.Append(delimiter);

                        // We can just use the original scan object now
                        sb.Append(scan.MassSpectrum.Count);
                        sb.Append(delimiter);

                        sb.Append(scan.MzRange);

                        writer.WriteLine(sb.ToString());
                    }
                }
            }
        }
    }
}