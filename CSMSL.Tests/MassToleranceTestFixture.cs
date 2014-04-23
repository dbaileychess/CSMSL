using NUnit.Framework;

namespace CSMSL.Tests
{
    [TestFixture, Category("Mass Tolerance")]
    public sealed class MassToleranceTestFixture
    {
        [Test]
        public void MassToleranceConstructorDaValue()
        {
            var tol = new Tolerance(ToleranceType.DA, 10);

            Assert.AreEqual(10, tol.Value);
        }

        [Test]
        public void MassToleranceConstructorDaType()
        {
            var tol = new Tolerance(ToleranceType.DA, 10);

            Assert.AreEqual(ToleranceType.DA, tol.Type);
        }

        [Test]
        public void MassToleranceFromDaValue()
        {
            var tol = Tolerance.FromDA(10);

            Assert.AreEqual(10, tol.Value);
        }

        [Test]
        public void MassToleranceFromDaType()
        {
            var tol = Tolerance.FromDA(10);

            Assert.AreEqual(ToleranceType.DA, tol.Type);
        }

        [Test]
        public void GetToleranceDaPositive()
        {
            double value = Tolerance.GetTolerance(10, 5, ToleranceType.DA);

            Assert.AreEqual(5, value);
        }

        [Test]
        public void GetToleranceDaNegative()
        {
            double value = Tolerance.GetTolerance(5, 10, ToleranceType.DA);

            Assert.AreEqual(-5, value);
        }

        [Test]
        public void GetToleranceDaZero()
        {
            double value = Tolerance.GetTolerance(10, 10, ToleranceType.DA);

            Assert.AreEqual(0, value);
        }

        [Test]
        public void GetTolerancePPMPositive()
        {
            double value = Tolerance.GetTolerance(500.001, 500.0, ToleranceType.PPM);

            Assert.AreEqual(1.9999999999527063, value);
        }
        
    }
}
