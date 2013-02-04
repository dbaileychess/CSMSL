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
        [Test]
        public void BasicChemicalFormulaMass()
        {
            ChemicalFormula water = new ChemicalFormula("H2O");
            water.Mass.Monoisotopic.Should().Equal(2 * Constants.HYDROGEN + Constants.OXYGEN);
        }

    }
}
