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
        public void ImplicitAddFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("H2O");
            ChemicalFormula formulaC = new ChemicalFormula("C2H5NO2");

            ChemicalFormula formulaD = formulaA + formulaB;

            formulaD.Should().Equal(formulaC);
        }

        [Test]
        public void ImplicitSubtractFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H5NO2");
            ChemicalFormula formulaB = new ChemicalFormula("H2O");
            ChemicalFormula formulaC = new ChemicalFormula("C2H3NO");

            ChemicalFormula formulaD = formulaA - formulaB;

            formulaD.Should().Equal(formulaC);
        }

        [Test]
        public void ImplicitMultipleFormulaLeft()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C4H6N2O2");           

            ChemicalFormula formulaC = formulaA * 2;

            formulaB.Should().Equal(formulaC);
        }

        [Test]
        public void ImplicitMultipleFormulaRight()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C4H6N2O2");

            ChemicalFormula formulaC = 2 * formulaA;

            formulaB.Should().Equal(formulaC);
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
        public void AddNullToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            formulaA.Add(NullChemicalFormula);

            formulaA.Should().Equal(formulaA);            
        }

        [Test]
        public void AddZeroIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Add(Element.PeriodicTable["H"][1], 0);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddIChemicalFormulaToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C18H31N5O11");
            CSMSL.Proteomics.Peptide pep = new CSMSL.Proteomics.Peptide("TEST");

            formulaA.Add(pep);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddFormulaToItself()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C4H6N2O2");

            formulaA.Add(formulaA);

            formulaA.Should().Equal(formulaB);
        }              

        [Test]
        public void AddNegativeIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H1NO");

            formulaA.Add(Element.PeriodicTable["H"][1], -2);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddNegativeFormulaToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C-1H-2");
            ChemicalFormula formulaC = new ChemicalFormula("CHNO");

            formulaA.Add(formulaB);

            formulaA.Should().Equal(formulaC);
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
        public void AddLargeIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NOFe");

            formulaA.Add(Element.PeriodicTable["Fe"], 1);

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
        public void RemoveNonExistantIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H5NO2");
            ChemicalFormula formulaB = new ChemicalFormula("Fe");
            ChemicalFormula formulaC = new ChemicalFormula("C2H5Fe-1NO2");

            formulaA.Remove(formulaB);

            formulaA.Should().Equal(formulaC);
        }

        [Test]
        public void RemoveFormulaFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H5NO2");
            ChemicalFormula formulaB = new ChemicalFormula("H2O");
            ChemicalFormula formulaC = new ChemicalFormula("C2H3NO");

            formulaA.Remove(formulaB);

            formulaA.Should().Equal(formulaC);
        }

        [Test]
        public void RemoveNullFormulaFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");     
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Remove(NullChemicalFormula);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveEmptyFormulaFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Remove(EmptyFormula);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2HNO");

            formulaA.Remove(Element.PeriodicTable["H"][1], 2);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveZeroIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Remove(Element.PeriodicTable["H"][1], 0);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveNegativeIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H5NO");

            formulaA.Remove(Element.PeriodicTable["H"][1], -2);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveIsotopeFromFromulaEquality()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3O");

            formulaA.Remove("N", 1);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveNullIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Remove(NullIsotope);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveIsotopeCompletelyFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2NO");

            formulaA.Remove(Element.PeriodicTable["H"][1]);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveElementCompletelyFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2NO");

            formulaA.Remove(Element.PeriodicTable["H"]);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveNullElementFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Remove(NullElement);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveElementCompletelyFromFromulaWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2C{13}H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("H3NO");

            formulaA.Remove(Element.PeriodicTable["C"]);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void RemoveElementCompletelyFromFromulaBySymbol()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2NO");

            formulaA.Remove("H");

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void ClearFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            formulaA.Clear();

            formulaA.AtomCount.Should().Equal(0);
        }

        [Test]
        public void HashCodeEquality()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("H3C2NO");

            formulaA.GetHashCode().Should().Equal(formulaB.GetHashCode());
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
        public void TotalNeutrons()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.GetNeutronCount().Should().Equal(19);
        }

        [Test]
        public void TotalProtons()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.GetProtonCount().Should().Equal(22);
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
        public void ParsingFormulaRepeatedElements()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CH3NOC");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void ParsingFormulaNoNumbers()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CCHHHNO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void ParsingFormulaNoNumbersRandomOrder()
        {
            ChemicalFormula formulaA = new ChemicalFormula("OCHHCHN");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void ParsingFormulaRepeatedElementsCancelEachOther()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NOC-2");
            ChemicalFormula formulaB = new ChemicalFormula("H3NO");

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void HillNotation()
        {
            ChemicalFormula formulaA = new ChemicalFormula("H3NC2O");

            formulaA.ToString().Should().Equal("C2H3NO");
        }

        [Test]
        public void HillNotationWithNegativeCount()
        {
            ChemicalFormula formulaA = new ChemicalFormula("H3NC-2O");

            formulaA.ToString().Should().Equal("C-2H3NO");
        }

        [Test]
        public void HillNotationWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("H3NC2C{13}2O");

            formulaA.ToString().Should().Equal("C2C{13}2H3NO");
        }

        [Test]
        public void HillNotationNoCarbon()
        {
            ChemicalFormula formulaA = new ChemicalFormula("HBr");

            formulaA.ToString().Should().Equal("BrH");
        }

        [Test]
        public void HillNotationNoCarbonNoHydrogen()
        {
            ChemicalFormula formulaA = new ChemicalFormula("Ca5O14Al6");

            formulaA.ToString().Should().Equal("Al6Ca5O14");
        }

        [Test]
        public void HillNotationNoHydrogen()
        {
            ChemicalFormula formulaA = new ChemicalFormula("NC2O");

            formulaA.ToString().Should().Equal("C2NO");
        }

        [Test]
        public void UniqueIsotopes()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.IsotopeCount.Should().Equal(4);
        }

        [Test]
        public void UniqueIsotopesWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CC{13}H3NO");

            formulaA.IsotopeCount.Should().Equal(5);
        }

        [Test]
        public void UniqueIsotopesOfEmptyFormula()
        {
            EmptyFormula.IsotopeCount.Should().Equal(0);
        }

        [Test]
        public void UniqueElements()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.ElementCount.Should().Equal(4);
        }

        [Test]
        public void UniqueElementsWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CC{13}H3NO");

            formulaA.ElementCount.Should().Equal(4);
        }

        [Test]
        public void UniqueElementsOfEmptyFormula()
        {
            EmptyFormula.ElementCount.Should().Equal(0);
        }

        [Test]
        public void NumberOfAtoms()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.AtomCount.Should().Equal(7);
        }
         
        [Test]
        public void NumberOfAtomsOfEmptyFormula()
        {
            EmptyFormula.AtomCount.Should().Equal(0);
        }

        //[Test]
        //public void CombineChemicalFormulas()
        //{
        //    ChemicalFormula formulaA = new ChemicalFormula("H2O");
        //    ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");
        //    ChemicalFormula formulaC = new ChemicalFormula("C5H2NO");
        //    ChemicalFormula formulaD = new ChemicalFormula("H2SO4");
        //    ChemicalFormula formulaE = new ChemicalFormula("N2O2C");
        //    ChemicalFormula formulaF = new ChemicalFormula("CCCCC");

        //    ChemicalFormula formulaG = new ChemicalFormula("C13H9N4O9S");

        //    ChemicalFormula formulaH = ChemicalFormula.Combine(formulaA, formulaB, formulaC, formulaD, formulaE, formulaF);

        //    formulaG.Should().Equal(formulaH);
        //}

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
