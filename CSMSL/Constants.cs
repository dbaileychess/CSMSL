///////////////////////////////////////////////////////////////////////////
//  Constants.cs - A collection of physical constants                     /
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
        /// The mass of the subatmoic particle with a single negative elementary charge in
        /// atomic units (u)
        /// </summary>
        public const double ELECTRON = 0.00054857990946;

        /// <summary>
        /// The mass of the subatomic particle with a single elementary charge in atomic
        /// units (u)
        /// </summary>
        public const double PROTON = 1.007276466812;

        #endregion Subatomic

        #region Atomic

        /// <summary>
        /// The mass of the most common isotope of carbon in atomic units (u)
        /// </summary>
        public const double CARBON = 12.00000000000;

        public const double CARBON13 = 13.0033548378;

        /// <summary>
        /// The mass of the most common isotope of hydrogen in atomic units (u)
        /// </summary>
        public const double HYDROGEN = 1.0078250321;

        public const double DEUTERIUM = 2.0141017778;

        public const double NITROGEN = 14.0030740048;

        public const double NITROGEN15 = 15.0001088982;
               
        /// <summary>
        /// The mass of the most common isotope of hydrogen in atomic units (u)
        /// </summary>
        public const double OXYGEN = 15.9949146221;

        public const double OXYGEN18 = 17.9991610;

        public const double SULFUR = 31.97207100;

        public const double SULFUR34 = 33.96786690;


        #endregion Atomic

        #region Molecular

        /// <summary>
        /// The mass of the molecule H20 given in atomic units (u) of the most common isotopes
        /// </summary>
        public const double WATER = HYDROGEN * 2 + OXYGEN;

        #endregion Molecular
    }
}