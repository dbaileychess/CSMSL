using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.Proteomics;
using NUnit.Framework;
using Should.Fluent;
using CSMSL.Chemistry;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture]
    [Category("Peptide")]
    public sealed class PeptideTestFixture
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
        public void PeptideMass()
        {
            MockPeptideEveryAminoAcid.Mass.Monoisotopic.Should().Equal(2394.12490682513);
        }

        [Test]      
        public void PeptideAminoAcidCount()
        {
            MockPeptideEveryAminoAcid.Length.Should().Equal(20);
        }

        [Test]
        public void CountNumberOfResidues()
        {
            MockTrypticPeptide.CountResidues('S').Should().Equal(7);
            MockTrypticPeptide.CountResidues('Q').Should().Equal(0);
            MockTrypticPeptide.CountResidues('T').Should().Equal(2);
            MockTrypticPeptide.CountResidues('G').Should().Equal(1);
        }

        [Test]
        public void SetAminoAcidModification()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, AminoAcid.Asparagine);

            MockPeptideEveryAminoAcid.ToString().Should().Equal("ACDEFGHIKLMN[Fe]PQRSTVWY");
        }

        [Test]
        public void SetAminoAcidCharacterModification()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, 'D');

            MockPeptideEveryAminoAcid.ToString().Should().Equal("ACD[Fe]EFGHIKLMNPQRSTVWY");
        }

        [Test]
        public void SetResiduePositionModification()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, 5);

            MockPeptideEveryAminoAcid.ToString().Should().Equal("ACDEF[Fe]GHIKLMNPQRSTVWY");
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException), ExpectedMessage="Residue number not in the correct range: [1-20] you specified: 25")]
        public void SetResiduePositionModificationOutOfRange()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, 25);           
        }
        
        [Test]
        public void SetCTerminusMod()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, Terminus.C);

            MockPeptideEveryAminoAcid.CTerminus.Should().Equal(formula);
        }

        [Test]
        public void SetCTerminusModStringRepresentation()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, Terminus.C);

            MockPeptideEveryAminoAcid.ToString().Should().Equal("ACDEFGHIKLMNPQRSTVWY-[Fe]");
        }

        [Test]
        public void SetCTerminusModStringRepresentationofChemicalModification()
        {
            IChemicalFormula formula = new ChemicalModification("Fe", "Test");
            MockPeptideEveryAminoAcid.SetModification(formula, Terminus.C);

            MockPeptideEveryAminoAcid.ToString().Should().Equal("ACDEFGHIKLMNPQRSTVWY-[Test]");
        }

        [Test]
        public void SetNTerminusMod()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, Terminus.N);

            MockPeptideEveryAminoAcid.NTerminus.Should().Equal(formula);
        }

        [Test]
        public void SetNAndCTerminusMod()
        {
            MockPeptideEveryAminoAcid.SetModification(new ChemicalModification("Fe"), Terminus.C);
            MockPeptideEveryAminoAcid.SetModification(new ChemicalModification("H2NO"), Terminus.N);

            MockPeptideEveryAminoAcid.ToString().Should().Equal("[H2NO]-ACDEFGHIKLMNPQRSTVWY-[Fe]");
        }


        [Test]
        public void SetSameNAndCTerminusMod()
        {
            MockPeptideEveryAminoAcid.SetModification(new ChemicalModification("Fe"), Terminus.C | Terminus.N);

            MockPeptideEveryAminoAcid.ToString().Should().Equal("[Fe]-ACDEFGHIKLMNPQRSTVWY-[Fe]");
        }
             
        [Test]
        public void ClearNTerminusMod()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, Terminus.N);

            MockPeptideEveryAminoAcid.ClearModification(Terminus.N);

            MockPeptideEveryAminoAcid.NTerminus.Should().Equal(AminoAcidPolymer.DefaultNTerminusModification);
        }

        [Test]
        public void ClearCTerminusMod()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            MockPeptideEveryAminoAcid.SetModification(formula, Terminus.C);

            MockPeptideEveryAminoAcid.ClearModification(Terminus.C);

            MockPeptideEveryAminoAcid.CTerminus.Should().Equal(AminoAcidPolymer.DefaultCTerminusModification);
        }

        [Test]
        public void PeptideEquality()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide("DEREK");            
            pepA.Should().Equals(pepB);
        }

        [Test]
        public void PeptideInEqualityAminoAcidSwitch()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide("DEERK");
            pepA.Should().Not.Equals(pepB);
        }

        [Test]
        public void PeptideInEqualityAminoAcidModification()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide("DEREK");
            pepB.SetModification(new ChemicalFormula("H2O"), 'R');

            pepA.Should().Not.Equals(pepB);
        }

        [Test]
        public void PeptideCloneEquality()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide(pepA);
            pepA.Should().Equals(pepB);
        }

        [Test]
        public void PeptideCloneNotSameReference()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide(pepA);

            pepA.Should().Not.Be.SameAs(pepB);
        }

        [Test]
        public void PeptideCloneWithModification()
        {
            Peptide pepA = new Peptide("DEREK");
            pepA.SetModification(new ChemicalFormula("H2O"), 'R');
            Peptide pepB = new Peptide(pepA);

            pepA.Should().Equals(pepB);
        }

    }
}
