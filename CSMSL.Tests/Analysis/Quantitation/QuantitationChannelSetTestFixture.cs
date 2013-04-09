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
        }

        [Test]
        public void UnmodifiedPeptideCount()
        {
            Peptide pep = new Peptide("DEREK");
            QuantitationChannelSet.GetUniquePeptides(pep).ToList().Count.Should().Equal(1);
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
        
    }
}
