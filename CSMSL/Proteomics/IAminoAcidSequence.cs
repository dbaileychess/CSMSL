using System.Collections.Generic;

namespace CSMSL.Proteomics
{
    public interface IAminoAcidSequence
    {
        string Sequence { get; }
        string GetLeucineSequence();
        int Length { get; }
    }

    public class AminoAcidSequenceComparer : IEqualityComparer<IAminoAcidSequence>
    {
        public bool Equals(IAminoAcidSequence x, IAminoAcidSequence y)
        {
            return x.Sequence.Equals(y.Sequence);
        }

        public int GetHashCode(IAminoAcidSequence obj)
        {
            return obj.Sequence.GetHashCode();
        }
    }

    public class AminoAcidLeucineSequenceComparer : IEqualityComparer<IAminoAcidSequence>
    {

        public int GetHashCode(IAminoAcidSequence obj)
        {
            return obj.GetLeucineSequence().GetHashCode();
        }

        public bool Equals(IAminoAcidSequence x, IAminoAcidSequence y)
        {
            return x.GetLeucineSequence().Equals(y.GetLeucineSequence());
        }
    }
}
