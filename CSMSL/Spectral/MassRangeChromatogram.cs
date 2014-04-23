namespace CSMSL.Spectral
{
    public class MassRangeChromatogram : Chromatogram
    {
        public DoubleRange Range { get; private set; }

        public MassRangeChromatogram(double[] times, double[] intensities, DoubleRange range)
            : base(times, intensities)
        {
            Range = range;
        }

        public MassRangeChromatogram(double[,] timeintensities, DoubleRange range)
            : base(timeintensities)
        {
            Range = range;
        }

        public MassRangeChromatogram(MassRangeChromatogram chromatogram)
            : base(chromatogram)
        {
            Range = chromatogram.Range;
        }

        public MassRangeChromatogram(Chromatogram chromatogram, DoubleRange range)
            : base(chromatogram)
        {
            Range = range;
        }

        public new MassRangeChromatogram Smooth(SmoothingType smoothing, int points)
        {
            Chromatogram chrom = base.Smooth(smoothing, points);
            return new MassRangeChromatogram(chrom, Range);
        }
    }
}
