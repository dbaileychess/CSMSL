using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should.Fluent;
using CSMSL.Proteomics;

namespace CSMSL.Tests.Proteomics
{
    [TestFixture ( Category = "Protease") ]
    public sealed class ProteaseTestFixture
    {
        private Protein ProteinA;

        [SetUp]
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
            List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin);
            Peptide pepA = new Peptide("TTGSSSSSSSK");
            peptides.Should().Contain.Item(pepA);
        }

        [Test]
        public void TryspinNoProlineRuleDigestion()
        {
            List<Peptide> peptides = ProteinA.Digest(Protease.TrypsinNoProlineRule);
            peptides.Should().Contain.Item(new Peptide("INLFR"));
            peptides.Should().Not.Contain.Item(new Peptide("INLFRP"));    
        }

        [Test]
        public void DigestionMaxMissedCleavages()
        {            
            List<Peptide> peptides = ProteinA.Digest(Protease.TrypsinNoProlineRule, maxMissedCleavages: 0);
            peptides.Should().Contain.One(pep => pep.CountResidues('K') + pep.CountResidues('R') == 0); // one C-terminal Peptide
            peptides.Should().Not.Contain.Any(pep => (pep.CountResidues('K') + pep.CountResidues('R')) > 1); // no peptide should have more than one K or R

            peptides = ProteinA.Digest(Protease.TrypsinNoProlineRule, maxMissedCleavages: 3);
            peptides.Should().Not.Contain.Any(pep => (pep.CountResidues('K') + pep.CountResidues('R')) > 4);
        }

        [Test]
        public void DigestionMinLength()
        {
            for (int length = 0; length < 50; length += 5)
            {                
                List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin, minLength: length);
                peptides.Should().Not.Contain.Any(pep => pep.Length < length);
            }
        }

        [Test]
        public void DigestionMaxLength()
        {
            for (int length = 0; length < 50; length += 5)
            {
                List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin, maxLength: length);
                peptides.Should().Not.Contain.Any(pep => pep.Length > length);
            }
        }

        [Test]
        public void DigestionMinMaxLength()
        {
            for (int minLength = 0; minLength < 50; minLength += 5)
            {
                for (int maxLength = 0; maxLength < 50; maxLength += 5)
                {
                    List<Peptide> peptides = ProteinA.Digest(Protease.Trypsin, minLength: minLength, maxLength: maxLength);
                    peptides.Should().Not.Contain.Any(pep => pep.Length > maxLength && pep.Length < minLength);
                }
            }
        }


    }
}
