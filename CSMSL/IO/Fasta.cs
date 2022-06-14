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

namespace CSMSL.IO
{
    public class Fasta : IEquatable<Fasta>
    {
        public static string[] DecoyIdentifiers = {"DECOY", "REVERSE"};

        private string _description;

        public Fasta(string sequence, string description)
        {
            Sequence = sequence;
            Description = description;
        }

        public override bool Equals(object obj)
        {
            var a = obj as Fasta;
            return a != null && Equals(a);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode();
        }

        /// <summary>
        /// Generate a decoy version of the given fasta
        /// </summary>
        /// <param name="preText">The pretext to add to the description to indicated this is a decoy fasta</param>
        /// <param name="method">The decoy generation type</param>
        /// <param name="excludeNTerminus">Exclude the n-terminal amino acid</param>
        /// <param name="onlyIfNTerminusIsMethionine">Exclude the n-terminal amino aicd only if it is a Methionine</param>
        /// <returns>The generated decoy fasta</returns>
        public Fasta ToDecoy(string preText = "DECOY_", DecoyDatabaseMethod method = DecoyDatabaseMethod.Reverse, bool excludeNTerminus = true, bool onlyIfNTerminusIsMethionine = true)
        {
            return new Fasta(GenerateDecoySequence(Sequence, method, excludeNTerminus, onlyIfNTerminusIsMethionine), preText + Description);
        }

        /**
         * Method to generate decoy sequence according to a given input sequence and other options like
         * decoy type and whether to exclude N-terminus.
         **/

        private static string GenerateDecoySequence(string sequence, DecoyDatabaseMethod decoyType, bool excludeNTerminus, bool onlyIfNTerminusIsMethionine)
        {
            char[] temp = new char[sequence.Length];
            bool keepNTerminus = excludeNTerminus && (!onlyIfNTerminusIsMethionine || sequence[0] == 'M');

            switch (decoyType)
            {
                case DecoyDatabaseMethod.Reverse:

                    temp = sequence.ToCharArray();
                    if (keepNTerminus)
                        Array.Reverse(temp, 1, temp.Length - 1);
                    else
                        Array.Reverse(temp);

                    break;

                case DecoyDatabaseMethod.Shuffle:

                    temp = sequence.ToCharArray();
                    if (keepNTerminus)
                        Shuffle(temp, 1, temp.Length - 1);
                    else
                        Shuffle(temp);

                    break;

                case DecoyDatabaseMethod.Random:

                    int index = 0;
                    if (keepNTerminus)
                        temp[index++] = sequence[0];

                    // Generate Random Characters
                    while (index < sequence.Length)
                    {
                        temp[index++] = AminoAcids[Random.Next(AminoAcids.Count)];
                    }

                    break;

                case DecoyDatabaseMethod.None:
                    break;
            }

            // Create decoy sequence string from temporary char array.
            return new string(temp);
        }

        private static readonly Random Random = new Random();

        private static readonly List<char> AminoAcids = new List<char>(new[] {'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y'});

        private static void Shuffle(char[] array)
        {
            Shuffle(array, 0, array.Length);
        }

        private static void Shuffle(char[] array, int startIndex, int length)
        {
            for (int i = startIndex + length; i > startIndex + 1; i--)
            {
                int k = Random.Next(i - startIndex) + startIndex;
                char temp = array[k];
                array[k] = array[i - 1];
                array[i - 1] = temp;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                foreach (string decoyIdentifier in DecoyIdentifiers)
                {
                    if (_description.StartsWith(decoyIdentifier))
                    {
                        IsDecoy = true;
                        break;
                    }
                }
            }
        }

        public string Sequence { get; set; }

        public bool IsDecoy { get; set; }

        public bool Equals(Fasta other)
        {
            return Sequence.Equals(other.Sequence);
        }
    }

    public enum DecoyDatabaseMethod
    {
        None,
        Reverse,
        Shuffle,
        Random
    }
}