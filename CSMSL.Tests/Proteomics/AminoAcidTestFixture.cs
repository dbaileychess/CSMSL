// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (AminoAcidTestFixture.cs) is part of CSMSL.Tests.
//
// CSMSL.Tests is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CSMSL.Tests is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL.Tests. If not, see <http://www.gnu.org/licenses/>.

using CSMSL.Proteomics;
using NUnit.Framework;

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