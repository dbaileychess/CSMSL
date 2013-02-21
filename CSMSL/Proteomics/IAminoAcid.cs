using System;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public interface IAminoAcid : IChemicalFormula, IMass
    {
        char Letter { get; }
        string Symbol { get; }
        ModificationSites Site { get; }
    }
}
