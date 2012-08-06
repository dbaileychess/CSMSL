using System;

namespace CSMSL
{
    public class Mass : IComparable<Mass>
    {
        internal double _mono;
        internal double _avg;

        public double Monoisotopic
        {
            get
            {
                return _mono;
            }
        }

        public double Average
        {
            get
            {
                return _avg;
            }
        }

        public Mass()
            : this(0, 0) { }

        public Mass(IMass item)
            : this(item.Mass._mono, item.Mass._avg) { }

        public Mass(Mass item)
            : this(item._mono, item._avg) { }

        public Mass(double monoisotopic, double average)
        {
            _mono = monoisotopic;
            _avg = average;
        }

        /// <summary>
        /// Calculates the m/z for the monoisotopic mass
        /// </summary>
        /// <param name="charge">The charge state</param>
        /// <returns>The m/z for the moniosotopic mass at a given charge state</returns>
        public double ToMz(int charge)
        {
            return MzFromMass(_mono, charge);
        }

        public override string ToString()
        {
            return string.Format("{0:F5} ({1:F5})", _mono, _avg);
        }

        public int CompareTo(Mass other)
        {
            return _mono.CompareTo(other._mono);
        }

        #region Static Methods

        /// <summary>
        /// Calculates the mass of a given m/z and charge, assuming a proton is the charge donator
        /// </summary>
        /// <param name="mz">The given m/z</param>
        /// <param name="charge">The given charge</param>
        /// <returns>The mass</returns>
        public static double MassFromMz(double mz, int charge)
        {
            if (mz == 0) return 0;
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
            if (mass == 0) return 0;
            return mass / Math.Abs(charge) + Math.Sign(charge) * Constants.PROTON;
        }

        #endregion Static Methods
    }
}