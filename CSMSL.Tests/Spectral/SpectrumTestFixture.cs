using System;
using CSMSL.Spectral;
using NUnit.Framework;

namespace CSMSL.Tests.Spectral
{
    [TestFixture, Category("Spectral")]
    public sealed class SpectrumTestFixture
    {
        private Spectrum SpectrumA;

        [SetUp]
        public void Setup()
        {
            double[] mz = { 328.73795, 329.23935, 447.73849, 448.23987, 482.23792, 482.57089, 482.90393, 500.95358, 501.28732, 501.62131, 611.99377, 612.32806, 612.66187, 722.85217, 723.35345 };
            double[] intensities = { 81007096.0, 28604418.0, 78353512.0, 39291696.0, 122781408.0, 94147520.0, 44238040.0, 71198680.0, 54184096.0, 21975364.0, 44514172.0, 43061628.0, 23599424.0, 56022696.0, 41019144.0 };

            SpectrumA = new Spectrum(mz, intensities);
        }
        
        [Test]
        public void EmptySpectrumCountIsZero()
        {
            Assert.AreEqual(0, Spectrum.Empty.Count);
        }

        #region Properties

        [Test]
        public void SpectrumCount()
        {
            Assert.AreEqual(15, SpectrumA.Count);
        }

        [Test]
        public void SpectrumFirstMZ()
        {
            Assert.AreEqual(328.73795, SpectrumA.FirstMz);
        }

        [Test]
        public void SpectrumLastMZ()
        {
            Assert.AreEqual(723.35345, SpectrumA.LastMZ);
        }

        #endregion

        [Test]
        public void SpectrumBasePeakIntensity()
        {
            double basePeakIntensity = SpectrumA.GetBasePeakIntensity();

            Assert.AreEqual(122781408.0, basePeakIntensity);
        }

        [Test]
        public void SpectrumTIC()
        {
            double tic = SpectrumA.GetTotalIonCurrent();

            Assert.AreEqual(843998894.0, tic);
        }

        [Test]
        public void SpectrumGetIntensityFirst()
        {
            double intensity = SpectrumA.GetIntensity(0);

            Assert.AreEqual(81007096.0, intensity);
        }

        [Test]
        public void SpectrumGetIntensityRandom()
        {
            double intensity = SpectrumA.GetIntensity(6);

            Assert.AreEqual(44238040.0, intensity);
        }

        [Test]
        public void SpectrumGetMassFirst()
        {
            double intensity = SpectrumA.GetMass(0);
          
            Assert.AreEqual(328.73795, intensity);
        }

        [Test]
        public void SpectrumGetMassRandom()
        {
            double intensity = SpectrumA.GetMass(6);

            Assert.AreEqual(482.90393, intensity);
        }

        #region Contains Peak

        [Test]
        public void SpectrumContainsPeak()
        {
            Assert.IsTrue(SpectrumA.ContainsPeak());
        }

        [Test]
        public void SpectrumContainsPeakInRange()
        {
            Assert.IsTrue(SpectrumA.ContainsPeak(448.23987 - 0.001, 448.23987 + 0.001));
        }

        [Test]
        public void SpectrumContainsPeakInRangeEnd()
        {
            Assert.IsTrue(SpectrumA.ContainsPeak(448.23987 - 0.001, 448.23987));
        }

        [Test]
        public void SpectrumContainsPeakInRangeStart()
        {
            Assert.IsTrue(SpectrumA.ContainsPeak(448.23987, 448.23987 + 0.001));
        }

        [Test]
        public void SpectrumContainsPeakInRangeStartEnd()
        {
            Assert.IsTrue(SpectrumA.ContainsPeak(448.23987, 448.23987));
        }

        [Test]
        public void SpectrumContainsPeakInRangeBackwards()
        {
            Assert.IsFalse(SpectrumA.ContainsPeak(448.23987 + 0.001, 448.23987 - 0.001));
        }
        
        [Test]
        public void SpectrumDoesntContainPeakInRange()
        {
            Assert.IsFalse(SpectrumA.ContainsPeak(603.4243 - 0.001, 603.4243 + 0.001));
        }

        #endregion

        [Test]
        public void SpectrumMassRange()
        {
            MzRange range = new MzRange(328.73795, 723.35345);

            Assert.AreEqual(range, SpectrumA.GetMzRange());
        }

        [Test]
        public void SpectrumFilterEmpty()
        {
            Spectrum filteredSpectrum = SpectrumA.Filter(0, 50);
            
            Assert.AreSame(Spectrum.Empty, filteredSpectrum);
        }

        [Test]
        public void SpectrumFilterCount()
        {
            Spectrum filteredSpectrum = SpectrumA.Filter(28604417, 28604419);

            Assert.AreEqual(1, filteredSpectrum.Count);
        }
    }
}
