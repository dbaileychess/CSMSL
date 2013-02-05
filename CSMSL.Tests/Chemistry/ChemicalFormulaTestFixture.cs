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
        private ChemicalFormula CAM = new ChemicalFormula("C2H3NO");      
        private ChemicalFormula NullChemicalFormula = null;
        private ChemicalFormula EmptyFormula = new ChemicalFormula();
        private Element NullElement = null;
        private Isotope NullIsotope = null;
            
        [Test]
        public void BasicChemicalFormulaMass()
        {
            ChemicalFormula water = new ChemicalFormula("H2O");
            water.Mass.Monoisotopic.Should().Equal(2 * Constants.HYDROGEN + Constants.OXYGEN);
        }

        [Test]
        public void ConstructorCopy()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula(formulaA);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void ImplicitConstructor()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = "C2H3NO";

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void ConstructorCopyReferencesAreDifferent()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula(formulaA);

            formulaA.Should().Not.Be.SameAs(formulaB);
        }

        [Test]
        public void AddIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H4NO");

            formulaA.Add(Element.PeriodicTable["H"][1], 1);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddFormulaToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("H2O");
            ChemicalFormula formulaC = new ChemicalFormula("C2H5NO2");

            formulaA.Add(formulaB);
    
            formulaA.Should().Equal(formulaC);
        }
        
        [Test]      
        public void FormulaEqualsItself()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.Should().Equal(formulaA);    
        }

        [Test]
        public void FormulaEqualsSameFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("NC2OH3");

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void FormulaDoesNotEqualOtherFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("NC1OH3");

            formulaA.Should().Not.Equal(formulaB);        
        }

        [Test]
        public void FormulaDoesNotEqualNullFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = null;

            formulaA.Should().Not.Equal(formulaB);
        }

        [Test]
        public void NullFormulaDoesNotEqualFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = null;

            formulaB.Should().Not.Equal(formulaA);
        }

        [Test]
        public void FormulaDoesNotEqualSameFormulaWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("CC{13}H3NO");

            formulaB.Should().Not.Equal(formulaA);
        }

        [Test]
        public void ContainsIsotope()
        {
            CAM.Contains(Element.PeriodicTable["C"].PrincipalIsotope).Should().Be.True();
            CAM.Contains(Element.PeriodicTable["Fe"].PrincipalIsotope).Should().Be.False();
            CAM.Contains(Element.PeriodicTable["O"]).Should().Be.True();
            CAM.Contains(NullIsotope).Should().Be.False();
            CAM.Contains(NullElement).Should().Be.False();
            CAM.Contains("N").Should().Be.True();
            CAM.Contains("H", 1).Should().Be.True();
            CAM.Contains("N", 15).Should().Be.False();
        }

        [Test]
        public void CountIsotope()
        {
            CAM.Count(Element.PeriodicTable["C"].PrincipalIsotope).Should().Equal(2);
            CAM.Count(Element.PeriodicTable["Fe"].PrincipalIsotope).Should().Equal(0);
            CAM.Count(Element.PeriodicTable["O"]).Should().Equal(1);
            CAM.Count(NullElement).Should().Equal(0);
            CAM.Count(NullIsotope).Should().Equal(0);
            CAM.Count("N").Should().Equal(1);
            CAM.Count("H", 1).Should().Equal(3);
            CAM.Count("N", 15).Should().Equal(0);
        }

        [Test]
        public void UniqueIsotopes()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.UniqueIsotopes.Should().Equal(4);
        }


        [Test]
        public void UniqueIsotopesWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CC{13}H3NO");

            formulaA.UniqueIsotopes.Should().Equal(5);
        }

        [Test]
        public void UniqueIsotopesOfEmptyFormula()
        {
            EmptyFormula.UniqueIsotopes.Should().Equal(0);
        }

        [Test]
        public void UniqueElements()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.UniqueElements.Should().Equal(4);
        }


        [Test]
        public void UniqueElementsWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CC{13}H3NO");

            formulaA.UniqueElements.Should().Equal(4);
        }

        [Test]
        public void UniqueElementsOfEmptyFormula()
        {
            EmptyFormula.UniqueElements.Should().Equal(0);
        }

        [Test]
        public void NumberOfAtoms()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.NumberOfAtoms.Should().Equal(7);
        }

        [Test]
        public void NumberOfAtomsOfEmptyFormula()
        {     
            EmptyFormula.NumberOfAtoms.Should().Equal(0);
        }
        
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void InvalidChemicalElement()
        {
            Element element = Element.PeriodicTable["Faa"];
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void InvalidElementIsotope()
        {
            Element element = Element.PeriodicTable["C"];
            Isotope isotope = element[100];
        }

    }
}
