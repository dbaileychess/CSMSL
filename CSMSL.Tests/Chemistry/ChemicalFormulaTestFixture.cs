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
        private ChemicalFormula EmptyFormula = new ChemicalFormula();
        private ChemicalFormula NullChemicalFormula = null;
        private Element NullElement = null;
        private Isotope NullIsotope = null;
        private IChemicalFormula NullIChemicalFormula = null;

        [Test]
        public void AddElementToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3N2O");

            Element n = Element.PeriodicTable["N"];

            formulaA.Add(n, 1);

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
        public void AddFormulaToItself()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C4H6N2O2");

            formulaA.Add(formulaA);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddIChemicalFormulaToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            IChemicalFormula formulaB = new ChemicalFormula("H2O");
            ChemicalFormula formulaC = new ChemicalFormula("C2H5NO2");

            formulaA.Add(formulaB);

            formulaA.Should().Equal(formulaC);
        }

        [Test]
        public void AddIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H4NO");

            Isotope h1 = Element.PeriodicTable["H"][1];

            formulaA.Add(h1, 1);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddLargeIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NOFe");

            Isotope fe = Element.PeriodicTable["Fe"].PrincipalIsotope;

            formulaA.Add(fe, 1);

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
        public void AddNegativeIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H1NO");

            Isotope h1 = Element.PeriodicTable["H"][1];

            formulaA.Add(h1, -2);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException), ExpectedMessage = "The element symbol 'Faa' is not found in the periodic table")]
        public void AddNonExistentSymbolToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");          
            
            formulaA.Add("Faa", 1);           
        }

        [Test]
        public void AddNullElementToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");
                       
            formulaA.Add(NullElement, 1);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddNullIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");
                       
            formulaA.Add(NullIsotope, 1);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddNullFormulaToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Add(NullChemicalFormula);

            formulaA.Should().Equal(formulaB);
        }


        [Test]
        public void AddNullIChemicalFormulaToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Add(NullIChemicalFormula);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddZeroElementToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            Element n = Element.PeriodicTable["N"];

            formulaA.Add(n, 0);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddZeroIsotopeToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            Isotope h1 = Element.PeriodicTable["H"][1];

            formulaA.Add(h1, 0);   
            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void AddZeroSymbolToFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");
                   
            formulaA.Add("H", 0);
            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void BasicChemicalFormulaMass()
        {
            ChemicalFormula water = new ChemicalFormula("H2O");
            water.Mass.Monoisotopic.Should().Equal(2 * Constants.HYDROGEN + Constants.OXYGEN);
        }

        [Test]        
        public void ClearFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            formulaA.Clear();

            formulaA.Should().Equal(EmptyFormula);
        }


        [Test]
        public void ConstructorBlankStringEqualsEmptyFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("");

            formulaA.Should().Equal(EmptyFormula);
        }

        [Test]
        public void ConstructorEmptyStringEqualsEmptyFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula(string.Empty);
           
            formulaA.Should().Equal(EmptyFormula);
        }


        [Test]
        public void ConstructorDefaultEqualsEmptyFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula();

            formulaA.Should().Equal(EmptyFormula);
        }

        [Test]
        public void CopyConstructorValueEquality()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula(formulaA);

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void CopyConstructorNullEqualsEmptyFormula()
        {           
            ChemicalFormula formulaA = new ChemicalFormula(NullChemicalFormula);

            formulaA.Should().Equal(EmptyFormula);
        }

        [Test]
        public void CopyConstructorReferenceInequality()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula(formulaA);

            formulaA.Should().Not.Be.SameAs(formulaB);
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
        public void EmptyFormulaMonoisotopicMassIsZero()
        {           
            EmptyFormula.Mass.Monoisotopic.Should().Equal(0.0);            
        }

        [Test]
        public void EmptyFormulaAverageMassIsZero()
        {
            EmptyFormula.Mass.Average.Should().Equal(0.0);
        }
        
        [Test]
        public void EmptyFormulaStringIsBlank()
        {
            EmptyFormula.Formula.Should().Be.Empty();
        }

        [Test]
        public void EmptyFormulaAtomCountIsZero()
        {
            EmptyFormula.AtomCount.Should().Equal(0);
        }

        [Test]
        public void EmptyFormulaElementCountIsZero()
        {
            EmptyFormula.ElementCount.Should().Equal(0);
        }

        [Test]
        public void EmptyFormulaIsotopeCountIsZero()
        {   
            EmptyFormula.IsotopeCount.Should().Equal(0);
        }

        [Test]
        public void FormulaValueInequalityNullFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.Should().Not.Equal(NullChemicalFormula);
        }

        [Test]
        public void FormulaValueInequality()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("NC1OH3");

            formulaA.Should().Not.Equal(formulaB);
        }

        [Test]
        public void FormulaValueInequalityHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("CC{13}H3NO");

            formulaB.Should().Not.Equal(formulaA);
        }

        [Test]
        public void FormulaValueEqualityItself()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.Should().Equal(formulaA);
        }

        [Test]
        public void FormulaValueEquality()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("NC2OH3");

            formulaA.Should().Equal(formulaB);
        }

        [Test]
        public void HashCodeEquality()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("H3C2NO");

            formulaA.GetHashCode().Should().Equal(formulaB.GetHashCode());
        }

        [Test]
        public void HillNotation()
        {
            ChemicalFormula formulaA = new ChemicalFormula("H3NC2O");

            formulaA.ToString().Should().Equal("C2H3NO");
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
        public void HillNotationWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("H3NC2C{13}2O");

            formulaA.ToString().Should().Equal("C2C{13}2H3NO");
        }

        [Test]
        public void HillNotationWithNegativeCount()
        {
            ChemicalFormula formulaA = new ChemicalFormula("H3NC-2O");

            formulaA.ToString().Should().Equal("C-2H3NO");
        }

        [Test]
        public void HillNotationWithHeavyIsotopeNegativeCount()
        {
            ChemicalFormula formulaA = new ChemicalFormula("H3NC2C{13}-2O");

            formulaA.ToString().Should().Equal("C2C{13}-2H3NO");
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
        public void ImplicitAddNullFormulaRight()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");     
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            ChemicalFormula formulaC = formulaA + NullChemicalFormula;

            formulaB.Should().Equal(formulaC);
        }

        [Test]
        public void ImplicitAddNullFormulaLeft()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            ChemicalFormula formulaC = NullChemicalFormula + formulaA;

            formulaB.Should().Equal(formulaC);
        }

        [Test]
        public void ImplicitAddNullFormulaLeftRight()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            ChemicalFormula formulaC = NullChemicalFormula + NullChemicalFormula;

            formulaC.Should().Equal(NullChemicalFormula);
        }

        [Test]
        public void ImplicitConstructor()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = "C2H3NO";

            formulaA.Should().Equal(formulaB);
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
        public void ImplicitSubtractFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H5NO2");
            ChemicalFormula formulaB = new ChemicalFormula("H2O");
            ChemicalFormula formulaC = new ChemicalFormula("C2H3NO");

            ChemicalFormula formulaD = formulaA - formulaB;

            formulaD.Should().Equal(formulaC);
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

        [Test]
        public void NullFormulaDoesNotEqualFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");          

            NullChemicalFormula.Should().Not.Equal(formulaA);
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


        [Test]
        public void NumberOfAtomsOfNegativeFormula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C-2H-3N-1O-1");

            formulaA.AtomCount.Should().Equal(-7);
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
        public void ParsingFormulaRepeatedElements()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CH3NOC");
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
        public void RemoveElementCompletelyFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2NO");

            formulaA.Remove(Element.PeriodicTable["H"]);

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
        public void RemoveElementCompletelyFromFromulaWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2C{13}H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("H3NO");

            formulaA.Remove(Element.PeriodicTable["C"]);

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
        public void RemoveFormulaFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H5NO2");
            ChemicalFormula formulaB = new ChemicalFormula("H2O");
            ChemicalFormula formulaC = new ChemicalFormula("C2H3NO");

            formulaA.Remove(formulaB);

            formulaA.Should().Equal(formulaC);
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
        public void RemoveIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2HNO");

            formulaA.Remove(Element.PeriodicTable["H"][1], 2);

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
        public void RemoveNegativeIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H5NO");

            formulaA.Remove(Element.PeriodicTable["H"][1], -2);

            formulaA.Should().Equal(formulaB);
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
        public void RemoveNullElementFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Remove(NullElement);

            formulaA.Should().Equal(formulaB);
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
        public void RemoveNullIsotopeFromFromula()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");
            ChemicalFormula formulaB = new ChemicalFormula("C2H3NO");

            formulaA.Remove(NullIsotope);

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
        public void UniqueElements()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.ElementCount.Should().Equal(4);
        }

        [Test]
        public void UniqueElementsOfEmptyFormula()
        {
            EmptyFormula.ElementCount.Should().Equal(0);
        }

        [Test]
        public void UniqueElementsWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CC{13}H3NO");

            formulaA.ElementCount.Should().Equal(4);
        }

        [Test]
        public void UniqueIsotopes()
        {
            ChemicalFormula formulaA = new ChemicalFormula("C2H3NO");

            formulaA.IsotopeCount.Should().Equal(4);
        }

        [Test]
        public void UniqueIsotopesOfEmptyFormula()
        {
            EmptyFormula.IsotopeCount.Should().Equal(0);
        }

        [Test]
        public void UniqueIsotopesWithHeavyIsotope()
        {
            ChemicalFormula formulaA = new ChemicalFormula("CC{13}H3NO");

            formulaA.IsotopeCount.Should().Equal(5);
        }
     
    }
}
