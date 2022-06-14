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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.MzTab;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.Util.Collections;
using System;
using System.Diagnostics;

namespace CSMSL.Examples
{
    internal class Examples
    {
        public static string BASE_DIRECTORY = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CSMSL Examples");

        [STAThread]
        private static void Main()
        {
            DirectoryInfo info = Directory.CreateDirectory(BASE_DIRECTORY);
            if (!info.Exists)
            {
                Console.WriteLine("Unable to create/use directory" + BASE_DIRECTORY);
            }
            else
            {
                Console.WriteLine("==CSMSL Examples==");

                //Console.WriteLine("Outputting Files to " + BASE_DIRECTORY);
                Console.WriteLine();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                StartExamples();
                GuiExamples();
                watch.Stop();
                Console.WriteLine();
                Console.WriteLine("==================");
                Console.WriteLine(watch.Elapsed);
            }
            Console.ReadKey();
        }

        private static void StartExamples()
        {



            //List<MzTabPSM> psms;
            //List<MzTabProtein> proteins;
            //List<MzTabPeptide> peptides;
            //List<MzTabSmallMolecule> smallMolecules;
            //MzTabMetaData metaData;
            
            //using (MzTabReader reader = new MzTabReader(@"E:\Desktop\mzTab Examples\iTRAQ_SQI.mzTab", false))
            //{
            //    reader.Open();
                
            //    metaData = reader.MetaData;

            //    psms = reader.GetPsms().ToList();
            //    proteins = reader.GetProteins().ToList();
            //    peptides = reader.GetPeptides().ToList();
            //    smallMolecules = reader.GetSmallMolecules().ToList();
            //}
   
            //using (MzTabWriter writer = new MzTabWriter(@"E:\Desktop\mzTab Examples\dereksTest.mzTab"))
            //{
            //    writer.WriteComment("Test Comment");
            //    writer.WriteMetaData(metaData);
            //    writer.WriteLine();
            //    writer.WriteProteinData(proteins);
            //    writer.WriteLine();
            //    writer.WritePsmData(psms);
            //    writer.WriteLine();
            //    writer.WritePeptideData(peptides);
            //    writer.WriteLine();
            //    writer.WriteSmallMoleculeData(smallMolecules);
            //}

            // Examples coding  
            //ChemicalFormulaExamples();
            //PeptideExamples();
            //ChemicalFormulaGeneratorExample();

            // Example Objects
            //VennDiagramExamples();

            // Example Digestion
            //TrypticDigestion.Start(minLength: 5, maxLength: 50, maxMissed:3, protease:Protease.Trypsin, storeSequenceString: false);
            //TrypticDigestion.ExampleDigestion();

            // Example Protein Grouping
            //ProteinGroupingExample.Start(Protease.Trypsin);
            //ProteinGroupingExample.ExampleProteinGrouping(Protease.Trypsin);
            //ProteinGroupingExample.StartRamp(Protease.Trypsin);

            //Example Isotopologue
            //IsotopologueExample();

            // Recalibrate Thermo Files on Lockmass
            ThermoRawFileExamples.RecalibrateThermoRawFile();
            ThermoRawFileExamples.LoopOverEveryScan();

            // Example IO
            //MsDataFileExamples.VendorNeutralDataAccess();

            // Omssa Reader
            //OmssaReader.Start();

            // MS/MS searching
            //MorpheusSearch.Start(Protease.Trypsin);

            // Writing data to files
            //FileOutputExamples.Start();

            //PepXmlExamples.ReadPepXml();

            //MzIdentMLExamples.ReadMzIdentML();
        }

        [STAThread]
        private static void GuiExamples()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Peptide Calculator GUI Example
            //Application.Run(new PeptideCalculatorForm());
        }

        private static void IsotopologueExample()
        {
            var iso1 = new Isotopologue("One", ModificationSites.K);
            iso1.AddModification(new ChemicalFormulaModification("C1", "Sample 1"));
            iso1.AddModification(new ChemicalFormulaModification("C2", "Sample 2"));

            var iso2 = new Isotopologue("Two", ModificationSites.R);
            iso2.AddModification(new ChemicalFormulaModification("C3", "Sample 3"));
            iso2.AddModification(Modification.Empty);

            Peptide peptide = new Peptide("DEREK");
            peptide.SetModification(iso1);
            peptide.SetModification(iso2);

            foreach (var iso in peptide.GenerateIsotopologues())
            {
                Console.WriteLine(iso + ", " + iso.MonoisotopicMass);
            }
        }

