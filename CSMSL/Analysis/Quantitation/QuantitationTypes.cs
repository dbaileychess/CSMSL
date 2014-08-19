// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (QuantitationTypes.cs) is part of CSMSL.
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

namespace CSMSL.Analysis.Quantitation
{
    [Flags]
    public enum QuantitationTypes
    {
        None = 0,
        ReporterTag = 1,
        SILAC = 2,
        Chemical = 4,
        Metabolic = 8,
        MS1Based = 16,
        MS2Based = 32
    }
}