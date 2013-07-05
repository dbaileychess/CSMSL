using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public interface IAminoAcid : IChemicalFormula
    {
        char Letter { get; }
        string Symbol { get; }
        ModificationSites Site { get; }
    }
}
