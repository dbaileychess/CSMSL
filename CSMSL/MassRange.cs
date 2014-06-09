namespace CSMSL
{
    public class MassRange : DoubleRange
    {
        public MassRange() { }

        public MassRange(double minMass, double maxMass)
            : base(minMass, maxMass) { }

        public MassRange(double meanMass, Tolerance toleranceWidth)
            : base(meanMass, toleranceWidth) { }

        public override string ToString()
        {
            return ToString("G9");
        }

        public override string ToString(string format)
        {
            return string.Format("{0} - {1} Da", Minimum.ToString(format), Maximum.ToString(format));
        }

        #region Static

        public static new MassRange FromPPM(double mean, double ppmTolerance)
        {
            return new MassRange(mean, new Tolerance(ToleranceType.PPM, ppmTolerance));
        }

        #endregion
    }
}
