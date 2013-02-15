using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public class ChromatographicPeak : IPeak
    {
        protected double _time;

        public double Time
        {
            get
            {
                return _time;
            }
        }

        protected double _intensity;

        public double Intensity
        {
            get
            {
                return _intensity;
            }
            private set
            {
                _intensity = value;
            }
        }

        public ChromatographicPeak(double time, double intensity)
        {
            _time = time;
            _intensity = intensity;
        }

        public void CombinePoints(ChromatographicPeak otherPoint)
        {
            if (!Time.Equals(otherPoint.Time))
            {
                throw new ArgumentException("The two chromatogram points don't have the same time");
            }
            this._intensity += otherPoint.Intensity;           
        }

        public override string ToString()
        {
            return string.Format("({0:G4}, {1:G4})", Time, Intensity);
        }

        public int CompareTo(double time)
        {
            return Time.CompareTo(time);
        }

        public int CompareTo(IPeak other)
        {
            return Time.CompareTo(other.X);
        }

        public int CompareTo(ChromatographicPeak other)
        {
            return Time.CompareTo(other.Time);
        }

        public int CompareTo(object other)
        {
            throw new NotImplementedException();            
        }

        double IPeak.X
        {
            get { return Time; }
        }

        double IPeak.Y
        {
            get { return Intensity; }
        }
    }
}
