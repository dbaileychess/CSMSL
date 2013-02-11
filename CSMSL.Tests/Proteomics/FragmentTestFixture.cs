using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Chemistry;
using NUnit.Framework;
using Should.Fluent;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture]
    [Category("Peptide Fragmentation")]
    public sealed class FragmentTestFixture
    {
        private Peptide MockPeptideEveryAminoAcid;
        private Peptide MockTrypticPeptide;

        [SetUp]
        public void SetUp()
        {
            MockPeptideEveryAminoAcid = new Peptide("ACDEFGHIKLMNPQRSTVWY");
            MockTrypticPeptide = new Peptide("TTGSSSSSSSK");
        }
       
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FragmentNumberToLow()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.b, 0);            
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FragmentNumberToHigh()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.b, 25);            
        }

        [Test]       
        public void FragmentChemicalFormulaAIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.a, 1);
            ChemicalFormula formula = new ChemicalFormula("C2H5N");

            fragment.ChemicalFormula.Should().Equal(formula);
        }

        [Test]
        public void FragmentChemicalFormulaBIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.b, 1);
            ChemicalFormula formula = new ChemicalFormula("C3H5NO");

            fragment.ChemicalFormula.Should().Equal(formula);
        }

        [Test]
        public void FragmentChemicalFormulaCIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.c, 1);
            ChemicalFormula formula = new ChemicalFormula("C3H8N2O");

            fragment.ChemicalFormula.Should().Equal(formula);
        }

        [Test]
        public void FragmentChemicalFormulaXIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.x, 1);
            ChemicalFormula formula = new ChemicalFormula("C10H9NO4");

            fragment.ChemicalFormula.Should().Equal(formula);
        }

        [Test]
        public void FragmentChemicalFormulaYIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.y, 1);
            ChemicalFormula formula = new ChemicalFormula("C9H11NO3");

            fragment.ChemicalFormula.Should().Equal(formula);
        }

        [Test]
        public void FragmentChemicalFormulaZIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentType.z, 1);
            ChemicalFormula formula = new ChemicalFormula("C9H8O3");

            fragment.ChemicalFormula.Should().Equal(formula);
        }
    }
}
