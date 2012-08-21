///////////////////////////////////////////////////////////////////////////
//  Peptide.cs - An amino acid residue that is the child of a protein     /
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

using System.Collections.Generic;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Peptide : AminoAcidPolymer
    {
        private int _startResidue;

        public int StartResidue
        {
            get { return _startResidue; }
            set { _startResidue = value; }
        }

        private int _endResidue;

        public int EndResidue
        {
            get { return _endResidue; }
            set { _endResidue = value; }
        }

        private Protein _parent;

        public Protein Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Peptide(string sequence)
            : this(sequence, null, 0) { }

        public Peptide(string sequence, Protein parent)
            : this(sequence, parent, 0) { }

        public Peptide(string sequence, Protein parent, int startResidue)
            : base(sequence)
        {
            _parent = parent;
            _startResidue = startResidue;
            _endResidue = startResidue + this.Length;
        }

        internal Peptide(IEnumerable<AminoAcidResidue> residues, ChemicalModification[] mods, Protein parent, int startResidue)
            : base(residues, mods)
        {
            _parent = parent;
            _startResidue = startResidue;
            _endResidue = startResidue + this.Length;
        }
    }
}