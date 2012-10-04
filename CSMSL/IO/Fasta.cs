///////////////////////////////////////////////////////////////////////////
//  Fasta.cs - Text-based file using the fasta format                     /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSMSL.Proteomics;

namespace CSMSL.IO
{
    public class Fasta
    {
        public static string[] DECOY_IDENTIFIERS = {"DECOY", "REVERSE"};

        private string _description;
        private string _sequence;
        private bool _isDecoy;

        public Fasta(string sequence, string description)
        {
            Sequence = sequence;
            Description = description;
        }

        public Fasta ToDecoy(string preText = "DECOY_", DecoyType Method = DecoyType.Reverse, bool excludeNTerminus = true, bool onlyIfNTerminusIsMethionine = true)
        {
            return new Fasta(GenerateDecoySequence(Sequence, Method, excludeNTerminus, onlyIfNTerminusIsMethionine), preText + Description);
        }

        /**
         * Method to generate decoy sequence according to a given input sequence and other options like
         * decoy type and whether to exclude N-terminus. 
         **/
        private static string GenerateDecoySequence(string sequence, DecoyType decoyDatabaseMethod, bool excludeNTerminus, bool onlyIfNTerminusIsMethionine)
        {
            char[] temp = new char[sequence.Length];
            bool keepNTerminus = excludeNTerminus && (!onlyIfNTerminusIsMethionine || sequence[0] == 'M');

            switch (decoyDatabaseMethod)
            {
                case DecoyType.Reverse:

                    temp = sequence.ToCharArray();
                    if (keepNTerminus)
                        Array.Reverse(temp, 1, temp.Length - 1);
                    else
                        Array.Reverse(temp);

                    break;

                case DecoyType.Shuffle:

                   temp = sequence.ToCharArray();
                    if (keepNTerminus)
                        Shuffle(temp, 1, temp.Length - 1);
                    else
                        Shuffle(temp);

                    break;

                case DecoyType.Random:

                    int index = 0;
                    if (keepNTerminus) 
                        temp[index++] = sequence[0];

                    // Generate Random Characters
                    while (index < sequence.Length)
                    {
                        temp[index++] = AMINO_ACIDS[RANDOM.Next(AMINO_ACIDS.Count)];
                    }

                    break;
            }

            // Create decoy sequence string from temporary char array.
            string decoy_sequence = new string(temp);

            return decoy_sequence;
        }

        private static readonly Random RANDOM = new Random();

        private static readonly List<char> AMINO_ACIDS = new List<char>(new char[] { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' });

        private static void Shuffle(char[] array)
        {
            Shuffle(array, 0, array.Length);
        }

        private static void Shuffle(char[] array, int startIndex, int Length)
        {
            for (int i = startIndex + Length; i > startIndex + 1; i--)
            {
                int k = RANDOM.Next(i - startIndex) + startIndex;
                char temp = array[k];
                array[k] = array[i - 1];
                array[i - 1] = temp;
            }
        }

        public string Description
        {
            get { return _description; }
            set { 
                _description = value;
                foreach(string decoyIdentifier in DECOY_IDENTIFIERS) {
                    if(_description.StartsWith(decoyIdentifier)) {
                        _isDecoy = true;
                        break;
                    }                        
                }
            }
        }

        public string Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        public bool IsDecoy
        {
            get { return _isDecoy; }
            set { _isDecoy = value; }
        }
    } // class Fasta

    public enum DecoyType
    {
        None = 0,
        Reverse = 1,
        Shuffle = 2,
        Random = 3
    }
}