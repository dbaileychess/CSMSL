// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (FragmentTestFixture.cs) is part of CSMSL.Tests.
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

using CSMSL.Chemistry;
using CSMSL.Proteomics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture, Category("Peptide Fragmentation")]
    public sealed class FragmentTestFixture
    {
        private Peptide _mockPeptideEveryAminoAcid;

        [SetUp]
        public void SetUp()
        {
            _mockPeptideEveryAminoAcid = new Peptide("ACDEFGHIKLMNPQRSTVWY");
        }

        [Test]
        public void FragmentNumberToLow()
        {
            Assert.Throws<IndexOutOfRangeException>(() => _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.b, 0).ToList());
        }

        [Test]
        public void FragmentNumberToHigh()
        {
            Assert.Throws<IndexOutOfRangeException>(() => _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.b, 25).ToList());
        }

        [Test]
        public void FragmentName()
        {
            Fragment fragment = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.a, 1).ToArray()[0];

            Assert.AreEqual("a1", fragment.ToString());
        }

        [Test]
        public void FragmentAllBIons()
        {
            List<Fragment> fragments = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.b).ToList();

            Assert.AreEqual(19, fragments.Count);
        }

        [Test]
        public void FragmentChemicalFormulaAIon()
        {
            Fragment fragment = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.a, 1).ToArray()[0];

            Assert.IsTrue(fragment.MassEquals(43.04219916514));
        }

        [Test]
        public void FragmentChemicalFormulaBIon()
        {
            Fragment fragment = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.b, 1).ToArray()[0];

            Assert.AreEqual(71.037113784709987, fragment.MonoisotopicMass);
        }

        [Test]
        public void FragmentChemicalFormulaCIon()
        {
            Fragment fragment = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.c, 1).ToArray()[0];

            Assert.AreEqual(88.063662885719992, fragment.MonoisotopicMass);
        }

        [Test]
        public void FragmentChemicalFormulaXIon()
        {
            Fragment fragment = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.x, 1).ToArray()[0];

            Assert.AreEqual(207.05315777167004, fragment.MonoisotopicMass);
        }

        [Test]
        public void FragmentChemicalFormulaYIon()
        {
            Fragment fragment = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.y, 1).ToArray()[0];

            Assert.AreEqual(181.07389321625004, fragment.MonoisotopicMass);
        }

        [Test]
        public void FragmentChemicalFormulaZIon()
        {
            Fragment fragment = _mockPeptideEveryAminoAcid.Fragment(FragmentTypes.z, 1).ToArray()[0];

            Assert.AreEqual(164.04734411524004, fragment.MonoisotopicMass);
        }
    }
}