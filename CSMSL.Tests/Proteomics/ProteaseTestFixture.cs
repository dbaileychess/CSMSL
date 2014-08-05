// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (ProteaseTestFixture.cs) is part of CSMSL.Tests.
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