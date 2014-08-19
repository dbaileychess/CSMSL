// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (IAminoAcidSequence.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Proteomics
{
    public interface IAminoAcidSequence
    {
        /// <summary>
        /// The amino acid sequence
        /// </summary>
        string Sequence { get; }

        /// <summary>
        /// The amino acid sequence with all 'I' replaced with 'L'
        /// </summary>
        /// <returns></returns>
        string GetLeucineSequence();

        /// <summary>
        /// The length of the amino acid sequence
        /// </summary>
        int Length { get; }
    }

    public static class IAminoAcidSequenceExtensions
    {
        public static double GetSequenceCoverageFraction(this IAminoAcidSequence baseSequence, IEnumerable<IAminoAcidSequence> sequences, bool useLeucineSequence = true)
        {
            int[] counts = baseSequence.GetSequenceCoverage(sequences, useLeucineSequence);
            return ((double) counts.Count(x => x > 0))/baseSequence.Length;
        }

        public static int[] GetSequenceCoverage(this IAminoAcidSequence baseSequence, IEnumerable<IAminoAcidSequence> sequences, bool useLeucineSequence = true)
        {
            int[] bits = new int[baseSequence.Length];

            string masterSequence = useLeucineSequence ? baseSequence.GetLeucineSequence() : baseSequence.Sequence;

            foreach (IAminoAcidSequence sequence in sequences)
            {
                string seq = useLeucineSequence ? sequence.GetLeucineSequence() : sequence.Sequence;

                int startIndex = 0;
                while (true)
                {
                    int index = masterSequence.IndexOf(seq, startIndex, StringComparison.InvariantCulture);

                    if (index < 0)
                    {
                        break;
                    }

                    for (int aa = index; aa < index + sequence.Length; aa++)
                    {
                        bits[aa]++;
                    }

                    startIndex = index + 1;
                }
            }
            return bits;
        }
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