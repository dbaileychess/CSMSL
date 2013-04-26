//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Data.Linq;
//using System.Text;
//using NUnit.Framework;
//using Should.Fluent;
//using Moq;
//using CSMSL.Analysis.Quantitation;
//using CSMSL.Spectral;
//using CSMSL.Proteomics;
//using CSMSL.Chemistry;
//using CSMSL.Analysis.Identification;

//namespace CSMSL.Tests.Analysis.Quantitation
//{
//    [TestFixture]
//    [Category("Quantitation")]    
//    public sealed class QuantifiedPeptideTestFixture
//    {
//        private double tolerance = 0.000000000001;
//        private MSDataScan Scan1;
//        private MSDataScan Scan2;
//        private MSDataScan Scan3;
//        private MsnDataScan Ms2Scan1;
//        private MsnDataScan Ms2Scan2;
//        private MsnDataScan Ms2Scan3;
//        private PeptideSpectralMatch Psm1;
//        private PeptideSpectralMatch Psm2;
//        private PeptideSpectralMatch Psm3;
        
        
//        // TMT peptide
//        private QuantifiedScan QuantScanTMT1;
//        private QuantifiedScan QuantScanTMT2;
//        private QuantifiedScan QuantScanTMT3;
//        private Peptide Peptide1;
//        private QuantifiedPeptide QuantPeptide1;
//        private MSDataScan DataScanTMT1;
//        private MSDataScan DataScanTMT2;
//        private MSDataScan DataScanTMT3;
//        private MsnDataScan DataScanTMT1Msn;
//        private MsnDataScan DataScanTMT2Msn;
//        private MsnDataScan DataScanTMT3Msn;
//        private PeptideSpectralMatch PsmTMT1;
//        private PeptideSpectralMatch PsmTMT2;
//        private PeptideSpectralMatch PsmTMT3;
//        private QuantifiedPeak QuantPeakTMT1;
//        private QuantifiedPeak QuantPeakTMT2;
//        private QuantifiedPeak QuantPeakTMT3;
//        private QuantifiedPeak QuantPeakTMT4;
//        private QuantifiedPeak QuantPeakTMT5;
//        private QuantifiedPeak QuantPeakTMT6;
//        private QuantifiedPeak QuantPeakTMT7;
//        private QuantifiedPeak QuantPeakTMT8;
//        private QuantifiedPeak QuantPeakTMT9;
//        private QuantifiedPeak QuantPeakTMT10;
//        private QuantifiedPeak QuantPeakTMT11;
//        private QuantifiedPeak QuantPeakTMT12;
//        private QuantifiedPeak QuantPeakTMT13;
//        private QuantifiedPeak QuantPeakTMT14;
//        private QuantifiedPeak QuantPeakTMT15;
//        private QuantifiedPeak QuantPeakTMT16;
//        private QuantifiedPeak QuantPeakTMT17;
//        private QuantifiedPeak QuantPeakTMT18;
//        private IQuantitationChannel IQuantitationChannelTMT1;
//        private IQuantitationChannel IQuantitationChannelTMT2;
//        private IQuantitationChannel IQuantitationChannelTMT3;
//        private IQuantitationChannel IQuantitationChannelTMT4;
//        private IQuantitationChannel IQuantitationChannelTMT5;
//        private IQuantitationChannel IQuantitationChannelTMT6;

//        // NeuCode peptide
//        private QuantifiedScan QuantScanNeuCode1;
//        private QuantifiedScan QuantScanNeuCode2;
//        private QuantifiedScan QuantScanNeuCode3;
//        private Peptide Peptide2;
//        private QuantifiedPeptide QuantPeptide2;
//        private MSDataScan DataScanNeuCode1;
//        private MSDataScan DataScanNeuCode2;
//        private MSDataScan DataScanNeuCode3;
//        private MsnDataScan DataScanNeuCode1Msn;
//        private MsnDataScan DataScanNeuCode2Msn;
//        private MsnDataScan DataScanNeuCode3Msn;
//        private PeptideSpectralMatch PsmNeuCode1;
//        private PeptideSpectralMatch PsmNeuCode2;
//        private PeptideSpectralMatch PsmNeuCode3;
//        private QuantifiedPeak QuantPeakNeuCode1;
//        private QuantifiedPeak QuantPeakNeuCode2;
//        private QuantifiedPeak QuantPeakNeuCode3;
//        private QuantifiedPeak QuantPeakNeuCode4;
//        private QuantifiedPeak QuantPeakNeuCode5;
//        private QuantifiedPeak QuantPeakNeuCode6;
//        private QuantifiedPeak QuantPeakNeuCode7;
//        private QuantifiedPeak QuantPeakNeuCode8;
//        private QuantifiedPeak QuantPeakNeuCode9;
//        private QuantifiedPeak QuantPeakNeuCode10;
//        private QuantifiedPeak QuantPeakNeuCode11;
//        private QuantifiedPeak QuantPeakNeuCode12;
//        private QuantifiedPeak QuantPeakNeuCode13;
//        private QuantifiedPeak QuantPeakNeuCode14;
//        private QuantifiedPeak QuantPeakNeuCode15;
//        private QuantifiedPeak QuantPeakNeuCode16;
//        private QuantifiedPeak QuantPeakNeuCode17;
//        private QuantifiedPeak QuantPeakNeuCode18;
//        private IQuantitationChannel IQuantitationChannelNeuCode1;
//        private IQuantitationChannel IQuantitationChannelNeuCode2;

