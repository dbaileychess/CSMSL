using NUnit.Framework;
using CSMSL.Proteomics;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture, Category("Amino Acid")]
    public sealed class AminoAcidTestFixture
    {
        [Test]
        public void GetResidueByCharacter()
        {
            AminoAcid aa = AminoAcid.GetResidue('A');

            Assert.AreEqual(aa.Name, "Alanine");
        }

        [Test]
        public void GetResidueByCharacterString()
        {
            AminoAcid aa = AminoAcid.GetResidue("A");

            Assert.AreEqual(aa.Name, "Alanine");
        }

        [Test]
        public void GetResidueBy3LetterSymbol()
        {
            AminoAcid aa = AminoAcid.GetResidue("Ala");

            Assert.AreEqual(aa.Name, "Alanine");
        }

        [Test]
        public void GetResidueByName()
        {
            AminoAcid aa = AminoAcid.GetResidue("Alanine");

            Assert.AreEqual(aa.Name, "Alanine");
        }

    }
}
