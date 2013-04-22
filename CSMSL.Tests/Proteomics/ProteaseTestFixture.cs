using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;
using Should.Fluent;
using CSMSL.Proteomics;
using CSMSL.Chemistry;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture ( Category = "Protease") ]
    public sealed class ProteaseTestFixture
    {        
        private Protein ProteinA;

        [TestFixtureSetUp]
        public void Setup()
        {
            ProteinA = new Protein( "MMRGFKQRLIKKTTGSSSSSSSKKKDKEKEKEKSSTTSSTSKKPASASSSSHGTTHSSASSTGSKSTTEKGKQSGSVPSQ" +
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
            List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin).ToList();
            Peptide pepA = new Peptide("TTGSSSSSSSK");
            peptides.Should().Contain.Item(pepA);
        }

        [Test]
        public void TryspinNoProlineRuleDigestion()
        {
            List<Peptide> peptides = ProteinA.Digest(Protease.TrypsinNoProlineRule).ToList();
            peptides.Should().Contain.Item(new Peptide("INLFR"));
            peptides.Should().Not.Contain.Item(new Peptide("INLFRP"));    
        }

        [Test]
        public void NullEnzymeDigestion()
        {
            IProtease protease = null;
            List<Peptide> peptides = ProteinA.Digest(protease).ToList();
            peptides.Should().Count.Exactly(1);
        }

        [Test]
        public void NoEnzymeDigestion()
        {
            List<Peptide> peptides = ProteinA.Digest(Protease.None, maxMissedCleavages: 8, minLength: 5).ToList(); 
            peptides.Should().Contain.One(new Peptide("SLYHPQ"));
        }

        [Test]
        public void MultipleProteaseDigestion()
        {
            List<IProtease> proteases = new List<IProtease>();
            proteases.Add(Protease.Trypsin);
            proteases.Add(Protease.GluC);

            List<Peptide> peptides = ProteinA.Digest(proteases, maxMissedCleavages: 1, maxLength: 5).ToList(); 
            peptides.Should().Contain.One(new Peptide("NWSK"));
            peptides.Should().Contain.One(new Peptide("ENWSK"));
        }

        [Test]
        public void DigestionMaxMissedCleavages()
        {
            List<Peptide> peptides = ProteinA.Digest(Protease.TrypsinNoProlineRule, maxMissedCleavages: 0).ToList();
            peptides.Should().Contain.One(pep => pep.ResidueCount('K') + pep.ResidueCount('R') == 0); // one C-terminal Peptide
            peptides.Should().Not.Contain.Any(pep => (pep.ResidueCount('K') + pep.ResidueCount('R')) > 1); // no peptide should have more than one K or R

            peptides = ProteinA.Digest(Protease.TrypsinNoProlineRule, maxMissedCleavages: 3).ToList();
            peptides.Should().Not.Contain.Any(pep => (pep.ResidueCount('K') + pep.ResidueCount('R')) > 4);
        }

        [Test]
        public void DigestionMinLength()
        {
            for (int length = 0; length < 50; length += 5)
            {
                List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin, minLength: length).ToList();
                peptides.Should().Not.Contain.Any(pep => pep.Length < length);
            }
        }

        [Test]
        public void DigestionMaxLength()
        {
            for (int length = 0; length < 50; length += 5)
            {
                List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin, maxLength: length).ToList();
                peptides.Should().Not.Contain.Any(pep => pep.Length > length);
            }
        }

        [Test]
        public void DigestionMinMaxLength()
        {
            for (int minLength = 0; minLength < 50; minLength += 10)
            {
                for (int maxLength = 0; maxLength < 50; maxLength += 10)
                {
                    List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin, minLength: minLength, maxLength: maxLength).ToList();
                    peptides.Should().Not.Contain.Any(pep => pep.Length > maxLength && pep.Length < minLength);
                }
            }
        }

        [Test]
        public void DigestionPerservesCTerminalModification()
        {
            Protein prot = new Protein("MMRGFKQRLIKKTTGSSSSSSSKKKDKEKEKEKSSTTSSTSKKPASASSSSHGTTHSSASSTGSKSTTEKGKQSGSVPSQ");
            prot.SetModification(NamedChemicalFormula.iTRAQ4Plex, Terminus.C);

            Peptide peptide = new Peptide("QSGSVPSQ");
            peptide.SetModification(NamedChemicalFormula.iTRAQ4Plex, Terminus.C);

            prot.Digest(Protease.Trypsin, 0, 5, 10).Should().Contain.Item(peptide);            
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InvalidMaxMissedClevages()
        {
            ProteinA.Digest(Protease.Trypsin, maxMissedCleavages: -1).ToList();  
        }


    }
}