//        [SetUp]
//        public void SetUp()
//        {
//            // Mock MS1 and MS2 scans
//            var MS1Scan1 = new Mock<MSDataScan>();
//            MS1Scan1.Setup(scan => scan.SpectrumNumber).Returns(4837);
//            MS1Scan1.Setup(scan => scan.MsnOrder).Returns(1);
//            MS1Scan1.Setup(scan => scan.InjectionTime).Returns(1.5);
//            Scan1 = MS1Scan1.Object;

//            var MS1Scan2 = new Mock<MSDataScan>();
//            MS1Scan2.Setup(scan => scan.SpectrumNumber).Returns(4900);
//            MS1Scan2.Setup(scan => scan.MsnOrder).Returns(1);
//            MS1Scan2.Setup(scan => scan.InjectionTime).Returns(1.1);
//            Scan2 = MS1Scan2.Object;

//            var MS1Scan3 = new Mock<MSDataScan>();
//            MS1Scan3.Setup(scan => scan.SpectrumNumber).Returns(4750);
//            MS1Scan3.Setup(scan => scan.MsnOrder).Returns(1);
//            MS1Scan3.Setup(scan => scan.InjectionTime).Returns(0.9);
//            Scan3 = MS1Scan3.Object;

//            var MS2Scan1 = new Mock<MsnDataScan>();
//            MS2Scan1.Setup(scan => scan.SpectrumNumber).Returns(4902);
//            MS2Scan1.Setup(scan => scan.MsnOrder).Returns(2);
//            MS2Scan1.Setup(scan => scan.InjectionTime).Returns(12.3);
//            MS2Scan1.Setup(scan => scan.PrecursorCharge).Returns(3);
//            Ms2Scan1 = MS2Scan1.Object;

//            var MS2Scan2 = new Mock<MsnDataScan>();
//            MS2Scan2.Setup(scan => scan.SpectrumNumber).Returns(5317);
//            MS2Scan2.Setup(scan => scan.MsnOrder).Returns(2);
//            MS2Scan2.Setup(scan => scan.InjectionTime).Returns(54.8);
//            MS2Scan2.Setup(scan => scan.PrecursorCharge).Returns(2);
//            Ms2Scan2 = MS2Scan2.Object;

//            var MS2Scan3 = new Mock<MsnDataScan>();
//            MS2Scan3.Setup(scan => scan.SpectrumNumber).Returns(4212);
//            MS2Scan3.Setup(scan => scan.MsnOrder).Returns(2);
//            MS2Scan3.Setup(scan => scan.InjectionTime).Returns(25.7);
//            MS2Scan3.Setup(scan => scan.PrecursorCharge).Returns(3);
//            Ms2Scan3 = MS2Scan3.Object;

//            // Mock PSMs
//            Psm1 = new PeptideSpectralMatch();
//            Psm2 = new PeptideSpectralMatch();
//            Psm3 = new PeptideSpectralMatch();
//            Psm1.Spectrum = Ms2Scan1;
//            Psm1.Score = 0.001;
//            Psm2.Spectrum = Ms2Scan2;
//            Psm2.Score = 0.00000001;
//            Psm3.Spectrum = Ms2Scan3;
//            Psm3.Score = 0.000001;
            
