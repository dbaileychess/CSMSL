using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Should.Fluent;
using CSMSL.Analysis.Quantitation;
using CSMSL.Proteomics;
using CSMSL.Chemistry;

namespace CSMSL.Tests.Analysis.Quantitation
{
    [TestFixture]
    [Category("Quantitation")]
    public sealed class QuantitationChannelSetTestFixture
    {
        private QuantitationChannelSet _TMT6plex;
        private QuantitationChannelSet _lysine6plex;
        private QuantitationChannelSet _mTRAQ3plex;
        private QuantitationChannelSet _lysine2plex;
        private QuantitationChannelSet _lysine3plex;
        private QuantitationChannelSet _arginine3plex;
        private QuantitationChannelSet _tag4plex;
        private double tolerance = 0.0000001;

        [SetUp]
        public void SetUp()
        {
            _TMT6plex = new QuantitationChannelSet("TMT 6-plex");
            _TMT6plex.Add(new IsobaricTag("C{12}8 H{1}16 N{14}1", "C{13}4 H{1}4 N{15}1 O{16}2", "126"));
            _TMT6plex.Add(new IsobaricTag("C{12}8 H{1}16 N{15}1", "C{13}4 H{1}4 N{14}1 O{16}2", "127"));
            _TMT6plex.Add(new IsobaricTag("C{12}6 C{13}2 H{1}16 N{14}1", "C{12}2 C{13}2 H{1}4 N{15}1 O{16}2", "128"));
            _TMT6plex.Add(new IsobaricTag("C{12}6 C{13}2 H{1}16 N{15}1", "C{12}2 C{13}2 H{1}4 N{14}1 O{16}2", "129"));
            _TMT6plex.Add(new IsobaricTag("C{12}4 C{13}4 H{1}16 N{14}1", "C{12}4 H{1}5 N{15}4 O{16}2", "130"));
            _TMT6plex.Add(new IsobaricTag("C{12}4 C{13}4 H{1}16 N{15}1", "C{12}4 H{1}5 N{14}4 O{16}2", "131"));

            _lysine6plex = new QuantitationChannelSet("Lysine 6-plex");
            _lysine6plex.Add(new Isotopologue("C-6 C{13}6 N-2 N{15}2", "C6 N2"));
            _lysine6plex.Add(new Isotopologue("C-4 C{13}4 N-2 N{15}2 H-2 H{2}2", "C4 N2 D2"));
            _lysine6plex.Add(new Isotopologue("C-5 C{13}5 N-1 N{15}1 H-2 H{2}2", "C5 N1 D2"));
            _lysine6plex.Add(new Isotopologue("C-3 C{13}3 N-1 N{15}1 H-4 H{2}4", "C3 N1 H4"));
            _lysine6plex.Add(new Isotopologue("C-4 C{13}4 H-4 H{2}4", "C4 D4"));
            _lysine6plex.Add(new Isotopologue("H-8 H{2}8", "D8"));

            _mTRAQ3plex = new QuantitationChannelSet("mTRAQ 3-plex");
            _mTRAQ3plex.Add(new Isotopologue("H{1}12 C{12}7 N{14}2 O{16}1", "O"));
            _mTRAQ3plex.Add(new Isotopologue("H{1}12 C{12}4 C{13}3 N{14}1 N{15}1 O{16}1", "C3 N1"));
            _mTRAQ3plex.Add(new Isotopologue("H{1}12 C{12}1 C{13}6 N{15}2 O{16}1", "C6 N2"));

            _lysine2plex = new QuantitationChannelSet("Lysine 2-plex");
            _lysine2plex.Add(new Isotopologue("C-6 C{13}6 N-2 N{15}2", "C6 N2"));
            _lysine2plex.Add(new Isotopologue("H-8 H{2}8", "D8"));

            _arginine3plex = new QuantitationChannelSet("Arginine 3-plex");
            _arginine3plex.Add(new Isotopologue("", "0"));
            _arginine3plex.Add(new Isotopologue("C-6 C{13}6", "C6"));
            _arginine3plex.Add(new Isotopologue("C-6 C{13}6 N-4 N{15}4", "C6 N4"));

            _tag4plex = new QuantitationChannelSet("Tag 4-plex");
            _tag4plex.Add(new Isotopologue("C{12}18 H{1}31 N{14}1 O{16}5 N{15}6", "N6"));
            _tag4plex.Add(new Isotopologue("C{12}16 H{1}31 N{14}3 O{16}5 N{15}4 C{13}2", "C2 N4"));
            _tag4plex.Add(new Isotopologue("C{12}14 H{1}31 N{14}5 O{16}5 N{15}2 C{13}4", "C6 N4"));
            _tag4plex.Add(new Isotopologue("C{12}12 H{1}31 N{14}7 O{16}5 C{13}6", "C6"));

            _lysine3plex = new QuantitationChannelSet("Lysine 3-plex");
            _lysine3plex.Add(new Isotopologue("", "0"));
            _lysine3plex.Add(new Isotopologue("H-4 H{2}4", "D4"));
            _lysine3plex.Add(new Isotopologue("C-6 C{13}6 N-2 N{15}2", "C6 N2"));
        }

        [Test]
        public void UnmodifiedPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            QuantitationChannelSet.GetUniquePeptides(pep).ToList().Count.Should().Equal(1);
        }

