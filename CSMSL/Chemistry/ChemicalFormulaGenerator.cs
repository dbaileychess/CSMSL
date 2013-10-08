using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMSL.Chemistry
{
    public class ChemicalFormulaGenerator
    {

        //private int[] minValues;
        //private int[] maxvalues;
        //private int maxUniqueID = 0;

        public ChemicalFormulaGenerator()
        {

        }

        public void AddConstraint(ChemicalFormula minimumChemicalFormula, ChemicalFormula maximumChemicalFormula)
        {
            
        }

        public void AddConstraint(Isotope isotope, int min, int max)
        {
            //if (isotope.UniqueId > maxUniqueID)
            //{
            //    maxUniqueID = isotope.UniqueId;
            //    Array.Resize(minValues, maxUniqueID);
            //    //etc..
            //}
            //minValues[isotope.UniqueId] = min;
        }

        public void AddConstraint(Isotope isotope, Range<int> range) 
        {

        }

        public void RemoveConstraint(Isotope isotope)
        {

        }

        public IEnumerable<ChemicalFormula> FromMass(MassRange range, int maxNumberOfResults = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChemicalFormula> FromMass(double lowMass, double highmass, int maxNumberOfResults = int.MaxValue)
        {
            throw new NotImplementedException();
        }
    }
}
