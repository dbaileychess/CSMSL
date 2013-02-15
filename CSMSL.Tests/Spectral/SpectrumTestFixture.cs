using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Should.Fluent;
using CSMSL.Spectral;

namespace CSMSL.Tests.Spectral
{
    [TestFixture]
    [Category("Spectral")]
    public sealed class SpectrumTestFixture
    {
        private Spectrum<MZPeak> mzSpectrum1000;
        private Random random;

        [SetUp]
        public void SetUp()
        {
            random = new Random(158243251);
            List<MZPeak> peaks = new List<MZPeak>();
            for (int i = 0; i < 1000; i++)
            {
                double mz = random.NextDouble() * 2000;
                double intensity = random.NextDouble() * 1e6;
                peaks.Add(new MZPeak(mz, intensity));
            }
            peaks.Sort();
            mzSpectrum1000 = new Spectrum<MZPeak>(peaks);
        }

        [Test]
        public void SpectrumPeakCount()
        {
            mzSpectrum1000.Count.Should().Equal(1000);
        }

        [Test]
        public void SpectrumTIC()
        {
            mzSpectrum1000.TIC.Should().Equal(483469778.88814604);
        }

        [Test]
        public void SpectrumClearPeaks()
        {
            mzSpectrum1000.Clear();

            mzSpectrum1000.Count.Should().Equal(0);
        }

        [Test]
        public void SpectrumTryGetPeaks()
        {
            List<MZPeak> peaks;
            mzSpectrum1000.TryGetPeaks(500, 501, out peaks);

            peaks.Count.Should().Equal(2);
        }
        
        [Test]
        public void SpectrumTryGetPeaksIsTrue()
        {
            List<MZPeak> peaks;
            mzSpectrum1000.TryGetPeaks(500, 501, out peaks).Should().Be.True();            
        }

        [Test]
        public void SpectrumTryGetPeaksIsFalse()
        {
            List<MZPeak> peaks;
            mzSpectrum1000.TryGetPeaks(2000, 2001, out peaks).Should().Be.False();
        }
    }
}
