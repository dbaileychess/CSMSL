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
     

    //public class MzRangeChromatogram : Chromatogram
    //{
    //    public MassRange MzRange { get; private set; }

    //    public MzRangeChromatogram(MassRange range, ChromatogramType type = ChromatogramType.BasePeak)
    //        : base(type)
    //    {
    //        MzRange = range;
    //    }
    //}

    //public class Chromatogram : Chromatogram<ChromatographicPeak>
    //{
    //    // from http://www.vias.org/tmdatanaleng/cc_savgol_coeff.html
    //    private static readonly int[] savitzkGolayH = new int[] { 35, 21, 231, 429, 143, 1105, 323, 2261, 3059, 805, 5175 };

    //    private static readonly int[,] savitzkGolayA = new int[,]{ { 17,12,-3,0,0,0,0,0,0,0,0,0,0},
    //                                           { 7,6,3,-2,0,0,0,0,0,0,0,0,0},
    //                                           { 59,54,39,14,-21,0,0,0,0,0,0,0,0},
    //                                           {89,84,69,44,9,-36,0,0,0,0,0,0,0},
    //                                           {25,24,21,16,9,0,-11,0,0,0,0,0,0},
    //                                           {167,162,147,122,87,42,-13,-78,0,0,0,0,0},
    //                                           {43,42,39,34,27,18,7,-6,-21,0,0,0,0},
    //                                           {269,264,249,224,189,144,89,24,-51,-136,0,0,0},
    //                                           {329,324,209,284,249,204,149,84,9,-76,-171,0,0},
    //                                           {79,78,75,70,63,54,43,30,15,-2,-21,-42,0},
    //                                           {467,462,447,422,687,343,287,222,147,62,-33,-138,-253} };

    //    public Chromatogram(ChromatogramType type = ChromatogramType.BasePeak)
    //        : base(type)
    //    {
    //    }

    //    public Chromatogram(IEnumerable<ChromatographicPeak> peaks, ChromatogramType type = ChromatogramType.BasePeak)
    //        : base(peaks, type)
    //    {
    //    }

    //    //public Chromatogram Smooth(SmoothingType type, int pts = 3)
    //    //{
    //    //    switch (type)
    //    //    {
    //    //        case SmoothingType.BoxCar:
    //    //            return BoxCar(pts);
    //    //        case SmoothingType.SavitzkyGolay:
    //    //            return SavitzkyGolay(pts);
    //    //        default:
    //    //            return this;
    //    //    }
    //    //}

    //    //private Chromatogram SavitzkyGolay(int pts)
    //    //{
    //    //    if (pts < 5 || pts > 25)
    //    //    {
    //    //        throw new ArgumentOutOfRangeException("The number of points must be between 5 and 25 and be odd, you specified " + pts + " points");
    //    //    }

    //    //    pts = pts - (1 - pts % 2);

    //    //    int index = (pts / 5) - 1;
    //    //    int h = savitzkGolayH[index];
    //    //    int center = (pts / 2);
    //    //    int[] easyCoeff = new int[center * 2 + 1];
    //    //    int count = 0;
    //    //    for (int i = center; i >= 0; i--)
    //    //    {
    //    //        easyCoeff[count++] = savitzkGolayA[index, i];
    //    //    }
    //    //    for (int i = 1; i <= center; i++)
    //    //    {
    //    //        easyCoeff[count++] = savitzkGolayA[index, i];
    //    //    }

    //    //    Chromatogram chrom = new Chromatogram(Type);

    //    //    var peaks = _curve.Values.ToArray();
    //    //    for (int x = center; x < peaks.Length - center; x++)
    //    //    {
    //    //        double peakX = peaks[x].Time;
    //    //        double peakY = 0;
    //    //        for (int j = -center; j < center; j++)
    //    //        {
    //    //            peakY += peaks[x + j].Intensity * easyCoeff[center + j];
    //    //        }
    //    //        peakY /= h;
    //    //        peakY = Math.Max(peakY, 0);

    //    //        chrom.AddPoint(new ChromatographicPeak(peakX, peakY));
    //    //    }

    //    //    return chrom;
    //    //}

    //    //private Chromatogram BoxCar(int pts)
    //    //{
    //    //    pts = pts - (1 - pts % 2);

    //    //    Chromatogram chrom = new Chromatogram(Type);

    //    //    var peaks = _curve.Values.ToArray();

    //    //    for (int i = pts; i < peaks.Length - pts; i++)
    //    //    {
    //    //        double x = 0;
    //    //        double y = 0;
    //    //        for (int j = i - pts; j < i + pts; j++)
    //    //        {
    //    //            var peak = peaks[j];
    //    //            x += peak.Time;
    //    //            y += peak.Intensity;
    //    //        }
    //    //        x /= ((pts * 2) );
    //    //        y /= ((pts * 2) );
    //    //        chrom.AddPoint(new ChromatographicPeak(x, y));
    //    //    }


    //    //    return chrom;
    //    //}
    //}

    public class Chromatogram<T> : IEnumerable<T> where T: IPeak
    {    
        public T this[int index]
        {
            get
            {
                return _curve.Values[index];
            }
        }

        protected readonly SortedList<double, T> _curve;
        
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
        
        public T GetMinPeak(T peak, double percentOfApex = 0.1)
        {
            int index = _curve.IndexOfValue(peak);           

            double threshold = _curve.Values[index].Y * percentOfApex;

            while (index >= 0)
            {                
                T tempPeak = _curve.Values[index];
                if (tempPeak.Y < threshold)
                {
                    return tempPeak;
                }
                index--;
            }
            return _curve.Values[0];
        }

        public double[] GetRetentionTimes()
        {           
            return _curve.Keys.ToArray();
        }

        public double[] GetIntensities()
        {
            return _curve.Values.Select(p => p.Y).ToArray();
        }

        public T GetMaxPeak(T peak, double percentOfApex = 0.1)
        {
            int index = _curve.IndexOfValue(peak);          

            double threshold = _curve.Values[index].Y * percentOfApex;

            while (index < _curve.Count)
            {
                T tempPeak = _curve.Values[index];
                if (tempPeak.Y < threshold)
                {
                    return tempPeak;
                }
                index++;
            }
            return _curve.Values[_curve.Count-1];
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

        public T GetApex(double rt, double timeSpread)
        {
            double minrt = rt - timeSpread;
            double maxrt = rt + timeSpread;

            T apex = default(T);
            double maxvalue = 0;
            foreach (T peak in _curve.Values)
            {
                if (peak.X < minrt)
                    continue;
                if (peak.X > maxrt)
                    break;

                if (peak.Y > maxvalue)
                {
                    maxvalue = peak.Y;
                    apex = peak;
                }
            }
            return apex;
        }      
    }


    public static class Extension
    {
        public static MassRangeChromatogram GetExtractedIonChromatogram(this IList<Tuple<ISpectrum, double>> spectra, DoubleRange range)
        {
            if (range == null)
            {
                throw new ArgumentException("A range must be declared for a m/z range chromatogram");
            }

            int count = spectra.Count;
            double[] times = new double[count];
            double[] intensities = new double[count];

            int i =0;
            foreach (Tuple<ISpectrum, double> data in spectra)
            {
                double intensity = 0;              
                ISpectrum spectrum = data.Item1;               
                spectrum.TryGetIntensities(range, out intensity);
                times[i] = data.Item2;
                intensities[i] = intensity;              
                i++;
            }

            return new MassRangeChromatogram(times, intensities, range);
        }

        //public static Chromatogram GetChromatogram(this IEnumerable<MSDataScan> scans, ChromatogramType type = ChromatogramType.BasePeak, MassRange range = null,  bool zeroFillMissingValues = true)
        //{
        //    Chromatogram chrom;
        //    switch (type)
        //    {
        //        default:
        //            chrom = new Chromatogram(type);
        //            foreach (MSDataScan scan in scans)
        //            {
        //                ChromatographicPeak point = new ChromatographicPeak(scan.RetentionTime, scan.MassSpectrum.BasePeak.Intensity);
        //                chrom.AddPoint(point);
        //            }
        //            break;
        //        case ChromatogramType.MzRange:
        //            //if (range == null)
        //            //{
        //            //    throw new ArgumentException("A range must be declared for a m/z range chromatogram");
        //            //}
        //            //chrom = new MzRangeChromatogram(range, type);
        //            //foreach (MSDataScan scan in scans)
        //            //{
        //            //    double intensity = 0;
        //            //    ISpectrum spectrum = scan.GetReadOnlySpectrum();
        //            //    if (spectrum.TryGetIntensities(range, out intensity))
        //            //    {                    
        //            //        chrom.AddPoint(new ChromatographicPeak(scan.RetentionTime, intensity));
        //            //    }
        //            //    else if (zeroFillMissingValues)
        //            //    {
        //            //        chrom.AddPoint(new ChromatographicPeak(scan.RetentionTime, 0.0));
        //            //    }
        //            //}
        //            break;

        //        case ChromatogramType.TotalIonCurrent:
        //            chrom = new Chromatogram(type);
        //            foreach (MSDataScan scan in scans)
        //            {
        //                ChromatographicPeak point = new ChromatographicPeak(scan.RetentionTime, scan.MassSpectrum.TotalIonCurrent);
        //                chrom.AddPoint(point);
        //            }
        //            break;
        //    }
        //    return chrom;
        //}
    }
}