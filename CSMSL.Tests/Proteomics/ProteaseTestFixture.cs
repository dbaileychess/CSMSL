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
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture, Category("Protease")]
    public sealed class ProteaseTestFixture
    {
        private Protein _proteinA;

        [TestFixtureSetUp]
        public void Setup()
        {
            _proteinA = new Protein("MMRGFKQRLIKKTTGSSSSSSSKKKDKEKEKEKSSTTSSTSKKPASASSSSHGTTHSSASSTGSKSTTEKGKQSGSVPSQ" +
                                    "GKHHSSSTSKTKTATTPSSSSSSSRSSSVSRSGSSSTKKTSSRKGQEQSKQSQQPSQSQKQGSSSSSAAIMNPTPVLTVT" +
                                    "KDDKSTSGEDHAHPTLLGAVSAVPSSPISNASGTAVSSDVENGNSNNNNMNINTSNTQDANHASSQSIDIPRSSHSFERL" +
                                    "PTPTKLNPDTDLELIKTPQRHSSSRFEPSRYTPLTKLPNFNEVSPEERIPLFIAKVDQCNTMFDFNDPSFDIQGKEIKRS" +
                                    "TLDELIEFLVTNRFTYTNEMYAHVVNMFKINLFRPIPPPVNPVGDIYDPDEDEPVNELAWPHMQAVYEFFLRFVESPDFN" +
                                    "HQIAKQYIDQDFILKLLELFDSEDIRERDCLKTTLHRIYGKFLSLRSFIRRSMNNIFLQFIYETEKFNGVAELLEILGSI" +
                                    "INGFALPLKEEHKVFLVRILIPLHKVRCLSLYHPQLAYCIVQFLEKDPLLTEEVVMGLLRYWPKINSTKEIMFLNEIEDI" +
                                    "FEVIEPLEFIKVEVPLFVQLAKCISSPHFQVAEKVLSYWNNEYFLNLCIENAEVILPIIFPALYELTSQLELDTANGEDS" +
                                    "ISDPYMLVEQAINSGSWNRAIHAMAFKALKIFLETNPVLYENCNALYLSSVKETQQRKVQREENWSKLEEYVKNLRINND" +
                                    "KDQYTIKNPELRNSFNTASENNTLNEENENDCDSEIQ");
        }

        [Test]
        public void TrypsinDigestion()
        {
            List<Peptide> peptides = _proteinA.Digest(Protease.GetProtease("Trypsin")).ToList();
            Peptide pepA = new Peptide("TTGSSSSSSSK");

            Assert.Contains(pepA, peptides);
        }

        [Test]
        public void MissedClevageNoneCTerminus()
        {
            int missedClevages = Protease.GetProtease("Trypsin").MissedCleavages("DEFEK");

            Assert.AreEqual(0, missedClevages);
        }

        [Test]
        public void MissedClevageNoneNTerminus()
        {
            int missedClevages = Protease.GetProtease("LysN").MissedCleavages("KERED");

            Assert.AreEqual(0, missedClevages);
        }

        [Test]
        public void MissedClevageOneInternalNTerminus()
        {
            int missedClevages = Protease.GetProtease("LysN").MissedCleavages("KEKED");

            Assert.AreEqual(1, missedClevages);
        }

        [Test]
        public void MissedClevageOneInternalCTerminus()
        {
            int missedClevages = Protease.GetProtease("Trypsin").MissedCleavages("DEKEK");

            Assert.AreEqual(1, missedClevages);
        }

        [Test]
        public void MissedClevageTwoInternalCTerminus()
        {
            int missedClevages = Protease.GetProtease("Trypsin").MissedCleavages("DEKAAKEK");

            Assert.AreEqual(2, missedClevages);
        }

        [Test]
        public void MissedClevageTwoInternalNTerminus()
        {
            int missedClevages = Protease.GetProtease("LysN").MissedCleavages("KEDEKEKED");

            Assert.AreEqual(2, missedClevages);
        }

        [Test]
        public void MissedClevageOneCTerminalProteinCTerminus()
        {
            int missedClevages = Protease.GetProtease("Trypsin").MissedCleavages("KEEEEEE");

            Assert.AreEqual(1, missedClevages);
        }

        [Test]
        public void MissedClevageOneNTerminalProteinNTerminus()
        {
            int missedClevages = Protease.GetProtease("LysN").MissedCleavages("EEEEEEEK");

            Assert.AreEqual(1, missedClevages);
        }

        [Test]
        public void TryspinNoProlineRuleDigestion()
        {
            List<Peptide> peptides = _proteinA.Digest(Protease.GetProtease("Trypsin No Proline Rule")).ToList();

            Assert.Contains(new Peptide("INLFR"), peptides);
        }

        [Test]
        public void NullEnzymeDigestion()
        {
            List<Peptide> peptides = _proteinA.Digest((IProtease)null, initiatorMethonine: false).ToList();

            Assert.AreEqual(1, peptides.Count);
        }

        [Test]
        public void NoEnzymeDigestion()
        {
            List<Peptide> peptides = _proteinA.Digest(Protease.GetProtease("None"), maxMissedCleavages: 8, minLength: 5).ToList();
            Assert.Contains(new Peptide("SLYHPQ"), peptides);
        }

        [Test]
        public void MultipleProteaseDigestion()
        {
            List<IProtease> proteases = new List<IProtease> { Protease.GetProtease("Trypsin"), Protease.GetProtease("GluC") };

            List<Peptide> peptides = _proteinA.Digest(proteases, maxMissedCleavages: 1, maxLength: 5).ToList();
            Assert.Contains(new Peptide("NWSK"), peptides);
            Assert.Contains(new Peptide("ENWSK"), peptides);
        }

        [Test]
        public void InitiatorMethonineCleaved()
        {
            List<Peptide> peptides = _proteinA.Digest(Protease.GetProtease("LysC"), 1, initiatorMethonine: true).ToList();
            Assert.Contains(new Peptide("MRGFK"), peptides);
            Assert.Contains(new Peptide("MMRGFK"), peptides);
        }

        [Test]
        public void SemiTrypiticDigestion()
        {
            Protein prot = new Protein("MMRGFKQRLIKKTTGSSSSSSSKKKDKEKEKEKSSTTSSTSKKPASASSSSHGTTHSSASSTGSKSTTEKGKQSGSVPSQ");

            var peptides = prot.Digest(Protease.GetProtease("Trypsin"), 0, 5, 10, semiDigestion: true).ToList();

            Assert.AreEqual(17, peptides.Count);
        }

        [Test]
        public void DuplicatePeptidesReturn()
        {
            Protein prot = new Protein("DEREKDEREK");

            var peptides = prot.Digest(Protease.GetProtease("LysC"), 0).ToList();
            Assert.AreEqual(peptides.Count, 2);
        }

        [Test]
        public void DuplicatePeptidesAreDistinct()
        {
            Protein prot = new Protein("DEREKDEREK");

            var peptides = prot.Digest(Protease.GetProtease("LysC"), 0).ToList();
            Assert.AreNotSame(peptides[0], peptides[1]);
        }

        [Test]
        public void DuplicatePeptidesAreEqualivant()
        {
            Protein prot = new Protein("DEREKDEREK");

            var peptides = prot.Digest(Protease.GetProtease("LysC"), 0).ToList();
            Assert.AreEqual(peptides[0], peptides[1]);
        }
    }
}