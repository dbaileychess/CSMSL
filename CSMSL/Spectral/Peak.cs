namespace CSMSL.Spectral
{
    public class Peak : IPeak
    {
        internal double _intensity;
        internal double _mz;

        public Peak()
            : this(0, 0) { }

        public Peak(double mz, double intensity)
        {
            _mz = mz;
            _intensity = intensity;
        }

        public double Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public double MZ
        {
            get { return _mz; }
            set { _mz = value; }
        }

        public int CompareTo(IPeak other)
        {
            return _mz.CompareTo(other.MZ);
        }

        public bool Equals(IPeak other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            return _mz.Equals(other.MZ) && _intensity.Equals(other.Intensity);
        }

        public override string ToString()
        {
            return string.Format("({0:G5}, {1:G5})", _mz, _intensity);
        }
    }
}