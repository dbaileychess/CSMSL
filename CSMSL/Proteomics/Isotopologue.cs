using System;
using System.Collections.Generic;
using System.Linq;
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class Isotopologue : Modification
    {
        private SortedList<double, Modification> _modifications;

        public Isotopologue(Modification modification)
            : this(modification, modification.Name) { }

        public Isotopologue(Modification modification, string name)
            : base(modification)
        {
            Name = name;
            _modifications = new SortedList<double, Modification>();
            AddModification(modification);
        }

        public Isotopologue(string name, ModificationSites sites)
            : base(0, name, sites)
        {
            _modifications = new SortedList<double, Modification>();
        }

        public Modification AddModification(Modification modification)
        {
            if (modification.Sites != Sites)
            {
                throw new ArgumentException("Unable to add a modification to an isotopologue with different modification sites.");
            }
            _modifications.Add(modification.MonoisotopicMass, modification); 
            MonoisotopicMass = _modifications.Keys.Average();
            return modification;
        }

        public Modification AddModification(string chemicalFormula, string name)
        {
            return AddModification(new Modification(chemicalFormula, name, Sites));
        }

        public IEnumerable<Modification> GetModifications()
        {
            return _modifications.Values;
        }

        public bool Contains(Modification modification)
        {
            return _modifications.ContainsValue(modification);
        }

        public IEnumerable<Peptide> GetUniquePeptides(Peptide peptide)
        {
            foreach (Modification mod in _modifications.Values)
            {
                Peptide pep2 = new Peptide(peptide, true);
                pep2.SetModification(mod);
                yield return pep2;
            }
        }

        /// <summary>
        /// Calculate the expected spacings between a group of Peptides (channels) in Th
        /// </summary>
        /// <param name="peptides">The peptides to calculate the spacings between</param>
        /// <param name="charge">The charge state of those peptides</param>
        /// <returns>An array of Th spacings between each peptide in increasing m/z</returns>
        public static double[] GetExpectedSpacings(IList<Peptide> peptides, int charge)
        {
            if (peptides.Count <= 1)
            {
                throw new ArgumentOutOfRangeException("Not enough peptides to calculate spacings");
            }

            // There is always 1 less spacings than peptides.
            double[] spacings = new double[peptides.Count - 1];

            // Ensure sorted order by mass
            Peptide[] sortedPeptides = peptides.OrderBy(p => p.MonoisotopicMass).ToArray();

            // Convert the first peptide to m/z space
            double previousMz = Mass.MzFromMass(sortedPeptides[0].MonoisotopicMass, charge);

            // Loop over each other peptide in sorted order
            for (int i = 1; i < sortedPeptides.Length; i++)
            {
                // Grab the current peptide's m/z
                double currentMZ = Mass.MzFromMass(peptides[i].MonoisotopicMass, charge);

                // Calculate the spacing between the two channels
                spacings[i - 1] = currentMZ - previousMz;

                // Save the current peptide m/z as the previous m/z
                previousMz = currentMZ;
            }
            return spacings;
        }
    }
}
