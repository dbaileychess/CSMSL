// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ChromatogramType.cs) is part of CSMSL.
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

namespace CSMSL.Spectral
{
    public enum ChromatogramType
    {
        /// <summary>
        /// Takes the base peak of a spectrum and plots its intensity over time
        /// </summary>
        BasePeak,

        /// <summary>
        /// Sums the intensity of peaks within a specific m/z range
        /// </summary>
        MzRange,

        /// <summary>
        /// The total ion current of the spectrum
        /// </summary>
        TotalIonCurrent
    }
}