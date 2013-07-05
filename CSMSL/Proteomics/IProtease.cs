using System.Collections.ObjectModel;

namespace CSMSL.Proteomics
{
    /// <summary>
    /// A proteolyic enzyme that cuts amino acids at specific residues.
    /// </summary>
    public interface IProtease
    {
        /// <summary>
        /// Finds the indicies of where this protease would cut in
        /// the given amino acid sequence
        /// </summary>
        /// <param name="aminoAcidSequence">The Amino Acid Polymer to cut</param>
        /// <returns>A set of the 1-based indicies to cut at</returns>
        ReadOnlyCollection<int> GetDigestionSites(string aminoAcidSequence);

        ReadOnlyCollection<int> GetDigestionSites(IAminoAcidSequence aminoAcidSequence);
    }
}
