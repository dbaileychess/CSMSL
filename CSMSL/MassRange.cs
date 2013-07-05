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
            Min = minimum;
            Max = maximum;
            _width = Max - Min;
            _mean = (Max + Min) / 2.0;
        }

        public MassRange(MassRange range)
            : this(range.Min, range.Max) { }

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
                    Min = _mean - tolerance.Value / 2;
                    Max = _mean + tolerance.Value / 2;
                    break;

                case MassToleranceType.MMU:
                    Min = _mean - tolerance.Value / 2000;
                    Max = _mean + tolerance.Value / 2000;
                    break;

                case MassToleranceType.PPM:
                    Min = _mean * (1 - (tolerance.Value / 2e6));
                    Max = _mean * (1 + (tolerance.Value / 2e6));
                    break;
            }
            _width = Max - Min;
        }

        public new double Maximum
        {
            get
            {
                return Max;
            }
            set
            {
                Max = value;
                _width = Max - Min;
                _mean = (Max + Min) / 2.0;
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
                Min = _mean - (_width / 2.0);
                Max = _mean + (_width / 2.0);
            }
        }

        public new double Minimum
        {
            get
            {
                return Min;
            }
            set
            {
                Min = value;
                _width = Max - Min;
                _mean = (Max + Min) / 2.0;
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
                Min = _mean - (_width / 2.0);
                Max = _mean + (_width / 2.0);
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
