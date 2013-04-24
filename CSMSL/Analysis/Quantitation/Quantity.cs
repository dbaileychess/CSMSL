using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Analysis.Quantitation
{
    public class Quantity
    {
        private double _value;

        public double Value { get { return _value; } }

        private HashSet<QuantifiedPeak> _quantPeaks;

        public Quantity()
        {         
            _quantPeaks = new HashSet<QuantifiedPeak>();
            _value = 0;
        }

        public void Add(QuantifiedPeak peak, string name = "mono", int number = 0)
        {            
            if (_quantPeaks.Add(peak))
            {
                _value += peak.DenormalizedIntensity();
            }
        }

        public override string ToString()
        {
            return Value.ToString("G5");
        }

    }
}
