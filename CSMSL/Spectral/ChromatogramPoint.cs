using System.Collections.Generic;
using System.Linq;
using System;

namespace CSMSL.Spectral
{
    public class ChromatogramPoint
    {
        private double _retentionTime;

        public double RetentionTime
        {
            get
            {
                return _retentionTime;
            }
        }

        private float _intensity;

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

        private List<IPeak> _peaks;

        public List<IPeak> MzPeaks
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

        public ChromatogramPoint(double time, IPeak peak)
        {
            _retentionTime = time;
            _peaks = new List<IPeak>();
            _peaks.Add(peak);
            _intensity = _peaks.Sum(p => p.Intensity);
        }

        public ChromatogramPoint(double time, IEnumerable<IPeak> peaks)
        {
            _retentionTime = time;
            _peaks = peaks.ToList();
            _intensity = _peaks.Sum(p => p.Intensity);
        }

        public ChromatogramPoint(double time, float intensity)
        {
            _retentionTime = time;
            _peaks = null;
            _intensity = intensity;
        }

        public void CombinePoints(ChromatogramPoint otherPoint)
        {
            if (!RetentionTime.Equals(otherPoint.RetentionTime))
            {
                throw new ArgumentException("The two chromatogram points don't have the same retention time");
            }
            this.Intensity += otherPoint.Intensity;
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
            return string.Format("({0:G4}, {1:G4})", RetentionTime, Intensity);
        }
    }
}