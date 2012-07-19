using System;

namespace CSMSL
{
    public class Mass
    {
        #region Static Methods

        /// <summary>
        /// Calculates the mass of a given m/z and charge, assuming a proton is the charge donator
        /// </summary>
        /// <param name="mz">The given m/z</param>
        /// <param name="charge">The given charge</param>
        /// <returns>The mass</returns>
        public static double MassFromMz(double mz, int charge)
        {
            return Math.Abs(charge) * mz - charge * Constants.PROTON;
        }

        /// <summary>
        /// Calculates the m/z of a given mass and chargem assuming a proton is the charge donator
        /// </summary>
        /// <param name="mass">The given mass</param>
        /// <param name="charge">The given charge</param>
        /// <returns>The m/z</returns>
        public static double MzFromMass(double mass, int charge)
        {              
            return mass / Math.Abs(charge) + Math.Sign(charge) * Constants.PROTON;
        }

        #endregion Static Methods
    }
}