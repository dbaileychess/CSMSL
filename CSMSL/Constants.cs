// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Constants.cs) is part of CSMSL.
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

namespace CSMSL
{
    /// <summary>
    /// A collection of immutable constants and physical properties.
    /// Masses are given for the most abundant isotope unless otherwise stated
    ///
    /// Sources include:
    /// http://physics.nist.gov/cuu/Constants/index.html
    /// </summary>
    public static class Constants
    {
        #region Subatomic

        /// <summary>
        /// The mass of the subatomic particle with a single negative elementary charge in
        /// atomic units (u)
        /// </summary>
        public const double Electron = 0.00054857990946;

        /// <summary>
        /// The mass of the subatomic particle with a single elementary charge in atomic
        /// units (u)
        /// </summary>
        public const double Proton = 1.007276466812;

        #endregion Subatomic

        #region Atomic

        /// <summary>
        /// The mass of the most common isotope of carbon in atomic units (u)
        /// </summary>
        public const double Carbon = 12.00000000000;

        public const double Carbon13 = 13.0033548378;

        /// <summary>
        /// The mass difference between carbon 13 and 12 isotopes, often used for isotopic distributions
        /// </summary>
        public const double C13C12Difference = Carbon13 - Carbon;

        /// <summary>
        /// The mass of the most common isotope of hydrogen in atomic units (u)
        /// </summary>
        public const double Hydrogen = 1.00782503207;

        public const double Deuterium = 2.0141017778;

        public const double Nitrogen = 14.0030740048;

        public const double Nitrogen15 = 15.0001088982;

        /// <summary>
        /// The mass of the most common isotope of hydrogen in atomic units (u)
        /// </summary>
        public const double Oxygen = 15.99491461956;

        public const double Oxygen18 = 17.9991610;

        public const double Sulfur = 31.97207100;

        public const double Sulfur34 = 33.96786690;

        #endregion Atomic

        #region Molecular

        /// <summary>
        /// The mass of the molecule H20 given in atomic units (u) of the most common isotopes
        /// </summary>
        public const double Water = Hydrogen*2 + Oxygen;

        #endregion Molecular
    }
}