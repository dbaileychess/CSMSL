///////////////////////////////////////////////////////////////////////////
//  FragmentType.cs - A collection of common fragment types               /
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

using System;

namespace CSMSL.Proteomics
{
    [Flags]
    public enum FragmentTypes
    {
        None =      0,
        a =         1 << 0,
        adot =      1 << 1,
        b =         1 << 2,
        bdot =      1 << 3,
        c =         1 << 4,
        cdot =      1 << 5,
        x =         1 << 6,
        xdot =      1 << 7,
        y =         1 << 8,
        ydot =      1 << 9,
        z =         1 << 10,
        zdot =      1 << 11,
        Internal =  1 << 12,    
        All = Int32.MaxValue
    }
}