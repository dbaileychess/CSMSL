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
        public void MassToleranceImplicitValue()
        {
            var tol = new Tolerance("10 ppm");

            Assert.AreEqual(10, tol.Value);
        }

        [Test]
        public void MassToleranceImplicitType()
        {
            var tol = new Tolerance("10 ppm");

            Assert.AreEqual(ToleranceType.PPM, tol.Type);
        }

        [Test]
        public void MassToleranceFromDaType()
        {
            var tol = Tolerance.FromDA(10);

            Assert.AreEqual(ToleranceType.DA, tol.Type);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus()
        {
            var tol = new Tolerance("+-10 ppm");

            Assert.IsTrue(tol.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus2()
        {
            var tol = new Tolerance("-+10 ppm");

            Assert.IsTrue(tol.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus3()
        {
            var tol = new Tolerance("±10 ppm");  // alt-code 241

            Assert.IsTrue(tol.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus4()
        {
            var tol = new Tolerance("± 10 ppm");  // alt-code 241

            Assert.IsTrue(tol.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus5()
        {
            var tol = new Tolerance("10 ppm");

            Assert.IsFalse(tol.PlusAndMinus);
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
