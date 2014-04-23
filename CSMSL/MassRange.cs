namespace CSMSL
{
    public class MassRange : DoubleRange, IRange<double>
    {
        public MassRange()
            : base() { }

        public MassRange(double minMass, double maxMass)
            : base(minMass, maxMass) { }

        public MassRange(double meanMass, Tolerance tolerance)
            : base(meanMass, tolerance) { }

     
    }
}
