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
    [Category("Chemical Formula")]
    public class ChemicalFormulaTestFixture
    {
        public static readonly PeriodicTable PERIODIC_TABLE = PeriodicTable.Instance;
        private ChemicalFormula CAM = new ChemicalFormula("C2H3NO");
        private Element NullElement = null;
        private Isotope NullIsotope = null;

        [Test]
        public void BasicChemicalFormulaMass()
        {
            ChemicalFormula water = new ChemicalFormula("H2O");
            water.Mass.Monoisotopic.Should().Equal(2 * Constants.HYDROGEN + Constants.OXYGEN);
        }

        [Test]
        public void ContainsIsotope()
        {
            CAM.Contains(PERIODIC_TABLE["C"].PrincipalIsotope).Should().Be.True();
            CAM.Contains(PERIODIC_TABLE["Fe"].PrincipalIsotope).Should().Be.False();
            CAM.Contains(PERIODIC_TABLE["O"]).Should().Be.True();
            CAM.Contains(NullIsotope).Should().Be.False();
            CAM.Contains(NullElement).Should().Be.False();
            CAM.Contains("N").Should().Be.True();
            CAM.Contains("H", 1).Should().Be.True();
            CAM.Contains("N", 15).Should().Be.False();
        }

        [Test]
        public void CountIsotope()
        {
            CAM.Count(PERIODIC_TABLE["C"].PrincipalIsotope).Should().Equal(2);
            CAM.Count(PERIODIC_TABLE["Fe"].PrincipalIsotope).Should().Equal(0);
            CAM.Count(PERIODIC_TABLE["O"]).Should().Equal(1);
            CAM.Count(NullElement).Should().Equal(0);
            CAM.Count(NullIsotope).Should().Equal(0);
            CAM.Count("N").Should().Equal(1);
            CAM.Count("H", 1).Should().Equal(3);
            CAM.Count("N", 15).Should().Equal(0);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void InvalidElement()
        {
            CAM.Count("Faa").Should().Equal(0);
        }

    }
}
