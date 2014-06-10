// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (ChemicalFormulaGeneratorTestFixture.cs) is part of CSMSL.Tests.
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

using CSMSL.Chemistry;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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