//            // Mock TMT peptide
//            Peptide1 = new Peptide("TTGSSSSSSSK");
//            QuantPeptide1 = new QuantifiedPeptide(Peptide1);
//            QuantScanTMT1 = new QuantifiedScan(Ms2Scan1, ((int)Ms2Scan1.PrecursorCharge));
//            QuantScanTMT2 = new QuantifiedScan(Ms2Scan2, ((int)Ms2Scan2.PrecursorCharge));
//            QuantScanTMT3 = new QuantifiedScan(Ms2Scan3, ((int)Ms2Scan3.PrecursorCharge));

//            // Mock NeuCode peptide
//            Peptide2 = new Peptide("TTGSSSSSSSK");
//            QuantPeptide2 = new QuantifiedPeptide(Peptide2);
//            QuantScanNeuCode1 = new QuantifiedScan(Scan1, 3);
//            QuantScanNeuCode2 = new QuantifiedScan(Scan2, 2);
//            QuantScanNeuCode3 = new QuantifiedScan(Scan3, 3);

//            QuantPeakTMT1 = new QuantifiedPeak(null, 126.0, 1, 10.0, 1.0);
//            QuantPeakTMT2 = new QuantifiedPeak(null,127.0, 1, 5.0, 1.0);
//            QuantPeakTMT3 = new QuantifiedPeak(null,128.0, 1, 10.0, 1.0);
//            QuantPeakTMT4 = new QuantifiedPeak(null,129.0, 1, 5.0, 1.0);
//            QuantPeakTMT5 = new QuantifiedPeak(null,130.0, 1, 0.0, 1.0);
//            QuantPeakTMT6 = new QuantifiedPeak(null,131.0, 1, 5.0, 1.0);
//            QuantPeakTMT7 = new QuantifiedPeak(null,126.1, 1, 25.2, 1.5);
//            QuantPeakTMT8 = new QuantifiedPeak(null,127.1, 1, 11.3, 1.5);
//            QuantPeakTMT9 = new QuantifiedPeak(null,128.1, 1, 18.9, 1.5);
//            QuantPeakTMT10 = new QuantifiedPeak(null,129.1, 1, 2.5, 1.5);
//            QuantPeakTMT11 = new QuantifiedPeak(null,130.1, 1, 1.2, 1.5);
//            QuantPeakTMT12 = new QuantifiedPeak(null,131.1, 1, 10.0, 1.5);
//            QuantPeakTMT13 = new QuantifiedPeak(null,126.2, 1, 62.8, 2.0);
//            QuantPeakTMT14 = new QuantifiedPeak(null,127.2, 1, 38.3, 2.0);
//            QuantPeakTMT15 = new QuantifiedPeak(null,128.2, 1, 75.2, 2.0);
//            QuantPeakTMT16 = new QuantifiedPeak(null,129.2, 1, 9.1, 2.0);
//            QuantPeakTMT17 = new QuantifiedPeak(null,130.2, 1, 0.0, 2.0);
//            QuantPeakTMT18 = new QuantifiedPeak(null,131.2, 1, 4.4, 2.0);

//            IQuantitationChannelTMT1 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-126");
//            IQuantitationChannelTMT2 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-127");
//            IQuantitationChannelTMT3 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-128");
//            IQuantitationChannelTMT4 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-129");
//            IQuantitationChannelTMT5 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-130");
//            IQuantitationChannelTMT6 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-131");

//            QuantScanTMT1.AddQuant(IQuantitationChannelTMT1, QuantPeakTMT1);
//            QuantScanTMT1.AddQuant(IQuantitationChannelTMT2, QuantPeakTMT2);
//            QuantScanTMT1.AddQuant(IQuantitationChannelTMT3, QuantPeakTMT3);
//            QuantScanTMT1.AddQuant(IQuantitationChannelTMT4, QuantPeakTMT4);
//            QuantScanTMT1.AddQuant(IQuantitationChannelTMT5, QuantPeakTMT5);
//            QuantScanTMT1.AddQuant(IQuantitationChannelTMT6, QuantPeakTMT6);
//            QuantScanTMT2.AddQuant(IQuantitationChannelTMT1, QuantPeakTMT7);
//            QuantScanTMT2.AddQuant(IQuantitationChannelTMT2, QuantPeakTMT8);
//            QuantScanTMT2.AddQuant(IQuantitationChannelTMT3, QuantPeakTMT9);
//            QuantScanTMT2.AddQuant(IQuantitationChannelTMT4, QuantPeakTMT10);
//            QuantScanTMT2.AddQuant(IQuantitationChannelTMT5, QuantPeakTMT11);
//            QuantScanTMT2.AddQuant(IQuantitationChannelTMT6, QuantPeakTMT12);
//            QuantScanTMT3.AddQuant(IQuantitationChannelTMT1, QuantPeakTMT13);
//            QuantScanTMT3.AddQuant(IQuantitationChannelTMT2, QuantPeakTMT14);
//            QuantScanTMT3.AddQuant(IQuantitationChannelTMT3, QuantPeakTMT15);
//            QuantScanTMT3.AddQuant(IQuantitationChannelTMT4, QuantPeakTMT16);
//            QuantScanTMT3.AddQuant(IQuantitationChannelTMT5, QuantPeakTMT17);
//            QuantScanTMT3.AddQuant(IQuantitationChannelTMT6, QuantPeakTMT18);

