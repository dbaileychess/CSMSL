using CSMSL.Chemistry;
using CSMSL.Proteomics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture ( Category = "Protease") ]
    public sealed class ProteaseTestFixture
    {        
        private Protein _proteinA;

        [TestFixtureSetUp]
        public void Setup()
        {
            _proteinA = new Protein( "MMRGFKQRLIKKTTGSSSSSSSKKKDKEKEKEKSSTTSSTSKKPASASSSSHGTTHSSASSTGSKSTTEKGKQSGSVPSQ" +
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
            List<Peptide> peptides = _proteinA.Digest(Protease.Trypsin).ToList();
            Peptide pepA = new Peptide("TTGSSSSSSSK");

            Assert.Contains(pepA, peptides);
        }

        [Test]
        public void TryspinNoProlineRuleDigestion()
        {
            List<Peptide> peptides = _proteinA.Digest(Protease.TrypsinNoProlineRule).ToList();

            Assert.Contains(new Peptide("INLFR"), peptides);
        }

        [Test]
        public void NullEnzymeDigestion()
        {
            List<Peptide> peptides = _proteinA.Digest((IProtease)null).ToList();

            Assert.AreEqual(1, peptides.Count);
        }

        [Test]
        public void NoEnzymeDigestion()
        {
            List<Peptide> peptides = _proteinA.Digest(Protease.None, maxMissedCleavages: 8, minLength: 5).ToList();
            Assert.Contains(new Peptide("SLYHPQ"), peptides);
        }

        [Test]
        public void MultipleProteaseDigestion()
        {
            List<IProtease> proteases = new List<IProtease> {Protease.Trypsin, Protease.GluC};

            List<Peptide> peptides = _proteinA.Digest(proteases, maxMissedCleavages: 1, maxLength: 5).ToList();
            Assert.Contains(new Peptide("NWSK"), peptides);
            Assert.Contains(new Peptide("ENWSK"), peptides);
        }
        
        [Test]
        public void DigestionPerservesCTerminalModification()
        {
            Protein prot = new Protein("MMRGFKQRLIKKTTGSSSSSSSKKKDKEKEKEKSSTTSSTSKKPASASSSSHGTTHSSASSTGSKSTTEKGKQSGSVPSQ");
            prot.SetModification(NamedChemicalFormula.iTRAQ4Plex, Terminus.C);

            Peptide peptide = new Peptide("QSGSVPSQ");
            peptide.SetModification(NamedChemicalFormula.iTRAQ4Plex, Terminus.C);

            var peptides = prot.Digest(Protease.Trypsin, 0, 5, 10).ToList();

            Assert.Contains(peptide, peptides);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InvalidMaxMissedClevages()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _proteinA.Digest(Protease.Trypsin, -1));
        }


    }
}
