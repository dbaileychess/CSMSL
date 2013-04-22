///////////////////////////////////////////////////////////////////////////
//  Protein.cs - An intact amino acid polymer                             /
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

using CSMSL.IO;
using System.Collections.Generic;

namespace CSMSL.Proteomics
{
    public class Protein : AminoAcidPolymer
    {
        private string _description;
        private bool _isDecoy;

        public Protein(string sequence)
            : this(sequence, string.Empty) { }

        public Protein(string sequence, string description, bool isDecoy = false)
            : base(sequence)
        {           
            _description = description;
            _isDecoy = isDecoy;
        }
     
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public bool IsDecoy
        {
            get { return _isDecoy; }
            set { _isDecoy = value; }
        }

        public Fasta ToFasta()
        {
            return new Fasta(Sequence, _description);
        }
    
    }
}