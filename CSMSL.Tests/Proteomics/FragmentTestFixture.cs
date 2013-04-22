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
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.b, 0);            
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FragmentNumberToHigh()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.b, 25);            
        }

        [Test]
        public void FragmentName()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.a, 1);
    
            fragment.ToString().Should().Equal("a1");
        }

        [Test]
        public void FragmentAllBIons()
        {
            List<Fragment> fragments = MockPeptideEveryAminoAcid.CalculateFragments(FragmentTypes.b).ToList();

            fragments.Should().Count.Equals(19);
        }

        [Test]
        public void FragmentNTerminalMod()
        {
            MockPeptideEveryAminoAcid.SetModification(NamedChemicalFormula.TMT6plex, Terminus.N);
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.a, 1);
            ChemicalFormula formula = new ChemicalFormula("C{13}4C10N{15}N2O2H25");
            
            fragment.Mass.Should().Equal(formula.Mass);
        }

        [Test]
        public void FragmentCTerminalMod()
        {
            MockPeptideEveryAminoAcid.SetModification(NamedChemicalFormula.TMT6plex, Terminus.C);
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.x, 1);          

            fragment.Mass.Monoisotopic.Should().Equal(436.21608990639004);
        }

        [Test]       
        public void FragmentChemicalFormulaAIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.a, 1);          

            fragment.Mass.Monoisotopic.Should().Equal(43.042199165149988);
        }

        [Test]
        public void FragmentChemicalFormulaBIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.b, 1);            

            fragment.Mass.Monoisotopic.Should().Equal(71.037113784709987);
        }

        [Test]
        public void FragmentChemicalFormulaCIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.c, 1);
 
            fragment.Mass.Monoisotopic.Should().Equal(88.063662885719992);
        }

        [Test]
        public void FragmentChemicalFormulaXIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.x, 1);

            fragment.Mass.Monoisotopic.Should().Equal(207.05315777167004);
        }

        [Test]
        public void FragmentChemicalFormulaYIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.y, 1);

            fragment.Mass.Monoisotopic.Should().Equal(181.07389321625004);
        }

        [Test]
        public void FragmentChemicalFormulaZIon()
        {
            Fragment fragment = MockPeptideEveryAminoAcid.CalculateFragment(FragmentTypes.z, 1);

            fragment.Mass.Monoisotopic.Should().Equal(164.04734411524004);
        }
    }
}
