///////////////////////////////////////////////////////////////////////////
//  AminoAcidPolymer.cs - A linear sequence of amino acid residues        /
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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public abstract class AminoAcidPolymer : IChemicalFormula, IMass
    {
        public static readonly IChemicalFormula DefaultCTerminusModification = new ChemicalFormula("OH");
        public static readonly IChemicalFormula DefaultNTerminusModification = new ChemicalFormula("H");

        private static readonly Dictionary<FragmentType, IChemicalFormula> _fragmentIonCaps = new Dictionary<FragmentType, IChemicalFormula>()
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

        internal IChemicalFormula[] _modifications;
        internal IAminoAcid[] _aminoAcids;

        internal List<AminoAcid> _residues;
        private ChemicalFormula _chemicalFormula;

        private bool _isDirty;
        private bool _isSequenceDirty;

        private StringBuilder _sequenceSB;

        public AminoAcidPolymer(string sequence)
            : this(sequence, DefaultNTerminusModification, DefaultCTerminusModification) { }

        public AminoAcidPolymer(string sequence, IChemicalFormula nTerm, IChemicalFormula cTerm)
        {
            int length = sequence.Length;
            _aminoAcids = new IAminoAcid[length];
            _modifications = new IChemicalFormula[length + 2]; // +2 for the n and c term
            NTerminus = nTerm;
            ParseSequence(sequence);
            CTerminus = cTerm;
        }

        /// <summary>
        /// Clone a complete copy of another amino acid polymer
        /// </summary>
        /// <param name="aminoAcidPolymer">The amino acid polymer to clone</param>
        public AminoAcidPolymer(AminoAcidPolymer aminoAcidPolymer)
            : this(aminoAcidPolymer, 0, aminoAcidPolymer.Length) { }      

        /// <summary>
        /// Clone a part of another amino acid polymer
        /// </summary>
        /// <param name="aminoAcidPolymer">The amino acid polymer to clone</param>
        /// <param name="firstResidue">The first residue to start cloning from</param>
        /// <param name="length">The number of amino acids to clone</param>
        public AminoAcidPolymer(AminoAcidPolymer aminoAcidPolymer, int firstResidue, int length)
        {
            if (length + firstResidue > aminoAcidPolymer.Length)
                length = aminoAcidPolymer.Length - firstResidue;
            _aminoAcids = new IAminoAcid[length];
            _modifications = new IChemicalFormula[length + 2];
            Array.Copy(aminoAcidPolymer._aminoAcids, firstResidue, _aminoAcids, 0, length);
            Array.Copy(aminoAcidPolymer._modifications, firstResidue + 1, _modifications, 1, length);
            NTerminus = (firstResidue == 0) ? aminoAcidPolymer.NTerminus : DefaultNTerminusModification;
            CTerminus = (length + firstResidue == aminoAcidPolymer.Length) ? aminoAcidPolymer.CTerminus : DefaultCTerminusModification;
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

        public IChemicalFormula CTerminus
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

        /// <summary>
        /// The number of amino acids in this amino acid polymer
        /// </summary>
        public int Length
        {
            get { return _aminoAcids.Length; }
        }

        public Mass Mass
        {
            get { return ChemicalFormula.Mass; }
        }

        public IChemicalFormula NTerminus
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

        internal string _sequence;

        public string Sequence
        {
            get
            {
                if (_isSequenceDirty)
                {
                    CleanUp();
                }
                return _sequence;
            }
        }
            
        public string SequenceWithModifications
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _sequenceSB.ToString();
            }
        }

        public int CountResidues(char residueChar)
        {           
            int count = 0;
            foreach (IAminoAcid aar in _aminoAcids)
            {
                if (aar.Letter.Equals(residueChar))
                    count++;
            }
            return count;
        }

        public IAminoAcid this[int index]
        {
            get
            {
                return _aminoAcids[index - 1];
            }
        }

        public Fragment CalculateFragment(FragmentType type, int number)
        {
            if (type == FragmentType.None || number < 1 || number > Length)
            {
                return null;
            }

            ChemicalFormula chemFormula = new ChemicalFormula(_fragmentIonCaps[type]);
            IChemicalFormula mod = null;
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
                chemFormula.Add(_aminoAcids[i]);
                if ((mod = _modifications[i + 1]) != null)
                {
                    chemFormula.Add(mod);
                }
            }

            return new Fragment(type, number, chemFormula, this);
        }

        public bool ContainsModification(IChemicalFormula modification)
        {
            foreach (IChemicalFormula mod in _modifications)
            {
                if (modification.Equals(mod))
                    return true;
            }
            return false;
        }

        public IEnumerable<Fragment> CalculateFragments(FragmentType types)
        {
            return CalculateFragments(types, 1, Length - 1);
        }

        public IEnumerable<Fragment> CalculateFragments(FragmentType types, int min, int max)
        {
            if (types == FragmentType.None)
            {
                yield break;
            }
            max = Math.Min(Length - 1, max);
            min = Math.Max(1, min);
            foreach (FragmentType type in Enum.GetValues(typeof(FragmentType)))
            {
                if (type == FragmentType.None || type == FragmentType.Internal) continue;
                if ((types & type) == type)
                {
                    // Calculate all the fragments given this peptide's length
                    // TODO make this faster by caching partial chemical formulas
                    for (int i = min; i <= max; i++)
                    {
                        yield return CalculateFragment(type, i);
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Sets the modification at the terminus of this amino acid polymer
        /// </summary>
        /// <param name="mod">The mod to set</param>
        /// <param name="terminus">The termini to set the mod at</param>
        public void SetModification(IChemicalFormula mod, Terminus terminus)
        {
            if ((terminus & Terminus.N) == Terminus.N)
            {
                NTerminus = mod;               
            }
            if ((terminus & Terminus.C) == Terminus.C)
            {
                CTerminus = mod;             
            }
        }

        /// <summary>
        /// Clears the modification set at the terminus of this amino acid polymer back
        /// to the default C or N modifications.
        /// </summary>
        /// <param name="terminus"></param>
        public void ClearModification(Terminus terminus)
        {
            if ((terminus & Terminus.N) == Terminus.N)
            {
                NTerminus = DefaultNTerminusModification;
            }
            if ((terminus & Terminus.C) == Terminus.C)
            {
                CTerminus = DefaultCTerminusModification;
            }
        }
        
        public int SetModification(IChemicalFormula mod, char letter)
        {
            int count = 0;
            for (int i = 0; i < _aminoAcids.Length; i++)
            {
                if (letter.Equals(_aminoAcids[i].Letter))
                {
                    _modifications[i + 1] = mod;
                    _isDirty = true;
                    count++;
                }
            }
            return count;         
        }

        public int SetModification(IChemicalFormula mod, IAminoAcid residue)
        {
            int count = 0;
            for (int i = 0; i < _aminoAcids.Length; i++)
            {
                if (residue.Equals(_aminoAcids[i]))
                {
                    _modifications[i + 1] = mod;
                    _isDirty = true;
                    count++;
                }
            }
            return count;        
        }

        public void SetModification(IChemicalFormula mod, int residueNumber)
        {
            if (residueNumber > Length || residueNumber < 1)
            {
                throw new IndexOutOfRangeException(string.Format("Residue number not in the correct range: [{0}-{1}] you specified: {2}", 1, Length, residueNumber));
            }
            _modifications[residueNumber] = mod;
            _isDirty = true;
        }

        public override string ToString()
        {
            return SequenceWithModifications;
        }

        private void CleanUp()
        {
            if (_chemicalFormula == null)
            {
                _chemicalFormula = new ChemicalFormula();
            }
            else
            {
                _chemicalFormula.Clear();
            }

            if (_sequenceSB == null)
            {
                _sequenceSB = new StringBuilder(_aminoAcids.Length + 2);
            }
            else
            {
                _sequenceSB.Clear();
            }

            StringBuilder baseSeqSB = new StringBuilder();
            IChemicalFormula mod = null;
                      
            // Handle N-Terminus
            if ((mod = _modifications[0]) != null)
            {
                _chemicalFormula.Add(mod);
                if (!mod.Equals(DefaultNTerminusModification))
                {
                    _sequenceSB.Append('[');
                    _sequenceSB.Append(mod);
                    _sequenceSB.Append("]-");
                }
            }

            // Handle Amino Acid Residues
            for (int i = 0; i < _aminoAcids.Length; i++)
            {
                IAminoAcid aa = _aminoAcids[i];
                _chemicalFormula.Add(aa);
                _sequenceSB.Append(aa.Letter);
                baseSeqSB.Append(aa.Letter);
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
                if (!mod.Equals(DefaultCTerminusModification))
                {
                    _sequenceSB.Append("-[");
                    _sequenceSB.Append(mod);
                    _sequenceSB.Append(']');
                }
            }

            _sequence = baseSeqSB.ToString();
            _isDirty = false;
        }
        
        public override int GetHashCode()
        {
            int hCode = 748;
            foreach (IAminoAcid aa in _aminoAcids)
            {
                hCode ^= aa.GetHashCode();
                hCode = hCode >> 7;
            }
            foreach (IChemicalFormula mod in _modifications)
            {
                if (mod != null)
                {
                    hCode ^= mod.GetHashCode();
                    hCode = hCode << 3;
                }
            }
            return hCode;
        }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is AminoAcidPolymer)) {
                return false;
            }
            return this.Equals((AminoAcidPolymer)obj);
        }

        public virtual bool Equals(AminoAcidPolymer other)
        {
            if (Object.ReferenceEquals(this, other)) return true;
            if (this.Length != other.Length) return false;         
            if (this.NTerminus != other.NTerminus || this.CTerminus != other.CTerminus)
                return false;
            
            int length = this.Length;
            for (int i = 1; i <= length; i++)
            {
                IChemicalFormula thisMod = this._modifications[i];
                IChemicalFormula otherMod = this._modifications[i];
                if (thisMod == null)
                {

                }
                if (!this._aminoAcids[i-1].Equals(other._aminoAcids[i-1]) || this._modifications[i] != other._modifications[i])
                    return false;
            }
            return true;
        }

        private void ParseSequence(string sequence)
        {
            AminoAcid residue = null;
            bool inMod = false;
            int index = 0;
            StringBuilder modSB = new StringBuilder(10);
            StringBuilder baseSeqSB = new StringBuilder(sequence.Length);
            foreach (char letter in sequence)
            {
                if (inMod)
                {
                    if (letter == ']')
                    {
                        inMod = false;
                        string modString = modSB.ToString();
                        modSB.Clear();

                        IChemicalFormula modification = null;
                        switch (modString)
                        {
                            case "#": // Make the modification unverisally heavy (all C12 and N14s are promoted to C13 and N15s)
                                modification = ChemicalModification.MakeHeavy(_aminoAcids[index - 1]);
                                break;
                            default:
                                modification = new ChemicalModification(modString);
                                break;
                        }
                        _modifications[index] = modification;
                    }
                    else
                    {
                        modSB.Append(letter);
                    }
                }
                else if (AminoAcid.TryGetResidue(letter, out residue))
                {
                    _aminoAcids[index++] = residue;
                    //_residues.Add(residue);
                    baseSeqSB.Append(letter);
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

            if (inMod)
            {
                throw new ArgumentException("Couldn't find the closing ] for a modification in this sequence");
            }

            _sequence = baseSeqSB.ToString();
            Array.Resize(ref _aminoAcids, index);
            Array.Resize(ref _modifications, index + 2);
            _isDirty = true;             
        }

        #region Statics

        public static IEnumerable<string> Digest(string sequence, Protease protease, int minMissedCleavages = 0, int maxMissedCleavages = 0, bool assumeInitiatorMethionineCleaved = true, int minLength = 1, int maxLength = int.MaxValue)
        {
            return Digest(sequence, new Protease[] { protease }, minMissedCleavages, maxMissedCleavages, assumeInitiatorMethionineCleaved, minLength, maxLength);
        }

        public static IEnumerable<string> Digest(string sequence, IEnumerable<Protease> proteases, int minMissedCleavages = 0, int maxMissedCleavages = 0, bool assumeInitiatorMethionineCleaved = true, int minLength = 1, int maxLength = int.MaxValue)
        {
            int length = sequence.Length;
            HashSet<int> locations = new HashSet<int>() { -1 };       
            foreach (Protease protease in proteases)
            {
                locations.UnionWith(protease.GetDigestionSites(sequence));
            }
            locations.Add(length - 1);

            List<int> indices = new List<int>(locations);
            indices.Sort();

            bool startsWithM = sequence[0].Equals('M') && !assumeInitiatorMethionineCleaved;
            for (int missed_cleavages = minMissedCleavages; missed_cleavages <= maxMissedCleavages; missed_cleavages++)
            {
                for (int i = 0; i < indices.Count - missed_cleavages - 1; i++)
                {
                    int len = indices[i + missed_cleavages + 1] - indices[i];
                    if (len >= minLength && len <= maxLength)
                    {
                        int begin = indices[i] + 1;
                        int end = begin + len + 1;
                        yield return sequence.Substring(begin, len);

                        if (startsWithM && begin == 0 && len - 1 >= minLength)
                        {
                            yield return sequence.Substring(1, len - 1);
                        }
                    }
                }
            }
            yield break;
        }

        public static double GetMass(string sequence)
        {
            double mass = Constants.WATER;
            AminoAcid residue = null;
            foreach (char letter in sequence)
            {
                if (AminoAcid.TryGetResidue(letter, out residue))
                {
                    mass += residue.Mass.Monoisotopic;
                }
            }
            return mass;
        }

        #endregion

    }
}