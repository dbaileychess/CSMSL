using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            _min = minimum;
            _max = maximum;
            _width = _max - _min;
            _mean = (_max + _min) / 2.0;
        }

        public MassRange(MassRange range)
            : this(range._min, range._max) { }

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
                case MassToleranceType.DA:
                    _min = _mean - tolerance.Value / 2;
                    _max = _mean + tolerance.Value / 2;
                    break;

                case MassToleranceType.MMU:
                    _min = _mean - tolerance.Value / 2000;
                    _max = _mean + tolerance.Value / 2000;
                    break;

                case MassToleranceType.PPM:
                    _min = _mean * (1 - (tolerance.Value / 2e6));
                    _max = _mean * (1 + (tolerance.Value / 2e6));
                    break;
            }
            _width = _max - _min;
        }

        public new double Maximum
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                _width = _max - _min;
                _mean = (_max + _min) / 2.0;
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
                _min = _mean - (_width / 2.0);
                _max = _mean + (_width / 2.0);
            }
        }

        public new double Minimum
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
                _width = _max - _min;
                _mean = (_max + _min) / 2.0;
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
                _min = _mean - (_width / 2.0);
                _max = _mean + (_width / 2.0);
            }
        }

    }
}
