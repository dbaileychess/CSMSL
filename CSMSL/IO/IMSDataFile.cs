using CSMSL.Spectral;
using System.Collections.Generic;

namespace CSMSL.IO
{
    public interface IMSDataFile : IEnumerable<IMSDataScan>
    {
        void Open();
        string Name { get; }
    }

    public interface IMSDataFile<out TSpectrum> : IMSDataFile, IEnumerable<IMSDataScan<TSpectrum>>
        where TSpectrum : ISpectrum
    {
      
    }
   
}
