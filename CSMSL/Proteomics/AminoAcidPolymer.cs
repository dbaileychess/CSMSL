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

    public abstract class AminoAcidPolymer : IEquatable<AminoAcidPolymer>, IMass
    {
        /// <summary>
        /// The default chemical formula of the C terminus
        /// </summary>
        public static readonly ChemicalFormula DefaultCTerminus = new ChemicalFormula("OH");

        /// <summary>
        /// The default chemical formula of the N terminus
        /// </summary>
        public static readonly ChemicalFormula DefaultNTerminus = new ChemicalFormula("H");


        private static readonly Regex _sequenceRegex = new Regex(@"([A-Z])(?:\[([\w\{\}]+)\])?", RegexOptions.Compiled);
        private static readonly Regex _validateSequenceRegex = new Regex("^(" + _sequenceRegex.ToString() + ")+$", RegexOptions.Compiled);
        
        private IChemicalFormula _cTerminus;
        private IChemicalFormula _nTerminus;
        private IMass[] _modifications;
        private IAminoAcid[] _aminoAcids;       
        private ChemicalFormula _baseChemicalFormula;
        private bool _isDirty;
        private bool _isSequenceDirty;
        private StringBuilder _sequenceSB;
        private int _length;
        private Mass _mass;

        #region Constructors

        public AminoAcidPolymer()
        {
            _aminoAcids = new IAminoAcid[0];
            _modifications = new IMass[2];
            NTerminus = DefaultNTerminus;
            CTerminus = DefaultCTerminus;
            _isDirty = true;
            _isSequenceDirty = true;
            _length = 0;
        }

        public AminoAcidPolymer(string sequence)
            : this(sequence, DefaultNTerminus, DefaultCTerminus) { }

        public AminoAcidPolymer(string sequence, IChemicalFormula nTerm, IChemicalFormula cTerm)
        {
            int length = sequence.Length;
            _aminoAcids = new IAminoAcid[length];
            _modifications = new IMass[length + 2]; // +2 for the n and c term         
            NTerminus = nTerm;
            CTerminus = cTerm;
            ParseSequence(sequence);      
        }

        public AminoAcidPolymer(AminoAcidPolymer aminoAcidPolymer, bool includeModifications = true)
            : this(aminoAcidPolymer, 0, aminoAcidPolymer.Length, includeModifications) { }      

        public AminoAcidPolymer(AminoAcidPolymer aminoAcidPolymer, int firstResidue, int length, bool includeModifications = true)
        {
            if (firstResidue < 0 || firstResidue > aminoAcidPolymer.Length)
                throw new IndexOutOfRangeException(string.Format("The first residue index is outside the valid range [{0}-{1}]", 0, aminoAcidPolymer.Length));

            if (length + firstResidue > aminoAcidPolymer.Length)
                length = aminoAcidPolymer.Length - firstResidue;
            _length = length;
            _aminoAcids = new IAminoAcid[length];
            _modifications = new IMass[length + 2];
            Array.Copy(aminoAcidPolymer._aminoAcids, firstResidue, _aminoAcids, 0, length);
            if (includeModifications)
            {
                Array.Copy(aminoAcidPolymer._modifications, firstResidue + 1, _modifications, 1, length);
                NTerminus = (firstResidue == 0) ? aminoAcidPolymer.NTerminus : DefaultNTerminus;
                CTerminus = (length + firstResidue == aminoAcidPolymer.Length) ? aminoAcidPolymer.CTerminus : DefaultCTerminus;
            }
            else
            {
                NTerminus = DefaultNTerminus;
                CTerminus = DefaultCTerminus;
            }
            _isDirty = true;
            _isSequenceDirty = true;
        }

        #endregion


        
        /// <summary>
        /// Gets or sets the modification of the C terminus on this amino acid polymer
        /// </summary>        
        public IMass CTerminusModification
        {
            get
            {
                return _modifications[_length + 1];
            }
            set
            {
                _modifications[_length + 1] = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the modification of the C terminus on this amino acid polymer
        /// </summary>        
        public IMass NTerminusModification
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
        
        /// <summary>
        /// Gets or sets the C terminus of this amino acid polymer
        /// </summary>        
        public IChemicalFormula CTerminus
        {
            get
            {               
                return _cTerminus;              
            }
            set
            {
                _cTerminus = value;               
                _isDirty = true;
            }
        }
        
        /// <summary>
        /// Gets or sets the N terminus of this amino acid polymer
        /// </summary>
        public IChemicalFormula NTerminus
        {
            get
            {
                return _nTerminus;
            }
            set
            {
                _nTerminus = value;
                _isDirty = true;
            }
        }
        
        /// <summary>
        /// Gets the number of amino acids in this amino acid polymer
        /// </summary>
        public int Length
        {
            get { return _length; }
        }          

        /// <summary>
        /// Gets the mass of the amino acid polymer with all modifications included
        /// </summary>
        public Mass Mass
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _mass; //ChemicalFormula.Mass;
            } 
        }
        
        private string _sequence;
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

        /// <summary>
        /// Gets the total number of amino acid residues in this amino acid polymer
        /// </summary>
        /// <returns>The number of amino acid residues</returns>
        public int ResidueCount()
        {
            return Length;
        }

        public int ResidueCount(IAminoAcid aminoAcid)
        {
            if (aminoAcid == null)
                return 0;

            int count = 0;
            foreach (IAminoAcid aar in _aminoAcids)
            {
                if (aar.Equals(aminoAcid))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Gets the number of amino acids residues in this amino acid polymer that
        /// has the specified residue letter
        /// </summary>
        /// <param name="residueChar">The residue letter to search for</param>
        /// <returns>The number of amino acid residues that have the same letter in this polymer</returns>
        public int ResidueCount(char residueChar)
        {           
            int count = 0;
            foreach (IAminoAcid aar in _aminoAcids)
            {
                if (aar.Letter.Equals(residueChar))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Gets the IAminoAcid at the specified position (1-based)
        /// </summary>
        /// <param name="index">The 1-based index of the amino acid to get</param>
        /// <returns>The IAminoAcid at the specified position</returns>
        public IAminoAcid this[int index]
        {
            get
            {
                if (index - 1 > _length || index < 1)
                    throw new IndexOutOfRangeException();
                return _aminoAcids[index - 1];
            }
        }

        #region Fragmentation

        public Fragment CalculateFragment(FragmentTypes type, int number)
        {
            if (number < 1 || number > _length)
            {
                throw new IndexOutOfRangeException();
            }

            if (type == FragmentTypes.None)
            {
                return null;
            }

            ChemicalFormula chemFormula = new ChemicalFormula();
            Mass mass = new Mass();
            IMass mod;
            IChemicalFormula chemMod;
            int start = 0;
            int end = number;

            if (type >= FragmentTypes.x)
            {
                start = Length - number;
                end = Length;
                chemFormula.Add(CTerminus);
                if (CTerminusModification != null)
                {
                    mass += CTerminusModification.Mass;
                    chemFormula.Add(CTerminusModification as IChemicalFormula);                                
                }
            }
            else
            {
                chemFormula.Add(NTerminus);
                if (NTerminusModification != null)
                {
                    mass += NTerminusModification.Mass;                 
                    chemFormula.Add(NTerminusModification as IChemicalFormula); 
                }
            }
            
            for (int i = start; i < end; i++)
            {
                chemFormula.Add(_aminoAcids[i].ChemicalFormula);
                if ((mod = _modifications[i + 1]) != null)
                {
                    mass += mod.Mass;
                    chemFormula.Add(mod as IChemicalFormula);                   
                }
            }

            return new Fragment(type, number, mass, chemFormula, this);
        }
        
        public IEnumerable<Fragment> CalculateFragments(FragmentTypes types)
        {
            return CalculateFragments(types, 1, Length - 1);
        }

        public IEnumerable<Fragment> CalculateFragments(FragmentTypes types, int min, int max)
        {
            if (types == FragmentTypes.None)
            {
                yield break;
            }

            if (min < 1 || max > Length - 1)
            {
                throw new IndexOutOfRangeException();
            }
            Mass mass = new Mass();
            IMass mod;
            foreach (FragmentTypes type in Enum.GetValues(typeof(FragmentTypes)))
            {
                if (type == FragmentTypes.None || type == FragmentTypes.Internal) continue;
                if ((types & type) == type)
                {
                    ChemicalFormula chemFormula = new ChemicalFormula();

                    int start = min;
                    int end = max;

                    if (type >= FragmentTypes.x)
                    {
                        chemFormula.Add(CTerminus);
                        if (CTerminusModification != null)
                        {
                            mass += CTerminusModification.Mass;
                            chemFormula.Add(CTerminusModification as IChemicalFormula);                             
                        }                           
                        for (int i = end; i >= start; i--)
                        {
                            chemFormula.Add(_aminoAcids[i].ChemicalFormula);
                            if ((mod = _modifications[i + 1]) != null)
                            {
                                mass += mod.Mass;
                                chemFormula.Add(mod as IChemicalFormula);   
                            }
                            int number = Length - i;
                            yield return new Fragment(type, number, new Mass(mass), new ChemicalFormula(chemFormula), this);
                        }
                    }
                    else
                    {
                        chemFormula.Add(NTerminus);
                        if (NTerminusModification != null)
                        {
                            mass += NTerminusModification.Mass;
                            chemFormula.Add(NTerminusModification as IChemicalFormula); 
                        }                            

                        for (int i = start; i <= end; i++)
                        {
                            chemFormula.Add(_aminoAcids[i - 1].ChemicalFormula);
                            if ((mod = _modifications[i]) != null)
                            {
                                mass += mod.Mass;
                                chemFormula.Add(mod as IChemicalFormula);                                
                            }
                            yield return new Fragment(type, i, new Mass(mass), new ChemicalFormula(chemFormula), this);
                        }
                    }
                }
            }
            yield break;
        }

        #endregion

        #region Chemical Modifications

        public bool ContainsModification(IMass modification)
        {
            foreach (IMass mod in _modifications)
            {
                if (modification.Equals(mod))
                    return true;
            }
            return false;
        }
            

        /// <summary>
        /// Sets the modification at the terminus of this amino acid polymer
        /// </summary>
        /// <param name="mod">The mod to set</param>
        /// <param name="terminus">The termini to set the mod at</param>
        public void SetModification(IMass mod, Terminus terminus)
        {
            if ((terminus & Terminus.N) == Terminus.N)
            {
                NTerminusModification = mod;
            }
            if ((terminus & Terminus.C) == Terminus.C)
            {
                CTerminusModification = mod;
            }
        }
        
        /// <summary>
        /// Sets the modification at specific sites on this amino acid polymer
        /// </summary>
        /// <param name="mod">The modification to set</param>
        /// <param name="sites">The sites to set the modification at</param>
        /// <returns>The number of modifications added to this amino acid polymer</returns>
        public virtual int SetModification(IMass mod, ModificationSites sites)
        {
            int count = 0;

            if ((sites & ModificationSites.NPep) == ModificationSites.NPep)
            {
                NTerminusModification = mod;
                count++;
            }

            for (int i = 0; i < _length; i++)
            {
                ModificationSites site = _aminoAcids[i].Site;
                if ((sites & site) == site)
                {
                    _modifications[i + 1] = mod;                   
                    count++;
                }
            }

            if ((sites & ModificationSites.PepC) == ModificationSites.PepC)
            {
                CTerminusModification = mod;
                count++;
            }

            if(count > 0)
                _isDirty = true;
            return count;
        }

        /// Clears the modification set at the terminus of this amino acid polymer back
        /// to the default C or N modifications.
        /// </summary>
        /// <param name="terminus">The termini to clear the mod at</param>
        public void ClearModification(Terminus terminus)
        {
            if ((terminus & Terminus.N) == Terminus.N)
            {
                NTerminusModification = null;
            }
            if ((terminus & Terminus.C) == Terminus.C)
            {
                CTerminusModification = null;
            }
        }

        public int SetModification(IMass mod, char letter)
        {
            int count = 0;
            for (int i = 0; i < _length; i++)
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

        public int SetModification(IMass mod, IAminoAcid residue)
        {
            int count = 0;
            for (int i = 0; i < _length; i++)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="residueNumber">(1-based) residue number</param>
        public void SetModification(IMass mod, int residueNumber)
        {
            if (residueNumber > _length || residueNumber < 1)
            {
                throw new IndexOutOfRangeException(string.Format("Residue number not in the correct range: [{0}-{1}] you specified: {2}", 1, Length, residueNumber));
            }
            _modifications[residueNumber] = mod;
            _isDirty = true;
        }

        /// <summary>
        /// Clear all modifications from this amino acid polymer.
        /// Includes N and C terminus modifications.
        /// </summary>       
        public void ClearModifications()
        {
            Array.Clear(_modifications, 0, _length + 2);
        }

        #endregion

        #region ChemicalFormula

        private bool _isExactChemicalFormula;

        public bool IsExactChemicalFormula
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _isExactChemicalFormula;
            }
        }

        public ChemicalFormula ChemicalFormula
        {
            get
            {
                if (_isDirty)
                {
                    CleanUp();
                }
                return _baseChemicalFormula;
            }
        }

        public bool TryGetChemicalFormula(out ChemicalFormula formula)
        {
            formula = new ChemicalFormula();
            IChemicalFormula chemMod;
            IMass mod;
            for (int i = 0; i < _length + 2; i++)
            {
                if ((mod = _modifications[i]) != null)
                {
                    chemMod = mod as IChemicalFormula;
                    if (chemMod == null)
                        return false;
                    formula.Add(chemMod.ChemicalFormula);
                }
            }

            // Handle N-Terminus
            formula.Add(NTerminus.ChemicalFormula);

            // Handle C-Terminus
            formula.Add(CTerminus.ChemicalFormula);

            // Handle Amino Acid Residues
            for (int i = 0; i < _length; i++)
            {
                IAminoAcid aa = _aminoAcids[i];
                formula.Add(aa.ChemicalFormula);
            }           

            return true;
        }

        #endregion

        public override string ToString()
        {
            return SequenceWithModifications;
        }
             
        public override int GetHashCode()
        {
            if (_aminoAcids == null && _modifications == null)
                return 0;            
            return Mass.GetHashCode();          
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            AminoAcidPolymer aap = obj as AminoAcidPolymer;
            if ((object)aap == null)
            {
                return false;
            }
            return Equals(aap);
        }

        public bool Equals(AminoAcidPolymer other)
        {
            if (this._length != other._length) return false; 
            if (!this.NTerminus.Equals(other.NTerminus) || !this.CTerminus.Equals(other.CTerminus))
                return false;

            int length = this.Length;
            for (int i = 1; i <= length; i++)
            {
                IMass thisMod = this._modifications[i];
                IMass otherMod = other._modifications[i];
                if (thisMod == null)
                {
                    if (otherMod != null)
                        return false;
                }
                else
                {
                    if (!thisMod.Equals(otherMod))
                        return false;
                }
                if (!this._aminoAcids[i - 1].Equals(other._aminoAcids[i - 1]))
                    return false;
            }
            return true;
        }

        #region Private Methods

        private void CleanUp()
        {                    
            if (_sequenceSB == null)
            {
                _sequenceSB = new StringBuilder(_length + 2);
            }
            else
            {
                _sequenceSB.Clear();
            }

            if (_baseChemicalFormula == null)
            {
                _baseChemicalFormula = new ChemicalFormula();
            }
            else
            {
                _baseChemicalFormula.Clear();
            }

            _isExactChemicalFormula = true;
            _mass = new Mass();
            
            StringBuilder baseSeqSB = new StringBuilder();
            IMass mod = null;
            IChemicalFormula chemMod = null;

            // Handle N-Terminus            
            _mass += _nTerminus.Mass;
            _baseChemicalFormula.Add(_nTerminus.ChemicalFormula);
            if ((mod = _modifications[0]) != null)
            {                
                _mass += mod.Mass;
                chemMod = mod as IChemicalFormula;               
                if (chemMod == null)
                {
                    _isExactChemicalFormula = false;
                }
                else
                {
                    _baseChemicalFormula.Add(chemMod.ChemicalFormula);
                }
                _sequenceSB.Append('[');
                _sequenceSB.Append(mod);
                _sequenceSB.Append("]-");
            }

            // Handle Amino Acid Residues
            for (int i = 0; i < _length; i++)
            {
                IAminoAcid aa = _aminoAcids[i];
                _baseChemicalFormula.Add(aa.ChemicalFormula);
                _mass += aa.Mass;
                char letter = aa.Letter;
                _sequenceSB.Append(letter);
                baseSeqSB.Append(letter);
                if ((mod = _modifications[i + 1]) != null)  // Mods are 1-based for the N and C-terminus
                {
                    _mass += mod.Mass;
                    chemMod = mod as IChemicalFormula;
                    if (chemMod == null)
                    {
                        _isExactChemicalFormula = false;
                    }
                    else
                    {
                        _baseChemicalFormula.Add(chemMod.ChemicalFormula);
                    }
                    _sequenceSB.Append('[');
                    _sequenceSB.Append(mod);
                    _sequenceSB.Append(']');
                }
            }

            // Handle C-Terminus          
            _mass += _cTerminus.Mass;
            _baseChemicalFormula.Add(_cTerminus.ChemicalFormula);
            if ((mod = _modifications[_length + 1]) != null)
            {               
                _mass += mod.Mass;
                chemMod = mod as IChemicalFormula;
                if (chemMod == null)
                {
                    _isExactChemicalFormula = false;
                }
                else
                {
                    _baseChemicalFormula.Add(chemMod.ChemicalFormula);
                }
                _sequenceSB.Append("-[");
                _sequenceSB.Append(mod);
                _sequenceSB.Append(']');
            }

            _sequence = baseSeqSB.ToString();
            _isDirty = false;
            _isSequenceDirty = false;
        }

        private void ParseSequence(string sequence)
        {
            AminoAcid residue = null;
            bool inMod = false;
            bool cterminalMod = false; // n or c terminal modification
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

                        NamedChemicalFormula modification = null;
                        switch (modString)
                        {
                            case "#": // Make the modification unverisally heavy (all C12 and N14s are promoted to C13 and N15s)
                                modification = NamedChemicalFormula.MakeHeavy(_aminoAcids[index - 1]);
                                break;
                            default:
                                if (NamedChemicalFormula.TryGetModification(modString, out modification))
                                {
                                    // do nothing
                                }
                                else if (ChemicalFormula.IsValidChemicalFormula(modString))
                                {
                                    modification = new NamedChemicalFormula(modString);
                                }
                                else
                                {
                                    throw new ArgumentException("Unable to correctly parse the following modification: " + modString);
                                }
                                break;
                        }

                        if (cterminalMod)
                        {
                            _modifications[index + 1] = modification;
                        }
                        else
                        {
                            _modifications[index] = modification;
                        }

                        cterminalMod = false;
                    }
                    else
                    {
                        modSB.Append(letter);
                    }
                }
                else if (AminoAcid.TryGetResidue(letter, out residue))
                {
                    _aminoAcids[index++] = residue;                 
                    baseSeqSB.Append(letter);
                }                
                else
                {
                    if (letter == '[')
                    {
                        inMod = true;
                    }
                    else if (letter == '-')
                    {                      
                        cterminalMod = (index > 0);
                    }
                    else if (letter == ' ')
                    {
                        // allow spaces by just skipping them.
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Amino Acid Letter {0} does not exist in the Amino Acid Dictionary", letter));
                    }
                }
            }

            if (inMod)
            {
                throw new ArgumentException("Couldn't find the closing ] for a modification in this sequence: " + sequence);
            }

            _sequence = baseSeqSB.ToString();
            _isSequenceDirty = false;
             
            Array.Resize(ref _aminoAcids, index);
            Array.Resize(ref _modifications, index + 2);
            _length = index;        
            _isDirty = true;             
        }

        #endregion

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