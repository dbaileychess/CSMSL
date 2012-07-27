using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CSMSL;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.IO;

namespace ExamplesCSMSL
{
    class Examples
    {
        static void Main(string[] args)
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
            ChemicalFormulaExamples();
        }

        private static void ChemicalFormulaExamples()
        {
            // Simple Chemical Formula creation
            ChemicalFormula formula1 = new ChemicalFormula("C2H3NO");
            WriteFormulaToConsole(formula1);

            // Input Order does not matter
            ChemicalFormula formula2 = new ChemicalFormula("NH3C2O");
            WriteFormulaToConsole(formula2);

            // Formulas are identicial if they have the exact same type of elements and count:
            Console.WriteLine("Are {0} and {1} equivalent? {2}", formula1, formula2, formula1.Equals(formula2));
        }

        /// <summary>
        /// Helper method to write formulas out to the console consistently
        /// </summary>
        /// <param name="formula"></param>
        private static void WriteFormulaToConsole(ChemicalFormula formula)
        {
            Console.WriteLine("Formula {0} mass is {1}", formula, formula.Mass.Monoisotopic);
        }

    }
}
