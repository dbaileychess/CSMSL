///////////////////////////////////////////////////////////////////////////
//  ExampleTrypticDigest.cs - An example tryptic digestion                /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSMSL.IO;
using CSMSL.Proteomics;

namespace CSMSL.Examples
{
    public class ExampleTrypticDigest
    {
        public static void Start()
        {
            Console.WriteLine("**Start Digestion**");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Protease trypsin = Protease.Trypsin;            
            List<Peptide> peps = new List<Peptide>();
            List<Protein> prots = new List<Protein>();
            List<double> allMzs = new List<double>();
            int maxMissed = 3;
            using (FastaReader reader = new FastaReader("Resources/yeast_uniprot_120226.fasta"))
            {
                foreach (Protein protein in reader.ReadNextProtein())
                {
                    foreach (Peptide peptide in protein.Digest(trypsin, maxMissed, 5, 35))
                    {
                        peps.Add(peptide);
                        allMzs.Add(peptide.Mass.ToMz(1));
                    }
                    prots.Add(protein);
                }
            }
            watch.Stop();
            Console.WriteLine("{0:N0} proteins produced {1:N0} peptides using {2:N0} missed clevages", prots.Count, peps.Count, maxMissed);
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
            Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
            Console.WriteLine("**End Digestion**");
        }
    }
}