using System;

namespace CSMSL.Spectral
{
    public interface IPeak : IComparable<IPeak>, IEquatable<IPeak>
    {
        double Intensity { get; set; }

        double MZ { get; set; }
    }
}