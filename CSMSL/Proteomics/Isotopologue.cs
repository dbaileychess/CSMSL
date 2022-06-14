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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Proteomics
{
    public class Isotopologue : Modification, IIsotopologue
    {
        private readonly SortedList<double, Modification> _modifications;

        public Modification this[int index]
        {
            get { return _modifications.Values[index]; }
        }

        public int Count
        {
            get { return _modifications.Count; }
        }

        public Isotopologue(string name, Modification modification)
            : base(modification.MonoisotopicMass, name)
        {
            _modifications = new SortedList<double, Modification>();
            Sites = modification.Sites;
            AddModification(modification);
        }

        public Isotopologue(string name, ModificationSites sites = ModificationSites.None)
            : base(0, name, sites)
        {
            _modifications = new SortedList<double, Modification>();
        }

        public Modification AddModification(Modification modification)
        {
            if (!Sites.ContainsSite(modification.Sites))
            {
                throw new ArgumentException("Unable to add a modification to an isotopologue with different modification sites.");
            }
            _modifications.Add(modification.MonoisotopicMass, modification);
            MonoisotopicMass = _modifications.Keys.Average();
            return modification;
        }

        public bool Contains(Modification modification)
        {
            return _modifications.ContainsValue(modification);
        }

        /// <summary>
        /// Calculate the expected spacings between a group of Peptides (channels) in Th
        /// </summary>
        /// <param name="peptides">The peptides to calculate the spacings between</param>
        /// <param name="charge">The charge state of those peptides</param>
        /// <returns>An array of Th spacings between each peptide in increasing m/z</returns>
        public static double[] GetExpectedSpacings<T2>(IList<T2> peptides, int charge) where T2 : AminoAcidPolymer
        {
            if (peptides.Count <= 1)
            {
                throw new ArgumentOutOfRangeException("peptides", "Not enough peptides to calculate spacings");
            }

            // There is always 1 less spacings than peptides.
            double[] spacings = new double[peptides.Count - 1];

            // Ensure sorted order by mass
            T2[] sortedPeptides = peptides.OrderBy(p => p.MonoisotopicMass).ToArray();

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

        public IEnumerator<Modification> GetEnumerator()
        {
            return _modifications.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _modifications.Values.GetEnumerator();
        }
    }
}