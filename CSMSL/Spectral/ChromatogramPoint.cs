using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public class ChromatogramPoint : IEnumerable<IPeak>
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

        public ChromatogramPoint(double time, IEnumerable<IPeak> peaks)
        {
            _retentionTime = time;
            _peaks = peaks.ToList();
            _intensity = peaks.Sum(peak => peak.Intensity);
        }

        public ChromatogramPoint(double time, float intensity)
        {
            _retentionTime = time;
            _peaks = null;
            _intensity = intensity;
        }

        public IEnumerator<IPeak> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
