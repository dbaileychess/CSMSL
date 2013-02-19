using System;

namespace CSMSL.Spectral
{
    /// <summary>
    /// Represents a peak in a 2-dimensional spectra
    /// </summary>
    public interface IPeak : IComparable<double>, IComparable<IPeak>, IComparable
    {
        /// <summary>
        /// The X value of this peak
        /// </summary>
        double X { get; }

        /// <summary>
        /// The Y value of this peak
        /// </summary>
        double Y { get; }
    }    
}