        [Test]
        public void UnmodifiedPeptideEquality()
        {
            Peptide pep = new Peptide("DEREK");
            QuantitationChannelSet.GetUniquePeptides(pep).ToList()[0].Should().Equal(pep);
        }

        [Test]
        public void UnmodifiedPeptideMass()
        {
            double mass = 675.31876792107;
            Peptide pep = new Peptide("DEREK");
            QuantitationChannelSet.GetUniquePeptides(pep).ToList()[0].Mass.Monoisotopic.Should().Be.InRange(mass - tolerance, mass + tolerance);
        }

        [Test]
        public void OneModIsotopologuePeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_lysine6plex, 'K');
            QuantitationChannelSet.GetUniquePeptides(pep).ToList().Count.Should().Equal(6);
        }

        [Test]
        public void OneModIsotopologuePeptideMass()
        {
            double mass = 683.33296673467;
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_lysine6plex, 'K');
            QuantitationChannelSet.GetUniquePeptides(pep).ToList()[0].Mass.Monoisotopic.Should().Be.InRange(mass - tolerance, mass + tolerance);
        }

        [Test]
        public void TwoModsIsotopologuePeptideCount()
        {
            Peptide pep = new Peptide("DEREKK");
            pep.SetModification(_lysine6plex, 'K');
            QuantitationChannelSet.GetUniquePeptides(pep).ToList().Count.Should().Equal(6);
        }

        [Test]
        public void TwoModsIsotopologuePeptideMass()
        {
            double mass = 819.44212856227;
            Peptide pep = new Peptide("DEREKK");
            pep.SetModification(_lysine6plex, 'K');
            QuantitationChannelSet.GetUniquePeptides(pep).ToList()[0].Mass.Monoisotopic.Should().Be.InRange(mass - tolerance, mass + tolerance);
        }

        [Test]
        public void OneModIsobaricPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_TMT6plex, 'K');
            QuantitationChannelSet.GetUniquePeptides(pep).ToList().Count.Should().Equal(6);
        }
        
        [Test]
        public void OneModIsobaricPeptideMass()
        {
            double mass = 904.48170005579;
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_TMT6plex, 'K');
            QuantitationChannelSet.GetUniquePeptides(pep).ToList()[0].Mass.Monoisotopic.Should().Be.InRange(mass - tolerance, mass + tolerance);
        }

        [Test]
        public void TwoModsIsobaricPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_TMT6plex, 'K');
            pep.SetModification(_TMT6plex, 1);
            QuantitationChannelSet.GetUniquePeptides(pep).ToList().Count.Should().Equal(6);
        }

        [Test]
        public void TwoModsIsobaricPeptideMass()
        {
            double mass = 1133.64463219051;
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_TMT6plex, 'K');
            pep.SetModification(_TMT6plex, 1);
            QuantitationChannelSet.GetUniquePeptides(pep).ToList()[0].Mass.Monoisotopic.Should().Be.InRange(mass - tolerance, mass + tolerance);
        }

        [Test]
        public void TMT6PlusSILAC3NoKPeptideCount()
        {
            Peptide pep = new Peptide("DERE");
            pep.SetModification(_TMT6plex, 1);
            pep.SetModification(_TMT6plex, 'K');
            pep.SetModification(_lysine3plex, 'K');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(6);
        }

        [Test]
        public void TMT6PlusSILAC3WithKPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            ModificationCollection col = new ModificationCollection();
            col.Add(_lysine3plex);
            col.Add(_TMT6plex);
            pep.SetModification(col, 'K');
            pep.SetModification(_TMT6plex, Terminus.N);
            //pep.SetModification(_TMT6plex, 'K');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(18);
        }

        [Test]
        public void NeuCode2PlusMTRAQ3NoKPeptideCount()
        {
            Peptide pep = new Peptide("DERE");
            pep.SetModification(_lysine2plex, 'K');
            pep.SetModification(_mTRAQ3plex, 1);
            pep.SetModification(_mTRAQ3plex, 'K');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(3);
        }

        [Test]
        public void NeuCode2PlusMTRAQ3WithKPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_lysine2plex, 'K');
            pep.SetModification(_mTRAQ3plex, 1);
            pep.SetModification(_mTRAQ3plex, 'K');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(6);
        }

        [Test]
        public void NeuCode2PlusSILACNoRPeptideCount()
        {
            Peptide pep = new Peptide("DEEK");
            pep.SetModification(_lysine2plex, 'K');
            pep.SetModification(_arginine3plex, 'R');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(2);
        }

        [Test]
        public void NeuCode2PlusSILACWithRPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_lysine2plex, 'K');
            pep.SetModification(_arginine3plex, 'R');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(6);
        }

        [Test]
        public void NeuCodeTag4PlusSILACNoKPeptideCount()
        {
            Peptide pep = new Peptide("DERE");
            pep.SetModification(_lysine3plex, 'K');
            pep.SetModification(_tag4plex, 1);
            pep.SetModification(_tag4plex, 'K');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(4);
        }

        [Test]
        public void NeuCodeTag4PlusSILACWithKPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            pep.SetModification(_lysine3plex, 'K');
            pep.SetModification(_tag4plex, 1);
            pep.SetModification(_tag4plex, 'K');
            List<Peptide> peps = QuantitationChannelSet.GetUniquePeptides(pep).ToList();
            peps.Count.Should().Equal(12);
        }
    }
}
