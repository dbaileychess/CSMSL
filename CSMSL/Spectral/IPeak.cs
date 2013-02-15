using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public interface IPeak : IComparable<double>, IComparable<IPeak>, IComparable
    {
        double X { get; }
        double Y { get; }
    }    
}
