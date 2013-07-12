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


namespace CSMSL.Proteomics
{
    public class Peptide : AminoAcidPolymer
    {
        public int StartResidue { get; set; }

        public int EndResidue { get; set; }

        public AminoAcidPolymer Parent { get; set; }

        public Peptide()
        {
            Parent = null;
            StartResidue = 0;
            EndResidue = 0;
        }

        public Peptide(AminoAcidPolymer aminoAcidPolymer, bool includeModifications = true)
            : base(aminoAcidPolymer, includeModifications)
        {
            Parent = aminoAcidPolymer;
            StartResidue = 0;
            EndResidue = StartResidue + Length - 1;
        }

        public Peptide(AminoAcidPolymer aminoAcidPolymer, int firstResidue, int length, bool includeModifications = true)
            : base(aminoAcidPolymer, firstResidue, length, includeModifications)
        {
            Parent = aminoAcidPolymer;
            StartResidue = firstResidue;
            EndResidue = firstResidue + length - 1;
        }

        public Peptide(AminoAcidPolymer aminoAcidPolymer)
            : this(aminoAcidPolymer, 0, aminoAcidPolymer.Length) { }
                 
        public Peptide(string sequence)
            : this(sequence, null, 0) { }

        public Peptide(string sequence, Protein parent)
            : this(sequence, parent, 0) { }

        public Peptide(string sequence, Protein parent, int startResidue)
            : base(sequence)
        {
            Parent = parent;
            StartResidue = startResidue;
            EndResidue = startResidue + Length - 1;
        }

        public Peptide GetSubPeptide(int firstResidue, int length)
        {
            return new Peptide(this, firstResidue, length);
        }

        public new bool Equals(AminoAcidPolymer other)
        {
            return base.Equals(other);
        }
    }
}