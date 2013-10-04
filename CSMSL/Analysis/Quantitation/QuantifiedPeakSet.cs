using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Analysis.Quantitation
{
    public class QuantifiedPeakSet
    {
        public double Intensity { get { return _peaks.Sum(p => p.Intensity); } }
        public double DeNormalizedIntensity { get { return _peaks.Sum(p => p.DenomalizedIntensity); } }

        private HashSet<QuantifiedPeak> _peaks;

        public QuantifiedPeakSet()
        {
            _peaks = new HashSet<QuantifiedPeak>();
        }

        public QuantifiedPeakSet(IEnumerable<QuantifiedPeak> peaks)
        {
            _peaks = new HashSet<QuantifiedPeak>(peaks);
        }

        public void Add(QuantifiedPeak peak)
        {
            _peaks.Add(peak);
        }

        public int Count { get { return _peaks.Count; } }
 
        public QuantifiedPeakSet Filter(Func<QuantifiedPeak, bool> predicate)
        {
            return new QuantifiedPeakSet(_peaks.Where(predicate));
        }

        public IEnumerable<QuantifiedPeak> GetPeaks()
        {
            return _peaks;
        }

        public double GetValue(Func<QuantifiedPeak,double> selector)
        {
            return _peaks.Sum(selector);
        }

        public override string ToString()
        {
            return string.Format("Int: {0:G5} Count: {1:N0}", Intensity, Count);
        }

    }
}
