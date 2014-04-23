namespace CSMSL.Spectral
{
    public interface ISpectrumTime : ISpectrum
    {    
        /// <summary>
        /// The time associated with this spectrum
        /// </summary>
        double Time { get; }
    }
}
