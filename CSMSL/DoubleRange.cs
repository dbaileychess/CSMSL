using System;

namespace CSMSL
{
    public class DoubleRange : Range<double>
    {
        public DoubleRange()
            : this(0, 0) { }

        public DoubleRange(double minimum, double maximum)
            : base(minimum, maximum) { }

        public DoubleRange(IRange<double> range)
            : this(range.Minimum, range.Maximum) { }

        /// <summary>
        /// Creates a full-width range around some mean value
        /// i.e. 10 ppm at 500 would give you 499.9975 - 500.0025
        /// which has a width of 0.005. Coverting back to ppm 
        /// (1e6) *0.005 / 500 = 10 ppm.
        /// The differnce from the mean value to an boundary is exactly
        /// half the tolearnce you specified
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="tolerance"></param>
        public DoubleRange(double mean, Tolerance tolerance)
        {
            SetTolerance(mean, tolerance);
        }

        private void SetTolerance(double mean, Tolerance tolerance)
        {
            if (tolerance == null)
            {
                Minimum = Maximum = mean;         
                return;
            }

            double value = Math.Abs(tolerance.Value);

            switch (tolerance.Type)
            {
                default:
                    Minimum = mean - value / 2.0;
                    Maximum = mean + value / 2.0;
                    break;

                case ToleranceType.MMU:
                    Minimum = mean - value / 2000.0;
                    Maximum = mean + value / 2000.0;
                    break;

                case ToleranceType.PPM:
                    Minimum = mean * (1 - (value / 2e6));
                    Maximum = mean * (1 + (value / 2e6));
                    break;
            }           
        }

        public double Mean
        {
            get
            {
                return (Maximum + Minimum) / 2.0;
            }
        }

        public double Width
        {
            get
            {
                return Maximum - Minimum;              
            }
        }

        public double ToPPM()
        {
            return 1e6 * Width / Mean;
        }

        public static DoubleRange FromPPM(double mean, double ppmTolerance)
        {
            return new DoubleRange(mean, new Tolerance(ToleranceType.PPM, ppmTolerance));
        }

        public static DoubleRange FromDa(double mean, double daTolerance)
        {
            return new DoubleRange(mean, new Tolerance(ToleranceType.DA, daTolerance));
        }

        public virtual string ToString(string format)
        {
            return string.Format("[{0} - {1}]", Minimum.ToString(format), Maximum.ToString(format));
        }
    }
}