//            QuantPeakNeuCode1 = new QuantifiedPeak(null,443.092, 3, 100.0, 10.0);
//            QuantPeakNeuCode2 = new QuantifiedPeak(null,443.103, 3, 50.0, 10.0);
//            QuantPeakNeuCode3 = new QuantifiedPeak(null,443.424, 3, 100.0, 10.0);
//            QuantPeakNeuCode4 = new QuantifiedPeak(null,443.433, 3, 50.0, 10.0);
//            QuantPeakNeuCode5 = new QuantifiedPeak(null,443.757, 3, 0.0, 10.0);
//            QuantPeakNeuCode6 = new QuantifiedPeak(null,443.770, 3, 50.0, 10.0);
//            QuantPeakNeuCode7 = new QuantifiedPeak(null,664.137, 2, 250.2, 10.5);
//            QuantPeakNeuCode8 = new QuantifiedPeak(null,664.153, 2, 110.3, 10.5);
//            QuantPeakNeuCode9 = new QuantifiedPeak(null,664.640, 2, 180.9, 10.5);
//            QuantPeakNeuCode10 = new QuantifiedPeak(null,664.655, 2, 20.5, 10.5);
//            QuantPeakNeuCode11 = new QuantifiedPeak(null,665.137, 2, 10.2, 10.5);
//            QuantPeakNeuCode12 = new QuantifiedPeak(null,665.160, 2, 100.0, 10.5);
//            QuantPeakNeuCode13 = new QuantifiedPeak(null,443.089, 3, 620.8, 20.0);
//            QuantPeakNeuCode14 = new QuantifiedPeak(null,443.100, 3, 380.3, 20.0);
//            QuantPeakNeuCode15 = new QuantifiedPeak(null,443.421, 3, 750.2, 20.0);
//            QuantPeakNeuCode16 = new QuantifiedPeak(null,443.435, 3, 90.1, 20.0);
//            QuantPeakNeuCode17 = new QuantifiedPeak(null,443.754, 3, 0.0, 20.0);
//            QuantPeakNeuCode18 = new QuantifiedPeak(null,443.764, 3, 40.4, 20.0);

//            IQuantitationChannelNeuCode1 = new Isotopologue("C-6 C{13}6 N-2 N{15}2", "NeuCode-K-13C6,15N2");
//            IQuantitationChannelNeuCode2 = new Isotopologue("H-8 H{2}8", "NeuCode-K-2H8");

//            QuantScanNeuCode1.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode1, 0);
//            QuantScanNeuCode1.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode2, 0);
//            QuantScanNeuCode1.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode3, 1);
//            QuantScanNeuCode1.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode4, 1);
//            QuantScanNeuCode1.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode5, 2);
//            QuantScanNeuCode1.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode6, 2);
//            QuantScanNeuCode2.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode7, 0);
//            QuantScanNeuCode2.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode8, 0);
//            QuantScanNeuCode2.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode9, 1);
//            QuantScanNeuCode2.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode10, 1);
//            QuantScanNeuCode2.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode11, 2);
//            QuantScanNeuCode2.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode12, 2);
//            QuantScanNeuCode3.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode13, 0);
//            QuantScanNeuCode3.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode14, 0);
//            QuantScanNeuCode3.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode15, 1);
//            QuantScanNeuCode3.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode16, 1);
//            QuantScanNeuCode3.AddQuant(IQuantitationChannelNeuCode1, QuantPeakNeuCode17, 2);
//            QuantScanNeuCode3.AddQuant(IQuantitationChannelNeuCode2, QuantPeakNeuCode18, 2);
//        }

