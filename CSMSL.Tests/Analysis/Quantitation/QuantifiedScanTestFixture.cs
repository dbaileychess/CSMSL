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
//using CSMSL.Chemistry;
//using CSMSL.Proteomics;

//namespace CSMSL.Tests.Analysis.Quantitation
//{
//    [TestFixture]
//    [Category("Quantitation")]
    
//    public sealed class QuantifiedScanTestFixture
//    {
//        private QuantifiedScan QuantScan1;
//        private MSDataScan dataScan;
//        private QuantifiedPeak QuantPeak1;
//        private QuantifiedPeak QuantPeak2;
//        private QuantifiedPeak QuantPeak3;
//        private QuantifiedPeak QuantPeak4;
//        private QuantifiedPeak QuantPeak5;
//        private QuantifiedPeak QuantPeak6;
//        private IQuantitationChannel IQuantitationChannel1;
//        private IQuantitationChannel IQuantitationChannel2;
//        private IQuantitationChannel IQuantitationChannel3;
//        private IQuantitationChannel IQuantitationChannel4;
//        private IQuantitationChannel IQuantitationChannel5;
//        private IQuantitationChannel IQuantitationChannel6;

//        [SetUp]
//        public void SetUp()
//        {
//            var mock = new Mock<MSDataScan>();                  
//            mock.Setup(scan => scan.SpectrumNumber).Returns(1203);
//            mock.Setup(scan => scan.InjectionTime).Returns(12.3);       
//            dataScan = mock.Object;

//            QuantScan1 = new QuantifiedScan(dataScan, 2);

//            QuantPeak1 = new QuantifiedPeak(QuantScan1.DataScan, 126.0, 1, 10.0, 1.0);
//            QuantPeak2 = new QuantifiedPeak(QuantScan1.DataScan, 127.0, 1, 5.0, 1.0);
//            QuantPeak3 = new QuantifiedPeak(QuantScan1.DataScan, 128.0, 1, 10.0, 1.0);
//            QuantPeak4 = new QuantifiedPeak(QuantScan1.DataScan, 129.0, 1, 5.0, 1.0);
//            QuantPeak5 = new QuantifiedPeak(QuantScan1.DataScan, 130.0, 1, 0.0, 1.0);
//            QuantPeak6 = new QuantifiedPeak(QuantScan1.DataScan, 131.0, 1, 5.0, 1.0);

//            IQuantitationChannel1 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-126");
//            IQuantitationChannel2 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-127");
//            IQuantitationChannel3 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-128");
//            IQuantitationChannel4 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-129");
//            IQuantitationChannel5 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-130");
//            IQuantitationChannel6 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-131");      
//        }

//        [Test]
//        public void ScanNumberEquals()
//        {
//            dataScan.SpectrumNumber.Should().Equal(1203);
//        }

//        [Test]
//        public void InjectionTimeEquals()
//        {
//            dataScan.InjectionTime.Should().Equal(12.3);
//        }

//        [Test]
//        public void NumIsotopesEquals()
//        {
//            QuantScan1.QuantifiedPeaks.Length.Should().Equal(3);
//        }

//        [Test]
//        public void NumIQuantitationChannelsEquals()
//        {
//            QuantScan1.AddQuant(IQuantitationChannel1, QuantPeak1);
//            QuantScan1.AddQuant(IQuantitationChannel2, QuantPeak2);
//            QuantScan1.AddQuant(IQuantitationChannel3, QuantPeak3);
//            QuantScan1.AddQuant(IQuantitationChannel4, QuantPeak4);
//            QuantScan1.AddQuant(IQuantitationChannel5, QuantPeak5);
//            QuantScan1.AddQuant(IQuantitationChannel6, QuantPeak6);
//            QuantScan1.IQuantitationChannelCount.Should().Equal(6);
//        }

//        [Test]
//        public void PeakSignalToNoiseEquals()
//        {
//            QuantScan1.AddQuant(IQuantitationChannel2, QuantPeak2);
//            QuantifiedPeak peak;
//            if (QuantScan1.TryGetQuantifiedPeak(IQuantitationChannel2, out peak, 0))
//            {
//                peak.SignalToNoise.Should().Equal(5.0);
//            }
//            else
//            {
//                Assert.Fail();
//            }
//        }

//        [Test]
//        public void PeakMzEquals()
//        {
//            QuantScan1.AddQuant(IQuantitationChannel3, QuantPeak3);
//            QuantifiedPeak peak;
//            if (QuantScan1.TryGetQuantifiedPeak(IQuantitationChannel3, out peak, 0))
//            {
//                peak.MZ.Should().Equal(128.0);
//            }
//            else
//            {
//                Assert.Fail();
//            }
//        }

//        [Test]
//        [ExpectedException(typeof(IndexOutOfRangeException))]
//        public void AddQuantInvalidIsotope()
//        {
//            QuantScan1.AddQuant(IQuantitationChannel1, QuantPeak2, -1);
//        }

//        [Test]
//        public void AddQuantNullPeakEquals()
//        {
//            QuantPeak3 = null;
//            QuantScan1.AddQuant(IQuantitationChannel3, QuantPeak3, 1);
//            QuantifiedPeak peak;
//            if (QuantScan1.TryGetQuantifiedPeak(IQuantitationChannel3, out peak, 1))
//            {
//                peak.MZ.Should().Equal(0.0);
//            }
//            else
//            {
//                Assert.Fail();
//            }
//        }

//        [Test]
//        [ExpectedException(typeof(NullReferenceException))]
//        public void AddQuantNullIQuantitationChannel()
//        {
//            IQuantitationChannel1 = null;
//            QuantScan1.AddQuant(IQuantitationChannel1, QuantPeak1);           
//        }

//        [Test]
//        [ExpectedException(typeof(DuplicateKeyException))]
//        public void AddQuantDuplicateIQuantitationChannel()
//        {
//            QuantScan1.AddQuant(IQuantitationChannel2, QuantPeak1);
//            QuantScan1.AddQuant(IQuantitationChannel2, QuantPeak2);
//        }

//    }
//}
