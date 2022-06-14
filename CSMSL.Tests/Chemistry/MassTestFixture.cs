// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CSMSL.Chemistry;
using NUnit.Framework;
using System;

namespace CSMSL.Tests.Chemistry
{
    [TestFixture, Category("Mass")]
    public class MassTestFixture
    {
        [Test]
        public void DefaultMassMonoisotopic()
        {
            Mass m = new Mass();

            Assert.AreEqual(0.0, m.MonoisotopicMass);
        }

        [Test]
        public void MonoisotopicOnlyMassInConstructor()
        {
            Mass m = new Mass(524.342);

            Assert.AreEqual(524.342, m.MonoisotopicMass);
        }

        [Test]
        public void MonoisotopicMassInConstructor()
        {
            Mass m = new Mass(524.342);

            Assert.AreEqual(524.342, m.MonoisotopicMass);
        }

        [Test]
        public void MassEquality()
        {
            Mass m1 = new Mass(524.342);
            Mass m2 = new Mass(524.342);

            Assert.AreEqual(m1, m2);
        }

        [Test]
        public void MassRefInequality()
        {
            Mass m1 = new Mass(524.342);
            Mass m2 = new Mass(524.342);

            Assert.AreNotSame(m1, m2);
        }

        [Test]
        public void MassMonoisotopicInequality()
        {
            Mass m1 = new Mass(524.342);
            Mass m2 = new Mass(524.343);

            Assert.AreNotEqual(m1, m2);
        }

        [Test]
        public void MassBothInequality()
        {
            Mass m1 = new Mass(524.342);
            Mass m2 = new Mass(524.343);

            Assert.AreNotEqual(m1, m2);
        }

        [Test]
        public void ConstructorIMass()
        {
            IMass m = new Mass(524.342);
            Mass m2 = new Mass(m);

            Assert.AreEqual(m, m2);
        }

        [Test]
        public void ConstructorIMassRefInequality()
        {
            IMass m = new Mass(524.342);
            Mass m2 = new Mass(m);

            Assert.AreNotSame(m, m2);
        }

        [Test]
        public void MassToMzPositiveCharge()
        {
            double mz = Mass.MzFromMass(1000, 2);
            Assert.AreEqual(501.00727646681202, mz);
        }

        [Test]
        public void MassToMzNegativeCharge()
        {
            double mz = Mass.MzFromMass(1000, -2);
            Assert.AreEqual(498.99272353318798, mz);
        }

        [Test]
        public void MassToMzZeroCharge()
        {
            var ex = Assert.Throws<DivideByZeroException>(() => Mass.MzFromMass(1000, 0));
            Assert.That(ex.Message, Is.EqualTo("Charge cannot be zero"));
        }

        [Test]
        public void MzToMassPostitiveCharge()
        {
            double mass = Mass.MassFromMz(524.3, 2);
            Assert.AreEqual(1046.585447066376, mass);
        }

        [Test]
        public void MzToMassNegativeCharge()
        {
            double mass = Mass.MassFromMz(524.3, -2);
            Assert.AreEqual(1050.6145529336238, mass);
        }

        [Test]
        public void MzTomassZeroCharge()
        {
            var ex = Assert.Throws<DivideByZeroException>(() => Mass.MassFromMz(524.3, 0));
            Assert.That(ex.Message, Is.EqualTo("Charge cannot be zero"));
        }

        [Test]
        public void MassIsIMass()
        {
            Mass m1 = new Mass(524.342);

            Assert.IsInstanceOf<IMass>(m1);
        }

        [Test]
        public void MassObjectToMzPositiveCharge()
        {
            Mass m1 = new Mass(1000);
            double mz = m1.ToMz(2);

            Assert.AreEqual(501.00727646681202, mz);
        }

        [Test]
        public void MassObjectToMzNegativeCharge()
        {
            Mass m1 = new Mass(1000);
            double mz = m1.ToMz(-2);

            Assert.AreEqual(498.99272353318798, mz);
        }

        [Test]
        public void MassObjectToMzZeroCharge()
        {
            Mass m1 = new Mass(1000);
            var ex = Assert.Throws<DivideByZeroException>(() => m1.ToMz(0));
            Assert.AreEqual(ex.Message, "Charge cannot be zero");
        }
    }
}