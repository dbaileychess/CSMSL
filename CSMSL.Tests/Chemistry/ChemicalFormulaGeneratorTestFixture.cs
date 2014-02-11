using System;
using System.Linq;
using System.Collections.Generic;
using CSMSL.Chemistry;
using NUnit.Framework;

namespace CSMSL.Tests.Chemistry
{
    [TestFixture]
    [Category("Chemical Formula")]
    public class ChemicalFormulaGeneratorTestFixture
    {

        [Test]
        public void ChemicalFormulaGeneratorContainsFormula()
        {
            ChemicalFormulaGenerator generator = new ChemicalFormulaGenerator(new ChemicalFormula("H2O"));

            List<ChemicalFormula> formulas = generator.AllFormulas().ToList();
            ChemicalFormula ho = new ChemicalFormula("HO");

            Assert.Contains(ho, formulas);
        }
    }
}
