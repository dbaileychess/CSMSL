using System.Collections.Generic;

namespace CSMSL.Proteomics
{
    public interface IProtease
    {
        SortedSet<int> GetDigestionSites(string aminoAcidSequence);
    }
}
