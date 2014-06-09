using NUnit.Framework;

namespace CSMSL.Tests
{
    [TestFixture, Category("Mass Tolerance")]
    public sealed class MassToleranceTestFixture
    {
        [Test]
        public void MassToleranceConstructorDaValue()
        {
            var tol = new Tolerance(ToleranceUnit.DA, 10);

            Assert.AreEqual(10, tol.Value);
        }

        [Test]
        public void MassToleranceConstructorDaType()
        {
            var tol = new Tolerance(ToleranceUnit.DA, 10);

            Assert.AreEqual(ToleranceUnit.DA, tol.Unit);
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

            Assert.AreEqual(ToleranceUnit.PPM, tol.Unit);
        }

        [Test]
        public void MassToleranceFromDaType()
        {
            var tol = Tolerance.FromDA(10);

            Assert.AreEqual(ToleranceUnit.DA, tol.Unit);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus()
        {
            var tol = new Tolerance("+-10 ppm");

            Assert.AreEqual(tol.Type, ToleranceType.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus2()
        {
            var tol = new Tolerance("-+10 ppm");

            Assert.AreEqual(tol.Type, ToleranceType.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus3()
        {
            var tol = new Tolerance("±10 ppm");  // alt-code 241

            Assert.AreEqual(tol.Type , ToleranceType.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus4()
        {
            var tol = new Tolerance("± 10 ppm");  // alt-code 241

            Assert.AreEqual(tol.Type, ToleranceType.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus5()
        {
            var tol = new Tolerance("10 ppm");
      
            Assert.AreEqual(tol.Type, ToleranceType.FullWidth);
        }

        [Test]
        public void GetToleranceDaPositive()
        {
            double value = Tolerance.GetTolerance(10, 5, ToleranceUnit.DA);
      
            Assert.AreEqual(5, value);
        }

        [Test]
        public void GetToleranceDaNegative()
        {
            double value = Tolerance.GetTolerance(5, 10, ToleranceUnit.DA);

            Assert.AreEqual(-5, value);
        }

        [Test]
        public void GetToleranceDaZero()
        {
            double value = Tolerance.GetTolerance(10, 10, ToleranceUnit.DA);

            Assert.AreEqual(0, value);
        }

        [Test]
        public void GetTolerancePPMPositive()
        {
            double value = Tolerance.GetTolerance(500.001, 500.0, ToleranceUnit.PPM);

            Assert.AreEqual(1.9999999999527063, value);
        }
        
    }
}
