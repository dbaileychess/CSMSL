///////////////////////////////////////////////////////////////////////////
//  Examples.cs - A collection of examples of how to use CSMSL            /
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using CSMSL;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Util;
using CSMSL.IO.Agilent;
using CSMSL.IO.Thermo;
using CSMSL.IO;
using CSMSL.Spectral;

namespace ExamplesCSMSL
{
    internal class Examples
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("==CSMSL Examples==");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            StartExamples();
            watch.Stop();
            Console.WriteLine("==================");
            Console.WriteLine(watch.Elapsed);
            Console.ReadKey();
        }

        private static void StartExamples()
        {
            // Examples coding
            //ChemicalFormulaExamples();
           // PeptideExamples();

            // Example Objects
            //VennDiagramExamples();

            // Example programs
            //ExampleTrypticDigest.Start();

            //Example IO
            MsIOExamples();
        }

        private static void MsIOExamples()
        {
            Console.WriteLine("**MS I/O Examples**");

            Console.WriteLine("{0,-4} {1,3} {2,-6:F4} {3,-5} {4,7} {5,-10} {6}", "SN", "Msn", "RT", "Polarity", "# Peaks", "Analyzer", "M/Z Range");
            string rawfile = @"C:\Users\Derek\Documents\promega\Promega_Dilutions\120731_Promega_PeptideMix1_Heavy_1.raw";// "Resources/ThermoRawFileMS1MS2.raw"
            // Ms Data Files implement IDispoable making using statements an excellent way to manage resources
            using (MsDataFile dataFile = new ThermoRawFile(rawfile, true))
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
               // Chromatogram chrom = dataFile.GetChromatogram(ChromatogramType.MzRange, new Range(553.79, 553.81));
                //foreach (MsScan scan in dataFile)
                //{
                //    Console.WriteLine("{0,-4} {1,3} {2,-6:F4} {3,-5} {4,7} {5,-10} {6}",
                //        scan.SpectrumNumber,
                //        scan.MsnOrder,
                //        scan.RetentionTime,
                //        scan.Polarity,
                //        scan.Spectrum.Count,
                //        scan.MzAnalyzer,
                //        scan.MzRange);
                //}
                watch.Stop();
                watch.Restart();
                Chromatogram chrom = dataFile.GetChromatogram(ChromatogramType.MzRange, new Range(553.79, 553.81));
                watch.Stop();
                Console.WriteLine("Time: {0}", watch.Elapsed);
            }

            //// Ms Data Files implment IDispoable making using statements an excellent way to manage resources
            //using(MsDataFile dataDirectory = new AgilentDDirectory("Resources/AgilentDDirectoryMS1MS2.d", true))
            //{
            //    foreach (MsScan scan in dataDirectory.OfType<MsScan>())
            //    {
            //        Console.WriteLine("{0,-4} {1,3} {2,-6:F4} {3,-5} {4,7} {5,-10} {6}",
            //            scan.SpectrumNumber,
            //            scan.MsnOrder,
            //            scan.RetentionTime,
            //            scan.Polarity,
            //            scan.Spectrum.Count,
            //            scan.MzAnalyzer,                      
            //            scan.MzRange);
            //    }              
            //}
            Console.WriteLine("Memory used: {0:N0} MB", System.Environment.WorkingSet / (1024 * 1024));
        }

        private static void VennDiagramExamples()
        {
            Console.WriteLine("**Venn Diagram Examples**");

            // Generate some random data of integers
            Console.WriteLine("Generating Random Values");
            int numberofsets = 5;
            int maxvalue = 100;
            int maxitems = 50;
            Random ran = new Random();
            VennSet<int>[] sets = new VennSet<int>[numberofsets];
            for (int i = 0; i < numberofsets; i++)
            {
                sets[i] = new VennSet<int>(string.Format("Set {0}", i));
                for (int j = 0; j < maxitems; j++)
                {
                    sets[i].Add(ran.Next(maxvalue));
                }
            }

            // Create diagram
            VennDiagram<int> diagram = VennDiagram<int>.CreateDiagram(sets);            
        }

        /// <summary>
        /// Basic overview of how peptides can be used and modified
        /// </summary>
        private static void PeptideExamples()
        {
            Console.WriteLine("**Peptide Examples**");

            // Simple Peptide creation
            Peptide peptide1 = new Peptide("ACDEFGHIKLMNPQRSTVWY");
            WritePeptideToConsole(peptide1);

            // Fragmenting a peptide is simple, you can include as many fragment types as you want
            Console.WriteLine("{0,-4} {1,-20} {2,-5}", "Type", "Formula", "Mass");
            foreach (Fragment fragment in peptide1.CalculateFragments(FragmentType.b | FragmentType.y))
            {
                WriteFragmentToConsole(fragment);
            }

            // Modifications can be applied to any residue or termini
            Console.WriteLine("Lets add some Iron to our peptide...");
            peptide1.SetModification(new ChemicalModification("Fe"), Terminus.C | Terminus.N);
            WritePeptideToConsole(peptide1);            

            // A chemicalmodification is a simple wrapper for a chemical formula. You can name your mods if you want
            Console.WriteLine("Add a modification of Oxygen with the name \"Oxidation\" to all Methionines");
            ChemicalModification oxMod = new ChemicalModification("O", "Oxidation");
            peptide1.SetModification(oxMod, 'M');
            WritePeptideToConsole(peptide1);

            // If you fragment a modified peptide, the modifications stay part of the fragments
            Console.WriteLine("{0,-4} {1,-20} {2,-5}", "Type", "Formula", "Mass");
            foreach (Fragment fragment in peptide1.CalculateFragments(FragmentType.b, 3, 5))
            {
                WriteFragmentToConsole(fragment);
            }
        }

        /// <summary>
        /// Basic overview of how chemical formulas can be used and modified
        /// </summary>
        private static void ChemicalFormulaExamples()
        {
            Console.WriteLine("**Chemical Formula Examples**");

            // Simple chemical formula creation
            ChemicalFormula formula1 = new ChemicalFormula("C2H3NO");
            WriteFormulaToConsole(formula1);

            // Input order does not matter
            ChemicalFormula formula2 = new ChemicalFormula("NH3C2O");
            WriteFormulaToConsole(formula2);

            // Formulas are identicial if they have the exact same type of elements and count
            Console.WriteLine("Are {0} and {1} equivalent? {2}", formula1, formula2, formula1.Equals(formula2));

            // You can modify exisiting chemical formulas in many ways.
            // You can add a chemical formula to a chemical formula
            formula1.Add(formula2);
            WriteFormulaToConsole(formula1);
            Console.WriteLine("Are {0} and {1} equivalent? {2}", formula1, formula2, formula1.Equals(formula2));

            // You can completely remove an element from a chemical formula
            formula1.Remove("C");
            WriteFormulaToConsole(formula1);

            // Even negative values are possible if not physically possible
            formula1.Remove(formula2);
            WriteFormulaToConsole(formula1);

            // Implicit arithmetic is also possible (supports +, -, and *)
            ChemicalFormula formula3 = formula2 - formula1;
            WriteFormulaToConsole(formula3);
            ChemicalFormula formula4 = formula3 + formula1;
            WriteFormulaToConsole(formula4);
            ChemicalFormula formula5 = formula2 * 5;
            WriteFormulaToConsole(formula5);

            // Formulas consist of a dictionary of isotopes and count, and by default, the most common (abundant) isotope of an element
            // is included (i.e. Carbon 12 instead of Carbon 13). You can explicitly state that you want another isotope in a chemical formula
            // by this notation: <Element Symbol>{<Mass Number>} (e.g. C{13}, N{15}, etc..)
            formula1 = new ChemicalFormula("C2C{13}2H3NO");
            formula2 = new ChemicalFormula("C4H3NO");
            WriteFormulaToConsole(formula1);
            WriteFormulaToConsole(formula2);
            Console.WriteLine("Are {0} and {1} equivalent? {2}", formula1, formula2, formula1.Equals(formula2));

            // No need to specify the mass number of the most abundant isotope for an element
            formula3 = new ChemicalFormula("C{12}2C2H3NO");
            formula4 = new ChemicalFormula("C4H3NO");
            Console.WriteLine("Are {0} and {1} equivalent? {2}", formula3, formula4, formula3.Equals(formula4));

            // Many physical things have chemical formulas (e.g. Peptides) and the interface IChemicalFormula allows many methods to work on a variety of different types of objects
            Peptide pep = new Peptide("DEREK");
            WriteFormulaToConsole(pep.ChemicalFormula);

            // Here, a new chemical formula is made in memory based off the chemical formula of the peptide. The two chemical formulas are identicial but are separated by each pointing
            // to a new place in memory. This allows you to edit the chemicalformula without disrupting the peptide
            ChemicalFormula pepFormula = new ChemicalFormula(pep);
            WriteFormulaToConsole(pepFormula);
        }

        /// <summary>
        /// Helper method to write formulas out to the console consistently
        /// </summary>
        /// <param name="formula"></param>
        private static void WriteFormulaToConsole(ChemicalFormula formula)
        {
            Console.WriteLine("Formula {0} mass is {1}", formula, formula.Mass.Monoisotopic);
        }

        private static void WritePeptideToConsole(Peptide peptide)
        {
            Console.WriteLine("{0,-5} {1,-5} {2,-5}", peptide, peptide.ChemicalFormula, peptide.ChemicalFormula.Mass.Monoisotopic);
        }

        private static void WriteFragmentToConsole(Fragment frag)
        {
            Console.WriteLine("{0,-4} {1,-20} {2,-5}", frag, frag.ChemicalFormula, frag.ChemicalFormula.Mass.Monoisotopic);
        }
    }
}