namespace CSMSL.Spectral
{
    public interface ISpectrumTime : ISpectrumTime<MZPeak> { }

    public interface ISpectrumTime<out T> : ISpectrum<T> where T : IPeak
    {    
        /// <summary>
        /// The time associated with this spectrum
        /// </summary>
        double Time { get; }
    }
}
