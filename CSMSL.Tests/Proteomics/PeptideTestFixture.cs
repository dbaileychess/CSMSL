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
        public void PeptideWithModifidedTermni()
        {
            MockPeptideEveryAminoAcid.SetModification(new ChemicalModification("Fe"), Terminus.C);
            MockPeptideEveryAminoAcid.SetModification(new ChemicalModification("H2NO"), Terminus.N);
            MockPeptideEveryAminoAcid.ToString().Should().Equal("[H2NO]-ACDEFGHIKLMNPQRSTVWY-[Fe]");
        }

        [Test]
        public void PeptideEquality()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide("DEREK");            
            pepA.Should().Equals(pepB);
        }

    }
}
