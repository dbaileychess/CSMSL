using System;
using System.Collections.Generic;
using System.Linq;

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

    public class Chromatogram : Chromatogram<ChromatographicPeak>
    {
        public Chromatogram(ChromatogramType type = ChromatogramType.BasePeak)
            : base(type)
        {
        }

        public Chromatogram(IEnumerable<ChromatographicPeak> peaks, ChromatogramType type = ChromatogramType.BasePeak)
            : base(peaks, type)
        {
        }
    }

    public class Chromatogram<T> : IEnumerable<T> where T: IPeak
    {
        public T this[int index]
        {
            get
            {
                return _curve.Values[index];
            }
        }

        private readonly SortedList<double, T> _curve;
        
        public double TotalIonCurrent { get; private set; }

        public int Count
        {
            get { return _curve.Count; }
        }

        public ChromatogramType Type { get; private set; }

        public T BasePeak { get; private set; }

        public Chromatogram(ChromatogramType type = ChromatogramType.BasePeak)
        {
            Type = type;
            _curve = new SortedList<double, T>();
            TotalIonCurrent = 0.0;
        }

        public Chromatogram(IEnumerable<T> points, ChromatogramType type = ChromatogramType.BasePeak)
        {
            Type = type;
            _curve = new SortedList<double, T>(points.ToDictionary(p => p.X));
            TotalIonCurrent = _curve.Values.Sum(p => p.Y);
        }

        public void AddPoint(T point)
        {
            _curve.Add(point.X, point);

            if (BasePeak == null)
            {
                BasePeak = point;
            }
            else
            {
                if (point.Y > BasePeak.Y)
                {
                    BasePeak = point;
                }
            }

            TotalIonCurrent += point.Y;
        }

        public override string ToString()
        {
            return string.Format("Count = {0:N0} TIC = {1:G4} ({2})", Count, TotalIonCurrent, Enum.GetName(typeof(ChromatogramType), Type));
        }
        
        public IEnumerator<T> GetEnumerator()
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