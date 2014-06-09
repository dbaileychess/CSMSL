namespace CSMSL
{
    public class MzRange : DoubleRange
    {
        public MzRange() { }

        public MzRange(double minMZ, double maxMZ)
            : base(minMZ, maxMZ) { }

        public MzRange(double meanMZ, Tolerance toleranceWidth)
            : base(meanMZ, toleranceWidth) { }

        public override string ToString()
        {
            return ToString("G9");
        }

        public override string ToString(string format)
        {
            return string.Format("{0} - {1} m/z", Minimum.ToString(format), Maximum.ToString(format));
        }

        #region Static

        public static new MzRange FromPPM(double mean, double ppmTolerance)
        {
            return new MzRange(mean, new Tolerance(ToleranceUnit.PPM, ppmTolerance));
        }

        #endregion
    }
}