//        private void AddAllPSMs()
//        {
//            QuantPeptide1.AddPeptideSpectralMatch(Psm1);
//            QuantPeptide1.AddPeptideSpectralMatch(Psm2);
//            QuantPeptide1.AddPeptideSpectralMatch(Psm3);

//            QuantPeptide2.AddPeptideSpectralMatch(Psm1);
//            QuantPeptide2.AddPeptideSpectralMatch(Psm2);
//            QuantPeptide2.AddPeptideSpectralMatch(Psm3);
//        }

//        private void AddAllQuantScans()
//        {
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT2);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT3);

//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode2);
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode3);
//        }
        
//        [Test]
//        public void AddPSM()
//        {
//            QuantPeptide1.AddPeptideSpectralMatch(Psm1);
//            QuantPeptide2.AddPeptideSpectralMatch(Psm1);
//            QuantPeptide2.AddPeptideSpectralMatch(Psm2);
//            QuantPeptide1.PsmCount.Should().Equal(1);
//            QuantPeptide2.PsmCount.Should().Equal(2);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void AddNullPSM()
//        {
//            Psm1 = null;
//            QuantPeptide1.AddPeptideSpectralMatch(Psm1);
//        }

//        [Test]     
//        public void AddDuplicatePSM()
//        {
//            Psm2 = Psm1;
//            QuantPeptide1.AddPeptideSpectralMatch(Psm1);
//            QuantPeptide1.AddPeptideSpectralMatch(Psm2);
//            QuantPeptide1.PsmCount.Should().Equal(1);
//        }

//        [Test]
//        public void BestPSM()
//        {
//            AddAllPSMs();
//            QuantPeptide1.BestPSM.Spectrum.SpectrumNumber.Should().Equal(5317);
//        }

//        [Test]
//        public void AddQuantScan()
//        {
//            AddAllPSMs();
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//            QuantPeptide1.QuantifiedScans[0].Charge.Should().Equal(3);
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//            QuantPeptide2.QuantifiedScans[0].Charge.Should().Equal(3);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void AddNullQuantScan()
//        {
//            QuantScanTMT1 = null;
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void AddDuplicateQuantScan()
//        {
//            AddAllPSMs();
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//            QuantPeptide2.QuantifiedScanCount.Should().Equal(2);

//            QuantScanTMT2 = QuantScanTMT1;
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT2);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentOutOfRangeException))]
//        public void AddInvalidQuantScan()
//        {
//            QuantPeptide1.AddPeptideSpectralMatch(Psm1);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT2);

//            QuantPeptide2.AddPeptideSpectralMatch(Psm1);
//            QuantPeptide2.AddPeptideSpectralMatch(Psm3);
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//        }

//        [Test]
//        public void SumIQuantitationChannelIntensity()
//        {
//            double value1 = 0.0;
//            double value2 = 225.00;

//            AddAllPSMs();

//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT2);
//            QuantPeptide1.GetIQuantitationChannelIntensity(IQuantitationChannelTMT5, IntensityWeightingType.Summed).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//            QuantPeptide2.GetIQuantitationChannelIntensity(IQuantitationChannelNeuCode2, IntensityWeightingType.Summed).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
        
//        }

//        [Test]
//        public void SumIQuantitationChannelIntensityNBC()
//        {
//            double value1 = 94.5;
//            double value2 = 315.0;

//            AddAllPSMs();

//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT2);
//            QuantPeptide1.GetIQuantitationChannelIntensity(IQuantitationChannelTMT5, IntensityWeightingType.Summed, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//            QuantPeptide2.GetIQuantitationChannelIntensity(IQuantitationChannelNeuCode1, IntensityWeightingType.Summed, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        //[Test]
//        //[ExpectedException(typeof(DivideByZeroException))]
//        //public void AverageIQuantitationChannelIntensity()
//        //{
//        //    double value2 = 193.5525;

//        //    AddAllPSMs();

//        //    QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//        //    QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode2);
//        //    QuantPeptide2.GetIQuantitationChannelIntensity(IQuantitationChannelNeuCode1, IntensityWeightingType.Average).Should().Be.InRange(value2 - tolerance, value2 + tolerance);

//        //    QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//        //    QuantPeptide1.AddQuantifiedScan(QuantScanTMT2);
//        //    QuantPeptide1.AddQuantifiedScan(QuantScanTMT3);
//        //    QuantPeptide1.GetIQuantitationChannelIntensity(IQuantitationChannelTMT5, IntensityWeightingType.Average);
//        //}

