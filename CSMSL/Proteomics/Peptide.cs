using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
