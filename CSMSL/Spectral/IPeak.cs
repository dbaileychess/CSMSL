using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    /// <summary>
    /// Represents a peak in a 2-dimensional spectra
    /// </summary>
    public interface IPeak : IComparable<double>, IComparable<IPeak>, IComparable
    {
        /// <summary>
        /// The X value
        /// </summary>
        double X { get; }

        /// <summary>
        /// The Y value
        /// </summary>
        double Y { get; }
    }    
}
