using System;

namespace CSMSL
{
    public enum ToleranceType { PPM, DA, MMU }

    public class Tolerance
    {
        private ToleranceType _type;
        private double _value;

        public Tolerance(ToleranceType type, double value)
        {
            _type = type;
            _value = value;
        }

        public Tolerance(ToleranceType type, double experimental, double theoretical)
            : this(type, GetTolerance(experimental, theoretical, type)) { }

        public ToleranceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public static double GetTolerance(double experimental, double theoretical, ToleranceType type)
        {
            switch (type)
            {
                case ToleranceType.MMU:
                    return (experimental - theoretical) * 1000.0;
                case ToleranceType.PPM:
                    return (experimental - theoretical) / theoretical * 1000000.0;
                case ToleranceType.DA:
                default:
                    return experimental - theoretical;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:f4} {1}", _value, Enum.GetName(typeof(ToleranceType), _type));
        }
    }
}