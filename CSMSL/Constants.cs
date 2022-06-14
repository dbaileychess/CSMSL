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