        private static void ChemicalFormulaGeneratorExample()
        {
            //// Create a generator with formula bounds. In this case, every formula needs to have at least H2 but no more than C4H2
            //ChemicalFormulaGenerator generator = new ChemicalFormulaGenerator(new ChemicalFormula("H2"), new ChemicalFormula("C4H2"));

            //var formulas = generator.FromMass(0, 100).ToList();
            //foreach (var formula in formulas)
            //{
            //    WriteFormulaToConsole(formula);
            //}

            //Console.WriteLine("Unique Formulas: " + formulas.Count);

            // Create a generator with formula bounds. In this case, every formula needs to have at least H2 but no more than C4H2
            ChemicalFormulaGenerator generator = new ChemicalFormulaGenerator(new ChemicalFormula("C10000N1000H1000O1000"));

            DoubleRange range = DoubleRange.FromPPM(new ChemicalFormula("C62H54N42O24").MonoisotopicMass, 0.1);
            double mass = range.Mean;
            int count = 0;
            foreach (var formula in generator.FromMass(range).Validate())
            {
                Console.WriteLine("{0,-15} {1:F10} {2,-5:G5} ppm", formula, formula.MonoisotopicMass,
                    Tolerance.GetTolerance(formula.MonoisotopicMass, mass, ToleranceUnit.PPM));
                count++;
            }
            Console.WriteLine("Unique Formulas: " + count);
        }

        private static void VennDiagramExamples()
        {
            Console.WriteLine("**Venn Diagram Examples**");

            // Generate some random data of integers
            Console.WriteLine("Generating Random Values");
            int numberofsets = 5;
            int maxvalue = 100;
            int maxitems = 50;
            Random ran = new Random(135121321);
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
            Console.WriteLine("{0,-4} {1,-20} {2,-10} {3,-10} {4,-5}", "Type", "Formula", "Mass", "m/z +1", "Sequence");
            foreach (Fragment fragment in peptide1.Fragment(FragmentTypes.b | FragmentTypes.y))
            {
                WriteFragmentToConsole(fragment);
            }

            // Modifications can be applied to any residue or termini
            Console.WriteLine("Lets add some Iron to our peptide...");
            peptide1.SetModification(new ChemicalFormula("Fe"), Terminus.C | Terminus.N);
            WritePeptideToConsole(peptide1);

            // A chemicalmodification is a simple wrapper for a chemical formula. You can name your mods if you want
            Console.WriteLine("Add a modification of Oxygen with the name \"Oxidation\" to all Methionines");
            ChemicalFormulaModification oxMod = new ChemicalFormulaModification("O", "Oxidation");
            peptide1.SetModification(oxMod, 'M');
            WritePeptideToConsole(peptide1);

            // If you fragment a modified peptide, the modifications stay part of the fragments
            Console.WriteLine("{0,-4} {1,-20} {2,-20} {3,-5}", "Type", "Formula", "Mass", "m/z +1");
            foreach (Fragment fragment in peptide1.Fragment(FragmentTypes.b | FragmentTypes.y, 2))
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
            ChemicalFormula formula5 = formula2*5;
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
        }

        /// <summary>
        /// Helper method to write formulas out to the console consistently
        /// </summary>
        /// <param name="formula"></param>
        private static void WriteFormulaToConsole(ChemicalFormula formula)
        {
            Console.WriteLine("Formula {0} mass is {1}", formula, formula.MonoisotopicMass);
        }

        private static void WritePeptideToConsole(Peptide peptide)
        {
            ChemicalFormula formula;
            peptide.TryGetChemicalFormula(out formula);
            Console.WriteLine("{0,-5} {1,-5} {2,-5}", peptide, formula, peptide.MonoisotopicMass);
        }

        private static void WriteFragmentToConsole(Fragment frag)
        {
            IChemicalFormula formula = frag as IChemicalFormula;
            string f = "";
            if (formula != null)
                f = formula.ChemicalFormula.ToString();
            Console.WriteLine("{0,-4} {1,-20} {2,10:F5} {3,10:F5} {4,-5}", frag, f, frag.MonoisotopicMass, frag.ToMz(1), frag.GetSequence());
        }
    }
}