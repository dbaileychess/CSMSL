namespace CSMSL
{
    public class MzRange : DoubleRange
    {
        public MzRange() { }

        public MzRange(double minMZ, double maxMZ)
            : base(minMZ, maxMZ) { }

        public MzRange(double meanMZ, Tolerance tolerance)
            : base(meanMZ, tolerance) { }
    }
}
