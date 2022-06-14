// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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