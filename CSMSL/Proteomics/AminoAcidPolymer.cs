using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public abstract class AminoAcidPolymer : IChemicalFormula, IMass
    {
        private static readonly AminoAcidDictionary AMINO_ACIDS = AminoAcidDictionary.Instance;

        private static readonly Regex _sequenceRegex = new Regex(@"([A-Z])(?:\[([\w\{\}]+)\])?");

        private static readonly Regex _validateSequenceRegex = new Regex("^(" + _sequenceRegex.ToString() + ")+$");
             
        internal static readonly ChemicalModification DefaultNTerm = new ChemicalModification("H");
        internal static readonly ChemicalModification DefaultCTerm = new ChemicalModification("OH");

        private static readonly Dictionary<FragmentType, ChemicalFormula> _fragmentIonCaps = new Dictionary<FragmentType, ChemicalFormula>()
        {
          {FragmentType.a, new ChemicalFormula("C-1H-1O-1")},
          {FragmentType.adot, new ChemicalFormula("C-1O-1")},
          {FragmentType.b, new ChemicalFormula("H-1")},
          {FragmentType.bdot, new ChemicalFormula()},
          {FragmentType.c, new ChemicalFormula("NH2")},
          {FragmentType.cdot, new ChemicalFormula("NH3")},
          {FragmentType.x, new ChemicalFormula("COH-1")},
          {FragmentType.xdot, new ChemicalFormula("CO")},
          {FragmentType.y, new ChemicalFormula("H")},
          {FragmentType.ydot, new ChemicalFormula("H2")},
          {FragmentType.z, new ChemicalFormula("N-1H-2")},
          {FragmentType.zdot, new ChemicalFormula("N-1H-1")},
        };

        internal List<AminoAcidResidue> _residues;
        internal ChemicalModification[] _modifications;

        private ChemicalFormula _chemicalFormula;

        private bool _isDirty;

        private StringBuilder _sequenceSB;
        private StringBuilder _baseSequenceSB;

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
            _baseSequenceSB.Clear();

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
                _baseSequenceSB.Append(aa.Letter);
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

        public string Sequence
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _baseSequenceSB.ToString();
            }
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
            _baseSequenceSB = new StringBuilder(amino_acids);
            _chemicalFormula = new ChemicalFormula();
            ParseSequence(sequence);
        }

        internal AminoAcidPolymer(IEnumerable<AminoAcidResidue> residues, ChemicalModification[] mods)
        {            
            _residues = new List<AminoAcidResidue>(residues);
            _modifications = mods;
            _sequenceSB = new StringBuilder(_residues.Count);
            _baseSequenceSB = new StringBuilder(_residues.Count);
            _chemicalFormula = new ChemicalFormula();
            _isDirty = true;
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

        public Fragment CalculateFragment(FragmentType type, int number)
        {
            if (type == FragmentType.None || number < 1 || number > Length)
            {
                return null;
            }

            ChemicalFormula chemFormula = new ChemicalFormula(_fragmentIonCaps[type]);

            int start = 0;
            int end = number;
            if (type >= FragmentType.x)
            {
                start = Length - number;
                end = Length;
                chemFormula.Add(this.CTerminus);
            }
            else
            {
                chemFormula.Add(this.NTerminus);
            }
           
            for (int i = start; i < end; i++)
            {
                chemFormula.Add(_residues[i]);
                chemFormula.Add(_modifications[i + 1]);
            }
           
            return new Fragment(type, number, chemFormula, this);
        }

        public IEnumerable<Fragment> CalculateFragments(FragmentType types)
        {
            if (types == FragmentType.None)
            {
                yield break;
            }
            foreach (FragmentType type in Enum.GetValues(typeof(FragmentType)))
            {
                if (type == FragmentType.None || type == FragmentType.Internal) continue;
                if ((types & type) == type)
                {
                    // Calculate all the fragments given this peptide's length
                    // TODO make this faster by caching partial chemical formulas
                    for (int i = 1; i < Length; i++)
                    {
                        yield return CalculateFragment(type, i);
                    }
                }
            }         
            yield break;
        }
    }
}