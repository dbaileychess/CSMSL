using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public abstract class AminoAcidPolymer : IChemicalFormula, IMass
    {
        internal static readonly ChemicalModification DefaultCTerm = new ChemicalModification("OH");
        internal static readonly ChemicalModification DefaultNTerm = new ChemicalModification("H");

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

        private static readonly Regex _sequenceRegex = new Regex(@"([A-Z])(?:\[([\w\{\}]+)\])?", RegexOptions.Compiled);
        private static readonly Regex _validateSequenceRegex = new Regex("^(" + _sequenceRegex.ToString() + ")+$", RegexOptions.Compiled);
        private static readonly AminoAcidDictionary AMINO_ACIDS = AminoAcidDictionary.Instance;
        internal ChemicalModification[] _modifications;
        internal List<AminoAcidResidue> _residues;
        private StringBuilder _baseSequenceSB;
        private ChemicalFormula _chemicalFormula;

        private bool _isDirty;
        private bool _isSequenceDirty;

        private StringBuilder _sequenceSB;

        public AminoAcidPolymer(string sequence)
            : this(sequence, DefaultNTerm, DefaultCTerm) { }

        public AminoAcidPolymer(string sequence, ChemicalModification nTerm, ChemicalModification cTerm)
        {
            _isSequenceDirty = true;
            _residues = new List<AminoAcidResidue>(sequence.Length);
            _sequenceSB = new StringBuilder(sequence.Length);
            _baseSequenceSB = new StringBuilder(sequence.Length);
            _modifications = new ChemicalModification[sequence.Length + 2]; // +2 for the n and c term
            _modifications[0] = nTerm;
            _chemicalFormula = new ChemicalFormula();
            ParseSequence(sequence);
            _modifications[_modifications.Length - 1] = cTerm;
        }

        internal AminoAcidPolymer(IEnumerable<AminoAcidResidue> residues, ChemicalModification[] mods)
        {
            _residues = new List<AminoAcidResidue>(residues);
            _modifications = mods;
            _sequenceSB = new StringBuilder(_residues.Count);
            _baseSequenceSB = new StringBuilder(_residues.Count);
            _chemicalFormula = new ChemicalFormula();
            _isDirty = true;
            _isSequenceDirty = true;
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

        public int Length
        {
            get { return _residues.Count; }
        }

        public Mass Mass
        {
            get { return ChemicalFormula.Mass; }
        }

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

        public string Sequence
        {
            get
            {
                if (_isSequenceDirty)
                {
                    CleanUp();
                }
                return _baseSequenceSB.ToString();
            }
        }

        public AminoAcidResidue this[int index]
        {
            get
            {
                return _residues[index - 1];
            }
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
                if (_modifications[i + 1] != null)
                {
                    chemFormula.Add(_modifications[i + 1]);
                }
            }

            return new Fragment(type, number, chemFormula, this);
        }

        public IEnumerable<Fragment> CalculateFragments(FragmentType types)
        {
            return CalculateFragments(types, 1, Length-1);
        }

        public IEnumerable<Fragment> CalculateFragments(FragmentType types, int min, int max)
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
                    max = Math.Min(Length-1, max);
                    for (int i = min; i <= max; i++)
                    {
                        yield return CalculateFragment(type, i);
                    }
                }
            }
            yield break;
        }

        public void SetModification(ChemicalModification mod, Terminus terminus)
        {
            if ((terminus & Terminus.N) == Terminus.N)
            {
                _modifications[0] = mod;
                _isDirty = true;
            }
            if ((terminus & Terminus.C) == Terminus.C)
            {
                _modifications[_modifications.Length - 1] = mod;
                _isDirty = true;
            }
        }

        public int SetModification(ChemicalModification mod, char letter)
        {
            AminoAcidResidue residue = null;
            if (AMINO_ACIDS._residuesLetter.TryGetValue(letter, out residue))
            {
                return SetModification(mod, residue);
            }
            else
            {
                return 0;
            }
        }

        public int SetModification(ChemicalModification mod, AminoAcidResidue residue)
        {
            int count = 0;
            for (int i = 0; i < _residues.Count; i++)
            {
                if (residue.Equals(_residues[i]))
                {
                    _modifications[i + 1] = mod;
                    _isDirty = true;
                    count++;
                }
            }
            return count;
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

        public override string ToString()
        {
            if (_isDirty)
            {
                CleanUp();
            }
            return _sequenceSB.ToString();
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

            _isSequenceDirty = false;
            _isDirty = false;
        }

        private void ParseSequence(string sequence)
        {
            AminoAcidResidue residue = null;
            bool inMod = false;
            int startcount = _residues.Count;
            StringBuilder modSB = new StringBuilder(10);
            _baseSequenceSB.Clear();
            foreach (char letter in sequence)
            {
                if (inMod)
                {
                    if (letter == ']')
                    {
                        inMod = false;
                        _modifications[_residues.Count] = new ChemicalModification(modSB.ToString());
                        modSB.Clear();
                    }
                    else
                    {
                        modSB.Append(letter);
                    }
                }
                else if (AMINO_ACIDS._residuesLetter.TryGetValue(letter, out residue))
                {
                    _residues.Add(residue);
                    _baseSequenceSB.Append(letter);
                }
                else
                {
                    if (letter == '[')
                    {
                        inMod = true;
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Amino Acid Letter {0} does not exist in the Amino Acid Dictionary", letter));
                    }
                }
            }
            _isSequenceDirty = false;
            int endCount = _residues.Count;
            _isDirty = endCount != startcount; // set the dirty flag once, instead of everytime you add a residue
            Array.Resize(ref _modifications, endCount + 2);
        }
    }
}