// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (MassToleranceTestFixture.cs) is part of CSMSL.Tests.
//
// CSMSL.Tests is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CSMSL.Tests is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL.Tests. If not, see <http://www.gnu.org/licenses/>.

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
            var tol = new Tolerance("±10 ppm"); // alt-code 241

            Assert.AreEqual(tol.Type, ToleranceType.PlusAndMinus);
        }

        [Test]
        public void MassToleranceImplicitPlusMinus4()
        {
            var tol = new Tolerance("± 10 ppm"); // alt-code 241

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

        [Test]
        public void ToleranceWithin1()
        {
            var tol = Tolerance.FromPPM(10);

            Assert.IsTrue(tol.Within(500, 500.005));
        }

        [Test]
        public void ToleranceWithin2()
        {
            var tol = Tolerance.FromPPM(10, ToleranceType.FullWidth);

            Assert.IsFalse(tol.Within(500, 500.005));
        }
    }
}