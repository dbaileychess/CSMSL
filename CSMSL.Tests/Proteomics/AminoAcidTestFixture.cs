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