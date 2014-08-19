// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (FragmentTypes.cs) is part of CSMSL.
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

namespace CSMSL.Proteomics
{
    [Flags]
    public enum FragmentTypes
    {
        None = 0,
        a = 1 << 0,
        adot = 1 << 1,
        b = 1 << 2,
        bdot = 1 << 3,
        c = 1 << 4,
        cdot = 1 << 5,
        x = 1 << 6,
        xdot = 1 << 7,
        y = 1 << 8,
        ydot = 1 << 9,
        z = 1 << 10,
        zdot = 1 << 11,
        Internal = 1 << 12,
        All = (1 << 12) - 1, // Handy way of setting all below the 12th bit
    }

    public static class FragmentTypesExtension
    {
        public static IEnumerable<FragmentTypes> GetIndividualFragmentTypes(this FragmentTypes fragmentTypes)
        {
            if (fragmentTypes == FragmentTypes.None)
                yield break;
            foreach (FragmentTypes site in Enum.GetValues(typeof (FragmentTypes)))
            {
                if (site == FragmentTypes.None || site == FragmentTypes.All || site == FragmentTypes.Internal)
                {
                    continue;
                }
                if ((fragmentTypes & site) == site)
                {
                    yield return site;
                }
            }
        }

        public static Terminus GetTerminus(this FragmentTypes fragmentType)
        {
            // Super handy: http://stackoverflow.com/questions/4624248/c-logical-riddle-with-bit-operations-only-one-bit-is-set
            if (fragmentType == FragmentTypes.None || (fragmentType & (fragmentType - 1)) != FragmentTypes.None)
            {
                throw new ArgumentException("Fragment Type must be a single value to determine the terminus", "fragmentType");
            }
            return fragmentType >= FragmentTypes.x ? Terminus.C : Terminus.N;
        }

        public static ChemicalFormula GetIonCap(this FragmentTypes fragmentType)
        {
            if (fragmentType == FragmentTypes.None || (fragmentType & (fragmentType - 1)) != FragmentTypes.None)
            {
                throw new ArgumentException("Fragment Type must be a single value to determine the ion cap", "fragmentType");
            }
            return FragmentIonCaps[fragmentType];
        }

        private static readonly Dictionary<FragmentTypes, ChemicalFormula> FragmentIonCaps = new Dictionary<FragmentTypes, ChemicalFormula>
        {
            {FragmentTypes.a, new ChemicalFormula("C-1H-1O-1")},
            {FragmentTypes.adot, new ChemicalFormula("C-1O-1")},
            {FragmentTypes.b, new ChemicalFormula("H-1")},
            {FragmentTypes.bdot, new ChemicalFormula()},
            {FragmentTypes.c, new ChemicalFormula("NH2")},
            {FragmentTypes.cdot, new ChemicalFormula("NH3")},
            {FragmentTypes.x, new ChemicalFormula("COH-1")},
            {FragmentTypes.xdot, new ChemicalFormula("CO")},
            {FragmentTypes.y, new ChemicalFormula("H")},
            {FragmentTypes.ydot, new ChemicalFormula("H2")},
            {FragmentTypes.z, new ChemicalFormula("N-1H-2")},
            {FragmentTypes.zdot, new ChemicalFormula("N-1H-1")},
        };
    }
}