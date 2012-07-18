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

        /// <summary>
        /// The mass of the most common isotope of hydrogen in atomic units (u)
        /// </summary>
        public const double HYDROGEN = 1.0078250321;

        /// <summary>
        /// The mass of the most common isotope of hydrogen in atomic units (u)
        /// </summary>
        public const double OXYGEN = 15.9949146221;

        #endregion Atomic

        #region Molecular

        /// <summary>
        /// The mass of the molecule H20 given in atomic units (u) of the most common isotopes
        /// </summary>
        public const double WATER = HYDROGEN * 2 + OXYGEN;

        #endregion Molecular
    }
}