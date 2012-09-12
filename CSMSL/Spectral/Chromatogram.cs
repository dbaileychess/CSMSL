using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Spectral
{
    public enum ChromatogramType
    {
        BasePeak = 1,
        MzRange = 2,
        TotalIonCurrent = 3
    }

    public class Chromatogram : IEnumerable<ChromatogramPoint>
    {

        private SortedList<double, ChromatogramPoint> _curve;
        private ChromatogramType _type;

        private Range _range;
        public Range MzRange
        {
            get            
            {
                return _range;
            }
        }

        private float _tic;
        public float TotalIonCurrent
        {
            get { return _tic; }
        }

        public int Count
        {
            get { return _curve.Count; }
        }
   
        public Chromatogram(ChromatogramType type = ChromatogramType.BasePeak, Range range = null)
        {
            _type = type;
            _curve = new SortedList<double, ChromatogramPoint>();
            _range = range;
            _tic = 0;
        }

        public void AddPoint(ChromatogramPoint point)
        {
            ChromatogramPoint otherPoint = null;
            if (_curve.TryGetValue(point.RetentionTime, out otherPoint))
            {               
                otherPoint.CombinePoints(point);
            }
            else
            {
                _curve.Add(point.RetentionTime, point);
            }
            _tic += point.Intensity;
        }

        public override string ToString()
        {
            return string.Format("{0} Count = {1:N0} TIC = {2:G4}", Enum.GetName(typeof(ChromatogramType), _type), Count, _tic);
        }

        public IEnumerator<ChromatogramPoint> GetEnumerator()
        {
            return _curve.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _curve.Values.GetEnumerator();
        }
    }
}
