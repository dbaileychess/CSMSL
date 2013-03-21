using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Analysis.Quantitation
{
    public class Channel
    {
        public string Name;
        public double Mz;
        public int Charge;

        public Channel(string name, double mz = 0, int charge = 0)
        {
            Name = name;
            Mz = mz;
            Charge = charge;
        }

    }
}
