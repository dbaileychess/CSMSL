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
        private Range _range;
   
        public Chromatogram(ChromatogramType type = ChromatogramType.BasePeak)
        {
            _curve = new SortedList<double, ChromatogramPoint>();
        }


        public IEnumerator<ChromatogramPoint> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
