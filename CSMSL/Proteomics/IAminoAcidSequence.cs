namespace CSMSL.Proteomics
{
    public interface IAminoAcidSequence
    {
        string Sequence { get; }
        string GetLeucineSequence();
        int Length { get; }
    }
}
