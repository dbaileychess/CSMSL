using System.Runtime.InteropServices;
using CSMSL.Spectral;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CSMSL.Tests.Spectral
{
    [TestFixture, Category("Spectral"), Guid("FEB664ED-51B1-4DD9-A224-E81E0E120AFE")]
    public sealed class SpectrumTestFixture
    {
        private MZSpectrum mzSpectrum1000;
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
            mzSpectrum1000 = new MZSpectrum(peaks);
        }

        [Test]
        public void SpectrumPeakCount()
        {
            Assert.AreEqual(1000, mzSpectrum1000.Count);
        }

        [Test]
        public void SpectrumTIC()
        {
            Assert.AreEqual(483469778.88814604, mzSpectrum1000.TotalIonCurrent);
        }

        [Test]
        public void SpectrumTryGetPeaks()
        {
            List<MZPeak> peaks;
            mzSpectrum1000.TryGetPeaks(500, 501, out peaks);

            Assert.AreEqual(2, peaks.Count);
        }
        
        [Test]
        public void SpectrumTryGetPeaksIsTrue()
        {
            List<MZPeak> peaks;
            Assert.IsTrue(mzSpectrum1000.TryGetPeaks(500, 501, out peaks));
        }

        [Test]
        public void SpectrumTryGetPeaksIsFalse()
        {
            List<MZPeak> peaks;
            Assert.IsFalse(mzSpectrum1000.TryGetPeaks(2000, 2001, out peaks));
        }
    }
}
