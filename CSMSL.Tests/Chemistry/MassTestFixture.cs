using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should.Fluent;
using CSMSL.Chemistry;

namespace CSMSL.Tests.Chemistry
{
    [TestFixture]
    [Category("Mass")]
    public sealed class MassTexsFixture
    {
        private double massTolerance = 0.000000000001;

        [Test]
        public void MassAndMZConversion()
        {
            double mz = 524.325;
            int z = 2;
            double mass = 1046.635447066376;

            // positive cases
            Mass.MassFromMz(mz, z).Should().Be.InRange(mass - massTolerance, mass + massTolerance);
            Mass.MzFromMass(mass, z).Should().Be.InRange(mz - massTolerance, mz + massTolerance);

            // negative cases
            z = -3;
            mass = 1575.996829400436;
            Mass.MassFromMz(mz, z).Should().Be.InRange(mass - massTolerance, mass + massTolerance);
            Mass.MzFromMass(mass, z).Should().Be.InRange(mz - massTolerance, mz + massTolerance);
            
            // zero-cases
            Mass.MzFromMass(0, 3).Should().Equal(0.0);
            Mass.MzFromMass(mz, 0).Should().Equal(0.0);

            Mass.MassFromMz(0, 3).Should().Equal(0.0);
            Mass.MassFromMz(mass, 0).Should().Equal(0.0);
        }

    }
}
