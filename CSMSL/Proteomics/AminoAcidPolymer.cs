using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public abstract class AminoAcidPolymer : IChemicalFormula, IMass
    {
        private static readonly AminoAcidDictionary AMINO_ACIDS = AminoAcidDictionary.Instance;

        private static readonly Regex _sequenceRegex = new Regex(@"([A-Z])(?:\[([\w\{\}]+)\])?");

        private static readonly Regex _validateSequenceRegex = new Regex("^(" + _sequenceRegex.ToString() + ")+$");

        private static readonly ChemicalModification DefaultNTerm = new ChemicalModification("H");
        private static readonly ChemicalModification DefaultCTerm = new ChemicalModification("OH");

        internal List<AminoAcidResidue> _residues;
        internal ChemicalModification[] _modifications;

        private ChemicalFormula _chemicalFormula;

        private bool _isDirty;

        private StringBuilder _sequenceSB;

        public ChemicalModification NTerminus
        {
            get
            {
                return _modifications[0];
            }
            set
            {
                _modifications[0] = value;
                _isDirty = true;
            }
        }

        public ChemicalModification CTerminus
        {
            get
            {
                return _modifications[_modifications.Length - 1];
            }
            set
            {
                _modifications[_modifications.Length - 1] = value;
                _isDirty = true;
            }
        }
                
        private void CleanUp()
        {            
            _chemicalFormula.Clear();
            _sequenceSB.Clear();

            ChemicalModification mod = null;

            // Handle N-Terminus
            if ((mod = _modifications[0]) != null)
            {
                _chemicalFormula.Add(mod);
                if (mod != DefaultNTerm)
                {
                    _sequenceSB.Append('[');
                    _sequenceSB.Append(mod);
                    _sequenceSB.Append("]-");
                }
            }

            // Handle Amino Acid Residues
            for (int i = 0; i < _residues.Count; i++)
            {
                AminoAcidResidue aa = _residues[i];
                _chemicalFormula.Add(aa);
                _sequenceSB.Append(aa.Letter);
                if ((mod = _modifications[i + 1]) != null)  // Mods are 1-based for the N and C-terminus             
                {           
                    _chemicalFormula.Add(mod);
                    _sequenceSB.Append('[');
                    _sequenceSB.Append(mod);
                    _sequenceSB.Append(']');
                }
            }

            // Handle C-Terminus
            if ((mod = _modifications[_modifications.Length - 1]) != null)
            {
                _chemicalFormula.Add(mod);
                if (mod != DefaultCTerm)
                {
                    _sequenceSB.Append("-[");
                    _sequenceSB.Append(mod);
                    _sequenceSB.Append(']');
                }
            }

            _isDirty = false;
        }

        public override string ToString()
        {
            if (_isDirty)
            {
                CleanUp();
            }
            return _sequenceSB.ToString();
        }

        public ChemicalFormula ChemicalFormula
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _chemicalFormula;
            }
        }

        public Mass Mass
        {
            get { return ChemicalFormula.Mass; }
        }

        public AminoAcidResidue this[int index]
        {
            get
            {
                return _residues[index];
            }
        }

        public int Length
        {
            get { return _residues.Count; }
        }

        public AminoAcidPolymer(string sequence)
            : this(sequence, DefaultNTerm, DefaultCTerm) { }

        public AminoAcidPolymer(string sequence, ChemicalModification nTerm, ChemicalModification cTerm)
        {
            int amino_acids = _sequenceRegex.Matches(sequence).Count;
            _residues = new List<AminoAcidResidue>(amino_acids);
            _modifications = new ChemicalModification[amino_acids + 2]; // +2 for the n and c term
            _modifications[0] = nTerm;
            _modifications[amino_acids + 1] = cTerm;
            _sequenceSB = new StringBuilder(sequence.Length);
            _chemicalFormula = new ChemicalFormula();
            ParseSequence(sequence);
        }

        private void ParseSequence(string sequence)
        {
            AminoAcidResidue residue = null;
            int residue_position = 1;
            foreach (Match match in _sequenceRegex.Matches(sequence))
            {
                char letter = char.Parse(match.Groups[1].Value);             // Group 1: Amino Acid Letter
                if (AMINO_ACIDS.TryGetResidue(letter, out residue))
                {
                    _residues.Add(residue);
                    _isDirty = true;
                    if (match.Groups[2].Success)  // Group 1: Chemical or Text Modification
                    {
                        ChemicalModification chemicalModification = new ChemicalModification(match.Groups[2].Value);
                        _modifications[residue_position] = chemicalModification;
                    }
                    residue_position++;
                }
                else
                {
                    throw new ArgumentException(string.Format("Amino Acid Letter {0} does not exist in the Amino Acid Dictionary", letter));
                }
            }
        }

        public void SetModification(ChemicalModification mod, int residueNumber)
        {
            if (residueNumber > Length || residueNumber < 1)
            {
                throw new ArgumentNullException("Residue number not correct");
            }
            _modifications[residueNumber] = mod;
            _isDirty = true;
        }


    }
}