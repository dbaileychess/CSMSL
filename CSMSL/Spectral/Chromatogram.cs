using System;
using System.Collections.Generic;

namespace CSMSL.Spectral
{
    public enum ChromatogramType
    {
        BasePeak = 1,
        MzRange = 2,
        TotalIonCurrent = 3
    }

    public enum SmoothingType
    {
        None = 0,
        BoxCar = 1,
        SavitzkyGolay = 2
    }

    public class MzRangeChromatogram : Chromatogram
    {
        private MassRange _range;

        public MassRange MzRange
        {
            get
            {
                return _range;
            }
        }

        public MzRangeChromatogram(MassRange range, ChromatogramType type = ChromatogramType.BasePeak)
            : base(type)
        {
            _range = range;
        }
    }

    public class Chromatogram : IEnumerable<ChromatogramPoint>
    {
        public ChromatogramPoint this[int index]
        {
            get
            {
                return _curve.Values[index];
            }
        }

        private SortedList<double, ChromatogramPoint> _curve;
        private ChromatogramType _type;

        private float _tic;

        public float TotalIonCurrent
        {
            get { return _tic; }
        }

        public int Count
        {
            get { return _curve.Count; }
        }

        private ChromatogramPoint _basePeak;
        public ChromatogramPoint BasePeak
        {
            get
            {
                return _basePeak;
            }
        }

        public Chromatogram(ChromatogramType type = ChromatogramType.BasePeak)
        {
            _type = type;
            _curve = new SortedList<double, ChromatogramPoint>();
            _tic = 0;
        }

        public void AddPoint(ChromatogramPoint point)
        {
            ChromatogramPoint otherPoint = null;
            if (_curve.TryGetValue(point.Time, out otherPoint))
            {
                otherPoint.CombinePoints(point);
            }
            else
            {
                _curve.Add(point.Time, point);
            }
            if (_basePeak == null)
            {
                _basePeak = point;
            }
            else
            {
                if (point.Intensity > _basePeak.Intensity)
                {
                    _basePeak = point;
                }
            }
            _tic += point.Intensity;
        }

        public override string ToString()
        {
            return string.Format("Count = {0:N0} TIC = {1:G4} ({2})", Count, _tic, Enum.GetName(typeof(ChromatogramType), _type));
        }

        private Chromatogram BoxCarSmooth(int points)
        {
            Chromatogram chrom = new Chromatogram(_type);
            for (int i = 0; i < this.Count; i++)
            {
                double time = 0;
                float intensity = 0;
                int j = 0;
                while (j < points)
                {
                    if (i + j >= this.Count) break;
                    time += this[i + j].Time;
                    intensity += this[i + j].Intensity;
                    j++;
                }
                chrom.AddPoint(new ChromatogramPoint(time / (double)j, intensity / (float)j));
            }
            return chrom;
        }

        private Chromatogram SavitzkyGolaySmooth(int points)
        {
            return this;
        }

        public Chromatogram Smooth(SmoothingType type, int points)
        {
            switch (type)
            {
                case SmoothingType.BoxCar:
                    return BoxCarSmooth(points);
                case SmoothingType.SavitzkyGolay:
                    return SavitzkyGolaySmooth(points);
                case SmoothingType.None:
                default:
                    return this;
            }
        }  

        public IEnumerator<ChromatogramPoint> GetEnumerator()
        {
            return _curve.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _curve.Values.GetEnumerator();
        }

        public MassRange DetectPeakBounds()
        {
            return DetectPeakBounds((float)(1.0 / Math.E));
        }

        public MassRange DetectPeakBounds(float fraction)
        {
            return DetectPeakBounds(BasePeak, BasePeak.Intensity * fraction);
        }

        public MassRange DetectPeakBounds(ChromatogramPoint point, float intensityThreshold)
        {
            MassRange bounds = new MassRange();           
            int index = _curve.IndexOfValue(point);
            int i = index;            
            while (i > 0 && _curve.Values[i].Intensity >= intensityThreshold)  
                i--;
            bounds.Minimum = _curve.Values[i].Time;
            i = index;
            while (i < _curve.Count && _curve.Values[i].Intensity >= intensityThreshold)
                i++;
            bounds.Maximum = _curve.Values[i-1].Time;
            return bounds;
        }
    }

    public static class Extension
    {
        public static Chromatogram GetChromatogram(this IEnumerable<MSDataScan> scans, ChromatogramType type = ChromatogramType.BasePeak, MassRange range = null)
        {
            Chromatogram chrom;
            switch (type)
            {
                default:
                case ChromatogramType.BasePeak:
                    chrom = new Chromatogram(type);
                    foreach (MSDataScan scan in scans)
                    {
                        LabeledChromatogramPoint point = new LabeledChromatogramPoint(scan.RetentionTime, scan.Spectrum.BasePeak);
                        chrom.AddPoint(point);
                    }
                    break;

                case ChromatogramType.MzRange:
                    if (range == null)
                    {
                        throw new ArgumentException("A range must be declared for a m/z range chromatogram");
                    }
                    chrom = new MzRangeChromatogram(range, type);
                    List<MZPeak> peaks = new List<MZPeak>();
                    foreach (MSDataScan scan in scans)
                    {
                        if (scan.Spectrum.TryGetPeaks(range, out peaks))
                        {
                            //LabeledChromatogramPoint point = new LabeledChromatogramPoint(scan.RetentionTime, peaks);
                            //chrom.AddPoint(point);
                        }
                    }
                    break;

                case ChromatogramType.TotalIonCurrent:
                    chrom = new Chromatogram(type);
                    foreach (MSDataScan scan in scans)
                    {
                        ChromatogramPoint point = new ChromatogramPoint(scan.RetentionTime, (float)scan.Spectrum.TotalIonCurrent);
                        chrom.AddPoint(point);
                    }
                    break;
            }
            return chrom;
        }
    }
}