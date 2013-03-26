using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Should.Fluent;
using Moq;
using CSMSL.Analysis.Quantitation;
using CSMSL.Spectral;
using CSMSL.Proteomics;

namespace CSMSL.Tests.Analysis.Quantitation
{
    [TestFixture]
    [Category("Quantitation")]

    public sealed class QuantifiedPeakTestFixture
    {
        private QuantifiedScan QuantScan1;
        private Peptide Peptide1;
        private QuantifiedPeptide QuantPeptide1;
        private MSDataScan dataScan;
        private QuantifiedPeak QuantPeak1;
        private QuantifiedPeak QuantPeak2;
        private QuantifiedPeak QuantPeak3;
        private QuantifiedPeak QuantPeak4;
        private QuantifiedPeak QuantPeak5;
        private QuantifiedPeak QuantPeak6;

        [SetUp]
        public void SetUp()
        {
            var mock = new Mock<MSDataScan>();
            mock.Setup(scan => scan.SpectrumNumber).Returns(1203);
            mock.Setup(scan => scan.InjectionTime).Returns(12.3);
            dataScan = mock.Object;

            Peptide1 = new Peptide("TTGSSSSSSSK");
            QuantPeptide1 = new QuantifiedPeptide(Peptide1);
            QuantScan1 = new QuantifiedScan(dataScan);

            QuantPeak1 = new QuantifiedPeak(126.0, 1, 10.0, 1.0);
            QuantPeak2 = new QuantifiedPeak(127.0, 1, 5.0, 1.0);
            QuantPeak3 = new QuantifiedPeak(128.0, 1, 10.0, 1.0);
            QuantPeak4 = new QuantifiedPeak(129.0, 1, 5.0, 1.0);
            QuantPeak5 = new QuantifiedPeak(130.0, 1, 0.0, 1.0);
            QuantPeak6 = new QuantifiedPeak(131.0, 1, 5.0, 1.0);
        }

        [Test]
        public void SignalToNoiseEquals()
        {
            QuantPeak1.SignalToNoise.Should().Equal(10.0);
        }

        [Test]
        public void InjectionTimeForNullParentScanEquals()
        {
            QuantPeak1.InjectionTime.Should().Equal(1.0);
        }

        [Test]
        public void InjectionTimeEquals()
        {
            QuantScan1.AddQuant(new Channel("TMT-126"), QuantPeak1);
            QuantPeak1.InjectionTime.Should().Equal(12.3);
        }

        [Test]
        public void DenormalizedIntensityEquals()
        {
            QuantScan1.AddQuant(new Channel("TMT-127"), QuantPeak2);
            QuantPeak2.DenormalizedIntensity().Should().Equal(61.5);
        }

        [Test]
        public void DenormalizedIntensityNoiseBandCappedTrueEquals()
        {
            QuantScan1.AddQuant(new Channel("TMT-130"), QuantPeak5);
            QuantPeak5.DenormalizedIntensity(true).Should().Equal(12.3);
        }

        [Test]
        public void DenormalizedIntensityNoiseBandCappedFalseEquals()
        {
            QuantScan1.AddQuant(new Channel("TMT-130"), QuantPeak5);
            QuantPeak5.DenormalizedIntensity().Should().Equal(0.0);
        }
    }
}
