namespace CSMSL
{
    public class MassRange : Range<double>, IRange<double>
    {
        protected double _mean;

        protected double _width;

        public MassRange()
            : this(0, 0) { }

        public MassRange(double minimum, double maximum)
        {
            base.Minimum = minimum;
            base.Maximum = maximum;
            _width = base.Maximum - base.Minimum;
            _mean = (base.Maximum + base.Minimum) / 2.0;
        }

        public MassRange(MassRange range)
            : this(range.Minimum, range.Maximum) { }

        public MassRange(double mean, MassTolerance tolerance)
        {
            _mean = mean;
            SetTolerance(tolerance);
        }

        private void SetTolerance(MassTolerance tolerance)
        {
            switch (tolerance.Type)
            {
                default:
                    base.Minimum = _mean - tolerance.Value / 2;
                    base.Maximum = _mean + tolerance.Value / 2;
                    break;

                case MassToleranceType.MMU:
                    base.Minimum = _mean - tolerance.Value / 2000;
                    base.Maximum = _mean + tolerance.Value / 2000;
                    break;

                case MassToleranceType.PPM:
                    base.Minimum = _mean * (1 - (tolerance.Value / 2e6));
                    base.Maximum = _mean * (1 + (tolerance.Value / 2e6));
                    break;
            }
            _width = base.Maximum - base.Minimum;
        }

        public new double Maximum
        {
            get
            {
                return base.Maximum;
            }
            set
            {
                base.Maximum = value;
                _width = base.Maximum - base.Minimum;
                _mean = (base.Maximum + base.Minimum) / 2.0;
            }
        }

        public double Mean
        {
            get
            {
                return _mean;
            }
            set
            {
                _mean = value;
                base.Minimum = _mean - (_width / 2.0);
                base.Maximum = _mean + (_width / 2.0);
            }
        }

        public new double Minimum
        {
            get
            {
                return base.Minimum;
            }
            set
            {
                base.Minimum = value;
                _width = base.Maximum - base.Minimum;
                _mean = (base.Maximum + base.Minimum) / 2.0;
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                base.Minimum = _mean - (_width / 2.0);
                base.Maximum = _mean + (_width / 2.0);
            }
        }

        public static MassRange FromPPM(double mean, double ppmTolerance)
        {
            return new MassRange(mean, new MassTolerance(MassToleranceType.PPM, ppmTolerance));
        }

        public static MassRange FromDa(double mean, double daTolerance)
        {
            return new MassRange(mean, new MassTolerance(MassToleranceType.DA, daTolerance));
        }

    }
}
