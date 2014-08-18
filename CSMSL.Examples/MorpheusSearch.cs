// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MorpheusSearch.cs) is part of CSMSL.
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