//        [Test]
//        public void AverageIQuantitationChannelIntensityNBC()
//        {
//            double value1 = 48.6333333333333;
//            double value2 = 228.073333333333;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetIQuantitationChannelIntensity(IQuantitationChannelTMT5, IntensityWeightingType.Average, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);
            
//            QuantPeptide2.GetIQuantitationChannelIntensity(IQuantitationChannelNeuCode1, IntensityWeightingType.Average, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void MedianIQuantitationChannelIntensity()
//        {
//            double value1 = 0;
//            double value2 = 174.495;

//            AddAllPSMs();

//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT1);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT2);
//            QuantPeptide1.AddQuantifiedScan(QuantScanTMT3);
//            QuantPeptide1.GetIQuantitationChannelIntensity(IQuantitationChannelTMT5, IntensityWeightingType.Median).Should().Be.InRange(value1 - tolerance, value1 + tolerance);
            
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode1);
//            QuantPeptide2.AddQuantifiedScan(QuantScanNeuCode2);
//            QuantPeptide2.GetIQuantitationChannelIntensity(IQuantitationChannelNeuCode1, IntensityWeightingType.Median).Should().Be.InRange(value2 - tolerance, value2 + tolerance);            
//        }

//        [Test]
//        public void MedianIQuantitationChannelIntensityNBC()
//        {
//            double value1 = 51.4;
//            double value2 = 150.0;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetIQuantitationChannelIntensity(IQuantitationChannelTMT5, IntensityWeightingType.Median, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetIQuantitationChannelIntensity(IQuantitationChannelNeuCode1, IntensityWeightingType.Median, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void OverallRatioSum()
//        {
//            double value1 = 0.534025889054;
//            double value2 = 0.438068631698;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetOverallRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Summed).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetOverallRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Summed).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void OverallRatioAverage()
//        {
//            double value1 = 0.534025889054;
//            double value2 = 0.375487398599;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetOverallRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Average).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetOverallRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Average).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void OverallRatioMedian()
//        {
//            double value1 = 0.448412698413;
//            double value2 = 0.342000379579;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetOverallRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Median).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetOverallRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Median).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void OverallRatioSumNBC()
//        {
//            double value1 = 0.534025889054;
//            double value2 = 0.442956943673;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetOverallRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Summed, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetOverallRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Summed, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void OverallRatioAverageNBC()
//        {
//            double value1 = 0.534025889054;
//            double value2 = 0.442956943673;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetOverallRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Average, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetOverallRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Average, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void OverallRatioMedianNBC()
//        {
//            double value1 = 0.448412698413;
//            double value2 = 0.5;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetOverallRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Median, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetOverallRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Median, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }
        
//        [Test]
//        public void RatioListAverage()
//        {
//            double value1 = 0.515183975112835;
//            double value2 = 0.381759728225259;

//            AddAllPSMs();
//            AddAllQuantScans();
            
//            QuantPeptide1.GetIndividualRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Average).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetIndividualRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Average).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void RatioListMedian()
//        {
//            double value1 = 0.5;
//            double value2 = 0.5;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetIndividualRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Median).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetIndividualRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Median).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void RatioListVariation()
//        {
//            double value1 = 1.16875065042592;
//            double value2 = 1.92927347721859;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetRatioVariation(IQuantitationChannelTMT2, IQuantitationChannelTMT1).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetRatioVariation(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void RatioListAverageNBC()
//        {
//            double value1 = 0.515183975112835;
//            double value2 = 0.65572270594158;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetIndividualRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Average, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);

//            QuantPeptide2.GetIndividualRatio(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, IntensityWeightingType.Average, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }

//        [Test]
//        public void RatioListMedianNBC()
//        {
//            double value1 = 0.5;
//            double value2 = 0.5;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetIndividualRatio(IQuantitationChannelTMT2, IQuantitationChannelTMT1, IntensityWeightingType.Median, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);
//        }

//        [Test]
//        public void RatioListVariationNBC()
//        {
//            double value1 = 1.16875065042592;
//            double value2 = 4.963445194379;

//            AddAllPSMs();
//            AddAllQuantScans();

//            QuantPeptide1.GetRatioVariation(IQuantitationChannelTMT2, IQuantitationChannelTMT1, true).Should().Be.InRange(value1 - tolerance, value1 + tolerance);
//            QuantPeptide2.GetRatioVariation(IQuantitationChannelNeuCode2, IQuantitationChannelNeuCode1, true).Should().Be.InRange(value2 - tolerance, value2 + tolerance);
//        }
//    }
//}
