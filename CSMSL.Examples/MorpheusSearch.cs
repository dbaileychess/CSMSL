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
using CSMSL.Analysis.Identification;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSMSL.Examples
{
    public class MorpheusSearch
    {
        public static void Start(IProtease protease, int maxMissed = 3, int minLength = 5, int maxLength = 35)
        {
            Console.WriteLine("**Start Morpheus Search**");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<int> hashCodes = new List<int>();
            // Generate peptide candidates

            HashSet<Peptide> peptides = new HashSet<Peptide>();
            using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(protease, maxMissed, minLength, maxLength))
                    {
                        peptides.Add(peptide);
                    }
                }
            }

            MSSearchEngine engine = new MorpheusSearchEngine();
            engine.PrecursorMassTolerance = Tolerance.FromPPM(100);
            engine.ProductMassTolerance = Tolerance.FromPPM(10);

            engine.LoadPeptides(peptides);
     
            watch.Stop();
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", Environment.WorkingSet/(1024*1024));
            Console.WriteLine("**End Morpheus Search**");
        }
    }
}