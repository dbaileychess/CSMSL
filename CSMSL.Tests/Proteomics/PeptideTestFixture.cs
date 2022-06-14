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

using CSMSL.Chemistry;
using CSMSL.Proteomics;
using NUnit.Framework;
using System;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture, Category("Peptide")]
    public sealed class PeptideTestFixture
    {
        private Peptide _mockPeptideEveryAminoAcid;
        private Peptide _mockTrypticPeptide;

        [SetUp]
        public void SetUp()
        {
            _mockPeptideEveryAminoAcid = new Peptide("ACDEFGHIKLMNPQRSTVWY");
            _mockTrypticPeptide = new Peptide("TTGSSSSSSSK");
        }

        [Test]
        public void PeptideMass()
        {
            Assert.AreEqual(_mockPeptideEveryAminoAcid.MonoisotopicMass, 2394.12490682513);
        }

        [Test]
        public void PeptideMassGlycine()
        {
            Peptide pep = new Peptide("G");
            ChemicalFormula formula = new ChemicalFormula("C2H5NO2");
            ChemicalFormula formula2;
            pep.TryGetChemicalFormula(out formula2);

            Assert.AreEqual(formula, formula2);
        }

        [Test]
        public void PeptideMassTryptic()
        {
            ChemicalFormula formula = new ChemicalFormula("C37H66N12O21");
            ChemicalFormula formula2;
            _mockTrypticPeptide.TryGetChemicalFormula(out formula2);
            Assert.AreEqual(formula, formula2);
        }

        [Test]
        public void PeptideAminoAcidCount()
        {
            Assert.AreEqual(20, _mockPeptideEveryAminoAcid.Length);
        }

        [Test]
        public void ParseNTerminalChemicalFormula()
        {
            Peptide peptide = new Peptide("[C2H3NO]-TTGSSSSSSSK");
            ChemicalFormula formulaA = new ChemicalFormula("C39H69N13O22");
            ChemicalFormula formulaB;
            peptide.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(formulaA, formulaB);
        }

        [Test]
        public void ParseCTerminalChemicalFormula()
        {
            Peptide peptide = new Peptide("TTGSSSSSSSK-[C2H3NO]");
            ChemicalFormula formulaA = new ChemicalFormula("C39H69N13O22");
            ChemicalFormula formulaB;
            peptide.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(formulaA, formulaB);
        }

        [Test]
        public void ParseCTerminalChemicalFormulaWithLastResidueMod()
        {
            Peptide peptide = new Peptide("TTGSSSSSSSK[H2O]-[C2H3NO]");
            ChemicalFormula formulaA = new ChemicalFormula("C39H71N13O23");
            ChemicalFormula formulaB;
            peptide.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(formulaA, formulaB);
        }

        [Test]
        public void ParseCTerminalChemicalFormulaWithLastResidueModStringRepresentation()
        {
            Peptide peptide = new Peptide("TTGSSSSSSSK[H2O]-[C2H3NO]");

            Assert.AreEqual("TTGSSSSSSSK[H2O]-[C2H3NO]", peptide.SequenceWithModifications);
        }

        [Test]
        public void ParseNAndCTerminalChemicalFormula()
        {
            Peptide peptide = new Peptide("[C2H3NO]-TTGSSSSSSSK-[C2H3NO]");
            ChemicalFormula formulaA = new ChemicalFormula("C41H72N14O23");
            ChemicalFormula formulaB;
            peptide.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(formulaA, formulaB);
        }

        [Test]
        public void ParseNTerminalNamedChemicalModification()
        {
            Peptide peptide = new Peptide("[Carbamidomethyl]-TTGSSSSSSSK");
            ChemicalFormula formulaA = new ChemicalFormula("C39H69N13O22");
            ChemicalFormula formulaB;
            peptide.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(formulaA, formulaB);
        }

        [Test]
        public void ParseCTerminalNamedChemicalModification()
        {
            Peptide peptide = new Peptide("TTGSSSSSSSK-[Carbamidomethyl]");
            ChemicalFormula formulaA = new ChemicalFormula("C39H69N13O22");
            ChemicalFormula formulaB;
            peptide.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(formulaA, formulaB);
        }

        [Test]
        public void ParseNAndCTerminalNamedChemicalModification()
        {
            Peptide peptide = new Peptide("[Carbamidomethyl]-TTGSSSSSSSK-[Carbamidomethyl]");
            ChemicalFormula formulaA = new ChemicalFormula("C41H72N14O23");
            ChemicalFormula formulaB;
            peptide.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(formulaA, formulaB);
        }

        [Test]
        public void EmptyStringPeptideConstructorLength()
        {
            Peptide peptide = new Peptide();

            Assert.AreEqual(0, peptide.Length);
        }

        [Test]
        public void EmptyStringPeptideConstructorToString()
        {
            Peptide peptide = new Peptide();

            Assert.AreEqual(string.Empty, peptide.ToString());
        }

        [Test]
        public void EmptyStringPeptideConstructorMassIsWater()
        {
            Peptide peptide = new Peptide();

            Assert.AreEqual(18.010564683699997, peptide.MonoisotopicMass);
        }

        [Test]
        public void ParseNamedChemicalNamedChemicalModification()
        {
            Peptide peptide = new Peptide("THGEAK[Acetyl]K");

            Assert.AreEqual(811.41881631550984, peptide.MonoisotopicMass);
        }

        [Test]
        public void ParseDoubleModificationToString()
        {
            Peptide peptide = new Peptide("THGEAK[25.132]K");

            Assert.AreEqual("THGEAK[25.132]K", peptide.ToString());
        }

        [Test]
        public void ParseNamedChemicalModificationToString()
        {
            Peptide peptide = new Peptide("THGEAK[Acetyl]K");

            Assert.AreEqual("THGEAK[Acetyl]K", peptide.ToString());
        }

        [Test]
        public void ParseNamedChemicalModificationRegisterNew()
        {
            ModificationDictionary.AddModification(new ChemicalFormulaModification("C2H3NO", "Test", ModificationSites.C));
            Peptide peptide = new Peptide("THGEAK[Test]K");

            Assert.AreEqual(826.42971535237984, peptide.MonoisotopicMass);
        }

        [Test]
        public void ParseSequenceWithSpaces()
        {
            Peptide peptide1 = new Peptide("TTGSSS SSS SK");
            Peptide peptide2 = new Peptide("TTGSSSSSSSK");

            Assert.AreEqual(peptide1, peptide2);
        }

        [Test]
        public void ParseSequenceWithTrailingSpaces()
        {
            Peptide peptide1 = new Peptide("TTGSSSSSSSK   ");
            Peptide peptide2 = new Peptide("TTGSSSSSSSK");

            Assert.AreEqual(peptide1, peptide2);
        }

        [Test]
        public void ParseSequenceWithPreceedingSpaces()
        {
            Peptide peptide1 = new Peptide("   TTGSSSSSSSK");
            Peptide peptide2 = new Peptide("TTGSSSSSSSK");

            Assert.AreEqual(peptide1, peptide2);
        }

        [Test]
        public void ParseSequenceWithSpacesEverywhere()
        {
            Peptide peptide1 = new Peptide("   TTGS  SSSS  SSK   ");
            Peptide peptide2 = new Peptide("TTGSSSSSSSK");

            Assert.AreEqual(peptide1, peptide2);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Unable to correctly parse the following modification: TMT 7-plex")]
        public void ParseNamedChemicalModificationInvalidName()
        {
            Peptide peptide = new Peptide("T[TMT 7-plex]HGEAK[Acetyl]K");
        }

        [Test]
        public void SetAminoAcidModification()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, AminoAcid.Asparagine);

            Assert.AreEqual("ACDEFGHIKLMN[Fe]PQRSTVWY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetAminoAcidModificationStronglyTyped()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, ModificationSites.N);

            Assert.AreEqual("ACDEFGHIKLMN[Fe]PQRSTVWY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetAminoAcidModificationStronglyTypedMultipleLocations()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, ModificationSites.N | ModificationSites.F | ModificationSites.V);

            Assert.AreEqual("ACDEF[Fe]GHIKLMN[Fe]PQRSTV[Fe]WY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetAminoAcidModificationStronglyTypedAny()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, ModificationSites.Any);

            Assert.AreEqual("ACDEFGHIKLMNPQRSTVWY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetAminoAcidModificationStronglyTypedAll()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, ModificationSites.All);

            Assert.AreEqual("[Fe]-A[Fe]C[Fe]D[Fe]E[Fe]F[Fe]G[Fe]H[Fe]I[Fe]K[Fe]L[Fe]M[Fe]N[Fe]P[Fe]Q[Fe]R[Fe]S[Fe]T[Fe]V[Fe]W[Fe]Y[Fe]-[Fe]", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetAminoAcidModificationStronglyTypedNone()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, ModificationSites.None);

            Assert.AreEqual("ACDEFGHIKLMNPQRSTVWY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetAminoAcidModificationStronglyTypedTermini()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, ModificationSites.NPep | ModificationSites.PepC);

            Assert.AreEqual("[Fe]-ACDEFGHIKLMNPQRSTVWY-[Fe]", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetAminoAcidCharacterModification()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, 'D');

            Assert.AreEqual("ACD[Fe]EFGHIKLMNPQRSTVWY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetResiduePositionModification()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, 5);

            Assert.AreEqual("ACDEF[Fe]GHIKLMNPQRSTVWY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException), ExpectedMessage = "Residue number not in the correct range: [1-20] you specified: 25")]
        public void SetResiduePositionModificationOutOfRangeUpper()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, 25);
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException), ExpectedMessage = "Residue number not in the correct range: [1-20] you specified: 0")]
        public void SetResiduePositionModificationOutOfRangeLower()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, 0);
        }

        [Test]
        public void SetCTerminusModStringRepresentation()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, Terminus.C);

            Assert.AreEqual("ACDEFGHIKLMNPQRSTVWY-[Fe]", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetCTerminusModStringRepresentationofChemicalModification()
        {
            IChemicalFormula formula = new ChemicalFormulaModification("Fe", "Test");
            _mockPeptideEveryAminoAcid.SetModification(formula, Terminus.C);

            Assert.AreEqual("ACDEFGHIKLMNPQRSTVWY-[Test]", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetNAndCTerminusMod()
        {
            _mockPeptideEveryAminoAcid.SetModification(new ChemicalFormula("Fe"), Terminus.C);
            _mockPeptideEveryAminoAcid.SetModification(new ChemicalFormula("H2NO"), Terminus.N);

            Assert.AreEqual("[H2NO]-ACDEFGHIKLMNPQRSTVWY-[Fe]", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void SetSameNAndCTerminusMod()
        {
            _mockPeptideEveryAminoAcid.SetModification(new ChemicalFormula("Fe"), Terminus.C | Terminus.N);

            Assert.AreEqual("[Fe]-ACDEFGHIKLMNPQRSTVWY-[Fe]", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void ClearNTerminusMod()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, Terminus.N);

            _mockPeptideEveryAminoAcid.ClearModifications(Terminus.N);

            Assert.IsNull(_mockPeptideEveryAminoAcid.NTerminusModification);
        }

        [Test]
        public void ClearCTerminusMod()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, Terminus.C);

            _mockPeptideEveryAminoAcid.ClearModifications(Terminus.C);

            Assert.IsNull(_mockPeptideEveryAminoAcid.CTerminusModification);
        }

        [Test]
        public void ClearAllMods()
        {
            ChemicalFormula formula = new ChemicalFormula("Fe");
            _mockPeptideEveryAminoAcid.SetModification(formula, ModificationSites.All);

            _mockPeptideEveryAminoAcid.ClearModifications();

            Assert.AreEqual("ACDEFGHIKLMNPQRSTVWY", _mockPeptideEveryAminoAcid.ToString());
        }

        [Test]
        public void ClearModificationsBySites()
        {
            var peptide = new Peptide("AC[Fe]DEFGHIKLMNP[Fe]QRSTV[Fe]WY");

            peptide.ClearModifications(ModificationSites.C | ModificationSites.V);

            Assert.AreEqual("ACDEFGHIKLMNP[Fe]QRSTVWY", peptide.ToString());
        }

        [Test]
        public void EmptyPeptideLengthIsZero()
        {
            Peptide pepA = new Peptide();

            Assert.AreEqual(0, pepA.Length);
        }

        [Test]
        public void EmptyPeptideSequenceIsEmpty()
        {
            Peptide pepA = new Peptide();

            Assert.AreEqual(String.Empty, pepA.Sequence);
        }

        [Test]
        public void EmptyPeptideFormulaIsH2O()
        {
            Peptide pepA = new Peptide();
            ChemicalFormula h2O = new ChemicalFormula("H2O");
            ChemicalFormula formulaB;
            pepA.TryGetChemicalFormula(out formulaB);

            Assert.AreEqual(h2O, formulaB);
        }

        [Test]
        public void PeptideEquality()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide("DEREK");
            Assert.AreEqual(pepA, pepB);
        }

        [Test]
        public void PeptideInEqualityAminoAcidSwitch()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide("DEERK");
            Assert.AreNotEqual(pepA, pepB);
        }

        [Test]
        public void PeptideInEqualityAminoAcidModification()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide("DEREK");
            pepB.SetModification(new ChemicalFormula("H2O"), 'R');

            Assert.AreNotEqual(pepA, pepB);
        }

        [Test]
        public void PeptideCloneEquality()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide(pepA);
            Assert.AreEqual(pepA, pepB);
        }

        [Test]
        public void PeptideCloneNotSameReference()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide(pepA);

            Assert.AreNotSame(pepA, pepB);
        }

        [Test]
        public void PeptideCloneWithModifications()
        {
            Peptide pepA = new Peptide("DER[Fe]EK");
            Peptide pepB = new Peptide(pepA);
            Assert.AreEqual("DER[Fe]EK", pepB.ToString());
        }

        [Test]
        public void PeptideCloneWithoutModifications()
        {
            Peptide pepA = new Peptide("DER[Fe]EK");
            Peptide pepB = new Peptide(pepA, false);
            Assert.AreEqual("DEREK", pepB.ToString());
        }

        [Test]
        public void PeptideCloneWithModification()
        {
            Peptide pepA = new Peptide("DEREK");
            pepA.SetModification(new ChemicalFormula("H2O"), 'R');
            Peptide pepB = new Peptide(pepA);

            Assert.AreEqual(pepB, pepA);
        }

        [Test]
        public void PeptideParitalCloneInternal()
        {
            Peptide pepA = new Peptide("DEREK");
            Peptide pepB = new Peptide(pepA, 1, 3);
            Peptide pepC = new Peptide("ERE");
            Assert.AreEqual(pepB, pepC);
        }

        [Test]
        public void PeptideParitalClonelWithInternalModification()
        {
            Peptide pepA = new Peptide("DER[Fe]EK");
            Peptide pepB = new Peptide(pepA, 2, 3);
            Peptide pepC = new Peptide("R[Fe]EK");
            Assert.AreEqual(pepB, pepC);
        }

        [Test]
        public void PeptideParitalClonelWithInternalModificationTwoMods()
        {
            Peptide pepA = new Peptide("DE[Al]R[Fe]EK");
            Peptide pepB = new Peptide(pepA, 2, 3);
            Peptide pepC = new Peptide("R[Fe]EK");
            Assert.AreEqual(pepB, pepC);
        }

        [Test]
        public void PeptideParitalCloneInternalWithCTerminusModification()
        {
            Peptide pepA = new Peptide("DEREK");
            pepA.SetModification(new ChemicalFormula("H2O"), Terminus.C);
            Peptide pepB = new Peptide(pepA, 2, 3);

            Peptide pepC = new Peptide("REK");
            pepC.SetModification(new ChemicalFormula("H2O"), Terminus.C);

            Assert.AreEqual(pepB, pepC);
        }

        [Test]
        public void GetLeucineSequence()
        {
            Peptide pepA = new Peptide("DERIEK");
            string leuSeq = pepA.GetLeucineSequence();

            Assert.AreEqual("DERLEK", leuSeq);
        }

        [Test]
        public void GetLeucineSequenceNoReplacement()
        {
            Peptide pepA = new Peptide("DERLEK");

            string leuSeq = pepA.GetLeucineSequence();

            Assert.AreEqual("DERLEK", leuSeq);
        }
    }
}