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
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSMSL.Examples
{
    public class TrypticDigestion
    {
        public static void Start(IProtease protease, int maxMissed = 1, int minLength = 0, int maxLength = int.MaxValue, bool storeSequenceString = true)
        {
            Console.WriteLine("**Start Digestion**");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Peptide> peps = new List<Peptide>();
            List<Protein> prots = new List<Protein>();
            List<double> allMzs = new List<double>();
            AminoAcidPolymer.StoreSequenceString = storeSequenceString;
            using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(protease, maxMissed, minLength, maxLength))
                    {
                        peps.Add(peptide);
                        allMzs.Add(peptide.ToMz(2)); // forces the calculation of the mass and thus chemical formula
                    }
                    prots.Add(protein);
                }
            }
            watch.Stop();
            Console.WriteLine("{0:N0} proteins produced {1:N0} peptides using {2:N0} missed cleavages", prots.Count, peps.Count, maxMissed);
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", Environment.WorkingSet/(1024*1024));
            Console.WriteLine("**End Digestion**");
        }

        public static void ExampleDigestion()
        {
            const string fastaFilePath = "Resources/yeast_uniprot_120226.fasta";
            IProtease trypsin = Protease.GetProtease("Trypsin");
            const int maxMissedCleavages = 3;
            const int minPeptideLength = 5;
            const int maxPeptideLength = 50;
            List<double> masses = new List<double>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (FastaReader reader = new FastaReader(fastaFilePath))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(trypsin, maxMissedCleavages, minPeptideLength, maxPeptideLength))
                    {
                        masses.Add(peptide.MonoisotopicMass);
                    }
                }
            }
            //Console.WriteLine("Average Peptide Mass = {0:F4}", masses.Average());
            watch.Stop();
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
        }
    }
}