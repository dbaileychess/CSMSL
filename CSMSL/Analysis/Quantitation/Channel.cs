using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public class Channel : ChemicalFormula, IEquatable<Channel>
    {
        public string Name;
        public double Mz;
        public bool IsIsotopologue;
        public ChemicalFormula ChemicalFormula;


        public Channel(string name, double mz = 0, Isotopologue isotopologue = null)
        {
            Name = name;
            Mz = mz;
            if (isotopologue != null)
            {
                IsIsotopologue = true;
            }
        }

        public bool Equals(Channel other)
        {
            return Name.Equals(other.Name);
        }
    }
}
