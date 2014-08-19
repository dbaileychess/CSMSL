// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Fasta.cs) is part of CSMSL.
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