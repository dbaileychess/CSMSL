using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Spectral
{
    public class LabeledChromatogramPoint : ChromatogramPoint
    {
        private List<IMZPeak> _peaks;

        public List<IMZPeak> MzPeaks
        {
            get
            {
                return _peaks;
            }
        }

        public int Count
        {
            get
            {
                if (_peaks != null)
                {
                    return _peaks.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public LabeledChromatogramPoint(double time, IMZPeak peak)
            : base(time,peak.Intensity)
        {           
            _peaks = new List<IMZPeak>();
            _peaks.Add(peak);           
        }

        public LabeledChromatogramPoint(double time, IEnumerable<IMZPeak> peaks)
            : base(time,peaks.Sum(p => p.Intensity))
        {           
            _peaks = peaks.ToList();          
        }

        public void CombinePoints(LabeledChromatogramPoint otherPoint)
        {
            if (!Time.Equals(otherPoint.Time))
            {
                throw new ArgumentException("The two chromatogram points don't have the same retention time");
            }
            this._intensity += otherPoint.Intensity;
            if (_peaks == null)
            {
                _peaks = otherPoint._peaks;
            }
            else
            {
                _peaks.AddRange(otherPoint._peaks);
            }
        }

        public override string ToString()
        {
            return string.Format("({0:G4}, {1:G4}) Peak Count = {2:G0}", Time, Intensity, Count);
        }
    }

    public class ChromatogramPoint : IComparable<ChromatogramPoint>
    {
        protected double _time;

        public double Time
        {
            get
            {
                return _time;
            }
        }

        protected float _intensity;

        public float Intensity
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

        public ChromatogramPoint(double time, float intensity)
        {
            _time = time;
            _intensity = intensity;
        }

        public void CombinePoints(ChromatogramPoint otherPoint)
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

        public int CompareTo(ChromatogramPoint other)
        {
            return this.Time.CompareTo(other.Time);
        }
    }
}