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
            List<Peptide> peptides = _proteinA.Digest(Protease.Trypsin).ToList();
            Peptide pepA = new Peptide("TTGSSSSSSSK");

            Assert.Contains(pepA, peptides);
        }

        [Test]
        public void MissedClevageNoneCTerminus()
        {
            int missedClevages = Protease.Trypsin.MissedCleavages("DEFEK");

            Assert.AreEqual(0, missedClevages);
        }
        
        [Test]
        public void MissedClevageNoneNTerminus()
        {
            int missedClevages = Protease.LysN.MissedCleavages("KERED");

            Assert.AreEqual(0, missedClevages);
        }

        [Test]
        public void MissedClevageOneInternalNTerminus()
        {
            int missedClevages = Protease.LysN.MissedCleavages("KEKED");

            Assert.AreEqual(1, missedClevages);
        }

        [Test]
        public void MissedClevageOneInternalCTerminus()
        {
            int missedClevages = Protease.Trypsin.MissedCleavages("DEKEK");

            Assert.AreEqual(1, missedClevages);
        }

        [Test]
        public void MissedClevageTwoInternalCTerminus()
        {
            int missedClevages = Protease.Trypsin.MissedCleavages("DEKAAKEK");

            Assert.AreEqual(2, missedClevages);
        }

        [Test]
        public void MissedClevageTwoInternalNTerminus()
        {
            int missedClevages = Protease.LysN.MissedCleavages("KEDEKEKED");

            Assert.AreEqual(2, missedClevages);
        }

        [Test]
        public void MissedClevageOneCTerminalProteinCTerminus()
        {
            int missedClevages = Protease.Trypsin.MissedCleavages("KEEEEEE");

            Assert.AreEqual(1, missedClevages);
        }

        [Test]
        public void MissedClevageOneNTerminalProteinNTerminus()
        {
            int missedClevages = Protease.LysN.MissedCleavages("EEEEEEEK");

            Assert.AreEqual(1, missedClevages);
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
            List<Peptide> peptides = _proteinA.Digest((IProtease)null, initiatorMethonine: false).ToList();

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
        public void InitiatorMethonineCleaved()
        {
            List<Peptide> peptides = _proteinA.Digest(Protease.LysC, 1, initiatorMethonine: true).ToList();
            Assert.Contains(new Peptide("MRGFK"), peptides);
            Assert.Contains(new Peptide("MMRGFK"), peptides);
        }
  

        [Test]
        public void SemiTrypiticDigestion()
        {
            Protein prot = new Protein("MMRGFKQRLIKKTTGSSSSSSSKKKDKEKEKEKSSTTSSTSKKPASASSSSHGTTHSSASSTGSKSTTEKGKQSGSVPSQ");
         
            var peptides = prot.Digest(Protease.Trypsin, 0, 5, 10, semiDigestion: true).ToList();

            Assert.AreEqual(17, peptides.Count);
        }

    }
}
