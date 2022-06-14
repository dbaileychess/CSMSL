// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using CSMSL.Chemistry;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSMSL.Proteomics
{
    public class AminoAcid : IAminoAcid
    {
        #region The Twenty Common Amino Acids

        public static AminoAcid Alanine { get; private set; }

        public static AminoAcid Arginine { get; private set; }

        public static AminoAcid Asparagine { get; private set; }

        public static AminoAcid AsparticAcid { get; private set; }

        public static AminoAcid Cysteine { get; private set; }

        public static AminoAcid GlutamicAcid { get; private set; }

        public static AminoAcid Glutamine { get; private set; }

        public static AminoAcid Glycine { get; private set; }

        public static AminoAcid Histidine { get; private set; }

        public static AminoAcid Isoleucine { get; private set; }

        public static AminoAcid Leucine { get; private set; }

        public static AminoAcid Lysine { get; private set; }

        public static AminoAcid Methionine { get; private set; }

        public static AminoAcid Phenylalanine { get; private set; }

        public static AminoAcid Proline { get; private set; }

        public static AminoAcid Selenocysteine { get; private set; }

        public static AminoAcid Serine { get; private set; }

        public static AminoAcid Threonine { get; private set; }

        public static AminoAcid Tryptophan { get; private set; }

        public static AminoAcid Tyrosine { get; private set; }

        public static AminoAcid Valine { get; private set; }

        #endregion The Twenty Common Amino Acids

        private static readonly Dictionary<string, AminoAcid> Residues;

        private static readonly AminoAcid[] ResiduesByLetter;

        /// <summary>
        /// Get the residue based on the residues's symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static AminoAcid GetResidue(string symbol)
        {
            return symbol.Length == 1 ? ResiduesByLetter[symbol[0]] : Residues[symbol];
        }

        /// <summary>
        /// Gets the resdiue based on the residue's one-character symbol
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static AminoAcid GetResidue(char letter)
        {
            return ResiduesByLetter[letter];
        }

        public static bool TryGetResidue(char letter, out AminoAcid residue)
        {
            residue = null;
            if (letter > 'z' || letter < 0)
                return false;
            residue = ResiduesByLetter[letter];
            return residue != null;
        }

        public static bool TryGetResidue(string symbol, out AminoAcid residue)
        {
            return Residues.TryGetValue(symbol, out residue);
        }

        public static AminoAcid AddResidue(string name, char oneLetterAbbreviation, string threeLetterAbbreviation, string chemicalFormula, ModificationSites site)
        {
            var residue = new AminoAcid(name, oneLetterAbbreviation, threeLetterAbbreviation, chemicalFormula, site);
            AddResidueToDictionary(residue);
            return residue;
        }

        /// <summary>
        /// Construct the actual amino acids
        /// </summary>
        static AminoAcid()
        {
            Residues = new Dictionary<string, AminoAcid>(66);
            ResiduesByLetter = new AminoAcid['z' + 1]; //Make it big enough for all the Upper and Lower characters
            Alanine = AddResidue("Alanine", 'A', "Ala", "C3H5NO", ModificationSites.A);
            Arginine = AddResidue("Arginine", 'R', "Arg", "C6H12N4O", ModificationSites.R);
            Asparagine = AddResidue("Asparagine", 'N', "Asn", "C4H6N2O2", ModificationSites.N);
            AsparticAcid = AddResidue("Aspartic Acid", 'D', "Asp", "C4H5NO3", ModificationSites.D);
            Cysteine = AddResidue("Cysteine", 'C', "Cys", "C3H5NOS", ModificationSites.C);
            GlutamicAcid = AddResidue("Glutamic Acid", 'E', "Glu", "C5H7NO3", ModificationSites.E);
            Glutamine = AddResidue("Glutamine", 'Q', "Gln", "C5H8N2O2", ModificationSites.Q);
            Glycine = AddResidue("Glycine", 'G', "Gly", "C2H3NO", ModificationSites.G);
            Histidine = AddResidue("Histidine", 'H', "His", "C6H7N3O", ModificationSites.H);
            Isoleucine = AddResidue("Isoleucine", 'I', "Ile", "C6H11NO", ModificationSites.I);
            Leucine = AddResidue("Leucine", 'L', "Leu", "C6H11NO", ModificationSites.L);
            Lysine = AddResidue("Lysine", 'K', "Lys", "C6H12N2O", ModificationSites.K);
            Methionine = AddResidue("Methionine", 'M', "Met", "C5H9NOS", ModificationSites.M);
            Phenylalanine = AddResidue("Phenylalanine", 'F', "Phe", "C9H9NO", ModificationSites.F);
            Proline = AddResidue("Proline", 'P', "Pro", "C5H7NO", ModificationSites.P);
            Selenocysteine = AddResidue("Selenocysteine", 'U', "Sec", "C3H5NOSe", ModificationSites.U);
            Serine = AddResidue("Serine", 'S', "Ser", "C3H5NO2", ModificationSites.S);
            Threonine = AddResidue("Threonine", 'T', "Thr", "C4H7NO2", ModificationSites.T);
            Tryptophan = AddResidue("Tryptophan", 'W', "Trp", "C11H10N2O", ModificationSites.W);
            Tyrosine = AddResidue("Tyrosine", 'Y', "Try", "C9H9NO2", ModificationSites.Y);
            Valine = AddResidue("Valine", 'V', "Val", "C5H9NO", ModificationSites.V);
        }

        private static void AddResidueToDictionary(AminoAcid residue)
        {
            Residues.Add(residue.Letter.ToString(CultureInfo.InvariantCulture), residue);
            Residues.Add(residue.Name, residue);
            Residues.Add(residue.Symbol, residue);
            ResiduesByLetter[residue.Letter] = residue;
            ResiduesByLetter[Char.ToLower(residue.Letter)] = residue;
        }

        internal AminoAcid(string name, char oneLetterAbbreviation, string threeLetterAbbreviation, string chemicalFormula, ModificationSites site)
            : this(name, oneLetterAbbreviation, threeLetterAbbreviation, new ChemicalFormula(chemicalFormula), site)
        {
        }

        internal AminoAcid(string name, char oneLetterAbbreviation, string threeLetterAbbreviation, ChemicalFormula chemicalFormula, ModificationSites site)
        {
            Name = name;
            Letter = oneLetterAbbreviation;
            Symbol = threeLetterAbbreviation;
            ChemicalFormula = chemicalFormula;
            MonoisotopicMass = ChemicalFormula.MonoisotopicMass;
            Site = site;
        }

        public ChemicalFormula ChemicalFormula { get; private set; }

        public char Letter { get; private set; }

        public ModificationSites Site { get; private set; }

        public double MonoisotopicMass { get; private set; }

        public string Name { get; private set; }

        public string Symbol { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", Letter, Symbol, Name);
        }

        public ChemicalFormulaModification ToHeavyModification(bool c, bool n)
        {
            var formula = new ChemicalFormula();
            if (c)
            {
                Element carbon = PeriodicTable.GetElement("C");
                int carbon12 = ChemicalFormula.Count(carbon[12]);
                formula.Add(carbon[12], -carbon12);
                formula.Add(carbon[13], carbon12);
            }

            if (n)
            {
                Element nitrogen = PeriodicTable.GetElement("N");
                int nitrogen14 = ChemicalFormula.Count(nitrogen[14]);
                formula.Add(nitrogen[14], -nitrogen14);
                formula.Add(nitrogen[15], nitrogen14);
            }

            return new ChemicalFormulaModification(formula, "#", Site);
        }
    }
}