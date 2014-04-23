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

            double value = System.Math.Abs(tolerance.Value);

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
            //set
            //{
            //    double width = Width;
            //    double mean = value;         
            //    base.Minimum = mean - (width / 2.0);
            //    base.Maximum = mean + (width / 2.0);
            //}
        }

        public double Width
        {
            get
            {
                return Maximum - Minimum;              
            }
            //set
            //{
            //    double width = value;
            //    double mean = Mean;
            //    base.Minimum = mean - (width / 2.0);
            //    base.Maximum = mean + (width / 2.0);
            //}
        }

        public static DoubleRange FromPPM(double mean, double ppmTolerance)
        {
            return new DoubleRange(mean, new Tolerance(ToleranceType.PPM, ppmTolerance));
        }

        public static DoubleRange FromDa(double mean, double daTolerance)
        {
            return new DoubleRange(mean, new Tolerance(ToleranceType.DA, daTolerance));
        }

        public new string ToString(string format = "F4")
        {
            return string.Format("[{0} - {1}]", Minimum.ToString(format), Maximum.ToString(format));
        }
    }
}
