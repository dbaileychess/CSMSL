using System;
using System.Linq;
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
        public MassRange MzRange { get; private set; }

        public MzRangeChromatogram(MassRange range, ChromatogramType type = ChromatogramType.BasePeak)
            : base(type)
        {
            MzRange = range;
        }
    }

    public class Chromatogram : IEnumerable<ChromatographicPeak>
    {
        public ChromatographicPeak this[int index]
        {
            get
            {
                return _curve.Values[index];
            }
        }

        private readonly SortedList<double, ChromatographicPeak> _curve;
        
        public double TotalIonCurrent { get; private set; }

        public int Count
        {
            get { return _curve.Count; }
        }

        public ChromatogramType Type { get; private set; }

        private int _basePeakIndex = -1;

        public ChromatographicPeak BasePeak { get; private set; }

        public Chromatogram(ChromatogramType type = ChromatogramType.BasePeak)
        {
            Type = type;
            _curve = new SortedList<double, ChromatographicPeak>();
            TotalIonCurrent = 0.0;
        }
   
        public void AddPoint(ChromatographicPeak point)
        {
            int index = _curve.IndexOfKey(point.Time);
            if (index > -1)
            {
                _curve.Values[index].CombinePoints(point);
            }
            else
            {
                _curve.Add(point.Time, point);
            }

            if (BasePeak == null)
            {
                BasePeak = point;
            }
            else
            {
                if (point.Intensity > BasePeak.Intensity)
                {
                    BasePeak = point;
                }
            }
            TotalIonCurrent += (float)point.Intensity;
        }

        public override string ToString()
        {
            return string.Format("Count = {0:N0} TIC = {1:G4} ({2})", Count, TotalIonCurrent, Enum.GetName(typeof(ChromatogramType), Type));
        }

        private Chromatogram BoxCarSmooth(int points)
        {
            Chromatogram chrom = new Chromatogram(Type);
            for (int i = 0; i < Count; i++)
            {
                double time = 0;
                float intensity = 0;
                int j = 0;
                while (j < points)
                {
                    if (i + j >= Count) 
                        break;
                    time += this[i + j].Time;
                    intensity += (float)this[i + j].Intensity;
                    j++;
                }
                chrom.AddPoint(new ChromatographicPeak(time/j, intensity/j));
            }
            return chrom;
        }

        private Chromatogram SavitzkyGolaySmooth(int points)
        {
            throw new NotImplementedException();
        }

        public Chromatogram Smooth(SmoothingType type, int points)
        {
            switch (type)
            {
                case SmoothingType.BoxCar:
                    return BoxCarSmooth(points);
                case SmoothingType.SavitzkyGolay:
                    return SavitzkyGolaySmooth(points);
                default:
                    return this;
            }
        }

        public IEnumerator<ChromatographicPeak> GetEnumerator()
        {
            return _curve.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _curve.Values.GetEnumerator();
        }
    }

    public static class Extension
    {
        public static Chromatogram GetChromatogram(this IEnumerable<MSDataScan> scans, ChromatogramType type = ChromatogramType.BasePeak, MassRange range = null, bool zeroFillMissingValues = true)
        {
            Chromatogram chrom;
            switch (type)
            {
                default:
                    chrom = new Chromatogram(type);
                    foreach (MSDataScan scan in scans)
                    {
                        ChromatographicPeak point = new ChromatographicPeak(scan.RetentionTime, scan.MassSpectrum.BasePeak.Intensity);
                        chrom.AddPoint(point);
                    }
                    break;
                case ChromatogramType.MzRange:
                    if (range == null)
                    {
                        throw new ArgumentException("A range must be declared for a m/z range chromatogram");
                    }
                    chrom = new MzRangeChromatogram(range, type);
                    foreach (MSDataScan scan in scans)
                    {
                        List<MZPeak> peaks;
                        if (scan.MassSpectrum.TryGetPeaks(range, out peaks))
                        {
                            ChromatographicPeak point = new ChromatographicPeak(scan.RetentionTime, peaks.Sum(p => p.Intensity));
                            chrom.AddPoint(point);
                        }
                        else if (zeroFillMissingValues)
                        {
                            chrom.AddPoint(new ChromatographicPeak(scan.RetentionTime, 0.0));
                        }
                    }
                    break;

                case ChromatogramType.TotalIonCurrent:
                    chrom = new Chromatogram(type);
                    foreach (MSDataScan scan in scans)
                    {
                        ChromatographicPeak point = new ChromatographicPeak(scan.RetentionTime, scan.MassSpectrum.TotalIonCurrent);
                        chrom.AddPoint(point);
                    }
                    break;
            }
            return chrom;
        }
    }
}