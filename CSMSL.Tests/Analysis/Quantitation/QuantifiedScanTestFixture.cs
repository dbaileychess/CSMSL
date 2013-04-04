using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
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
    
    public sealed class QuantifiedScanTestFixture
    {
        private QuantifiedScan QuantScan1;
        private MSDataScan dataScan;
        private QuantifiedPeak QuantPeak1;
        private QuantifiedPeak QuantPeak2;
        private QuantifiedPeak QuantPeak3;
        private QuantifiedPeak QuantPeak4;
        private QuantifiedPeak QuantPeak5;
        private QuantifiedPeak QuantPeak6;
        private Channel Channel1;
        private Channel Channel2;
        private Channel Channel3;
        private Channel Channel4;
        private Channel Channel5;
        private Channel Channel6;

        [SetUp]
        public void SetUp()
        {
            var mock = new Mock<MSDataScan>();                  
            mock.Setup(scan => scan.SpectrumNumber).Returns(1203);
            mock.Setup(scan => scan.InjectionTime).Returns(12.3);       
            dataScan = mock.Object;

            QuantScan1 = new QuantifiedScan(dataScan, 2);

            QuantPeak1 = new QuantifiedPeak(126.0, 1, 10.0, 1.0);
            QuantPeak2 = new QuantifiedPeak(127.0, 1, 5.0, 1.0);
            QuantPeak3 = new QuantifiedPeak(128.0, 1, 10.0, 1.0);
            QuantPeak4 = new QuantifiedPeak(129.0, 1, 5.0, 1.0);
            QuantPeak5 = new QuantifiedPeak(130.0, 1, 0.0, 1.0);
            QuantPeak6 = new QuantifiedPeak(131.0, 1, 5.0, 1.0);

            Channel1 = new Channel("TMT-126");
            Channel2 = new Channel("TMT-127");
            Channel3 = new Channel("TMT-128");
            Channel4 = new Channel("TMT-129");
            Channel5 = new Channel("TMT-130");
            Channel6 = new Channel("TMT-131");      
        }

        [Test]
        public void ScanNumberEquals()
        {
            dataScan.SpectrumNumber.Should().Equal(1203);
        }

        [Test]
        public void InjectionTimeEquals()
        {
            dataScan.InjectionTime.Should().Equal(12.3);
        }

        [Test]
        public void NumIsotopesEquals()
        {
            QuantScan1.QuantifiedPeaks.Length.Should().Equal(3);
        }

        [Test]
        public void NumChannelsEquals()
        {
            QuantScan1.AddQuant(Channel1, QuantPeak1);
            QuantScan1.AddQuant(Channel2, QuantPeak2);
            QuantScan1.AddQuant(Channel3, QuantPeak3);
            QuantScan1.AddQuant(Channel4, QuantPeak4);
            QuantScan1.AddQuant(Channel5, QuantPeak5);
            QuantScan1.AddQuant(Channel6, QuantPeak6);
            QuantScan1.ChannelCount.Should().Equal(6);
        }

        [Test]
        public void PeakSignalToNoiseEquals()
        {
            QuantScan1.AddQuant(Channel2, QuantPeak2);
            QuantifiedPeak peak;
            if (QuantScan1.TryGetQuantifiedPeak(Channel2, out peak, 0))
            {
                peak.SignalToNoise.Should().Equal(5.0);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void PeakMzEquals()
        {
            QuantScan1.AddQuant(Channel3, QuantPeak3);
            QuantifiedPeak peak;
            if (QuantScan1.TryGetQuantifiedPeak(Channel3, out peak, 0))
            {
                peak.Mz.Should().Equal(128.0);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AddQuantInvalidIsotope()
        {
            QuantScan1.AddQuant(Channel1, QuantPeak2, -1);
        }

        [Test]
        public void AddQuantNullPeakEquals()
        {
            QuantPeak3 = null;
            QuantScan1.AddQuant(Channel3, QuantPeak3, 1);
            QuantifiedPeak peak;
            if (QuantScan1.TryGetQuantifiedPeak(Channel3, out peak, 1))
            {
                peak.Mz.Should().Equal(0.0);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddQuantNullChannel()
        {
            Channel1 = null;
            QuantScan1.AddQuant(Channel1, QuantPeak1);           
        }

        [Test]
        [ExpectedException(typeof(DuplicateKeyException))]
        public void AddQuantDuplicateChannel()
        {
            QuantScan1.AddQuant(Channel2, QuantPeak1);
            QuantScan1.AddQuant(Channel2, QuantPeak2);
        }

    }
}
