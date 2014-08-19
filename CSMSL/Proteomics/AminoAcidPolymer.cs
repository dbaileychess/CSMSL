// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (AminoAcidPolymer.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using CSMSL.Chemistry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Proteomics
{
    /// <summary>
    /// A linear polymer of amino acids
    /// </summary>
    public abstract class AminoAcidPolymer : IEquatable<AminoAcidPolymer>, IMass, IAminoAcidSequence
    {
        #region Static Properties

        /// <summary>
        /// The default chemical formula of the C terminus (hydroxyl group)
        /// </summary>
        public static readonly ChemicalFormula DefaultCTerminus = new ChemicalFormula("OH");

        /// <summary>
        /// The default chemical formula of the N terminus (hydrogen)
        /// </summary>
        public static readonly ChemicalFormula DefaultNTerminus = new ChemicalFormula("H");

        /// <summary>
        /// Defines if newly generated Amino Acid Polymers will store the amino acid sequence as a string
        /// or generate the string dynamically. If true, certain operations will be quicker at the cost of
        /// increased memory consumption. Default value is True.
        /// </summary>
        public static bool StoreSequenceString { get; set; }

        #endregion Static Properties

        #region Instance Variables

        /// <summary>
        /// The C-terminus chemical formula cap. This is different from the C-Terminus modification.
        /// </summary>
        private IChemicalFormula _cTerminus;

        /// <summary>
        /// The N-terminus chemical formula cap. This is different from the N-Terminus modification.
        /// </summary>
        private IChemicalFormula _nTerminus;

        /// <summary>
        /// All of the modifications indexed by position from N to C. This array is 2 bigger than the amino acid array
        /// as index 0 and Count - 1 represent the N and C terminus, respectively
        /// </summary>
        private IMass[] _modifications;

        /// <summary>
        /// All of the amino acid residues indexed by position from N to C.
        /// </summary>
        private AminoAcid[] _aminoAcids;

        /// <summary>
        /// The amino acid sequence with modification names interspersed. Is ignored if 'StoreSequenceString' is false
        /// </summary>
        private string _sequenceWithMods;

        /// <summary>
        /// The amino acid sequence. Is ignored if 'StoreSequenceString' is false
        /// </summary>
        private string _sequence;

        /// <summary>
        /// The internal flag to represent that the sequence with modifications have been changed and need to be updated
        /// </summary>
        internal bool IsDirty { get; set; }

        #endregion Instance Variables

        #region Constructors

        /// <summary>
        /// Static constructor, sets the default parameters for all amino acid polymers
        /// </summary>
        static AminoAcidPolymer()
        {
            StoreSequenceString = true;
        }

        protected AminoAcidPolymer()
            : this(string.Empty, DefaultNTerminus, DefaultCTerminus)
        {
        }

        protected AminoAcidPolymer(string sequence)
            : this(sequence, DefaultNTerminus, DefaultCTerminus)
        {
        }

        protected AminoAcidPolymer(string sequence, IChemicalFormula nTerm, IChemicalFormula cTerm)
        {
            MonoisotopicMass = 0;
            Length = sequence.Length;
            _aminoAcids = new AminoAcid[Length];
            NTerminus = nTerm;
            CTerminus = cTerm;
            ParseSequence(sequence);
        }

        protected AminoAcidPolymer(AminoAcidPolymer aminoAcidPolymer, bool includeModifications = true)
            : this(aminoAcidPolymer, 0, aminoAcidPolymer.Length, includeModifications)
        {
        }

        protected AminoAcidPolymer(AminoAcidPolymer aminoAcidPolymer, int firstResidue, int length, bool includeModifications = true)
        {
            if (firstResidue < 0 || firstResidue > aminoAcidPolymer.Length)
                throw new IndexOutOfRangeException(string.Format("The first residue index is outside the valid range [{0}-{1}]", 0, aminoAcidPolymer.Length));
            if (length + firstResidue > aminoAcidPolymer.Length)
                throw new ArgumentOutOfRangeException("length", "The length + firstResidue value is too large");

            Length = length;
            _aminoAcids = new AminoAcid[length];

            bool isNterm = firstResidue == 0;
            bool isCterm = length + firstResidue == aminoAcidPolymer.Length;

            _nTerminus = isNterm ? aminoAcidPolymer.NTerminus : DefaultNTerminus;
            _cTerminus = isCterm ? aminoAcidPolymer.CTerminus : DefaultCTerminus;

            double monoMass = _nTerminus.MonoisotopicMass + _cTerminus.MonoisotopicMass;

            AminoAcid[] otherAminoAcids = aminoAcidPolymer._aminoAcids;

            if (includeModifications && aminoAcidPolymer.ContainsModifications())
            {
                _modifications = new IMass[length + 2];
                for (int i = 0; i < length; i++)
                {
                    var aa = otherAminoAcids[i + firstResidue];
                    _aminoAcids[i] = aa;
                    monoMass += aa.MonoisotopicMass;

                    IMass mod = aminoAcidPolymer._modifications[i + firstResidue + 1];
                    if (mod == null)
                        continue;

                    _modifications[i + 1] = mod;
                    monoMass += mod.MonoisotopicMass;
                }
            }
            else
            {
                for (int i = 0, j = firstResidue; i < length; i++, j++)
                {
                    var aa = _aminoAcids[i] = otherAminoAcids[j];
                    monoMass += aa.MonoisotopicMass;
                }
            }

            MonoisotopicMass = monoMass;

            if (includeModifications)
            {
                if (isNterm)
                    NTerminusModification = aminoAcidPolymer.NTerminusModification;

                if (isCterm)
                    CTerminusModification = aminoAcidPolymer.CTerminusModification;
            }

            IsDirty = true;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the C terminus of this amino acid polymer
        /// </summary>
        public IChemicalFormula CTerminus
        {
            get { return _cTerminus; }
            set { ReplaceTerminus(ref _cTerminus, value); }
        }

        /// <summary>
        /// Gets or sets the N terminus of this amino acid polymer
        /// </summary>
        public IChemicalFormula NTerminus
        {
            get { return _nTerminus; }
            set { ReplaceTerminus(ref _nTerminus, value); }
        }

        /// <summary>
        /// Gets the number of amino acids in this amino acid polymer
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The total monoisotopic mass of this peptide and all of its modifications
        /// </summary>
        public double MonoisotopicMass { get; private set; }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// The internal data store for the modifications (2 larger than the length to handle the N and C termini)
        /// </summary>
        internal IMass[] Modifications
        {
            get { return _modifications; }
        }

        /// <summary>
        /// The internal data store for the amino acids
        /// </summary>
        internal AminoAcid[] AminoAcids
        {
            get { return _aminoAcids; }
        }

        #endregion Internal Properties

        #region Amino Acid Sequence

        public AminoAcid GetResidue(int position)
        {
            if (position < 0 || position >= Length)
                return null;
            return _aminoAcids[position];
        }

        /// <summary>
        /// Returns the amino acid sequence with all isoleucines (I) replaced with leucines (L);
        /// </summary>
        /// <returns>The amino acid sequence with all I's into L's</returns>
        public virtual string GetLeucineSequence()
        {
            return Sequence.Replace('I', 'L');
        }

        /// <summary>
        /// Checks if an amino acid residue with the value of 'residue' is contained in this polymer
        /// </summary>
        /// <param name="residue">The character code for the amino acid residue</param>
        /// <returns>True if any amino acid residue is the same as the specified character</returns>
        public bool Contains(char residue)
        {
            return _aminoAcids.Any(aa => aa.Letter.Equals(residue));
        }

        /// <summary>
        /// Checks if the amino acid residue is contained in this polymer
        /// </summary>
        /// <param name="residue">The residue to check for</param>
        /// <returns>True if the polymer contains the specified residue, False otherwise</returns>
        public bool Contains(IAminoAcid residue)
        {
            return _aminoAcids.Contains(residue);
        }

        /// <summary>
        /// Gets the base amino acid sequence
        /// </summary>
        public string Sequence
        {
            get
            {
                // Don't store the string if we don't have too, just recreate it on the fly
                if (!StoreSequenceString)
                    return new string(_aminoAcids.Select(aa => aa.Letter).ToArray());

                // Generate the sequence if the stored version is null or empty
                if (string.IsNullOrEmpty(_sequence))
                {
                    _sequence = new string(_aminoAcids.Select(aa => aa.Letter).ToArray());
                }

                return _sequence;
            }
        }

        /// <summary>
        /// Gets the amino acid sequence with modifications
        /// </summary>
        public string SequenceWithModifications
        {
            get
            {
                // Don't store the string if we don't have too, just recreate it on the fly
                if (!StoreSequenceString)
                    return GetSequenceWithModifications();

                if (!IsDirty && !string.IsNullOrEmpty(_sequenceWithMods))
                    return _sequenceWithMods;

                _sequenceWithMods = GetSequenceWithModifications();
                IsDirty = false;
                return _sequenceWithMods;
            }
        }

        public string GetSequenceWithModifications(bool leucineSequence = false)
        {
            if (_modifications == null)
                return (leucineSequence) ? GetLeucineSequence() : Sequence;

            StringBuilder modSeqSb = new StringBuilder(Length);

            IMass mod;

            // Handle N-Terminus Modification
            if ((mod = _modifications[0]) != null && !Modification.Empty.Equals(mod) && !mod.MassEquals(0))
            {
                modSeqSb.Append('[');
                modSeqSb.Append(mod);
                modSeqSb.Append("]-");
            }

            // Handle Amino Acid Residues
            for (int i = 0; i < Length; i++)
            {
                if (leucineSequence && _aminoAcids[i].Letter == 'I')
                    modSeqSb.Append('L');
                else
                    modSeqSb.Append(_aminoAcids[i].Letter);

                // Handle Amino Acid Modification (1-based)
                if ((mod = _modifications[i + 1]) != null && !Modification.Empty.Equals(mod) && !mod.MassEquals(0))
                {
                    modSeqSb.Append('[');
                    modSeqSb.Append(mod);
                    modSeqSb.Append(']');
                }
            }

            // Handle C-Terminus Modification
            if ((mod = _modifications[Length + 1]) != null && !Modification.Empty.Equals(mod) && !mod.MassEquals(0))
            {
                modSeqSb.Append("-[");
                modSeqSb.Append(mod);
                modSeqSb.Append(']');
            }

            return modSeqSb.ToString();
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
            return aminoAcid == null ? 0 : _aminoAcids.Count(aar => aar.Equals(aminoAcid));
        }

        /// <summary>
        /// Gets the number of amino acids residues in this amino acid polymer that
        /// has the specified residue letter
        /// </summary>
        /// <param name="residueChar">The residue letter to search for</param>
        /// <returns>The number of amino acid residues that have the same letter in this polymer</returns>
        public int ResidueCount(char residueChar)
        {
            return _aminoAcids.Count(aar => aar.Letter.Equals(residueChar));
        }

        public int ResidueCount(char residueChar, int index, int length)
        {
            return _aminoAcids.SubArray(index, length).Count(aar => aar.Letter.Equals(residueChar));
        }

        public int ResidueCount(IAminoAcid aminoAcid, int index, int length)
        {
            return _aminoAcids.SubArray(index, length).Count(aar => aar.Equals(aminoAcid));
        }

        public int ElementCount(string element)
        {
            // Residues count
            int count = _aminoAcids.Sum(aar => aar.ChemicalFormula.Count(element));
            // Modifications count (if the mod is a IChemicalFormula)
            if (_modifications != null)
                count += _modifications.Where(mod => mod is IChemicalFormula).Cast<IChemicalFormula>().Sum(mod => mod.ChemicalFormula.Count(element));
            return count;
        }

        public int ElementCount(Isotope isotope)
        {
            // Residues count
            int count = _aminoAcids.Sum(aar => aar.ChemicalFormula.Count(isotope));
            // Modifications count (if the mod is a IChemicalFormula)
            if (_modifications != null)
                count += _modifications.Where(mod => mod is IChemicalFormula).Cast<IChemicalFormula>().Sum(mod => mod.ChemicalFormula.Count(isotope));
            return count;
        }

        public bool Contains(IAminoAcidSequence item)
        {
            return Contains(item.Sequence);
        }

        public bool Contains(string sequence)
        {
            return Sequence.Contains(sequence);
        }

        #endregion Amino Acid Sequence

        #region Fragmentation

        /// <summary>
        /// Calculates the fragments that are different between this and another aminoacidpolymer
        /// </summary>
        /// <param name="other"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Fragment> GetSiteDeterminingFragments(AminoAcidPolymer other, FragmentTypes type)
        {
            return GetSiteDeterminingFragments(this, other, type);
        }

        /// <summary>
        /// Calculates all the fragments of the types you specify
        /// </summary>
        /// <param name="types"></param>
        /// <param name="calculateChemicalFormula"></param>
        /// <returns></returns>
        public IEnumerable<Fragment> Fragment(FragmentTypes types, bool calculateChemicalFormula = false)
        {
            return Fragment(types, 1, Length - 1, calculateChemicalFormula);
        }

        public IEnumerable<Fragment> Fragment(FragmentTypes types, int number, bool calculateChemicalFormula = false)
        {
            return Fragment(types, number, number, calculateChemicalFormula);
        }

        public IEnumerable<Fragment> Fragment(FragmentTypes types, int min, int max, bool calculateChemicalFormula = false)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException();

            if (min < 1 || max > Length - 1)
                throw new IndexOutOfRangeException();

            foreach (FragmentTypes type in types.GetIndividualFragmentTypes())
            {
                bool isChemicalFormula = calculateChemicalFormula;
                ChemicalFormula capFormula = type.GetIonCap();
                bool isCTerminal = type.GetTerminus() == Terminus.C;

                double monoMass = capFormula.MonoisotopicMass;
                ChemicalFormula formula = new ChemicalFormula(capFormula);

                IChemicalFormula terminus = isCTerminal ? CTerminus : NTerminus;
                monoMass += terminus.MonoisotopicMass;

                if (isChemicalFormula)
                {
                    formula += terminus;
                }

                bool first = true;
                bool hasMod = _modifications != null;

                for (int i = 0; i <= max; i++)
                {
                    int aaIndex = isCTerminal ? Length - i : i - 1;

                    // Handle the terminus mods first in a special case
                    IMass mod;
                    if (first)
                    {
                        first = false;
                        if (hasMod)
                        {
                            mod = _modifications[aaIndex + 1];
                            if (mod != null)
                            {
                                monoMass += mod.MonoisotopicMass;

                                if (isChemicalFormula)
                                {
                                    IChemicalFormula modFormula = mod as IChemicalFormula;
                                    if (modFormula != null)
                                    {
                                        formula.Add(modFormula);
                                    }
                                    else
                                    {
                                        isChemicalFormula = false;
                                    }
                                }
                            }
                        }
                        continue;
                    }

                    monoMass += _aminoAcids[aaIndex].MonoisotopicMass;
                    formula.Add(_aminoAcids[aaIndex]);

                    if (hasMod)
                    {
                        mod = _modifications[aaIndex + 1];

                        if (mod != null)
                        {
                            monoMass += mod.MonoisotopicMass;

                            if (isChemicalFormula)
                            {
                                IChemicalFormula modFormula = mod as IChemicalFormula;
                                if (modFormula != null)
                                {
                                    formula.Add(modFormula);
                                }
                                else
                                {
                                    isChemicalFormula = false;
                                }
                            }
                        }
                    }

                    if (i < min)
                        continue;

                    if (isChemicalFormula)
                    {
                        yield return new ChemicalFormulaFragment(type, i, formula, this);
                    }
                    else
                    {
                        yield return new Fragment(type, i, monoMass, this);
                    }
                }
            }
        }

        #endregion Fragmentation

        #region Modifications

        public bool ContainsModifications()
        {
            return _modifications != null && _modifications.Any(m => m != null);
        }

        public IMass[] GetModifications()
        {
            IMass[] mods = new IMass[Length + 2];
            if (_modifications != null)
                Array.Copy(_modifications, mods, _modifications.Length);
            return mods;
        }

        public ISet<T> GetUniqueModifications<T>() where T : IMass
        {
            HashSet<T> uniqueMods = new HashSet<T>();

            if (_modifications == null)
                return uniqueMods;

            foreach (IMass mod in _modifications)
            {
                if (mod is T)
                    uniqueMods.Add((T) mod);
            }
            return uniqueMods;
        }

        /// <summary>
        /// Gets or sets the modification of the C terminus on this amino acid polymer
        /// </summary>
        public IMass CTerminusModification
        {
            get { return GetModification(Length + 1); }
            set { ReplaceMod(Length + 1, value); }
        }

        /// <summary>
        /// Gets or sets the modification of the C terminus on this amino acid polymer
        /// </summary>
        public IMass NTerminusModification
        {
            get { return GetModification(0); }
            set { ReplaceMod(0, value); }
        }

        /// <summary>
        /// Counts the total number of modifications on this polymer that are not null
        /// </summary>
        /// <returns>The number of modifications</returns>
        public int ModificationCount()
        {
            return _modifications == null ? 0 : _modifications.Count(mod => mod != null);
        }

        /// <summary>
        /// Counts the total number of the specified modification on this polymer
        /// </summary>
        /// <param name="modification">The modification to count</param>
        /// <returns>The number of modifications</returns>
        public int ModificationCount(IMass modification)
        {
            if (modification == null || _modifications == null)
                return 0;

            return _modifications.Count(modification.Equals);
        }

        /// <summary>
        /// Determines if the specified modification exists in this polymer
        /// </summary>
        /// <param name="modification">The modification to look for</param>
        /// <returns>True if the modification is found, false otherwise</returns>
        public bool Contains(IMass modification)
        {
            if (modification == null || _modifications == null)
                return false;

            return _modifications.Contains(modification);
        }

        /// <summary>
        /// Get the modification at the given residue number
        /// </summary>
        /// <param name="residueNumber">The amino acid residue number</param>
        /// <returns>The modification at the site, null if there isn't any modification present</returns>
        public IMass GetModification(int residueNumber)
        {
            return _modifications == null ? null : _modifications[residueNumber];
        }

        public bool TryGetModification(int residueNumber, out IMass mod)
        {
            if (residueNumber > Length || residueNumber < 1 || _modifications == null)
            {
                mod = null;
                return false;
            }
            mod = _modifications[residueNumber];
            return mod != null;
        }

        public bool TryGetModification<T>(int residueNumber, out T mod) where T : class, IMass
        {
            IMass outMod;
            if (TryGetModification(residueNumber, out outMod))
            {
                mod = outMod as T;
                return mod != null;
            }
            mod = default(T);
            return false;
        }

        /// <summary>
        /// Sets the modification at the terminus of this amino acid polymer
        /// </summary>
        /// <param name="mod">The modification to set</param>
        /// <param name="terminus">The termini to set the mod at</param>
        public virtual void SetModification(IMass mod, Terminus terminus)
        {
            if ((terminus & Terminus.N) == Terminus.N)
                NTerminusModification = mod;

            if ((terminus & Terminus.C) == Terminus.C)
                CTerminusModification = mod;
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

            for (int i = 0; i < Length; i++)
            {
                ModificationSites site = _aminoAcids[i].Site;
                if ((sites & site) == site)
                {
                    ReplaceMod(i + 1, mod);
                    count++;
                }
            }

            if ((sites & ModificationSites.PepC) == ModificationSites.PepC)
            {
                CTerminusModification = mod;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Sets the modification at specific sites on this amino acid polymer
        /// </summary>
        /// <param name="mod">The modification to set</param>
        /// <param name="letter">The residue character to set the modification at</param>
        /// <returns>The number of modifications added to this amino acid polymer</returns>
        public virtual int SetModification(IMass mod, char letter)
        {
            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                if (!letter.Equals(_aminoAcids[i].Letter))
                    continue;

                ReplaceMod(i + 1, mod);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Sets the modification at specific sites on this amino acid polymer
        /// </summary>
        /// <param name="mod">The modification to set</param>
        /// <param name="residue">The residue to set the modification at</param>
        /// <returns>The number of modifications added to this amino acid polymer</returns>
        public virtual int SetModification(IMass mod, IAminoAcid residue)
        {
            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                if (!residue.Equals(_aminoAcids[i]))
                    continue;

                ReplaceMod(i + 1, mod);
                count++;
            }
            return count;
        }

        /// <summary>
        /// Sets the modification at specific sites on this amino acid polymer
        /// </summary>
        /// <param name="mod">The modification to set</param>
        /// <param name="residueNumber">The residue number to set the modification at</param>
        public virtual void SetModification(IMass mod, int residueNumber)
        {
            if (residueNumber > Length || residueNumber < 1)
                throw new IndexOutOfRangeException(string.Format("Residue number not in the correct range: [{0}-{1}] you specified: {2}", 1, Length, residueNumber));

            ReplaceMod(residueNumber, mod);
        }

        public void SetModifications(IEnumerable<Modification> modifications)
        {
            if (modifications == null)
                return;
            foreach (Modification mod in modifications)
            {
                SetModification(mod, mod.Sites);
            }
        }

        public void SetModification(Modification mod)
        {
            SetModification(mod, mod.Sites);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="residueNumbers">(1-based) residue number</param>
        public void SetModification(IMass mod, params int[] residueNumbers)
        {
            foreach (int residueNumber in residueNumbers)
            {
                SetModification(mod, residueNumber);
            }
        }

        /// <summary>
        /// Replaces all instances of the old modification with the new modification in this polymer
        /// </summary>
        /// <param name="oldMod">The modification to remove</param>
        /// <param name="newMod">The modification to replace it with</param>
        /// <returns>The number of modifications added to this amino acid polymer</returns>
        public virtual int ReplaceModification(IMass oldMod, IMass newMod)
        {
            if (oldMod == null)
                throw new ArgumentException("Cannot replace a null modification");

            // No need to replace identical mods
            if (oldMod.Equals(newMod))
                return 0;

            int count = 0;
            for (int i = 0; i < Length + 2; i++)
            {
                IMass mod = GetModification(i);
                if (mod == null || !oldMod.Equals(mod))
                    continue;

                ReplaceMod(i, newMod);
                count++;
            }
            return count;
        }

        /// <summary>
        /// Adds the modification at the terminus of this amino acid polymer, combining modifications if a modification is already present
        /// </summary>
        /// <param name="modification">The modification to set</param>
        /// <param name="terminus">The termini to set the mod at</param>
        public virtual int AddModification(IMass modification, Terminus terminus)
        {
            IMass currentMod;
            int count = 0;

            if ((terminus & Terminus.N) == Terminus.N)
            {
                currentMod = NTerminusModification;
                NTerminusModification = currentMod == null ? modification : new ModificationCollection(currentMod, modification);
                count++;
            }

            if ((terminus & Terminus.C) == Terminus.C)
            {
                currentMod = CTerminusModification;
                CTerminusModification = currentMod == null ? modification : new ModificationCollection(currentMod, modification);
                count++;
            }
            return count;
        }

        public virtual int AddModification(Modification modification)
        {
            return AddModification(modification, modification.Sites);
        }

        public virtual int AddModification(IMass modification, ModificationSites sites)
        {
            if (_modifications == null)
                _modifications = new IMass[Length + 2];

            int count = 0;
            IMass currentMod;
            if ((sites & ModificationSites.NPep) == ModificationSites.NPep)
            {
                currentMod = NTerminusModification;
                NTerminusModification = currentMod == null ? modification : new ModificationCollection(currentMod, modification);
                count++;
            }

            for (int i = 0; i < Length; i++)
            {
                ModificationSites site = _aminoAcids[i].Site;
                if ((sites & site) == site)
                {
                    currentMod = _modifications[i + 1];
                    ReplaceMod(i + 1, currentMod == null ? modification : new ModificationCollection(currentMod, modification));
                    count++;
                }
            }

            if ((sites & ModificationSites.PepC) == ModificationSites.PepC)
            {
                currentMod = CTerminusModification;
                CTerminusModification = currentMod == null ? modification : new ModificationCollection(currentMod, modification);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds the modification at specific sites on this amino acid polymer, combining modifications if a modification is already present
        /// </summary>
        /// <param name="modification">The modification to set</param>
        /// <param name="residueNumber">The residue number to set the modification at</param>
        public virtual void AddModification(IMass modification, int residueNumber)
        {
            if (residueNumber > Length || residueNumber < 1)
                throw new IndexOutOfRangeException(string.Format("Residue number not in the correct range: [{0}-{1}] you specified: {2}", 1, Length, residueNumber));

            IMass currentMod = GetModification(residueNumber);
            ReplaceMod(residueNumber, currentMod == null ? modification : new ModificationCollection(currentMod, modification));
        }

        /// <summary>
        /// Clears the modification set at the terminus of this amino acid polymer back
        /// to the default C or N modifications.
        /// </summary>
        /// <param name="terminus">The termini to clear the mod at</param>
        public void ClearModifications(Terminus terminus)
        {
            if (_modifications == null)
                return;

            if ((terminus & Terminus.N) == Terminus.N)
                NTerminusModification = null;

            if ((terminus & Terminus.C) == Terminus.C)
                CTerminusModification = null;
        }

        /// <summary>
        /// Clear the modifications from the specified sites(s)
        /// </summary>
        /// <param name="sites">The sites to remove modifications from</param>
        public void ClearModifications(ModificationSites sites)
        {
            if (_modifications == null)
                return;

            if ((sites & ModificationSites.NPep) == ModificationSites.NPep || (sites & ModificationSites.NProt) == ModificationSites.NProt)
            {
                ReplaceMod(0, null);
            }

            for (int i = 0; i < Length; i++)
            {
                int modIndex = i + 1;

                if (_modifications[modIndex] == null)
                    continue;

                ModificationSites curSite = _aminoAcids[i].Site;

                if ((curSite & sites) == curSite)
                {
                    ReplaceMod(modIndex, null);
                }
            }

            if ((sites & ModificationSites.PepC) == ModificationSites.PepC || (sites & ModificationSites.ProtC) == ModificationSites.ProtC)
            {
                ReplaceMod(Length + 1, null);
            }
        }

        /// <summary>
        /// Clear all modifications from this amino acid polymer.
        /// Includes N and C terminus modifications.
        /// </summary>
        public void ClearModifications()
        {
            if (!ContainsModifications())
                return;

            for (int i = 0; i <= Length + 1; i++)
            {
                if (_modifications[i] == null)
                    continue;

                MonoisotopicMass -= _modifications[i].MonoisotopicMass;
                _modifications[i] = null;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Removes the specified mod from all locations on this polymer
        /// </summary>
        /// <param name="mod">The modification to remove from this polymer</param>
        public void ClearModifications(IMass mod)
        {
            if (mod == null || _modifications == null)
                return;

            for (int i = 0; i <= Length + 1; i++)
            {
                if (!mod.Equals(_modifications[i]))
                    continue;

                MonoisotopicMass -= mod.MonoisotopicMass;
                _modifications[i] = null;
                IsDirty = true;
            }
        }

        #endregion Modifications

        #region ChemicalFormula

        /// <summary>
        /// Gets the chemical formula of this amino acid polymer.
        /// If a modification attached to this polymer does not
        /// have a chemical formula, it is not included in the output,
        /// thus the return chemical formula may not be accurate.
        /// See <see cref="TryGetChemicalFormula"/> for more details
        /// </summary>
        /// <returns></returns>
        public ChemicalFormula GetChemicalFormula()
        {
            var formula = new ChemicalFormula();

            // Handle Modifications
            if (ContainsModifications())
            {
                for (int i = 0; i < Length + 2; i++)
                {
                    IChemicalFormula chemMod = _modifications[i] as IChemicalFormula;

                    if (chemMod == null)
                        continue;

                    formula.Add(chemMod.ChemicalFormula);
                }
            }

            // Handle N-Terminus
            formula.Add(NTerminus.ChemicalFormula);

            // Handle C-Terminus
            formula.Add(CTerminus.ChemicalFormula);

            // Handle Amino Acid Residues
            for (int i = 0; i < Length; i++)
            {
                formula.Add(_aminoAcids[i].ChemicalFormula);
            }

            return formula;
        }

        /// <summary>
        /// Try and get the chemical formula for the whole amino acid polymer. Modifications
        /// may not always be of IChemicalFormula and this method will return false if any
        /// modification is not a chemical formula
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public bool TryGetChemicalFormula(out ChemicalFormula formula)
        {
            formula = new ChemicalFormula();

            // Handle Modifications
            if (ContainsModifications())
            {
                for (int i = 0; i < Length + 2; i++)
                {
                    IMass mod;
                    if ((mod = _modifications[i]) == null)
                        continue;

                    IChemicalFormula chemMod = mod as IChemicalFormula;
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
            for (int i = 0; i < Length; i++)
            {
                formula.Add(_aminoAcids[i].ChemicalFormula);
            }

            return true;
        }

        #endregion ChemicalFormula

        #region Isoelectric point

        public double CalculateIsoelectricPoint(double precision = 0.01)
        {
            return CalculateIsoelectricPoint(Sequence, precision);
        }

        #endregion Isoelectric point

        #region Object

        public override string ToString()
        {
            return SequenceWithModifications;
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            AminoAcidPolymer aap = obj as AminoAcidPolymer;
            return aap != null && Equals(aap);
        }

        #endregion Object

        #region IEquatable

        public bool Equals(AminoAcidPolymer other)
        {
            if (other == null ||
                Length != other.Length ||
                !NTerminus.Equals(other.NTerminus) ||
                !CTerminus.Equals(other.CTerminus))
                return false;

            bool containsMod = ContainsModifications();

            if (containsMod != other.ContainsModifications())
                return false;

            for (int i = 0; i <= Length + 1; i++)
            {
                if (containsMod && !Equals(_modifications[i], other._modifications[i]))
                    return false;

                if (i == 0 || i == Length + 1)
                    continue; // uneven arrays, so skip these two conditions

                if (!_aminoAcids[i - 1].Equals(other._aminoAcids[i - 1]))
                    return false;
            }
            return true;
        }

        #endregion IEquatable

        #region Private Methods

        private bool ReplaceTerminus(ref IChemicalFormula terminus, IChemicalFormula value)
        {
            if (Equals(value, terminus))
                return false;

            if (terminus != null)
                MonoisotopicMass -= terminus.MonoisotopicMass;

            terminus = value;

            if (value != null)
                MonoisotopicMass += value.MonoisotopicMass;

            return true;
        }

        /// <summary>
        /// Replaces a modification (if present) at the specific index in the residue (0-based for N and C termini)
        /// </summary>
        /// <param name="index">The residue index to replace at</param>
        /// <param name="mod">The modification to replace with</param>
        private bool ReplaceMod(int index, IMass mod)
        {
            // No error checking here as all validation will occur before this method is call. This is to prevent
            // unneeded bounds checking

            if (_modifications == null)
            {
                _modifications = new IMass[Length + 2];
            }

            IMass oldMod = _modifications[index]; // Get the mod at the index, if present

            if (Equals(mod, oldMod))
                return false; // Same modifications, no change is required

            IsDirty = true;

            if (oldMod != null)
                MonoisotopicMass -= oldMod.MonoisotopicMass; // remove the old mod mass

            _modifications[index] = mod;

            if (mod != null)
                MonoisotopicMass += mod.MonoisotopicMass; // add the new mod mass

            return true;
        }

        /// <summary>
        /// Parses a string sequence of amino acids characters into a peptide object
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private bool ParseSequence(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
                return false;

            bool inMod = false;
            bool cterminalMod = false; // n or c terminal modification
            int index = 0;

            double monoMass = 0;

            StringBuilder sb = null;
            bool storeSequenceString = StoreSequenceString;
            if (storeSequenceString)
                sb = new StringBuilder(sequence.Length);

            StringBuilder modSb = new StringBuilder(10);
            foreach (char letter in sequence)
            {
                if (inMod)
                {
                    if (letter == ']')
                    {
                        inMod = false; // end the modification phase

                        string modString = modSb.ToString();
                        modSb.Clear();
                        IMass modification;
                        switch (modString)
                        {
                            case "#": // Make the modification unverisally heavy (all C12 and N14s are promoted to C13 and N15s)
                                modification = _aminoAcids[index - 1].ToHeavyModification(true, true);
                                break;

                            default:
                                double mass;
                                Modification mod;
                                if (ModificationDictionary.TryGetModification(modString, out mod))
                                {
                                    modification = mod;
                                }
                                else if (ChemicalFormula.IsValidChemicalFormula(modString))
                                {
                                    modification = new ChemicalFormula(modString);
                                }
                                else if (double.TryParse(modString, out mass))
                                {
                                    modification = new Mass(mass);
                                }
                                else
                                {
                                    throw new ArgumentException("Unable to correctly parse the following modification: " + modString);
                                }
                                break;
                        }

                        monoMass += modification.MonoisotopicMass;

                        if (_modifications == null)
                            _modifications = new IMass[Length + 2];

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
                        modSb.Append(letter);
                    }
                }
                else
                {
                    AminoAcid residue;
                    //char upperletter = char.ToUpper(letter); // moved to amino acid dictionary
                    if (AminoAcid.TryGetResidue(letter, out residue))
                    {
                        _aminoAcids[index++] = residue;
                        if (storeSequenceString)
                            sb.Append(residue.Letter);
                        monoMass += residue.MonoisotopicMass;
                    }
                    else
                    {
                        switch (letter)
                        {
                            case '[': // start of a modification
                                inMod = true;
                                break;

                            case '-': // start of a c-teriminal modification
                                cterminalMod = (index > 0);
                                break;

                            case ' ': // ignore spaces
                                break;

                            case '*': // ignore *
                                break;

                            default:
                                throw new ArgumentException(string.Format("Amino Acid Letter {0} does not exist in the Amino Acid Dictionary. {0} is also not a valid character", letter));
                        }
                    }
                }
            }

            if (inMod)
            {
                throw new ArgumentException("Couldn't find the closing ] for a modification in this sequence: " + sequence);
            }

            if (storeSequenceString)
                _sequence = sb.ToString();

            Length = index;
            MonoisotopicMass += monoMass;
            Array.Resize(ref _aminoAcids, Length);
            if (_modifications != null)
                Array.Resize(ref _modifications, Length + 2);
            IsDirty = true;

            return true;
        }

        #endregion Private Methods

        #region Statics Methods

        #region Isoelectric Point

        public static double CalculateIsoelectricPoint(string sequence, double precision = 0.01)
        {
            int d, e, c, y, h, k, r;
            d = e = c = y = h = k = r = 0;
            foreach (char aa in sequence)
            {
                switch (aa)
                {
                    case 'D':
                        d++;
                        break;

                    case 'E':
                        e++;
                        break;

                    case 'C':
                        c++;
                        break;

                    case 'Y':
                        y++;
                        break;

                    case 'H':
                        h++;
                        break;

                    case 'K':
                        k++;
                        break;

                    case 'R':
                        r++;
                        break;
                }
            }

            double pH = 6.5;
            double minpH = 0.0;
            double maxpH = 14.0;

            while (true)
            {
                double QN1 = -1/(1 + Math.Pow(10, (3.65 - pH)));
                double QN2 = -d/(1 + Math.Pow(10, (3.90 - pH)));
                double QN3 = -e/(1 + Math.Pow(10, (4.07 - pH)));
                double QN4 = -c/(1 + Math.Pow(10, (8.37 - pH)));
                double QN5 = -y/(1 + Math.Pow(10, (10.46 - pH)));
                double QP1 = h/(1 + Math.Pow(10, (pH - 6.04)));
                double QP2 = 1/(1 + Math.Pow(10, (pH - 8.2)));
                double QP3 = k/(1 + Math.Pow(10, (pH - 10.54)));
                double QP4 = r/(1 + Math.Pow(10, (pH - 12.48)));

                double NQ = QN1 + QN2 + QN3 + QN4 + QN5 + QP1 + QP2 + QP3 + QP4;

                if (NQ < 0)
                {
                    maxpH = pH;
                    pH = pH - ((pH - minpH)/2);
                }
                else
                {
                    minpH = pH;
                    pH = pH + ((maxpH - pH)/2);
                }

                if ((pH - minpH < precision) && (maxpH - pH < precision))
                    break;
            }

            return pH;
        }

        public static double CalculateIsoelectricPoint(IAminoAcidSequence sequence, double precision = 0.01)
        {
            return CalculateIsoelectricPoint(sequence.Sequence, precision);
        }

        #endregion Isoelectric Point

        #region Fragmentation

        public static IEnumerable<Fragment> GetSiteDeterminingFragments(AminoAcidPolymer peptideA, AminoAcidPolymer peptideB, FragmentTypes types)
        {
            if (peptideA == null)
            {
                // Only b is not null, return all of its fragments
                if (peptideB != null)
                {
                    return peptideB.Fragment(types);
                }
                throw new ArgumentNullException("peptideA", "Cannot be null");
            }

            if (peptideB == null)
            {
                return peptideA.Fragment(types);
            }
            HashSet<Fragment> aFrags = new HashSet<Fragment>(peptideA.Fragment(types));
            HashSet<Fragment> bfrags = new HashSet<Fragment>(peptideB.Fragment(types));

            aFrags.SymmetricExceptWith(bfrags);
            return aFrags;
        }

        #endregion Fragmentation

        #region Digestion

        public static IEnumerable<Tuple<int, int>> GetDigestionPoints(string sequence, IProtease protease, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue, bool methionineInitiator = true, bool semiDigestion = false)
        {
            return GetDigestionPoints(sequence, new[] {protease}, maxMissedCleavages, minLength, maxLength, methionineInitiator, semiDigestion);
        }

        /// <summary>
        /// Gets the digestion points (starting index and length) of a amino acid sequence
        /// </summary>
        /// <param name="sequence">The sequence to cleave</param>
        /// <param name="proteases">The proteases to cleave with</param>
        /// <param name="maxMissedCleavages">The maximum number of missed clevages to allow</param>
        /// <param name="minLength">The minimum amino acid length of the peptides</param>
        /// <param name="maxLength">The maximum amino acid length of the peptides</param>
        /// <param name="methionineInitiator"></param>
        /// <param name="semiDigestion"></param>
        /// <returns>A collection of clevage points and the length of the cut (Item1 = index, Item2 = length)</returns>
        public static IEnumerable<Tuple<int, int>> GetDigestionPoints(string sequence, IEnumerable<IProtease> proteases, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue, bool methionineInitiator = true, bool semiDigestion = false)
        {
            if (maxMissedCleavages < 0)
                throw new ArgumentOutOfRangeException("maxMissedCleavages", "The maximum number of missed cleavages must be >= 0");

            int[] indices = GetCleavageIndices(sequence, proteases).ToArray();

            bool includeMethionineCut = methionineInitiator && sequence[0] == 'M';

            int indiciesCount = indices.Length - 1;

            for (int missedCleavages = 0; missedCleavages <= maxMissedCleavages; missedCleavages++)
            {
                int max = indiciesCount - missedCleavages;
                int offset = missedCleavages + 1;
                for (int i = 0; i < max; i++)
                {
                    int begin = indices[i];
                    int len = indices[i + offset] - begin;

                    // Case for initiator methionine
                    if (begin == -1 && includeMethionineCut)
                    {
                        int newLength = len - 1;
                        if (newLength >= minLength && newLength <= maxLength)
                        {
                            yield return new Tuple<int, int>(begin + 2, newLength);
                            if (semiDigestion)
                            {
                                int start = begin + 1;
                                for (int j = 1; j < len; j++)
                                {
                                    if (len - j >= minLength && len - j <= maxLength)
                                        yield return new Tuple<int, int>(start + j, len - j);
                                    if (j >= minLength && j <= maxLength)
                                        yield return new Tuple<int, int>(start, j);
                                }
                            }
                        }
                    }

                    if (len < minLength || len > maxLength)
                        continue;

                    yield return new Tuple<int, int>(begin + 1, len);
                    if (semiDigestion)
                    {
                        int start = begin + 1;
                        for (int j = 1; j < len; j++)
                        {
                            if (len - j >= minLength && len - j <= maxLength)
                                yield return new Tuple<int, int>(start + j, len - j);
                            if (j >= minLength && j <= maxLength)
                                yield return new Tuple<int, int>(start, j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the location of all the possible cleavage points for a given sequence and set of proteases
        /// </summary>
        /// <param name="sequence">The sequence to determine the cleavage points for</param>
        /// <param name="proteases">The proteases to cleave with</param>
        /// <param name="includeTermini">Include the N and C terminus (-1 and Length + 1)</param>
        /// <returns>A collection of all the sites where the proteases would cleave</returns>
        public static IEnumerable<int> GetCleavageIndices(string sequence, IEnumerable<IProtease> proteases, bool includeTermini = true)
        {
            // Combine all the proteases digestion sites
            SortedSet<int> locations = new SortedSet<int>();
            foreach (IProtease protease in proteases.Where(protease => protease != null))
            {
                locations.UnionWith(protease.GetDigestionSites(sequence));
            }

            if (!includeTermini)
                return locations;

            locations.Add(-1);
            locations.Add(sequence.Length - 1);

            return locations;
        }

        public static IEnumerable<string> Digest(string sequence, Protease protease, int maxMissedCleavages = 0, int minLength = 1, int maxLength = int.MaxValue, bool methionineInitiator = true, bool semiDigestion = false)
        {
            return Digest(sequence, new[] {protease}, maxMissedCleavages, minLength, maxLength, methionineInitiator, semiDigestion);
        }

        public static IEnumerable<string> Digest(string sequence, IEnumerable<IProtease> proteases, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue, bool methionineInitiator = true, bool semiDigestion = false)
        {
            return GetDigestionPoints(sequence, proteases, maxMissedCleavages, minLength, maxLength, methionineInitiator, semiDigestion).Select(points => sequence.Substring(points.Item1, points.Item2));
        }

        public static IEnumerable<string> Digest(IAminoAcidSequence sequence, IProtease protease, int maxMissedCleavages = 3, int minLength = 1, int maxLength = int.MaxValue, bool methionineInitiator = true, bool semiDigestion = false)
        {
            return Digest(sequence.Sequence, new[] {protease}, maxMissedCleavages, minLength, maxLength, methionineInitiator, semiDigestion);
        }

        #endregion Digestion

        public static double GetMass(string sequence)
        {
            double mass = Constants.Water;
            foreach (char letter in sequence)
            {
                AminoAcid residue;
                if (AminoAcid.TryGetResidue(letter, out residue))
                {
                    mass += residue.MonoisotopicMass;
                }
            }
            return mass;
        }

        #endregion Statics Methods
    }
}