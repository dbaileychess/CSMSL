// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (Terminus.cs) is part of CSMSL.
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

using System;

namespace CSMSL.Proteomics
{
    /// <summary>
    /// The terminus of an amino acid polymer N-[Amino Acids]-C
    /// </summary>
    [Flags]
    public enum Terminus
    {
        /// <summary>
        /// The N-terminus (amino-terminus)
        /// </summary>
        N = 1,

        /// <summary>
        /// The C-terminus (carboxyl-terminus)
        /// </summary>
        C = 2
    }
}